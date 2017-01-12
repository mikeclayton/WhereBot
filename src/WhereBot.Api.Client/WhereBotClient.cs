namespace WhereBot.Api.Client
{

    public class WhereBotClient
    {

        public WhereBotClient(string rootUri)
        {
            this.RootUri = rootUri;
            this.Locations = new LocationClient(this);
            this.Resources = new ResourceClient(this);
            this.Maps = new MapClient(this);
        }

        public string RootUri
        {
            get;
            private set;
        }

        public LocationClient Locations
        {
            get;
            private set;
        }

        public ResourceClient Resources
        {
            get;
            private set;
        }

        public MapClient Maps
        {
            get;
            private set;
        }

    }

}
