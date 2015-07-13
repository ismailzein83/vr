using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.BaseDB
{
    public abstract class DBReceiveAdapter : BaseReceiveAdapter
    {
        #region Properties

            public abstract string ConnectionString { get; set; }

            public abstract string Description { get; set; }

            public abstract string Query { get; set; }


        # endregion
    }
}
