using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public static class ExtensionMethods
    {
        public static List<T> GetConnectedEntities<T>(this List<T> entities, DateTime effectiveOn) where T : IExistingEntity
        {
            if (entities == null)
                throw new MissingArgumentValidationException("entities");

            if (entities.Count == 0)
                return null;

            List<T> connectedEntities = new List<T>();
            int currentElementIndex = -1;

            for (int i = 0; i < entities.Count(); i++)
            {
                var currentElement = entities[i];
                if (currentElement.BED <= effectiveOn && currentElement.EED.VRGreaterThan(effectiveOn))
                {
                    currentElementIndex = i;
                    break;
                }
            }

            List<int> overlappedIndices = new List<int>();

            if (currentElementIndex != -1)
                overlappedIndices.Add(currentElementIndex);

            if (currentElementIndex == -1)
            {
                int indexToTheRight = entities.FindIndex(item => item.BED >= effectiveOn);
                if (indexToTheRight != -1)
                    overlappedIndices.Add(indexToTheRight);
                else
                    overlappedIndices.Add(entities.FindLastIndex(item => item.BED < effectiveOn));

            }

            List<int> changedEntitiesIndices = new List<int>();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].ChangedEntity != null)
                    changedEntitiesIndices.Add(i);
            }


            if (changedEntitiesIndices.Count > 0)
            {
                IEnumerable<int> intersectIndices = changedEntitiesIndices.Intersect<int>(overlappedIndices);
                currentElementIndex = intersectIndices.Count() == 0 ? changedEntitiesIndices.Last() : intersectIndices.First();
            }
            else
                currentElementIndex = overlappedIndices.First();


            connectedEntities.AddRange(GetConnectedEntitiesToTheLeft(entities.GetRange(0, currentElementIndex + 1)));
            connectedEntities.Add(entities[currentElementIndex]);
            connectedEntities.AddRange(GetConnectedEntitiesToTheRight(entities.GetRange(currentElementIndex, entities.Count() - currentElementIndex)));


            return connectedEntities;
        }

        private static List<T> GetConnectedEntitiesToTheRight<T>(List<T> entities) where T : IExistingEntity
        {
            List<T> connectedEntities = new List<T>();

            for (int i = 0; i < entities.Count() - 1; i++)
            {
                var currentElement = entities[i];
                var nextElement = entities[i + 1];

                if (currentElement.EED.HasValue && currentElement.EED == nextElement.BED && currentElement.IsSameEntity(nextElement))
                    connectedEntities.Add(nextElement);
                else
                    break;
            }

            return connectedEntities;
        }

        private static List<T> GetConnectedEntitiesToTheLeft<T>(List<T> entities) where T : IExistingEntity
        {
            List<T> connectedEntities = new List<T>();

            for (int i = entities.Count() - 1; i > 0; i--)
            {
                var currentElement = entities[i];
                var previousElement = entities[i - 1];

                if (previousElement.EED.HasValue && currentElement.BED == previousElement.EED && currentElement.IsSameEntity(previousElement))
                    connectedEntities.Add(previousElement);
                else
                    break;
            }

            return connectedEntities;
        }

    }
}
