using System;

namespace WhereBot.Api.Models
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

        public int Floor
        {
            get;
            private set;
        }

        public string Name
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
            return Math.Sqrt(Math.Pow(this.X - location.X, 2) + Math.Pow(this.Y - location.Y, 2));
        }

        #endregion

    }

}
