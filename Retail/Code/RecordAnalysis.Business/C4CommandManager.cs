using RecordAnalysis.Entities;
using System;
using System.Collections.Generic;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace RecordAnalysis.Business
{
    public class C4CommandManager
    {
        public static Guid s_beDefinitionId = new Guid("7c9bed54-7e1f-46d9-a738-f0588b8244ad");

        public void AddC4Command(C4CommandType commandType, string command)
        {
            Dictionary<string, object> fieldValues = new Dictionary<string, object>();
            fieldValues.Add("Type", (int)commandType);
            fieldValues.Add("Command", command);

            GenericBusinessEntityToAdd genericBusinessEntityToAdd = new GenericBusinessEntityToAdd()
            {
                BusinessEntityDefinitionId = s_beDefinitionId,
                FieldValues = fieldValues
            };

            new GenericBusinessEntityManager().AddGenericBusinessEntity(genericBusinessEntityToAdd, null);
        }
    }
}