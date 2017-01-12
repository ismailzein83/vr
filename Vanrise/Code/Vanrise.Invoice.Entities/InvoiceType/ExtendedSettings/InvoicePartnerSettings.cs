using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class InvoicePartnerSettings
    {
        public virtual string PartnerSelector { get; set; }
        public virtual string PartnerFilterSelector { get; set; }
        public abstract string GetPartnerName(IPartnerNameManagerContext context);
        public abstract dynamic GetPartnerInfo(IPartnerManagerInfoContext context);
        public abstract dynamic GetActualPartnerId(IActualPartnerContext context);
        public abstract int GetPartnerDuePeriod(IPartnerDuePeriodContext context);
        public abstract string GetPartnerSerialNumberPattern(IPartnerSerialNumberPatternContext context);
    }
    public interface IBasePartnerManagerContext
    {
        string PartnerId { get; }
    }
    public interface IPartnerManagerInfoContext : IBasePartnerManagerContext
    {
        string InfoType { get; }
        InvoicePartnerSettings PartnerSettings { get; }
    }
    public interface IPartnerNameManagerContext : IBasePartnerManagerContext
    {
    }
    public interface IPartnerDuePeriodContext : IBasePartnerManagerContext
    {
    }
    public interface IActualPartnerContext : IBasePartnerManagerContext
    {
    }
    public interface IPartnerSerialNumberPatternContext : IBasePartnerManagerContext
    {
    }
}
