using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        #region Methods

        internal static Resource FromJson(JToken token)
        {
            var locationToken = token.Value<JToken>("location");
            var location = default(Location);
            if((locationToken != null) && (locationToken.Type != JTokenType.Null))
            {
                location = LocationClient.FromJson(locationToken);
            }
            return new Resource.Builder
            {
                Id = token.Value<int>("id"),
                Name = token.Value<string>("name"),
                Location = location
            }.Build();
        }

        public Resource AddResource(string resourceName)
        {
            var client = new WebClient();
            var uri = string.Format("{0}/resources/add?name={1}", this.Client.RootUri, WebUtility.UrlEncode(resourceName));
            var responseJson = client.UploadString(uri, "POST", string.Empty);
            var jsonObject = (JToken)JsonConvert.DeserializeObject(responseJson);
            var resource = ResourceClient.FromJson(jsonObject);
            return resource;
        }

        public List<Resource> SearchByResourceName(string name)
        {
            var client = new WebClient();
            var uri = string.Format("{0}/search/resources/byResourceName/{1}", this.Client.RootUri, WebUtility.UrlEncode(name));
            var responseJson = client.DownloadString(uri);
            var jsonObjects = (JArray)JsonConvert.DeserializeObject(responseJson);
            var resources = jsonObjects.Select(l => ResourceClient.FromJson(l)).ToList();
            return resources;
        }

        public Resource MoveResource(Resource resource, Location newLocation)
        {
            var client = new WebClient();
            var uri = string.Format("{0}/resources/{1}/moveTo/{2}", this.Client.RootUri, resource.Id, newLocation.Id);
            var responseJson = client.UploadString(uri, "POST", string.Empty);
            var jsonObject = (JToken)JsonConvert.DeserializeObject(responseJson);
            var newResource = ResourceClient.FromJson(jsonObject);
            return newResource;
        }


        #endregion

    }

}
