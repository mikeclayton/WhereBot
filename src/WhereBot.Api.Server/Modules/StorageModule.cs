using Nancy;

namespace WhereBot.Api.Server.Modules
{

    public class StorageModule : NancyModule
    {

        public StorageModule() : base("/storage")
        {

            Post["/clear"] = parameters =>
            {
                this.Repository.ClearStorage();
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
