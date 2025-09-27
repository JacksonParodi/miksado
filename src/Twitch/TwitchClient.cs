using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;

namespace Miksado.Twitch
{
    internal class TwitchClient
    {
        private static readonly string ClientId = "2gcvz7kjkkcmcxgvspd9wb1dddz92u";
        //private static readonly string RedirectUri = "https://localhost:8081";
        //private static readonly string AuthUrl = "https://id.twitch.tv/oauth2/authorize";
        //private static readonly string[] Scopes = ["user:read:chat"];
        private static readonly string DummyToken = "5vx8b5r4uru2s3qijmujdkvs48nmip";
        private static readonly string JacksonTwitchId = "43920828";

        public TwitchAPI TwitchApi;
        public readonly EventSubWebsocketClient EventSubClient;
        //private readonly ILogger<EventSubWebsocketClient> Logger;
        //private readonly IEnumerable<INotificationHandler> Handlers;
        //private readonly IServiceProvider ServiceProvider;
        //private readonly WebsocketClient WebsocketClient;

        public bool _test = false;

        public TwitchClient()
        {
            TwitchApi = new();
            TwitchApi.Settings.ClientId = ClientId;
            TwitchApi.Settings.AccessToken = DummyToken;

            //Logger = null;
            //WebsocketClient = new();
            //ServiceProvider = null;
            //Handlers = Array.Empty<INotificationHandler>();

            EventSubClient = new EventSubWebsocketClient();
            EventSubClient.WebsocketConnected += OnWebsocketConnected;
            //EventSubClient.ChannelChatMessage += OnChannelChatMessage;

            // Set up event handlers
            //EventSubClient.WebsocketConnected += OnWebsocketConnected;
            //EventSubClient.WebsocketDisconnected += OnWebsocketDisconnected;
            //EventSubClient.ErrorOccurred += OnErrorOccurred;
        }

        private async Task OnWebsocketConnected(object sender, WebsocketConnectedArgs e)
        {
            if (!e.IsRequestedReconnect)
            {
                var condition = new Dictionary<string, string> { { "broadcaster_user_id", JacksonTwitchId }, { "user_id", JacksonTwitchId } };

                await TwitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.chat.message", "1", condition, EventSubTransportMethod.Websocket,
                EventSubClient.SessionId, accessToken: "5vx8b5r4uru2s3qijmujdkvs48nmip");
            }
        }

        //private async Task OnChannelChatMessage(object sender, ChannelChatMessageArgs e)
        //{
        //    string t = e.Payload.Event.Message.Text;
        //    //var eventData = e.Notification.Payload.Event;
        //    //_logger.LogInformation($"{eventData.UserName} followed {eventData.BroadcasterUserName} at {eventData.FollowedAt}");
        //}

        public async Task ConnectAsync()
        {
            await EventSubClient.ConnectAsync();
        }
    }
}
