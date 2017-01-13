using Nancy;
using System.Linq;

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
                var resources = this.Repository.GetResources();
                return Response.AsJson(resources);
            };

            Get["/search"] = parameters =>
            {
                var querystring = (DynamicDictionary)Request.Query;
                var filter = this.Repository.GetResources().AsEnumerable();
                if (querystring.ContainsKey("id"))
                {
                    var id = int.Parse((string)querystring["id"]);
                    filter = filter.Where(r => r.Id == id);
                }
                if (querystring.ContainsKey("name"))
                {
                    var name = (string)querystring["name"];
                    filter = filter.Where(r => r.Name == name);
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
                var locations = filter.ToList();
                return Response.AsJson(locations);
            };

            Get["/nearLocation/{locationId}"] = parameters =>
            {
                var querystring = (DynamicDictionary)Request.Query;
                var locationId = int.Parse((string)parameters.locationId);
                var searchRadius = int.Parse((string)querystring["searchRadius"]);
                var locations = this.Repository.GetLocations();
                var location = locations.Single(l => l.Id == locationId);
                var nearbyLocations = locations.Where(l => (l.Id != locationId) && (l.GetDistanceFrom(location) <= searchRadius)).ToList();
                var nearbyResources = this.Repository.GetResources().Where(r => nearbyLocations.Contains(r.Location)).ToList();
                return Response.AsJson(nearbyResources);
            };

            Get["/nearResource/{resourceId}"] = parameters =>
            {
                var querystring = (DynamicDictionary)Request.Query;
                var resourceId = int.Parse((string)parameters.resourceId);
                var searchRadius = int.Parse((string)querystring["searchRadius"]);
                var resources = this.Repository.GetResources();
                var resource = resources.Single(r => r.Id == resourceId);
                var nearbyLocations = this.Repository.GetLocations().Where(l => (l.Id != resource.Location.Id) && (l.GetDistanceFrom(resource.Location) <= searchRadius)).ToList();
                var nearbyResources = this.Repository.GetResources().Where(r => nearbyLocations.Contains(r.Location)).ToList();
                return Response.AsJson(nearbyResources);
            };

            #endregion

            #region Action Routes

            Post["/add"] = parameters =>
            {
                var name = (string)this.Request.Query["name"];
                //var locationId = int.Parse((string)this.Request.Query["locationId"]);
                //var location = (new LocationService(this.Repository)).GetLocationById(locationId);
                var resource = this.Repository.AddResource(name);
                return Response.AsJson(resource);
            };

            Post["/move/{resourceId}/to/{locationId}"] = parameters =>
            {
                var oldResourceId = int.Parse((string)parameters.resourceId);
                var newLocationId = int.Parse((string)parameters.locationId);
                lock (this.Repository.LockObject)
                {
                    var resources = this.Repository.GetResources().ToList();
                    var oldResource = resources.Single(r => r.Id == oldResourceId);
                    var newLocation = this.Repository.GetLocations().Single(l => l.Id == newLocationId);
                    this.Repository.RemoveResource(oldResource.Id);
                    var newResource = this.Repository.AddResource(oldResource.Id, oldResource.Name, newLocation);
                    return Response.AsJson(newResource);
                }
            };

            #endregion

        }

        #endregion

    }

}
