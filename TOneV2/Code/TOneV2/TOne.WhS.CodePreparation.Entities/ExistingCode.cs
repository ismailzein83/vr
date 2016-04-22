using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public interface IZone : Vanrise.Entities.IDateEffectiveSettings
    {
        long ZoneId { get; }

        string Name { get; }

        List<AddedCode> AddedCodes { get; }
    }

    public class ZonesByName : Dictionary<string, List<IZone>>
    {

    }

    public class ExistingZone : IZone
    {
        public BusinessEntity.Entities.SaleZone ZoneEntity { get; set; }

        public ChangedZone ChangedZone { get; set; }

        public long ZoneId
        {
            get { return ZoneEntity.SaleZoneId; }
        }

        public string Name
        {
            get { return ZoneEntity.Name; }
        }

        public int CountryId
        {
            get { return ZoneEntity.CountryId; }
        }

        public DateTime BED
        {
            get { return ZoneEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedZone != null ? ChangedZone.EED : ZoneEntity.EED; }
        }

        List<ExistingCode> _existingCodes = new List<ExistingCode>();
        public List<ExistingCode> ExistingCodes
        {
            get
            {
                return _existingCodes;
            }
        }

        List<AddedCode> _addedCodes = new List<AddedCode>();
        public List<AddedCode> AddedCodes
        {
            get
            {
                return _addedCodes;
            }
        }
    }

    public class ExistingCode : Vanrise.Entities.IDateEffectiveSettings
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SaleCode CodeEntity { get; set; }

        public ChangedCode ChangedCode { get; set; }

        public DateTime BED
        {
            get { return CodeEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedCode != null ? ChangedCode.EED : CodeEntity.EED; }
        }
    }

    public class ExistingZonesByName : Dictionary<string, List<ExistingZone>>
    {

    }

    public class ExistingCodesByCodeValue : Dictionary<string, List<ExistingCode>>
    {

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

    public class AddedCode : Vanrise.Entities.IDateEffectiveSettings
    {
        public long CodeId { get; set; }

        public string Code { get; set; }

        public IZone Zone { get; set; }

        public int CodeGroupId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class AddedZone : IZone
    {
        public long ZoneId { get; set; }

        public int CountryId { get; set; }

        public string Name { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        List<AddedCode> _addedCodes = new List<AddedCode>();
        public List<AddedCode> AddedCodes
        {
            get
            {
                return _addedCodes;
            }
        }
    }

}
