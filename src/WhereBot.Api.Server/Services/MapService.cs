using System.Linq;
using WhereBot.Api.Models;

namespace WhereBot.Api.Server.Services
{

    public sealed class MapService
    {

        #region Constructors

        public MapService(DataSet repository)
        {
            this.Repository = repository;
        }

        #endregion

        #region Properties

        private DataSet Repository
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public Map GetMapById(int id)
        {
            return this.Repository.GetMaps().SingleOrDefault(m => m.Id == id);
        }

        #endregion

    }

}
