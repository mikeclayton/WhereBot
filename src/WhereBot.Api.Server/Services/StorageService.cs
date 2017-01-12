namespace WhereBot.Api.Server.Services
{

    public sealed class StorageService
    {

        #region Constructors

        public StorageService(DataSet repository)
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

        public void Clear()
        {
            this.Repository.ClearStorage();
        }

        #endregion

    }

}
