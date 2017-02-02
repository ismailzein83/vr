using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartCompanyProfileDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("1aff2bf7-1f15-4e0b-accf-457edf36a342"); } }

        public bool IncludeArabicName { get; set; }

        public List<CompanyProfileContactType> ContactTypes { get; set; }


        public override List<GenericFieldDefinition> GetFieldDefinitions()
        {
            var list = new List<GenericFieldDefinition>();

            if(this.ContactTypes != null)
            foreach (var a in this.ContactTypes)
            {
                list.Add(new GenericFieldDefinition()
                {
                    Name = string.Format("{0}_Name",a.Name),
                    Title = string.Format("{0} Name", a.Name),
                    FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                });
                list.Add(new GenericFieldDefinition()
                {
                    Name = string.Format("{0}_Email", a.Name),
                    Title = string.Format("{0} Email", a.Name),
                    FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                });
                list.Add(new GenericFieldDefinition()
                {
                    Name = string.Format("{0}_PhoneNumbers", a.Name),
                    Title = string.Format("{0} Phone Numbers", a.Name),
                    FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                });
            }

           if (this.IncludeArabicName == true)
               list.Add(new GenericFieldDefinition()
                    {
                        Name = "ArabicName",
                        Title = "Arabic Name",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    });
           return list;
        }
    }
    public class CompanyProfileContactType
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}
