using System;
using System.Collections.Generic;

namespace TABS.Extensibility
{
    /// <summary>
    /// Defines A Switch Manager Interface. Does the following:
    /// - A switch manager insures the proper communication of Routing Information to the switch.
    /// - A switch manager gathers Cooked CDR information from the plugged in switches
    /// </summary>
    public interface ISwitchManager
    {
        /// <summary>
        /// The type of the switch that this Manager is Compatible with.
        /// The manager should look for "Switch" objects with their type including this compatibility identifier.
        /// </summary>
        string[] CompatibleSwitches { get; }
        string Version { get; }
        string CurrentActivity { get; }
        string LastActivity { get; }
        Exception LastException { get; }

        /// <summary>
        /// Updates the Routing Tables on the given switch
        /// </summary>
        /// <param name="compatibleSwitch">The switch to Update</param>
        /// <returns>True on success of the operation</returns>
        bool SynchRouting(TABS.Switch compatibleSwitch, RouteSynchType synchType);
        

        /// <summary>
        /// Gets the current state of the proxy
        /// </summary>
        /// <param name="compatibleSwitch"></param>
        /// <returns>array of 2 strings where the first is the current state, and the second is the latest error if available</returns>
        List<TABS.Addons.Utilities.ProxyCommon.ProxyState> GetProxyStates(TABS.Switch compatibleSwitch);

        IEnumerable<TABS.Addons.Utilities.Extensibility.CDR> GetCDR(Switch sourceSwitch);

        /// <summary>
        /// Fill the Billing CDR information from the given standard CDR. Should return true only when the CDR
        /// is considered valid for Billing.
        /// </summary>
        /// <param name="sourceSwitch">The source switch for the CDR</param>
        /// <param name="standardCDR">The CDR to get billing info for</param>
        /// <param name="baseCDR">The Billing CDR record</param>
        /// <returns>True when the billingCDR can be considered for billing (not invalid)</returns>
        bool FillCDRInfo(Switch sourceSwitch, TABS.Addons.Utilities.Extensibility.CDR standardCDR, Billing_CDR_Base billingCDR);

        SwitchConfigurationBase GetSwitchConfiguration(Switch ownerSwitch, string XML);

        TABS.Addons.WebControls.SwitchConfigurationControl GetSwitchConfigurationWebControl();

        /// <summary>
        /// Update the Switch's last CDR Tag from the last actual CDR
        /// </summary>
        /// <param name="theSwitch">The switch to update</param>
        /// <param name="lastCDR">The Last CDR</param>
        void UpdateLastCDRTag(Switch theSwitch, CDR lastCDR);

        /// <summary>
        /// Update the specified carrier account routing status
        /// </summary>
        /// <param name="account">The carrier account to update</param>
        /// <returns>True if operation is successful, false if not (or if not directly supported)</returns>
        bool UpdateAccountRoutingStatus(TABS.Switch compatibleSwitch, CarrierAccount account);

        /// <summary>
        /// Return a list of possible identifiers for a given Customer. 
        /// This is mainly used to select raw CDRs for this particular customer.
        /// </summary>
        /// <param name="customer">The customer in question</param>
        /// <returns>A List of identifiers (strings)</returns>
        List<string> GetCustomerCdrIdentifiers(TABS.Switch compatibleSwitch, CarrierAccount customer);

        /// <summary>
        /// Return a list of possible identifiers for a given Supplier. 
        /// This is mainly used to select raw CDRs for this particular supplier.
        /// </summary>
        /// <param name="customer">The supplier in question</param>
        /// <returns>A List of identifiers (strings)</returns>
        List<string> GetSupplierCdrIdentifiers(TABS.Switch compatibleSwitch, CarrierAccount supplier);
    }

    public interface ISwitchManagerExtended
    {

        /// <summary>
        /// Return a list of possible identifiers for a given Supplier. 
        /// This is mainly used to select raw CDRs for this particular supplier.
        /// </summary>
        /// <param name="customer">The supplier in question</param>
        /// <returns>A List of identifiers (strings)</returns>
        List<string> GetSupplierCdrIdentifiersRepricing(TABS.Switch compatibleSwitch, CarrierAccount supplier);


        /// <summary>
        /// Return a list of possible identifiers for a given Customer. 
        /// This is mainly used to select raw CDRs for this particular customer.
        /// </summary>
        /// <param name="customer">The customer in question</param>
        /// <returns>A List of identifiers (strings)</returns>
        List<string> GetCustomerCdrIdentifiersRepricing(TABS.Switch compatibleSwitch, CarrierAccount customer);
    }
}