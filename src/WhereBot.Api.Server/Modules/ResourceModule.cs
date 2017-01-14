using Nancy;
using System.Linq;
using WhereBot.Api.Data;

namespace WhereBot.Api.Server.Modules
{

    public class ResourceModule : NancyModule
    {

        #region Constructors

        public ResourceModule() : base("/resources")
        {
            this.InitRoutes();
        }

        #endregion

        #region Properties

        private DbContext DbContext
        {
            get
            {
                return Globals.DbContext;
            }
        }

        #endregion

        #region Methods

        private void InitRoutes()
        {

            #region Query Routes

            Get["/search"] = parameters =>
            {
                var querystring = (DynamicDictionary)Request.Query;
                var filter = this.DbContext.GetResources().AsEnumerable();
                if (querystring.ContainsKey("id"))
                {
                    var id = int.Parse((string)querystring["id"]);
                    filter = filter.Where(r => r.Id == id);
                }
                if (querystring.ContainsKey("name"))
                {
                    var name = ((string)querystring["name"]).ToLower();
                    filter = filter.Where(r => r.Name.ToLowerInvariant() == name);
                }
                if (querystring.ContainsKey("locationId"))
                {
                    var locationId = int.Parse((string)querystring["locationId"]);
                    filter = filter.Where(r => (r.Location != null) && (r.Location.Id == locationId));
                }
                if (querystring.ContainsKey("mapId"))
                {
                    var mapId = int.Parse((string)querystring["mapId"]);
                    filter = filter.Where(r => (r.Location != null) && (r.Location.Map != null) && (r.Location.Map.Id == mapId));
                }
                var resources = filter.ToList();
                return Response.AsJson(resources);
            };

            Get["/nearLocation/{locationId}"] = parameters =>
            {
                var querystring = (DynamicDictionary)Request.Query;
                var locationId = int.Parse((string)parameters.locationId);
                var searchRadius = int.Parse((string)querystring["searchRadius"]);
                var locations = this.DbContext.GetLocations();
                var location = locations.Single(l => l.Id == locationId);
                var nearbyLocations = locations.Where(l => (l.Id != locationId) && (l.GetDistanceFrom(location) <= searchRadius)).ToList();
                var nearbyResources = this.DbContext.GetResources().Where(r => nearbyLocations.Contains(r.Location)).ToList();
                return Response.AsJson(nearbyResources);
            };

            Get["/nearResource/{resourceId}"] = parameters =>
            {
                var querystring = (DynamicDictionary)Request.Query;
                var resourceId = int.Parse((string)parameters.resourceId);
                var searchRadius = int.Parse((string)querystring["searchRadius"]);
                var resources = this.DbContext.GetResources();
                var resource = resources.Single(r => r.Id == resourceId);
                var nearbyLocations = this.DbContext.GetLocations().Where(l => (l.Id != resource.Location.Id) && (l.GetDistanceFrom(resource.Location) <= searchRadius)).ToList();
                var nearbyResources = this.DbContext.GetResources().Where(r => nearbyLocations.Contains(r.Location)).ToList();
                return Response.AsJson(nearbyResources);
            };

            #endregion

            #region Action Routes

            Post["/add"] = parameters =>
            {
                var name = (string)this.Request.Query["name"];
                //var locationId = int.Parse((string)this.Request.Query["locationId"]);
                //var location = (new LocationService(this.Repository)).GetLocationById(locationId);
                var resource = this.DbContext.AddResource(name);
                return Response.AsJson(resource);
            };

            Post["{resourceId}/moveTo/{locationId}"] = parameters =>
            {
                var oldResourceId = int.Parse((string)parameters.resourceId);
                var newLocationId = int.Parse((string)parameters.locationId);
                lock (this.DbContext.LockObject)
                {
                    var resources = this.DbContext.GetResources().ToList();
                    var oldResource = resources.Single(r => r.Id == oldResourceId);
                    var newLocation = this.DbContext.GetLocations().Single(l => l.Id == newLocationId);
                    this.DbContext.RemoveResource(oldResource.Id);
                    var newResource = this.DbContext.AddResource(oldResource.Id, oldResource.Name, newLocation);
                    return Response.AsJson(newResource);
                }
            };

            #endregion

        }

        #endregion

    }

}
