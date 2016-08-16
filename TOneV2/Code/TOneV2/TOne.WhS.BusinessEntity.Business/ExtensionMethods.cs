using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public static class ExtensionMethods
    {
        public static List<T> GetLinkedEntities<T>(this List<T> entities) where T : IDateEffectiveSettings
        {
            if (entities == null)
                throw new MissingArgumentValidationException("entities");

            if (entities.Count == 0)
                return null;

            List<T> linkedEntities = new List<T>();
            linkedEntities.Add(entities[entities.Count - 1]);

            for (int i = entities.Count() - 1; i > 1; i--)
            {
                var currentElement = entities[i];
                var nextElement = entities[i + 1];

                if (currentElement.EED == nextElement.BED)
                    linkedEntities.Add(currentElement);
                else
                    break;
            }

            return linkedEntities;
        }
    }
}
