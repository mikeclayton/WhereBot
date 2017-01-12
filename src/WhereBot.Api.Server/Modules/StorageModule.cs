using Nancy;
using WhereBot.Api.Server.Services;

namespace WhereBot.Api.Server.Modules
{

    public class StorageModule : NancyModule
    {

        public StorageModule() : base("/storage")
        {

            Post["/clear"] = parameters =>
            {
                new StorageService(this.Repository).Clear();
                return null;
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
