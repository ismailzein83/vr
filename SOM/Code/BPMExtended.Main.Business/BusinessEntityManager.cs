using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using System.ComponentModel;
using BPMExtended.Main.SOMAPI;
using SOM.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class BusinessEntityManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }    
        public Array GetLineOfBusniessArray()
        {
            var lineOfBusinessArray = Enum.GetNames(typeof(LineOfBusiness));
            return lineOfBusinessArray;
        }

        public Packages GetServicePackagesEntity()
        {
            var packages = new Packages();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");

            esq.AddColumn("StPOSServicePackage");
            esq.AddColumn("StCoreServicePackage");
            esq.AddColumn("StTelephonyServices");
            esq.AddColumn("StXDSLServices");
            esq.AddColumn("StLeasedLineServices");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "08CBBB9F-043C-47D9-8AC4-97D25081681E");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                packages.POS = entities[0].GetColumnValue("StPOSServicePackage").ToString();
                packages.Core = entities[0].GetColumnValue("StCoreServicePackage").ToString();
                packages.Telephony = entities[0].GetColumnValue("StTelephonyServices").ToString();
                packages.XDSL = entities[0].GetColumnValue("StXDSLServices").ToString();
                packages.LeasedLine = entities[0].GetColumnValue("StLeasedLineServices").ToString();
            }

            return packages;
        }
        public List<OperationTypeDescriptiveObject> GetOperationTypeInfo()
        {
            var operationTypeDescriptiveObjectList = new List<OperationTypeDescriptiveObject>();
            var operationTypeArray = Enum.GetValues(typeof(OperationType));

            for (int i = 0; i < operationTypeArray.Length; i++)
            {
                var operationTypeInstance = (OperationType)operationTypeArray.GetValue(i);
                var operationTypeDecriptiveObj = new OperationTypeDescriptiveObject()
                {
                    Id = i,
                    Description = Utilities.GetEnumAttribute<OperationType, DescriptionAttribute>(operationTypeInstance).Description
                };
                operationTypeDescriptiveObjectList.Add(operationTypeDecriptiveObj);
            }
            return operationTypeDescriptiveObjectList;
        }

        public List<OperationTypeDescriptiveObject> GetAvailableOperationType()
        {
            List<string> operationsIds = new List<string>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StTelephonyOperationsInCatalog");
            var toId = esq.AddColumn("StOperationId");
            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StGeneralSetting", "F8A758B9-3A76-47CD-9FCF-374C934A2C31");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                foreach (var item in entities)
                {
                    operationsIds.Add(item.GetTypedColumnValue<string>(toId.Name));
                }
            }

            List<OperationTypeDescriptiveObject> operationInfoItems = GetOperationTypeInfo();

            var availableOperations = new List<OperationTypeDescriptiveObject>();
            availableOperations = operationInfoItems.Where(p => !operationsIds.Any(p2 => p2.ToString() == p.Id.ToString())).ToList();
            return availableOperations;
        }

        public List<Year> GetYearsInfo()
        {
            var yearsList = new List<Year>();
            var currentYear = DateTime.Today.Year;
            int yearsNumbers = new CatalogManager().GetCallDetailsYearsNumber();

            for (int i = 0; i < yearsNumbers; i++)
            {
                var yearObj = new Year()
                {
                    Id = i,
                    Description = (currentYear - i).ToString()
                };
                yearsList.Add(yearObj);
            }
            return yearsList;
        }

        public int getMonthNumberByMonthName (string monthName)
        {
            int monthNumber = 0;

            var esqCities = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Month");
            esqCities.AddColumn("Name");
            esqCities.AddColumn("Number");

            var esqFirstFilter = esqCities.CreateFilterWithParameters(FilterComparisonType.Equal, "Name", monthName);

            esqCities.Filters.Add(esqFirstFilter);

            var entities = esqCities.GetEntityCollection(BPM_UserConnection);
            if (entities.Count != 0)
            {
              monthNumber = (int)entities[0].GetColumnValue("Number");
            }
            return monthNumber;
        }

    }

}
