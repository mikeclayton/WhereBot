using Nancy;

namespace WhereBot.Api.Server.Modules
{

    public class LocationModule : NancyModule
    {

        public LocationModule() : base("/locations")
        {

            Get["/all"] = parameters =>
            {
                var locations = this.Repository.GetLocations();
                return Response.AsJson(locations);
            };

            Post["/add"] = parameters =>
            {
                var floor = int.Parse((string)this.Request.Query["floor"]);
                var name = (string)this.Request.Query["name"];
                var x = int.Parse((string)this.Request.Query["x"]);
                var y = int.Parse((string)this.Request.Query["y"]);
                var location = this.Repository.AddLocation(floor, name, x, y);
                return Response.AsJson(location);
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
