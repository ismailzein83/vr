using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public enum CodeValidationType { NoCodeGroup, CodeGroupWrongCountry, RetroActiveMovedCode, RetroActiveNewCode }

    public class CodeValidation
    {
        public string Code { get; set; }

        public CodeValidationType ValidationType { get; set; }
    }
    public interface IZone  : Vanrise.Entities.IDateEffectiveSettings
    {
        long ZoneId { get; }

        string Name { get; }

        List<NewCode> NewCodes { get; }

        List<NewRate> NewRates { get; }

        int CountryId { get; }
    }

    public class ZonesByName : Dictionary<string, List<IZone>>
    {

    }
    //public enum ItemStatus { NotChanged = 0, New = 1, Updated = 2 }

    public class NewZone : IZone
    {
        public long ZoneId { get; set; }

        public int CountryId { get; set; }

        public string Name { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        List<NewCode> _newCodes = new List<NewCode>();
        public List<NewCode> NewCodes
        {
            get
            {
                return _newCodes;
            }
        }

        List<NewRate> _newRates = new List<NewRate>();
        public List<NewRate> NewRates
        {
            get
            {
                return _newRates;
            }
        }
    }

    public class NewZonesByName : Dictionary<string, List<NewZone>>
    {

    }
   
    public enum CodeChangeType { NotChanged = 0, New = 1, Deleted = 2, Moved = 3 }
    
    public class ImportedCode : Vanrise.Entities.IDateEffectiveSettings, IRuleTarget
    {
        public ImportedCode()
        {
            this.ProcessInfo = new CodeProcessInfo();
        }

        public string Code { get; set; }

        public string ZoneName { get; set; }

        public int? CodeGroupId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public CodeChangeType ChangeType { get; set; }

        List<NewCode> _newCodes = new List<NewCode>();
        public List<NewCode> NewCodes
        {
            get
            {
                return _newCodes;
            }
        }

        List<ExistingCode> _changedExistingCodes = new List<ExistingCode>();
        public List<ExistingCode> ChangedExistingCodes
        {
            get
            {
                return _changedExistingCodes;
            }
        }

        public CodeProcessInfo ProcessInfo { get; set; }

        public MessageSeverity Severity { get; set; }

        public string Message 
        { 
            get 
            { 
                return string.Format("Code {0} is not assigned to any code group", Code);
            } 
        }

        public void SetExecluded()
        {
            this.IsExecluded = true;
        }

        public bool IsExecluded { get; set; }
    }

    public class CodeProcessInfo
    {
        public string RecentZoneName { get; set; }
    }

    public class ImportedCodesByCodeValue : Dictionary<string, ImportedCode>
    {

    }

    public class ImportedRate : Vanrise.Entities.IDateEffectiveSettings
    {
        public ImportedRate()
        {
            this.ProcessInfo = new RateProcessInfo();
        }

        public string ZoneName { get; set; }

        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> OtherRates { get; set; }

        public int? CurrencyId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
        
        public RateChangeType ChangeType { get; set; }

        List<NewRate> _newRates = new List<NewRate>();
        public List<NewRate> NewRates
        {
            get
            {
                return _newRates;
            }
        }

        List<ExistingRate> _changedExistingRates = new List<ExistingRate>();
        public List<ExistingRate> ChangedExistingRates
        {
            get
            {
                return _changedExistingRates;
            }
        }

        public RateProcessInfo ProcessInfo { get; set; }
    }

    public class RateProcessInfo
    {
        public Decimal? RecentRate { get; set; }
    }

    public class ImportedZone : IRuleTarget
    {
        public string ZoneName { get; set; }

        public List<ImportedCode> ImportedCodes { get; set; }

        public List<ImportedRate> ImportedRates { get; set; }

        public MessageSeverity Severity { get; set; }

        public string Message { get { return string.Format("Zone {0} is execluded", ZoneName); } }

        public void SetExecluded()
        {
            this.IsExecluded = true;
        }

        public bool IsExecluded { get; set; }
    }

    public class ImportedCountry
    {
        public int CountryId { get; set; }

        public List<ImportedCode> ImportedCodes { get; set; }

        public List<ImportedRate> ImportedRates { get; set; }
    }

    public class NewCode : Vanrise.Entities.IDateEffectiveSettings
    {
        public long CodeId { get; set; }

        public string Code { get; set; }

        public IZone Zone { get; set; }

        public int CodeGroupId { get; set; }
        
        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public enum RateChangeType { NotChanged = 0, New = 1, Increase = 2, Decrease = 3 }

    public class NewRate : Vanrise.Entities.IDateEffectiveSettings
    {
        public long RateId { get; set; }

        public IZone Zone { get; set; }

        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> OtherRates { get; set; }

        public int? CurrencyId { get; set; }
        
        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class ChangedZone
    {
        public long ZoneId { get; set; }

        public DateTime EED { get; set; }
    }

    public class ChangedCode
    {
        public long CodeId { get; set; }

        public DateTime EED { get; set; }
    }

    public class ChangedRate
    {
        public long RateId { get; set; }

        public DateTime EED { get; set; }
    }
}
