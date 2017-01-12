using Nancy;
using System.Linq;
using WhereBot.Api.Server.Services;

namespace WhereBot.Api.Server.Modules
{

    public class SearchModule : NancyModule
    {

        public SearchModule() : base("/search")
        {

            Get["/locations/byLocationName/{locationName}"] = parameters =>
            {
                var locationName = (string)parameters.locationName;
                var locations = new LocationService(this.Repository).GetLocationsByName(locationName).ToList();
                return Response.AsJson(locations);
            };

            Get["/resources/byResourceId/{resourceId}"] = parameters =>
            {
                var resourceId = int.Parse((string)parameters.resourceId);
                var resources = new ResourceService(this.Repository).GetResourceById(resourceId);
                return Response.AsJson(resources);
            };

            Get["/resources/byResourceName/{resourceName}"] = parameters =>
            {
                var resourceName = (string)parameters.resourceName;
                var resources = new ResourceService(this.Repository).GetResourcesByName(resourceName).ToList();
                return Response.AsJson(resources);
            };

            Get["/resources/byLocationId/{locationId}"] = parameters =>
            {
                var locationId = int.Parse((string)parameters.locationId);
                var resources = new ResourceService(this.Repository).GetResourcesByLocationId(locationId).ToList();
                return Response.AsJson(resources);
            };

            Get["/resources/nearLocation/{locationId}"] = parameters =>
            {
                var locationId = int.Parse((string)parameters.locationId);
                var searchRadius = int.Parse((string)parameters.searchRadius);
                var resources = new ResourceService(this.Repository).GetResourcesNearLocationId(locationId, searchRadius).ToList();
                return Response.AsJson(resources);
            };

        }

        #region Properties

        private DataSet Repository
        {
            get
            {
                return Globals.Repository;
            }
        }

        #endregion

    }

}
