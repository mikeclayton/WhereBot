using System;
using System.Collections.Generic;
using System.Linq;
using WhereBot.Api.Models;

namespace WhereBot.Api.Server.Services
{

    public sealed class LocationService
    {

        #region Constructors

        public LocationService(DataSet repository)
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

        public Location GetLocationById(int id)
        {
            return this.Repository.GetLocations().SingleOrDefault(l => l.Id == id);
        }

        public IEnumerable<Location> GetLocationsByName(string name)
        {
            return this.Repository.GetLocations().Where(l => string.Equals(l.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

    }

}
