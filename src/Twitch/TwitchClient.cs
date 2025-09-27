using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.EventSub.Websockets;

namespace Miksado.Twitch
{
    internal class TwitchClient
    {
        private static readonly string ClientId = "2gcvz7kjkkcmcxgvspd9wb1dddz92u";
        private static readonly string RedirectUri = "https://localhost:8081";
        private static readonly string AuthUrl = "https://id.twitch.tv/oauth2/authorize";
        private static readonly string[] Scopes = ["user:read:chat"];
        private static readonly string DummyToken = "5vx8b5r4uru2s3qijmujdkvs48nmip";

        private readonly EventSubWebsocketClient EventSubClient;
        //private readonly ILogger<EventSubWebsocketClient> Logger;
        //private readonly IEnumerable<INotificationHandler> Handlers;
        //private readonly IServiceProvider ServiceProvider;
        //private readonly WebsocketClient WebsocketClient;

        public TwitchClient()
        {
            TwitchAPI twitchApi = new();
            twitchApi.Settings.ClientId = ClientId;
            twitchApi.Settings.AccessToken = DummyToken;

            //Logger = null;
            //WebsocketClient = new();
            //ServiceProvider = null;
            //Handlers = Array.Empty<INotificationHandler>();

            EventSubClient = new EventSubWebsocketClient();

            // Set up event handlers
            //EventSubClient.WebsocketConnected += OnWebsocketConnected;
            //EventSubClient.WebsocketDisconnected += OnWebsocketDisconnected;
            //EventSubClient.ErrorOccurred += OnErrorOccurred;
        }

        private Task OnWebsocketConnected(object sender, EventArgs e)
        {
            Console.WriteLine("WebSocket connected.");
            return Task.CompletedTask;
            // Subscribe to events here
        }

        public async Task ConnectAsync()
        {
            await EventSubClient.ConnectAsync();
        }

        public string GetAuthorizationUrl()
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] stateBytes = new byte[32];
            rng.GetBytes(stateBytes);
            string state = System.Convert.ToBase64String(stateBytes);

            var parameters = new Dictionary<string, string>
            {
                ["client_id"] = ClientId,
                ["force_verify"] = "false",
                ["redirect_uri"] = RedirectUri,
                ["response_type"] = "token",
                ["scope"] = string.Join(" ", Scopes),
                ["state"] = state,
            };

            string query = string.Join("&", parameters.Select(p => $"{System.Uri.EscapeDataString(p.Key)}={System.Uri.EscapeDataString(p.Value)}"));
            string authUrl = $"https://id.twitch.tv/oauth2/authorize?{query}";
            Console.WriteLine($"Authorization URL: {authUrl}");
            return authUrl;
        }


    }
}
