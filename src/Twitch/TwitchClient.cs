using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;

namespace Miksado.Twitch
{
    public enum SubTier
    {
        Tier1,
        Tier2,
        Tier3
    }

    internal class TwitchClient
    {
        private static readonly string ClientId = "2gcvz7kjkkcmcxgvspd9wb1dddz92u";
        private static readonly string RedirectUri = "http://localhost:8081/";
        private static readonly string[] Scopes = [
            "user:read:chat",
            "bits:read",
            "channel:read:redemptions",
            "channel:manage:redemptions",
            "channel:read:polls",
            "channel:manage:polls",
            ];

        public string? TwitchId;
        public string? Token;
        private bool _isConnected = false;

        public TwitchAPI TwitchApi;
        public EventSubWebsocketClient EventSubClient;

        public TwitchClient()
        {
            TwitchApi = new();
            EventSubClient = new();
        }
        public async Task ConnectAsync()
        {
            await EventSubClient.ConnectAsync();
        }

        public async Task OnWebsocketConnected(Logger.Logger logger, object _, WebsocketConnectedArgs e)
        {
            logger.Debug("websocket connected event received, starting subscriptions");

            if (TwitchId == null)
            {
                logger.Error("TwitchId is null, cannot create subscriptions");
                return;
            }

            if (!e.IsRequestedReconnect)
            {
                var chatMsgCondition = new Dictionary<string, string> { { "broadcaster_user_id", TwitchId }, { "user_id", TwitchId } };
                await TwitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.chat.message",
                    "1",
                    chatMsgCondition,
                    EventSubTransportMethod.Websocket,
                    EventSubClient.SessionId,
                    accessToken: Token);

                var chatNotiCondition = new Dictionary<string, string> { { "broadcaster_user_id", TwitchId }, { "user_id", TwitchId } };
                await TwitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.chat.notification",
                    "1",
                    chatNotiCondition,
                    EventSubTransportMethod.Websocket,
                    EventSubClient.SessionId,
                    accessToken: Token);

                var redeemCondition = new Dictionary<string, string> { { "broadcaster_user_id", TwitchId } };
                await TwitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.channel_points_custom_reward_redemption.add",
                    "1",
                    redeemCondition,
                    EventSubTransportMethod.Websocket,
                    EventSubClient.SessionId,
                    accessToken: Token);

