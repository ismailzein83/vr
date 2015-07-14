using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.BaseDB
{
    public abstract class DBReceiveAdapter : BaseReceiveAdapter
    {
        #region Properties

            public string ConnectionString { get; set; }

            public string Description { get; set; }

            public string Query { get; set; }


        # endregion
    }
}
