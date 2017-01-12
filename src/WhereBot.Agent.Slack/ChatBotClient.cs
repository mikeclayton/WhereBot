using log4net;
using SlackAPI;
using SlackAPI.WebSocketMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using WhereBot.Api.Client;

namespace WhereBot.Agent.Slack
{

    public sealed class ChatBotClient
    {

        #region Fields

        private SlackSocketClient _slackSocketClient;

        #endregion

        #region Constructors

        public ChatBotClient(ILog logger, string wherebotApiUrl, string slackApiToken)
        {
            this.Logger = logger;
            this.WhereBotApiUrl = wherebotApiUrl;
            this.SlackApiToken = slackApiToken;
        }

        #endregion

        #region Properties

        private ILog Logger
        {
            get;
            set;
        }

        private string WhereBotApiUrl
        {
            get;
            set;
        }

        private string SlackApiToken
        {
            get;
            set;
        }

        #endregion

        #region IChatSession Interface

        public void Connect()
        {

            // operating system has to support websockets otherwise SlackSocketClient.Connect() silently fails,
            // so flush this error out before we hit it
            var x = new ClientWebSocket();

            this._slackSocketClient = new SlackSocketClient(this.SlackApiToken);
            _slackSocketClient.OnHello += socketClient_OnHello;
            _slackSocketClient.OnMessageReceived += socketClient_OnMessageReceived;

            var clientReady = new ManualResetEventSlim(false);
            var onConnected = new Action<LoginResponse>((loginResponse) => {
                var message = "rtm start command sent!";
                Console.WriteLine(message);
                this.Logger.Info(message);
                // This is called once the client has emitted the RTM start command
                clientReady.Set();
            });

            var onSocketConnected = new Action(() =>
            {
                // This is called once the RTM client has connected to the end point
                this.Logger.Info("rtm client connected!");
            });

            this._slackSocketClient.Connect(onConnected, onSocketConnected);
            clientReady.Wait();

            if (!_slackSocketClient.IsConnected)
            {
                throw new InvalidOperationException();
            }

        }

        public void Disconnect()
        {
            this.Logger.Info("Disconnecting");
            _slackSocketClient.CloseSocket();
        }

        #endregion

        #region Event Handlers

        private void socketClient_OnHello()
        {
            this.Logger.Info("OnHello");
        }

        private void socketClient_OnMessageReceived(NewMessage message)
        {
            var chatbotId = this._slackSocketClient.MySelf.id;
            var messageText = message.text;
            // skip blank messages
            if (string.IsNullOrEmpty(messageText))
            {
                return;
            }
            // skip messages that originate from the chatbot
            if (message.user == chatbotId)
            {
                return;
            }
            // find any user mentions in the message
            var userMentions = Regex.Matches(messageText, "<@(?<mention>.*?)>").Cast<Match>().ToList();
            var userMentionIds = userMentions.Select(m => m.Groups["mention"].Value).ToList();
            // if this is a group message, make sure the chatbot was mentioned
            var chatbotMentions = Enumerable.Range(0, userMentions.Count).Where(i => userMentionIds[i] == chatbotId).ToList();
            if (this._slackSocketClient.ChannelLookup.ContainsKey(message.channel) && (chatbotMentions.Count == 0))
            {
                return;
            }
            // remove any chatbot mentions and replace user mentions with their real names
            messageText = ChatBotClient.ReplaceUserMentions(messageText, userMentions, this._slackSocketClient.UserLookup, new List<string> { chatbotId });
            // split the message into arguments
            var messageArgs = ChatBotClient.ParseMessageArguments(messageText);

            switch(messageArgs[0])
            {
                case "where":
                    this.ExecuteWhereCommand(message, messageArgs);
                    break;
                case "set":
                    this.ExecuteSetCommand(message, messageArgs);
                    break;
                default:
                    this._slackSocketClient.SendMessage((messageReceived) => { }, message.channel, "unknown command!");
                    break;
            };

        }

        #endregion

        #region Helper Methods

        private static string ReplaceUserMentions(string messageText, List<Match> userMentions, Dictionary<string, User> userLookups, List<string> excludeUserIds)
        {
            var messageBuilder = new StringBuilder();
            var messagePosition = 0;
            foreach (var userMention in userMentions)
            {
                if (userMention.Index > messagePosition)
                {
                    messageBuilder.Append(messageText.Substring(messagePosition, userMention.Index - messagePosition));
                }
                var userMentionId = userMention.Groups["mention"].Value;
                if (!excludeUserIds.Contains(userMentionId))
                {
                    messageBuilder.AppendFormat("\"{0}\"", userLookups[userMentionId].profile.real_name);
                }
                messagePosition = userMention.Index + userMention.Length;
            }
            if (messagePosition < messageText.Length)
            {
                messageBuilder.Append(messageText.Substring(messagePosition));
            }
            return messageBuilder.ToString();
        }

        private static string NormalizeWhitespace(string messageText)
        {
            messageText = messageText.Replace("\t", " ");
            messageText = messageText.Replace("\r", " ");
            messageText = messageText.Replace("\n", " ");
            messageText = messageText.Trim();
            while (messageText.Contains("  "))
            {
                messageText = messageText.Replace("  ", " ");
            }
            return messageText;
        }

