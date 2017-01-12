using log4net;
using System.Threading;
using WhereBot.Chat.Properties;

namespace WhereBot.Agent.Slack
{

    public sealed class ChatBotService
    {

        #region Constructors

        public ChatBotService(ILog logger)
        {
            this.Logger = logger;
        }

        #endregion

        #region Properties

        private ILog Logger
        {
            get;
            set;
        }

        private ManualResetEvent Waiter
        {
            get;
            set;
        }

        private Thread Worker
        {
            get;
            set;
        }


        #endregion

        #region Methods

        public void Start()
        {
            this.Waiter = new ManualResetEvent(false);
            this.Worker = new Thread(this.ChatBotMain);
            this.Worker.Name = "WhereBot.ChatService";
            this.Worker.IsBackground = true;
            this.Worker.Start();
        }

        public void Stop()
        {
            this.Waiter.Set();
            if(!this.Worker.Join(10000))
            {
                this.Worker.Abort();
            }
        }

        private void ChatBotMain()
        {
            var wherebotApiUrl = Settings.Default.WhereBotApiUrl;
            var slackApiToken = Settings.Default.SlackApiToken;
            var client = new ChatBotClient(this.Logger, wherebotApiUrl, slackApiToken);
            client.Connect();
            this.Waiter.WaitOne();
            client.Disconnect();
        }

        #endregion

    }

}
