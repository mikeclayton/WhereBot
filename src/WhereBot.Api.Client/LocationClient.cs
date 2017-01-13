using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WhereBot.Api.Models;

namespace WhereBot.Api.Client
{

    public sealed class LocationClient
    {

        #region Constructors

        internal LocationClient(WhereBotClient client)
        {
            this.Client = client;
        }

        #endregion

        #region Properties

        internal WhereBotClient Client
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        internal static Location FromJson(JToken token)
        {
            var mapToken = token.Value<JToken>("map");
            var map = default(Map);
            if ((mapToken != null) && (mapToken.Type != JTokenType.Null))
            {
                map = MapClient.FromJson(mapToken);
            }
            return new Location.Builder
            {
                Id = token.Value<int>("id"),
                Name = token.Value<string>("name"),
                Map = map,
                X = token.Value<int>("x"),
                Y = token.Value<int>("y")
            }.Build();
        }

        public List<Location> SearchByLocationName(string name)
        {
            var client = new WebClient();
            var uri = string.Format("{0}/search/locations/byLocationName/{1}", this.Client.RootUri, WebUtility.UrlEncode(name));
            var responseJson = client.DownloadString(uri);
            var jsonObjects = (JArray)JsonConvert.DeserializeObject(responseJson);
            var locations = jsonObjects.Select(l => LocationClient.FromJson(l)).ToList();
            return locations;
        }

        #endregion

    }

}
