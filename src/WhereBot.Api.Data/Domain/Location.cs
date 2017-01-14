using System;

namespace WhereBot.Api.Data.Domain
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

            public string Name
            {
                get;
                set;
            }

            public Map Map
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
                    Map = this.Map,
                    X = this.X,
                    Y = this.Y,
                };
            }

        }

        public object ToList()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Constructors

        private Location()
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

        public Map Map
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

        public double GetDistanceFrom(Location location)
        {
            // we can't measure the distance if they're on different maps
            if(this.Map != location.Map)
            {
                throw new InvalidOperationException();
            }
            return Math.Sqrt(Math.Pow(this.X - location.X, 2) + Math.Pow(this.Y - location.Y, 2));
        }

        #endregion

        #region Object Interface

        public override string ToString()
        {
            var map = (this.Map == null) ? "null" : string.Format("\"{0}\"", this.Map.Name);
            return string.Format("Id={0},Name=\"{1}\",Map={2}", this.Id, this.Name, map);
        }

        #endregion

    }

}
