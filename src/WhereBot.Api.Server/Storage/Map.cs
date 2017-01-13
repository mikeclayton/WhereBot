namespace WhereBot.Api.Server.Storage
{

    public sealed class Map
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

            public string Description
            {
                get;
                set;
            }

            public string Filename
            {
                get;
                set;
            }

            public Map Build()
            {
                return new Map
                {
                    Id = this.Id,
                    Name = this.Name,
                    Description = this.Description,
                    Filename = this.Filename
                };
            }

        }

        #endregion

        #region Constructors

        private Map()
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

        public string Description
        {
            get;
            private set;
        }

        public string Filename
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public static Map FromModel(Models.Map map)
        {
            return new Map.Builder
            {
                Id = map.Id,
                Name = map.Name,
                Filename = map.Filename
            }.Build();
        }

        public static Models.Map ToModel(Map map)
        {
            return new Models.Map.Builder
            {
                Id = map.Id,
                Name = map.Name,
                Filename = map.Filename
            }.Build();
        }

        public Models.Map ToModel()
        {
            return Map.ToModel(this);
        }

        #endregion

    }

}
