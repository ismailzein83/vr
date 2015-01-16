using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TABS.SpecialSystemParameters
{
    public class EmailDetailsEvaluator
    {
        log4net.ILog log = log4net.LogManager.GetLogger(typeof(EmailDetailsEvaluator));

        static EmailDetailsEvaluator()
        {
            // Register all TABS classes for evaluation
            foreach (Type tabsType in typeof(EmailDetailsEvaluator).Assembly.GetTypes())
            {
                Spring.Core.TypeResolution.TypeRegistry.RegisterType(tabsType.Name, tabsType);
            }
        }

        public static EmailDetails GetEmailDetails(object context)
        {
            MailTemplateType type = MailTemplateType.Pricelist;

            switch (context.GetType().Name)
            {
                case "User":
                    type = MailTemplateType.UserCreationNotification;
                    break;
                case "PriceList":
                    type = MailTemplateType.Pricelist;
                    break;
                case "Billing_Invoice":
                    Billing_Invoice invoice = (Billing_Invoice)context;
                    type = invoice.Supplier.Equals(TABS.CarrierAccount.SYSTEM) ? MailTemplateType.Invoice : MailTemplateType.InvoiceSupplier;
                    break;
                case "Billing_InvoiceCustomerExceededDueDate":
                    TABS.Addons.Utilities.Billing_InvoiceCustomerExceededDueDate invoiceNotPaid = (TABS.Addons.Utilities.Billing_InvoiceCustomerExceededDueDate)context;
                    type = MailTemplateType.InvoiceCustomerExceededDueDate;
                    break;
                case "Alert":
                    type = MailTemplateType.Alert;
                    break;
                case "FaultTicketUpdate":
                    FaultTicketUpdate history = (FaultTicketUpdate)context;
                    if (history.FaultTicket.TicketType == TicketType.IN)
                    {
                        if (history.Status != TicketStatus.Open) type = MailTemplateType.TroubleTicketInUpdate;
                        else type = MailTemplateType.TroubleTicketIN;
                    }
                    else
                    {
                        if (history.Status == TicketStatus.Open) type = MailTemplateType.TroubleTicketOUT;
                        else
                            if (history.Status == TicketStatus.Closed) type = MailTemplateType.TroubleTicketCloseOUT;
                            else
                                type = MailTemplateType.TroubleTicketOutUpdate;
                    }
                    break;
                case "PrepaidPostpaidOptions":
                    PrepaidPostpaidOptions option = (PrepaidPostpaidOptions)context;
                    
                        type = (option.IsPrepaid)
                                   ? ((option.IsCustomer)
                                                ? (((option.Actions & EventActions.Block) == EventActions.Block)
                                                            ? MailTemplateType.PrepaidCustomerBlockAction
                                                            : MailTemplateType.PrepaidCustomerAction)
                                                : MailTemplateType.PrepaidSupplierAction)
                                   : ((option.IsCustomer)
                                                ? (((option.Actions & EventActions.Block) == EventActions.Block)
                                                        ? MailTemplateType.PostpaidCustomerBlockAction
                                                        : MailTemplateType.PostpaidCustomerAction)
                                                : (((option.Actions & EventActions.Block) == EventActions.Block)
                                                    ? MailTemplateType.PostpaidSupplierBlockAction
                                                    : MailTemplateType.PostpaidSupplierAction));
                    
                    if (option._MailTemplateType == 1)
                    {
                        if ((option.Actions & EventActions.SMS) == EventActions.SMS)
                            type = (option.IsPrepaid)
                                                ? ((option.IsCustomer)
                                                              ? MailTemplateType.PrepaidSMSCustomer
                                                              : MailTemplateType.PrepaidCustomerAction)
                                                : ((option.IsCustomer)
                                                              ? MailTemplateType.PostpaidSMSCustomer
                                                              : MailTemplateType.PostpaidSMSSupplier);
                    }
                    break;
                case "PostpaidAmount":
                case "PrepaidAmount":
                    PrePostTransaction preposAmount = (PrePostTransaction)context;
                    switch (preposAmount._MailTemplateType)
                    {
                        case 0:
                            type = MailTemplateType.MailCustomerPayment;
                            break;
                        case 1:
                            type = MailTemplateType.SMSCustomerPayment;
                            break;
                        case 2:
                            type = MailTemplateType.MailSupplierPayment;
                            break;
                        case 4:
                            type = MailTemplateType.SMSSupplierPayment;
                            break;
                    }
                    break;
                case "Event":
                    type = ((TABS.Components.Event)context).MailType;
                    context = ((TABS.Components.Event)context).Context;
                    break;
                case "BillReminder":
                    type = MailTemplateType.BillReminder;
                    break;
            }

            foreach (EmailDetails details in EmailDetails.Get(SystemParameter.MailDetails))
            {
                if (details.Type == type)
                {
                    return details;
                }
            }

            throw new NotSupportedException("there is no EmailTemplate for this type:" + type.GetType().Name);
        }

        static readonly Regex ExpressionFinder = new Regex("([$][$]{(?<GLOBAL>[^${}]+)})|([$]{(?<CONTEXT>[^${}]+)})", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture);

        public string Evaluate(Match match)
        {
            bool global = match.Groups["GLOBAL"].Success;
            string expression = match.Groups[global ? "GLOBAL" : "CONTEXT"].Value;
            string evaluated = string.Empty;
            try
            {
                if (global) expression = "$" + expression;
                var context = this.Context is TABS.Components.Event ? ((TABS.Components.Event)this.Context).Context : this.Context;
                evaluated = Spring.Expressions.ExpressionEvaluator.GetValue
                    (global ? null : context
                    , expression).ToString();
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error Evaluating {0} for {1}", expression, Context), ex);
                evaluated = "**ERROR**";
            }
            return evaluated;
        }

        public object Context { get; set; }
        protected MatchEvaluator _Evaluator;
        protected List<MailExpression> _ExpressionList;


        public EmailDetailsEvaluator(object context)
        {
            this.Context = context;
            _Evaluator = new MatchEvaluator(this.Evaluate);
            _ExpressionList = MailExpression.Get(SystemParameter.MailExpressions);
        }

        protected string Evaluate(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            foreach (MailExpression expression in _ExpressionList)
                value = value.Replace(expression.Value, expression.Mapping);
            value = ExpressionFinder.Replace(value, _Evaluator);
            return value;
        }

        public EmailDetails Evaluate()
        {
            EmailDetails details = GetEmailDetails(this.Context);

            details.From = Evaluate(details.From);
            details.To = Evaluate(details.To);
            details.CC = Evaluate(details.CC);
            details.Bcc = Evaluate(details.Bcc);
            details.Subject = Evaluate(details.Subject);
            details.SMS = Evaluate(details.SMS);
            details.Body = Evaluate(details.Body);

            return details;
        }
        //--------------------------------------------------------------------------------
        public string GetSubjectEvaluator(string subj)
        {
            return Evaluate(subj);
        }
        //---------------------------------------------------------------------------------
        public static System.Net.Mail.MailMessage GetMailMessage(object context)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

            EmailDetailsEvaluator evaluator = new EmailDetailsEvaluator(context);
            EmailDetails details = evaluator.Evaluate();
            if (!string.IsNullOrEmpty(details.From))
                message.From = new System.Net.Mail.MailAddress(details.From);
            if (!string.IsNullOrEmpty(details.To)) details.To.Trim()
                                                             .Split(',', ';')
                                                             .ToList().ForEach(t => message.To.Add(t.Trim()));


            if (!string.IsNullOrEmpty(details.CC)) details.CC.Trim()
                                                             .Split(',', ';')
                                                             .ToList().ForEach(t => message.CC.Add(t.Trim()));
            if (!string.IsNullOrEmpty(details.Bcc)) details.Bcc.Trim()
                                                            .Split(',', ';')
                                                            .ToList().ForEach(t => message.Bcc.Add(t.Trim()));
            message.Subject = details.Subject;
            message.IsBodyHtml = true;
            message.Body = details.Body;

            return message;
        }
        public static SMS GetSMSMessage(object context)
        {
            EmailDetailsEvaluator evaluator = new EmailDetailsEvaluator(context);
            EmailDetails details = evaluator.Evaluate();
            SMS sms = new SMS();
            sms.To = details.SMS;
            sms.Body = details.Body;
            return sms;
        }
    }
}
