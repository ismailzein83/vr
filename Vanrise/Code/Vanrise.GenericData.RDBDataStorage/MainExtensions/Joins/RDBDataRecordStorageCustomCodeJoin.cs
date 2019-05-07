using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.RDBDataStorage.MainExtensions.Joins
{
    public class RDBDataRecordStorageCustomCodeJoin : RDBDataRecordStorageJoinSettings
    {
        public override Guid ConfigId => new Guid("FF293CA2-522F-47F1-808A-6178DF488E11");

        public string CustomCode { get; set; }

        public List<string> DependentJoins { get; set; }

        public override string GetCode(IRDBDataRecordStorageJoinExpressionGetCodeContext context)
        {
            return this.CustomCode;
        }

        public override List<string> GetDependentJoins(IRDBDataRecordStorageJoinExpressionGetDependentJoinsContext context)
        {
            return this.DependentJoins;
        }

        public override string StorageFieldEditor
        {
            get
            {
                return "vr-genericadata-rdbdatarecordstoragesettings-customcodejoin-storagefieldeditor";
            }
        }
    }
}
