using System;
using System.Collections.Generic;
using System.Linq;
using WhereBot.Api.Models;

namespace WhereBot.Api.Server.Services
{

    public sealed class ResourceService
    {

        #region Constructors

        public ResourceService(DataSet repository)
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

        public Resource GetResourceById(int id)
        {
            return this.Repository.GetResources().SingleOrDefault(r => r.Id == id);
        }

        public IEnumerable<Resource> GetResourcesByName(string name)
        {
            var resources = this.Repository.GetResources();
            var results = resources.Where(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
            return results;
        }

        public IEnumerable<Resource> GetResourcesByLocationId(int locationId)
        {
            return this.Repository.GetResources().Where(r => (r.Location != null) && (r.Location.Id == locationId));
        }

        public IEnumerable<Resource> GetResourcesByLocation(Location location)
        {
            return this.Repository.GetResources().Where(r => r.Location == location);
        }

        public IEnumerable<Resource> GetResourcesNearLocationId(int locationId, int searchRadius)
        {
            var locations = this.Repository.GetLocations().ToList();
            var location = locations.Single(l => l.Id == locationId);
            var nearby = locations.Where(l => (l.Id != locationId) && (l.GetDistanceFrom(location) <= searchRadius));
            var resources = this.Repository.GetResources().Where(r => nearby.Contains(r.Location)).ToList();
            return resources;
        }

        public Resource MoveResource(int resourceId, int newLocationId)
        {
            lock (this.Repository.LockObject)
            {
                var resources = this.Repository.GetResources().ToList();
                var oldResource = resources.Single(r => r.Id == resourceId);
                var newLocation = this.Repository.GetLocations().Single(l => l.Id == newLocationId);
                this.Repository.RemoveResource(oldResource.Id);
                var newResource = this.Repository.AddResource(oldResource.Id, oldResource.Name, newLocation);
                return newResource;
            }
        }

        #endregion

    }

}
