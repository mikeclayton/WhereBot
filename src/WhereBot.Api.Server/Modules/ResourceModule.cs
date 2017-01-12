using Nancy;
using WhereBot.Api.Server.Services;

namespace WhereBot.Api.Server.Modules
{

    public class ResourceModule : NancyModule
    {

        public ResourceModule() : base("/resources")
        {

            Get["/all"] = parameters =>
            {
                var resources = this.Repository.GetResources();
                return Response.AsJson(resources);
            };

            Post["/add"] = parameters =>
            {
                var name = (string)this.Request.Query["name"];
                //var locationId = int.Parse((string)this.Request.Query["locationId"]);
                //var location = (new LocationService(this.Repository)).GetLocationById(locationId);
                var resource = this.Repository.AddResource(name);
                return Response.AsJson(resource);
            };

            Post["/{resourceId}/moveTo/{locationId}"] = parameters =>
            {
                var resourceService = new ResourceService(this.Repository);
                var oldResourceId = int.Parse((string)parameters.resourceId);
                var newLocationId = int.Parse((string)parameters.locationId);
                var newResource = resourceService.MoveResource(oldResourceId, newLocationId);
                return Response.AsJson(newResource);
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
