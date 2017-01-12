namespace WhereBot.Api.Server
{

    public static class Globals
    {

        private static readonly DataSet repository = new DataSet();

        public static DataSet Repository
        {
            get
            {
                return Globals.repository;
            }
        }

    }

}
