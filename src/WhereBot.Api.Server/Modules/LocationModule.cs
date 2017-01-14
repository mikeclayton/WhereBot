using Nancy;
using System.Linq;

namespace WhereBot.Api.Server.Modules
{

    public class LocationModule : NancyModule
    {

        #region Constructors

        public LocationModule() : base("/locations")
        {
            this.InitRoutes();
        }

        #endregion

        #region Properties

        private DataSet Repository
        {
            get
            {
                return Globals.Repository;
            }
        }

        #endregion

        #region Methods

        private void InitRoutes()
        {

            #region Query Routes

            Get["/all"] = parameters =>
            {
                var locations = this.Repository.GetLocations();
                return Response.AsJson(locations);
            };

            Get["/search"] = parameters =>
            {
                var querystring = (DynamicDictionary)Request.Query;
                var filter = this.Repository.GetLocations().AsEnumerable();
                if (querystring.ContainsKey("id"))
                {
                    var id = int.Parse((string)querystring["id"]);
                    filter = filter.Where(l => l.Id == id);
                }
                if (querystring.ContainsKey("name"))
                {
                    var name = ((string)querystring["name"]).ToLowerInvariant();
                    filter = filter.Where(l => l.Name.ToLower() == name);
                }
                if (querystring.ContainsKey("mapId"))
                {
                    var mapId = int.Parse((string)querystring["mapId"]);
                    filter = filter.Where(l => (l.Map != null) && (l.Map.Id == mapId));
                }
                var locations = filter.ToList();
                return Response.AsJson(locations);
            };

            #endregion

            #region Action Routes

            Post["/add"] = parameters =>
            {
                var mapId = int.Parse((string)this.Request.Query["mapId"]);
                var map = this.Repository.GetMaps().Single(m => m.Id == mapId); ;
                var name = (string)this.Request.Query["name"];
                var x = int.Parse((string)this.Request.Query["x"]);
                var y = int.Parse((string)this.Request.Query["y"]);
                var location = this.Repository.AddLocation(map, name, x, y);
                return Response.AsJson(location);
            };

            #endregion

        }

        #endregion

    }

}
