using System.Linq;

namespace WhereBot.Api.Data.Storage
{

    internal sealed class Location
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

            public int? MapId
            {
                get;
                set;
            }

            public int X
            {
                get;
                set;
            }

            public int Y
            {
                get;
                set;
            }

            public Location Build()
            {
                return new Location
                {
                    Id = this.Id,
                    Name = this.Name,
                    MapId = this.MapId,
                    X = this.X,
                    Y = this.Y,
                };
            }

        }

        #endregion

        #region Constructors

        public Location()
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

        public int? MapId
        {
            get;
            private set;
        }

        public int X
        {
            get;
            private set;
        }

        public int Y
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public static Location FromModel(Domain.Location location)
        {
            return new Location.Builder
            {
                Id = location.Id,
                Name = location.Name,
                MapId = (location.Map == null) ? new int?() : location.Map.Id,
                X = location.X,
                Y = location.Y
            }.Build();
        }

        public static Domain.Location ToModel(Location location, DbContext dbContext)
        {
            var maps = dbContext.GetMaps();
            var map = location.MapId.HasValue ? maps.Single(m => m.Id == location.MapId.Value) : null;
            return new Domain.Location.Builder
            {
                Id = location.Id,
                Name = location.Name,
                Map = map,
                X = location.X,
                Y = location.Y
            }.Build();
        }

        public Domain.Location ToModel(DbContext dbContext)
        {
            return Location.ToModel(this, dbContext);
        }

        #endregion

    }

}
