using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class WhSJazzTransactionTypeCodeInfoFilter
    {
        public IEnumerable<IWhSJazzTransactionTypeCodeFilter> Filters { get; set; }

    }
    public interface IWhSJazzTransactionTypeCodeFilter
    {
        bool IsMatch(IWhSJazzTransactionTypeCodeFilterContext context);
    }

    public interface IWhSJazzTransactionTypeCodeFilterContext
    {
        WhSJazzTransactionTypeCode TransactionTypeCode { get; }
    }

    public class WhSJazzTransactionTypeCodeFilterContext : IWhSJazzTransactionTypeCodeFilterContext
    {
        public WhSJazzTransactionTypeCode TransactionTypeCode { get; set; }
    }
}