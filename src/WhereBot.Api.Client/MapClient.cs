using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using WhereBot.Api.Data.Domain;

namespace WhereBot.Api.Client
{

    public sealed class MapClient
    {

        #region Constructors

        internal MapClient(WhereBotClient client)
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

        #region Map Methods

        public static Map FromJson(JToken token)
        {
            return new Map.Builder
            {
                Id = token.Value<int>("id"),
                Name = token.Value<string>("name"),
                Description = token.Value<string>("description"),
                Filename = token.Value<string>("filename"),
            }.Build();
        }

        public byte[] GetMapBytes(int mapId, List<int> locationIds, List<int> resourceIds)
        {
            var client = new WebClient();
            var uri = string.Format(
                "{0}/maps/render/fromIds?locationIds={1}&resourceIds={2}",
                this.Client.RootUri,
                string.Join(",", locationIds),
                string.Join(",", resourceIds));
            var responseBytes = client.DownloadData(uri);
            return responseBytes;
        }

        #endregion


    }

}
