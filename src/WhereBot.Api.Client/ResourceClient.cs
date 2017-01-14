using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WhereBot.Api.Models;

namespace WhereBot.Api.Client
{

    public sealed class ResourceClient
    {

        #region Constructors

        internal ResourceClient(WhereBotClient client)
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

        internal static IEnumerable<Resource> FromJson(string json)
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
                var results = ResourceClient.FromJArray(array);
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
                yield return ResourceClient.FromJToken(token);
                yield break;
            }
            // unknown type
            throw new InvalidOperationException();
        }

        internal static IEnumerable<Resource> FromJArray(JArray array)
        {
            return array.Select(r => ResourceClient.FromJToken(r));
        }

        internal static Resource FromJToken(JToken token)
        {
            var locationToken = token.Value<JToken>("location");
            var location = default(Location);
            if ((locationToken != null) && (locationToken.Type != JTokenType.Null))
            {
                location = LocationClient.FromJToken(locationToken);
            }
            return new Resource.Builder
            {
                Id = token.Value<int>("id"),
                Name = token.Value<string>("name"),
                Location = location
            }.Build();
        }

        #endregion

        #region API Methods

        public IEnumerable<Resource> All(string name)
        {
            return this.Search();
        }

        public IEnumerable<Resource> Search(int? id = null, string name = null, int? locationId = null, int? mapId = null)
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
            if (locationId.HasValue)
            {
                query.Add("locationId", locationId.ToString());
            }
            if (mapId.HasValue)
            {
                query.Add("mapId", mapId.ToString());
            }
            var uri = new StringBuilder();
            uri.AppendFormat("{0}/resources/search", this.Client.RootUri);
            if (query.Count > 0)
            {
                uri.Append("?");
                uri.Append(string.Join("&", query.Select(kvp => string.Format("{0}={1}", WebUtility.UrlEncode(kvp.Key), WebUtility.UrlEncode(kvp.Value)))));
            }
            var json = client.DownloadString(uri.ToString());
            var resources = ResourceClient.FromJson(json);
            return resources;
        }

        public Resource AddResource(string resourceName)
        {
            var client = new WebClient();
            var uri = string.Format("{0}/resources/add?name={1}", this.Client.RootUri, WebUtility.UrlEncode(resourceName));
            var json = client.UploadString(uri, "POST", string.Empty);
            var resource = ResourceClient.FromJson(json).Single();
            return resource;
        }

        public Resource MoveResource(int resourceId, int newLocationId)
        {
            var client = new WebClient();
            var uri = string.Format("{0}/resources/{1}/moveTo/{2}", this.Client.RootUri, resourceId, newLocationId);
            var json = client.UploadString(uri, "POST", string.Empty);
            var resource = ResourceClient.FromJson(json).Single();
            return resource;
        }

        #endregion

    }

}
