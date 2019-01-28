using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class TransactionTypeInfoFilter
    {
        public IEnumerable<ITransactionTypeFilter> Filters { get; set; }

    }
    public interface ITransactionTypeFilter
    {
        bool IsMatch(ITransactionTypeFilterContext context);
    }

    public interface ITransactionTypeFilterContext
    {
        TransactionType TransactionType { get; }
    }

    public class TransactionTypeFilterContext : ITransactionTypeFilterContext
    {
        public TransactionType TransactionType { get; set; }
    }
}