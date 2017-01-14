using System.Linq;

namespace WhereBot.Api.Data.Storage
{

    internal sealed class Resource
    {

        #region Builder

        public sealed class Builder
        {

            public int Id
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            public int? LocationId
            {
                get;
                set;
            }

            public Resource Build()
            {
                return new Resource
                {
                    Id = this.Id,
                    Name = this.Name,
                    LocationId = this.LocationId
                };
            }

        }

        #endregion

        #region Constructors

        public Resource()
        {
        }

        #endregion

        #region Properties

        public int Id
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public int? LocationId
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public static Resource FromModel(Domain.Resource resource)
        {
            return new Resource.Builder
            {
                Id = resource.Id,
                Name = resource.Name,
                LocationId = (resource.Location == null) ? new int?() : resource.Location.Id
            }.Build();
        }

        public static Domain.Resource ToModel(Resource resource, DbContext dbContext)
        {
            var locations = dbContext.GetLocations();
            var location = resource.LocationId.HasValue ? locations.Single(l => l.Id == resource.LocationId.Value) : null;
            return new Domain.Resource.Builder
            {
                Id = resource.Id,
                Name = resource.Name,
                Location = location
            }.Build();
        }

        public Domain.Resource ToModel(DbContext dbContext)
        {
            return Resource.ToModel(this, dbContext);
        }

        #endregion

    }

}
