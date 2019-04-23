using System;
using System.Collections.Generic;

namespace Retail.BusinessEntity.Entities
{
    public interface IAccountPackageHandler
    {
        AccountPackageProvider GetAccountPackageProvider(IGetAccountPackageProviderContext context);
    }

    public interface IGetAccountPackageProviderContext
    {
    }

    public class GetAccountPackageProviderContext : IGetAccountPackageProviderContext
    {
    }
}