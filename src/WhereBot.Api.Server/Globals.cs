using WhereBot.Api.Data;

namespace WhereBot.Api.Server
{

    public static class Globals
    {

        private static readonly DbContext dbContext= new DbContext();

        public static DbContext DbContext
        {
            get
            {
                return Globals.dbContext;
            }
        }

    }

}
