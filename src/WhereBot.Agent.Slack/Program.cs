using log4net;
using Topshelf;

namespace WhereBot.Agent.Slack
{

    class Program
    {

        static void Main(string[] args)
        {

            Program.Logger = LogManager.GetLogger(typeof(Program));

            HostFactory.Run(x =>
            {

                x.Service<ChatBotService>(svc =>
                {
                    svc.ConstructUsing(name => new ChatBotService(Program.Logger));
                    svc.WhenStarted(bot => bot.Start());
                    svc.WhenStopped(bot => bot.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("WhereBot.ChatService");
                x.SetDisplayName("WhereBot Chat Service");
                x.SetDescription("WhereBot Chat Service");

            });

        }

        private static ILog Logger
        {
            get;
            set;
        }

    }

}
