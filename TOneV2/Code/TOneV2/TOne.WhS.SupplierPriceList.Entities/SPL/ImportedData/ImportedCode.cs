using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ImportedCode : Vanrise.Entities.IDateEffectiveSettings, IRuleTarget,IExclude
    {
        public ImportedCode()
        {
            this.ProcessInfo = new CodeProcessInfo();
        }

        public string Code { get; set; }

        public string ZoneName { get; set; }

        public CodeGroup CodeGroup { get; set; }

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
        #region IExclude Implementation
        public bool IsExcluded { get; set; }
        public void SetAsExcluded()
        {
            IsExcluded = true;

            foreach (var newCode in NewCodes)
                newCode.IsExcluded = true;
            
            foreach (var changedCode in ChangedExistingCodes)
                changedCode.ChangedCode.IsExcluded = true;
        }
        #endregion

        List<ExistingCode> _changedExistingCodes = new List<ExistingCode>();
        public List<ExistingCode> ChangedExistingCodes
        {
            get
            {
                return _changedExistingCodes;
            }
        }

        public CodeProcessInfo ProcessInfo { get; set; }

        #region IRuleTarget Implementation

        public object Key
        {
            get { return this.Code; }
        }

        #endregion


        public string TargetType
        {
            get { return "Code"; }
        }
    }

    public class CodeProcessInfo
    {
        public string RecentZoneName { get; set; }
    }

    public class ImportedCodesByCodeValue : Dictionary<string, ImportedCode>
    {

    }
   
  

    public enum CodeChangeType
    {

        [Description("Not Changed")]
        NotChanged = 0,

        [Description("New")]
        New = 1,

        [Description("Deleted")]
        Deleted = 2,

        [Description("Moved")]
        Moved = 3
    }
}
