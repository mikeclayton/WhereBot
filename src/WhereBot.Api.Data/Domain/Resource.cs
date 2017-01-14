namespace WhereBot.Api.Data.Domain
{

    public sealed class Resource
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

            public Location Location
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
                    Location = this.Location
                };
            }

        }

        #endregion

        #region Constructors

        private Resource()
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

        public Location Location
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public Resource MoveTo(Location newLocation)
        {
            var newResource = (Resource)this.MemberwiseClone();
            newResource.Location = newLocation;
            return newResource;
        }

        #endregion

        #region Object Interface

        public override string ToString()
        {
            var location = (this.Location == null) ? "null" : string.Format("\"{0}\"", this.Location.Name);
            return string.Format("Id={0},Name=\"{1}\",Location={2}", this.Id, this.Name, location);
        }

        #endregion

    }

}
