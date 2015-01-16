using System;
using System.Collections.Generic;

namespace TABS.Extensibility
{
    /// <summary>
    /// The Signature for a CDR Store. (Filesystem, database table, etc.)    
    /// </summary>
    public interface ICDRStore
    {
        /// <summary>
        /// The name of the CDR Store. Should be a short identifier
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// User-Filled Description of the CDR store and what/how it stores CDRs.
        /// </summary>
        string Description { get; set; }
        
        /// <summary>
        /// A Helper text (HTML format) for usage and configuration of the CDR Store.
        /// </summary>
        string HelpHtml { get; }
        
        /// <summary>
        /// User-Filled Configuration String for the CDR Store.
        /// </summary>
        string ConfigString { get; set; }

        /// <summary>
        /// User-Filled Configuration Options for the CDR Store.
        /// </summary>        
        string ConfigOptions { get; set; }

        /// <summary>
        /// A flag to enable-disable this CDR Store
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Put the given CDRs in the underlying store
        /// </summary>
        /// <param name="session">A helper session</param>
        /// <param name="cdrs">The cdrs to put</param>
        void Put(NHibernate.ISession session, IEnumerable<CDR> cdrs);
        
        /// <summary>
        /// Get an enumerable collection of CDRs from the underlying store in the specified period (from/till)
        /// </summary>
        /// <param name="session">A helper Nhibernate session (TABS)</param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <returns></returns>
        IEnumerable<CDR> Get(NHibernate.ISession session, DateTime from, DateTime till);
    }
}