using RecordAnalysis.Entities;
using System;
using System.Collections.Generic;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace RecordAnalysis.Business
{
    public class CommandManager
    {
        private Guid beDefinitionId;

        public CommandManager(Guid beDefinitionId)
        {
            this.beDefinitionId = beDefinitionId;
        }
        public void AddCommand(int commandType, string command)
        {
            Dictionary<string, object> fieldValues = new Dictionary<string, object>();
            fieldValues.Add("Type", commandType);
            fieldValues.Add("Command", command);

            GenericBusinessEntityToAdd genericBusinessEntityToAdd = new GenericBusinessEntityToAdd()
            {
                BusinessEntityDefinitionId = beDefinitionId,
                FieldValues = fieldValues
            };

            new GenericBusinessEntityManager().AddGenericBusinessEntity(genericBusinessEntityToAdd, null);
        }
    }
}