        /// <summary>
        /// Break a message into component parts, allowing for quoted strings.
        ///
        /// For example:
        ///
        ///   where is "Michael Clayton"
        ///
        /// becomes
        ///
        /// { "where", "is", "Michael Clayton" }
        ///
        /// </summary>
        /// <param name="messageText"></param>
        /// <returns></returns>
        private static List<string> ParseMessageArguments(string messageText)
        {
            var parts = new List<string>();
            var part = default(String);
            var isQuoted = false;
            for(var i = 0; i < messageText.Length; i++)
            {
                var messageChar = messageText[i];
                if (messageChar == '"')
                {
                    if (isQuoted)
                    {
                        // end of quoted word
                        if (!string.IsNullOrEmpty(part))
                        {
                            parts.Add(part);
                        }
                        part = string.Empty;
                        isQuoted = false;
                    }
                    else
                    {
                        // start of quoted word
                        isQuoted = true;
                    }
                }
                else if (isQuoted)
                {
                    // continuing quoted word
                    part += messageChar;
                }
                else if (char.IsWhiteSpace(messageChar))
                {
                    // end of word
                    if (!string.IsNullOrEmpty(part))
                    {
                        parts.Add(part);
                    }
                    part = string.Empty;
                }
                else
                {
                    // continuing current word
                    part += messageChar;
                }
            }
            // finish any dangling parts
            if (isQuoted)
            {
                throw new InvalidOperationException("unmatched quote in text");
            }
            else if (!string.IsNullOrEmpty(part))
            {
                parts.Add(part);
            }
            return parts;
        }

        #endregion

        #region Chat Commands

        private void ExecuteWhereCommand(NewMessage message, List<string> messageArgs)
        {
            // where am i
            // where [is|are] <resource> <resource> <group> <group> [on [map] <map>]
            // where [is|are] <location> <location> [on [map] <map>]
            var argIndex = 0;
            var whereNames = new List<string>();
            // where
            if (messageArgs[argIndex] != "where")
            {
                throw new InvalidOperationException("'where' expected");
            }
            argIndex += 1;
            // [ is | are ]
            if ((messageArgs[argIndex] == "is") || (messageArgs[argIndex] == "are"))
            {
                argIndex += 1;
            }
            // names
            while ((argIndex < messageArgs.Count) && (messageArgs[argIndex] != "on"))
            {
                whereNames.Add(messageArgs[argIndex]);
                argIndex += 1;
            }
            // [on]
            var mapName = default(string);
            if (argIndex < messageArgs.Count)
            {
                argIndex += 1;
                // [map]
                if (messageArgs[argIndex] == "map")
                {
                    argIndex += 1;
                }
                // <map>
                mapName = string.Join(" ", messageArgs.Skip(argIndex).ToArray());
            }
            // find any matching locations and resources
            var client = new WhereBotClient(this.WhereBotApiUrl);
            var locations = whereNames.SelectMany(n => client.Locations.SearchByLocationName(n)).ToList();
            var resources = whereNames.SelectMany(n => client.Resources.SearchByResourceName(n)).ToList();
            // generate the response message
            var messageText = new StringBuilder();
            messageText.AppendFormat("I found {0} results:", locations.Count + resources.Count);
            messageText.AppendLine();
            if (locations.Count > 0)
            {
                messageText.Append(string.Join("\r\n", locations.Select(l => string.Format("Location {0}", l.Name)).ToArray()));
            }
            if (resources.Count > 0)
            {
                if (locations.Count > 0)
                {
                    messageText.AppendLine();
                }
                messageText.Append(string.Join("\r\n", resources.Select(r => string.Format("{0} at {1}", r.Name, r.Location.Name)).ToArray()));
            }
            this._slackSocketClient.SendMessage((messageReceived) => { }, message.channel, messageText.ToString());
            // generate the map
            var imageBytes = client.Maps.GetMapBytes(0, locations.Select(l => l.Id).ToList(), resources.Select(r => r.Id).ToList());
            this._slackSocketClient.UploadFile((messageReceived) => { }, imageBytes, "map.png", new string[] { message.channel });
        }

        private void ExecuteSetCommand(NewMessage message, List<string> messageArgs)
        {
            // set [ [my] location ] <location>
            var argIndex = 0;
            // set
            if (messageArgs[argIndex] != "set")
            {
                throw new InvalidOperationException("'set' expected");
            }
            argIndex += 1;
            // [ [my] location ]
            switch (messageArgs[argIndex])
            {
                case "my":
                    argIndex += 1;
                    if ((messageArgs[argIndex] != "location"))
                    {
                        throw new InvalidOperationException("'location' expected");
                    }
                    argIndex += 1;
                    break;
                case "location":
                    argIndex += 1;
                    break;
            }
            // <location>
            var locationName = string.Join(" ", messageArgs.Skip(argIndex).ToArray());
            // find or create a resource for the person
            var client = new WhereBotClient(this.WhereBotApiUrl);
            var resourceName = this._slackSocketClient.UserLookup[message.user].profile.real_name;
            var resource = client.Resources.SearchByResourceName(resourceName).SingleOrDefault();
            if (resource == null)
            {
                resource = client.Resources.AddResource(resourceName);
            }
            // find any matching locations
            var locations = client.Locations.SearchByLocationName(locationName);
            switch (locations.Count)
            {
                case 0:
                    this._slackSocketClient.SendMessage((messageReceived) => { }, message.channel, "Sorry, I don't know where that location is!");
                    return;
                case 1:
                    break;
                default:
                    this._slackSocketClient.SendMessage((messageReceived) => { }, message.channel, "I found more than one location with that name!");
                    return;
            }
            // set the location
            var location = locations.Single();
            var newResource = client.Resources.MoveResource(resource, location);
            this._slackSocketClient.SendMessage((messageReceived) => { }, message.channel, string.Format("Done. Your new location is '{0}'!", locationName));
        }

        #endregion

    }

}