                var bitsCondition = new Dictionary<string, string> { { "broadcaster_user_id", TwitchId } };
                await TwitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.bits.use",
                    "1",
                    bitsCondition,
                    EventSubTransportMethod.Websocket,
                    EventSubClient.SessionId,
                    accessToken: Token);

                var pollEndCondition = new Dictionary<string, string> { { "broadcaster_user_id", TwitchId } };
                await TwitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.poll.end",
                    "1",
                    pollEndCondition,
                    EventSubTransportMethod.Websocket,
                    EventSubClient.SessionId,
                    accessToken: Token);
            }
        }

        public async Task OnErrorOccurred(object? sender, ErrorOccuredArgs e)
        {
            await Task.CompletedTask;
        }

        public async Task OnWebsocketReconnected(object? sender, WebsocketReconnectedArgs e)
        {
            await Task.CompletedTask;
        }

        public async Task OnWebsocketDisconnected(object? sender, WebsocketDisconnectedArgs e)
        {
            await Task.CompletedTask;
        }

        private async Task<HttpListenerContext?> GetContextWithTimeout(HttpListener listener, TimeSpan timeout)
        {
            var task = listener.GetContextAsync();
            var delay = Task.Delay(timeout);

            var completed = await Task.WhenAny(task, delay);

            if (completed == delay)
            {
                return null; // Timeout
            }

            return await task;
        }

        public async Task OnAuthorizeButtonClicked(Logger.Logger logger)
        {
            logger.Info("Starting Twitch authorization...");
            logger.Debug("redirect uri: " + RedirectUri);

            HttpListener listener = new();
            listener.Prefixes.Add(RedirectUri);

            logger.Debug("Listening for OAuth redirect...");

            try
            {
                listener.Start();
                logger.Debug("Opening browser for user authorization...");

                string authUrl = GetAuthorizationUrl();
                logger.Debug("auth url: " + authUrl);

                Process.Start(new ProcessStartInfo { FileName = authUrl, UseShellExecute = true });
                logger.Debug("Waiting for user to complete authorization...");

                // it's hanging here for some reason
                // deleting https:// in address bar makes it work... why?

                HttpListenerContext? context = await GetContextWithTimeout(listener, TimeSpan.FromSeconds(10));
                if (context == null)
                {
                    logger.Error("Timeout waiting for OAuth redirect. Please ensure your browser is not blocking the redirect.");
                }

                //HttpListenerContext context = await listener.GetContextAsync();
                logger.Debug("done waiting for user to complete authorization...");

                string bridgePage = @"
                    <html>
                    <head><title>Authorization</title></head>
                    <body>
                      <h1>Authorizing...</h1>
                      <script>
                        if (window.location.hash) {
                          const params = new URLSearchParams(window.location.hash.substring(1));
                          const token = params.get('access_token');
                          if (token) {
                            // Send to backend
                            fetch('/', {
                              method: 'POST',
                              headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                              body: 'access_token=' + encodeURIComponent(token)
                            }).then(() => {
                              document.body.innerHTML = '<h1>Success!</h1><p>You can close this window.</p>';
                            });
                          }
                        }
                      </script>
                    </body>
                    </html>";

                if (context == null)
                {
                    logger.Error("context is null, cannot send bridge page");
                }
                else
                {
                    await SendResponse(context.Response, bridgePage);
                }


                logger.Debug("Waiting for token POST request...");

                var tokenContext = await listener.GetContextAsync();
                logger.Debug($"tokenContext abspath: {tokenContext.Request.Url.AbsolutePath}");

                if (tokenContext.Request.HttpMethod == "POST")
                {
                    logger.Debug($"Received token POST request, processing...");
                    logger.Debug($"Request URL: {tokenContext.Request.Url}");
                    logger.Debug($"Request Method: {tokenContext.Request.HttpMethod}");
                    logger.Debug($"Request Content Length: {tokenContext.Request.ContentLength64}");
                    logger.Debug($"Request Content Type: {tokenContext.Request.ContentType}");
                    logger.Debug($"Request Input Stream: {tokenContext.Request.InputStream}");

                    using (var reader = new System.IO.StreamReader(tokenContext.Request.InputStream, tokenContext.Request.ContentEncoding))
                    {
                        var body = await reader.ReadToEndAsync();
                        var parsed = HttpUtility.ParseQueryString(body);
                        Token = parsed.Get("access_token");
                        try
                        {
                            // Validate the token first
                            logger.Debug("Validating access token...");
                            ValidateAccessTokenResponse tokenValidation = await TwitchApi.Auth.ValidateAccessTokenAsync(Token);

                            if (tokenValidation != null)
                            {
                                logger.Debug($"Token valid. User ID: {tokenValidation.UserId}, Login: {tokenValidation.Login}");
                                TwitchId = tokenValidation.UserId;
                                logger.Info($"Authorized as {tokenValidation.Login} (ID: {tokenValidation.UserId})");
                                TwitchApi.Settings.AccessToken = Token;
                                TwitchApi.Settings.ClientId = ClientId;
                                _isConnected = true;
                            }
                            else
                            {
                                logger.Info("Token validation failed - token is invalid");
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Info($"Token validation failed: {ex.Message}");
                            logger.Info($"Exception details: {ex}");
                        }
                    }
                    string successResponse2 = "<html><body><h1>Success!</h1><p>You can close this window.</p></body></html>";
                    await SendResponse(tokenContext.Response, successResponse2);
                }
            }
            catch
            {
                throw new Exception("authorization error");
            }
            finally
            {
                listener.Stop();
            }
        }

        private async Task SendResponse(HttpListenerResponse response, string html)
        {
            var buffer = Encoding.UTF8.GetBytes(html);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        private string GetAuthorizationUrl()
        {
            string twitchBaseUrl = "https://id.twitch.tv/oauth2/authorize";
            bool forceVerify = false;
            string responseType = "token";
            string scope = string.Join(" ", Scopes);
            string state = "_placeholder_state_";

            string authUrl = twitchBaseUrl;
            authUrl += $"?client_id={ClientId}";
            authUrl += $"&force_verify={forceVerify.ToString().ToLower()}";
            authUrl += $"&redirect_uri={RedirectUri}";
            authUrl += $"&response_type={responseType}";
            authUrl += $"&scope={scope}";
            authUrl += $"&state={state}";
            return authUrl;
        }

        public bool IsConnected()
        {
            // maybe include more sophisticated validation call?
            return _isConnected;
        }
    }
}
