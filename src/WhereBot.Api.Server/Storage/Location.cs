namespace WhereBot.Api.Server.Storage
{

    public sealed class Location
    {

        #region Builder

        public sealed class Builder
        {

            public int Id
            {
                get;
                set;
            }

            public int Floor
            {
                get;
                set;
            }

            public string Name
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
                    Floor = this.Floor,
                    Name = this.Name,
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
            set;
        }

        public int Floor
        {
            get;
            set;
        }

        public string Name
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

        #endregion

        #region Methods

        public static Location FromModel(Models.Location location)
        {
            return new Location.Builder
            {
                Id = location.Id,
                Floor = location.Floor,
                Name = location.Name,
                X = location.X,
                Y = location.Y
            }.Build();
        }

        public static Models.Location ToModel(Location location)
        {
            return new Models.Location.Builder
            {
                Id = location.Id,
                Floor = location.Floor,
                Name = location.Name,
                X = location.X,
                Y = location.Y
            }.Build();
        }

        public Models.Location ToModel()
        {
            return Location.ToModel(this);
        }

        #endregion

    }

}
