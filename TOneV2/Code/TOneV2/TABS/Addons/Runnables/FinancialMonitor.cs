using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using TABS.SpecialSystemParameters;


namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Financial Monitor", "Monitors and updates Prepaid and Postpaid Customer/Supplier Accounts then takes appropriate actions.")]
    public class FinancialMonitor : RunnableBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(FinancialMonitor));

        protected decimal _tolerance { get; set; }
        protected PrepaidPostpaidOptions _prepaidPostpaidOptions { get; set; }

        public enum CarrierType { Customer, Supplier, Netting };

        protected IList<TABS.PrepaidPostpaidOptions> options(string id, bool isProfile, bool isCustomer, bool isPrepaid)
        {
            string typeCarrier = isProfile
                                  ? (isCustomer ? " CustomerProfileID " : " SupplierProfileID ")
                                  : (isCustomer ? " CustomerID " : " SupplierID ");

            string hql = string.Format(@"SELECT ppo 
                                         FROM PrepaidPostpaidOptions ppo 
                                         WHERE 
                                            {0} = :ID
                                            AND IsPrepaid = :isPrepaid
                                         ORDER BY {1} desc"
                                      , typeCarrier
                                      , isPrepaid ? "Amount" : "Percentage");

            IList<TABS.PrepaidPostpaidOptions> Options = TABS.ObjectAssembler.CurrentSession.CreateQuery(hql)
                .SetParameter("ID", id)
                .SetParameter("isPrepaid", isPrepaid ? true : false)
                .List<TABS.PrepaidPostpaidOptions>();
            return (Options != null && Options.Count > 0) ? Options : null;
        }

        public void PrepaidPostPaidAction(PaymentType paymentType, CarrierType carrierType)
        {
            try
            {
                log.Info(string.Format("Starting PrepaidPostPaidAction of payment type ({0}) and carrier type ({1})", paymentType.ToString(), carrierType.ToString()));
                bool isNetting = "Netting".Equals(carrierType.ToString());
                bool isCustomer = (carrierType == CarrierType.Customer);
                bool isPrepaid = (paymentType == PaymentType.Prepaid);
                DataTable TmpData;
                string tmpSql;
                string email = "";
                if (!isNetting)
                {
                    tmpSql = string.Concat("EXEC bp_", paymentType.ToString(), "CarrierTotal @ShowCustomerTotal=@P1, @ShowSupplierTotal=@P2");

                    TmpData = TABS.DataHelper.GetDataTable(tmpSql,
                        carrierType == CarrierType.Customer ? "Y" : "N",
                        carrierType == CarrierType.Customer ? "N" : "Y");
                }
                else
                {
                    tmpSql = @"EXEC bp_PostpaidCarrierTotal @ShowCustomerTotal=@P1, @ShowSupplierTotal=@P2, @IsNettingEnabled=@P3";
                    TmpData = TABS.DataHelper.GetDataTable(tmpSql, "Y", "Y", "Y");
                }

                TABS.CarrierAccount ca = null;
                TABS.CarrierProfile cp = null;
                bool isProfile = false;

                foreach (DataRow row in TmpData.Rows)
                {
                    string id = "";
                    int creditLimit = 0;
                    EventActions action = EventActions.None;

                    IList<TABS.PrepaidPostpaidOptions> ppos;
                    if (row["CarrierID"] != DBNull.Value)
                    {
                        isProfile = false;
                        id = row["CarrierID"].ToString();
                        try { ca = TABS.CarrierAccount.All[id]; }
                        catch { continue; }
                        creditLimit = (!isPrepaid)
                                                ? (!isNetting)
                                                    ? (isCustomer) ? ca.CustomerCreditLimit : ca.SupplierCreditLimit
                                                    : ((decimal.Parse(row["Balance"].ToString()) > 0) ? ca.SupplierCreditLimit : ca.CustomerCreditLimit)
                                              : 0;
                    }
                    else
                    {
                        isProfile = true;
                        id = row["ProfileID"].ToString();
                        try { cp = TABS.CarrierProfile.All[int.Parse(id)]; }
                        catch { continue; }
                        creditLimit = (!isPrepaid)
                                            ? (!isNetting)
                                                ? (isCustomer) ? cp.CustomerCreditLimit : cp.SupplierCreditLimit
                                                : ((decimal.Parse(row["Balance"].ToString()) > 0) ? cp.SupplierCreditLimit : cp.CustomerCreditLimit)
                                            : 0;
                    }

                    ppos = (!isNetting)
                                    ? options(id, isProfile, isCustomer, isPrepaid)
                                     : options(id, isProfile, (decimal.Parse(row["Balance"].ToString()) > 0) ? false : true, isPrepaid);

                    _tolerance = (row["Tolerance"] != DBNull.Value) ? decimal.Parse(row["Tolerance"].ToString()) : 0m;

                    if (ppos != null && ppos.Count > 0)
                    {
                        _prepaidPostpaidOptions = (!isNetting)
                                                    ? ((isPrepaid)
                                                        ? ppos.Where(p => p.Amount >= _tolerance).Min()
                                                        : ppos.Where(p => (p.Percentage * creditLimit / 100) >= _tolerance).Min())
                                                    : (ppos.Where(p => (p.Percentage * creditLimit / 100) >= _tolerance).Min());
                        if (_prepaidPostpaidOptions != null)
                        {
                            _prepaidPostpaidOptions.NetTolerance = Math.Abs(_tolerance);
                            _prepaidPostpaidOptions.Tolerance = _tolerance - ((isPrepaid) ? ppos.Last().Amount : (creditLimit * ppos.Last().Percentage / 100));
                            _prepaidPostpaidOptions.Balance = (row["Balance"] != DBNull.Value) ? decimal.Parse(row["Balance"].ToString()) : 0m;
                            action = _prepaidPostpaidOptions.Actions;
                            email = _prepaidPostpaidOptions.Email;
                        }
                        bool hideInactive = TABS.SystemParameter.HidePrepaidPostpaidInactiveCarrierAccounts.BooleanValue.Value;
                        if (((hideInactive) ? ppos[0].isActiveCarrier : true)
                            && action != TABS.EventActions.None
                            && (!isCustomer || !(!isProfile ?
                            _prepaidPostpaidOptions.Customer.RoutingStatus == RoutingStatus.Blocked || _prepaidPostpaidOptions.Customer.RoutingStatus == RoutingStatus.BlockedInbound
                            : _prepaidPostpaidOptions.CustomerProfile.Accounts.All(c => c.RoutingStatus == RoutingStatus.Blocked || c.RoutingStatus == RoutingStatus.BlockedInbound))))
                            TakeAction(id, isProfile, carrierType, isPrepaid, action, creditLimit, email);
                    }
                }
                log.Info(string.Format("PrepaidPostPaidAction of payment type ({0}) and carrier type ({1}) Finished.", paymentType.ToString(), carrierType.ToString()));
            }
            catch (Exception ex)
            {
                log.Error(string.Format("PrepaidPostPaidAction exception : {0}", ex.ToString()));
            }
        }

        protected void TakeAction(string id, bool isProfile, CarrierType carrierType, bool isPrepaid, EventActions action, int creditLimit, string email)
        {
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                string name;
                CarrierProfile cf = new CarrierProfile();
                CarrierAccount ca = new CarrierAccount();
                if (isProfile)
                {
                    cf = TABS.CarrierProfile.All[int.Parse(id)];
                    name = cf.Name;
                }
                else
                {
                    ca = TABS.CarrierAccount.All[id];
                    name = ca.CarrierProfile.Name;
                }
                string tag = string.Format("{0}({1}) - {2}, {3}{4}, {5} "
                                                       , name
                                                       , id
                                                       , (isProfile) ? "Profile" : "Account"
                                                       , carrierType
                                                       , (carrierType == CarrierType.Netting) ? ((_prepaidPostpaidOptions.Balance > 0) ? " Supplier" : " Customer") : ""
                                                       , (isPrepaid) ? "Prepaid" : "Postpaid");
                string description = string.Format("Action(s): {0} - Currency: {1} {2} - {3} - Balance: {4} - Tolerance: {5}",
                                                            action,
                                                            (isProfile) ? cf.Currency.Symbol : ca.CarrierProfile.Currency.Symbol,
                                                            (!isPrepaid) ? string.Concat("- Credit Limit:", string.Format("{0:#,#0}", creditLimit)) : "",
                                                            (isPrepaid) ? ("Threshold: " + string.Format("{0:#,#0.##}", _prepaidPostpaidOptions.Amount)) : ("Threshold: " + string.Format("{0:#,#0.##}", _prepaidPostpaidOptions.Percentage * creditLimit / 100)),
                                                            string.Format("{0:#,#0.##}", _prepaidPostpaidOptions.Balance),
                                                            string.Format("{0:#,#0.##}", _prepaidPostpaidOptions.Tolerance));
                string source = "Financial Monitor";
                if ((action & EventActions.Alert) == EventActions.Alert)
                {
                    try
                    {
                        Alert alert = new Alert();
                        alert.Created = DateTime.Now;
                        alert.IsVisible = true;
                        alert.Level = AlertLevel.Medium;
                        alert.Progress = AlertProgress.None;
                        alert.Source = source;
                        alert.Tag = string.Concat(tag, " - Alert Notification");
                        alert.Description = description;
                        session.Save(alert);
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Error Generating alert for {0}", action), ex);
                    }
                }
                // Email? 
                if ((action & EventActions.Email) == EventActions.Email)
                {
                    try
                    {
                        // Check to see if an email notification was sent since a min time (MinimumActionEmailInterval)
                        IList<Alert> lastAlerts = session.CreateQuery("FROM Alert WHERE Tag=:Tag AND Source=:Source AND Created > :MinDate")
                                .SetParameter("Tag", string.Concat(tag, " - Email Notification"))
                                .SetParameter("Source", source)
                                .SetParameter("MinDate", DateTime.Now.Add(_prepaidPostpaidOptions.MinimumActionEmailInterval.HasValue
                                                                                                    ? _prepaidPostpaidOptions.MinimumActionEmailInterval.Value.Negate()
                                                                                                    : TABS.SystemParameter.MinimumActionEmailInterval.TimeSpanValue.Value.Negate()))
                                .SetMaxResults(1)
                                .List<Alert>();

                        // If email was not sent
                        if (lastAlerts.Count == 0)
                        {
                            _prepaidPostpaidOptions._MailTemplateType = 0;
                            System.Net.Mail.MailMessage message = EmailDetailsEvaluator.GetMailMessage(_prepaidPostpaidOptions);
                            Exception ex;
                            if (Components.EmailSender.Send(message, out ex))
                            {
                                try
                                {
                                    Alert alert = new Alert();
                                    alert.Created = DateTime.Now;
                                    alert.IsVisible = true;
                                    alert.Level = AlertLevel.High;
                                    alert.Progress = AlertProgress.None;
                                    alert.Source = source;
                                    alert.Tag = string.Concat(tag, " - Email Notification");
                                    alert.Description = description;
                                    session.Save(alert);
                                }
                                catch (Exception exAlert)
                                {
                                    log.Error(string.Format("Error Generating alert for Email Notification {0} ", action), exAlert);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Error sending email: {0} ", tag), ex);
                    }
                }
                //SMS
                if ((action & EventActions.SMS) == EventActions.SMS)
                {
                    try
                    {
                        _prepaidPostpaidOptions._MailTemplateType = 1;
                        SMS message = EmailDetailsEvaluator.GetSMSMessage(_prepaidPostpaidOptions);

                        IList<Alert> lastAlerts = session.CreateQuery("FROM Alert WHERE Tag=:Tag AND Source=:Source AND Created > :MinDate")
                               .SetParameter("Tag", string.Concat(tag, " - SMS Notification"))
                               .SetParameter("Source", source)
                               .SetParameter("MinDate", DateTime.Now.Add(_prepaidPostpaidOptions.MinimumActionEmailInterval.HasValue
                                                                                                   ? _prepaidPostpaidOptions.MinimumActionEmailInterval.Value.Negate()
                                                                                                   : TABS.SystemParameter.MinimumActionEmailInterval.TimeSpanValue.Value.Negate()))
                               .SetMaxResults(1)
                               .List<Alert>();
                        if (lastAlerts.Count == 0)
                        {
                            Exception ex = null;
                            string result = Components.SmsSender.SendViaSMSService(message, out ex);
                            if (result == "1")
                                log.InfoFormat("{0}({1}) - SMS sent successfully from financial monitor", name, id);
                            else
                                if (ex == null)
                                    log.ErrorFormat("{0}({1}) - {2}", name, id, result);
                                else
                                    log.Error(ex.ToString());
                            try
                            {
                                Alert alert = new Alert();
                                alert.Created = DateTime.Now;
                                alert.IsVisible = true;
                                alert.Level = AlertLevel.High;
                                alert.Progress = AlertProgress.None;
                                alert.Source = source;
                                alert.Tag = string.Concat(tag, " - SMS Notification");
                                alert.Description = description;
                                session.Save(alert);
                            }
                            catch (Exception exAlert)
                            {
                                log.Error(string.Format("Error Generating alert for Email Notification {0} ", action), exAlert);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }

                // Block?
                if ((action & EventActions.Block) == EventActions.Block || (action & EventActions.BlockAsCustomer) == EventActions.BlockAsCustomer)
                {
                    try
                    {
                        bool blockIn = false;
                        bool blockOut = false;

                        if (carrierType == CarrierType.Netting)
                        {
                            if ((action & EventActions.Block) == EventActions.Block)
                            {
                                blockIn = _prepaidPostpaidOptions.Balance <= 0;
                                blockOut = _prepaidPostpaidOptions.Balance > 0;
                            }
                            if ((action & EventActions.BlockAsCustomer) == EventActions.BlockAsCustomer)
                            {
                                blockIn = true;
                            }
                        }
                        else
                        {
                            if ((action & EventActions.Block) == EventActions.Block)
                            {
                                blockIn = (carrierType == CarrierType.Customer);
                                blockOut = (carrierType == CarrierType.Supplier);
                            }
                        }
                        if (blockIn || blockOut)
                        {
                            List<CarrierAccount> accounts = new List<CarrierAccount>();
                            if (!isProfile) accounts.Add(ca);
                            if (isProfile) accounts.AddRange((cf).Accounts);

                            foreach (CarrierAccount account in accounts)
                            {
                                bool statusNeedsUpdate = false;
                                if (account.ActivationStatus != ActivationStatus.Inactive)
                                {
                                    if (blockIn && (account.RoutingStatus == RoutingStatus.Enabled || account.RoutingStatus == RoutingStatus.BlockedOutbound))
                                    {
                                        statusNeedsUpdate = true;
                                        account.RoutingStatus = account.RoutingStatus == RoutingStatus.Enabled ? RoutingStatus.BlockedInbound : RoutingStatus.Blocked;
                                    }
                                    if (blockOut && (account.RoutingStatus == RoutingStatus.Enabled || account.RoutingStatus == RoutingStatus.BlockedInbound))
                                    {
                                        statusNeedsUpdate = true;
                                        account.RoutingStatus = account.RoutingStatus == RoutingStatus.Enabled ? RoutingStatus.BlockedOutbound : RoutingStatus.Blocked;
                                    }
                                }
                                if (statusNeedsUpdate)
                                {
                                    session.Update(account);
                                    foreach (Switch attachedSwitch in TABS.Switch.All.Values)
                                    {
                                        if (attachedSwitch.Enable_Routing)
                                        {
                                            string successMessage = string.Format("{0} : Performed Block ({1}) for {2} on {3}, Successful: ", (isPrepaid) ? "Prepaid" : "Postpaid", carrierType, account, attachedSwitch);
                                            string errorMessage = string.Format("{0} : Error Performing Block ({1}) for {2} on {3}", (isPrepaid) ? "Prepaid" : "Postpaid", carrierType, account, attachedSwitch);
                                            TABS.Addons.Utilities.SwitchJobQueueHandler.QueueSwitchJob
                                                (new TABS.Addons.Utilities.RouteStatusSwitchJob
                                                {
                                                    OperationType = "Update_Route_Status",
                                                    SwitchID = attachedSwitch.SwitchID,
                                                    AccountID = account.CarrierAccountID,
                                                    SuccessMessage = successMessage,
                                                    ErrorMessage = errorMessage
                                                });
                                        }
                                    }
                                }
                            }


                            // Check to see if an block notification was sent since a min time (MinimumActionEmailInterval)
                            IList<Alert> lastAlerts = session.CreateQuery("FROM Alert WHERE Tag=:Tag AND Source=:Source AND Created > :MinDate")
                                .SetParameter("Tag", string.Concat(tag, " - Block Notification"))
                                .SetParameter("Source", source)
                                .SetParameter("MinDate", DateTime.Now.Add(_prepaidPostpaidOptions.MinimumActionEmailInterval.HasValue
                                                                                                ? _prepaidPostpaidOptions.MinimumActionEmailInterval.Value.Negate()
                                                                                                : TABS.SystemParameter.MinimumActionEmailInterval.TimeSpanValue.Value.Negate()))
                            .SetMaxResults(1)
                            .List<Alert>();

                            // If email was not sent
                            if (lastAlerts.Count == 0)
                            {
                                try
                                {
                                    System.Net.Mail.MailMessage message = EmailDetailsEvaluator.GetMailMessage(_prepaidPostpaidOptions);
                                    Exception ex = null;
                                    if (!Components.EmailSender.Send(message, out ex))
                                        log.Error(string.Format("Error sending email(block action): {0} ", tag), ex);
                                }
                                catch (Exception exM)
                                {
                                    log.Error(string.Format("Error sending email(block action): {0} ", tag), exM);
                                }

                                try
                                {
                                    Alert alert = new Alert();
                                    alert.Created = DateTime.Now;
                                    alert.IsVisible = true;
                                    alert.Level = AlertLevel.Urgent;
                                    alert.Progress = AlertProgress.None;
                                    alert.Source = source;
                                    alert.Tag = string.Concat(tag, " - Block Notification");
                                    alert.Description = description;
                                    session.Save(alert);
                                }
                                catch (Exception exAlert)
                                {
                                    log.Error(string.Format("Error Generating alert for Block Notification {0} ", action), exAlert);
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("{0} : Error generating block for {1}", (isPrepaid) ? "Prepaid" : "Postpaid", action), ex);
                    }
                }
                session.Flush();
                session.Clear();
            }
        }

        /// <summary>
        /// Run the Financial Monitor
        /// </summary>
        public override void Run()
        {
            this.IsRunning = true;
            this.IsLastRunSuccessful = true;

            try
            {
                // Update Daily Prepaid Amounts
                log.Info("Executing the stored procedure bp_PrepaidDailyTotalUpdate");
                DataHelper.ExecuteNonQuery("EXEC bp_PrepaidDailyTotalUpdate");
                log.Info("the stored procedure bp_PrepaidDailyTotalUpdate Executed");
            }
            catch (Exception ex)
            {
                log.Error("Error Updating Prepaid Billing Amounts", ex);
                this.IsLastRunSuccessful = false;
                this.Exception = ex;
            }

            try
            {
                // Update Daily Postpaid Amounts
                log.Info("Executing the stored procedure bp_PostpaidDailyTotalUpdate");
                DataHelper.ExecuteNonQuery("EXEC bp_PostpaidDailyTotalUpdate");
                log.Info("the stored procedure bp_PostpaidDailyTotalUpdate Executed");
            }
            catch (Exception ex)
            {
                log.Error("Error Updating Postpaid Billing Amounts", ex);
                this.IsLastRunSuccessful = false;
                this.Exception = ex;
            }

            try
            {
                PrepaidPostPaidAction(PaymentType.Prepaid, CarrierType.Customer);
                PrepaidPostPaidAction(PaymentType.Prepaid, CarrierType.Supplier);
            }
            catch (Exception ex)
            {
                log.Error("Error Checking Prepaid Balances", ex);
                this.IsLastRunSuccessful = false;
                this.Exception = ex;
            }

            try
            {
                PrepaidPostPaidAction(PaymentType.Postpaid, CarrierType.Customer);
                PrepaidPostPaidAction(PaymentType.Postpaid, CarrierType.Supplier);
            }
            catch (Exception ex)
            {
                log.Error("Error Checking Postpaid Balances", ex);
                this.IsLastRunSuccessful = false;
                this.Exception = ex;
            }

            try
            {
                PrepaidPostPaidAction(PaymentType.Postpaid, CarrierType.Netting);
            }
            catch (Exception ex)
            {
                log.Error("Error Checking Netting Postpaid Balances", ex);
                this.IsLastRunSuccessful = false;
                this.Exception = ex;
            }
            this.IsRunning = false;
        }

        /// <summary>
        /// Request a stop for the operation
        /// </summary>
        /// <returns></returns>
        public override bool Stop()
        {
            bool result = false;
            if (this.IsRunning)
            {
                result = base.Stop();
            }
            return result;
        }

        public override string Status { get { return string.Empty; } }
    }
}
