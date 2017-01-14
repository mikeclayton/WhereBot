using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WhereBot.Api.Data.Domain;

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

        #region Conversion Methods

        internal static IEnumerable<Location> FromJson(string json)
        {
            var obj = JsonConvert.DeserializeObject(json);
            // is it a null?
            if (obj == null)
            {
                yield break;
            }
            // is it an array of objects?
            var array = obj as JArray;
            if (array != null)
            {
                var results = LocationClient.FromJArray(array);
                foreach (var result in results)
                {
                    yield return result;
                }
                yield break;
            }
            // is it a single object?
            var token = obj as JToken;
            if (token != null)
            {
                yield return LocationClient.FromJToken(token);
                yield break;
            }
            // unknown type
            throw new InvalidOperationException();
        }

        internal static IEnumerable<Location> FromJArray(JArray array)
        {
            return array.Select(l => LocationClient.FromJToken(l));
        }

        internal static Location FromJToken(JToken token)
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

        #endregion

        #region API Methods

        public IEnumerable<Location> All(string name)
        {
            return this.Search();
        }

        public IEnumerable<Location> Search(int? id = null, string name = null, int? mapId = null)
        {
            var client = new WebClient();
            var query = new Dictionary<string, string>();
            if (id.HasValue)
            {
                query.Add("id", id.ToString());
            }
            if (!string.IsNullOrEmpty(name))
            {
                query.Add("name", name);
            }
            if (mapId.HasValue)
            {
                query.Add("mapId", mapId.ToString());
            }
            var uri = new StringBuilder();
            uri.AppendFormat("{0}/locations/search", this.Client.RootUri);
            if (query.Count > 0)
            {
                uri.Append("?");
                uri.Append(string.Join("&", query.Select(kvp => string.Format("{0}={1}", WebUtility.UrlEncode(kvp.Key), WebUtility.UrlEncode(kvp.Value)))));
            }
            var json = client.DownloadString(uri.ToString());
            var locations = LocationClient.FromJson(json);
            return locations;
        }

        #endregion

    }

}
