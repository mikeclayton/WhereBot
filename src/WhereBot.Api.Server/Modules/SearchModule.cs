using Nancy;

namespace WhereBot.Api.Server.Modules
{

    public class SearchModule : NancyModule
    {

        public SearchModule() : base("/search")
        {


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
