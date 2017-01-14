using Nancy;
using WhereBot.Api.Data;

namespace WhereBot.Api.Server.Modules
{

    public class StorageModule : NancyModule
    {

        public StorageModule() : base("/storage")
        {
            this.InitRoutes();
        }

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

            Post["/clear"] = parameters =>
            {
                this.DbContext.ClearStorage();
                return null;
            };

        }

        #endregion

    }

    }
