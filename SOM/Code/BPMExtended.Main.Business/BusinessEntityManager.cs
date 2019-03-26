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
        //public UserConnection BPM_UserConnection
        //{
        //    get
        //    {
        //        return (UserConnection)HttpContext.Current.Session["UserConnection"];
        //    }
        //}    
        public Array GetLineOfBusniessArray()
        {
            var lineOfBusinessArray = Enum.GetNames(typeof(LineOfBusiness));
            return lineOfBusinessArray;
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

        public class OperationTypeDescriptiveObject
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

    }

}
