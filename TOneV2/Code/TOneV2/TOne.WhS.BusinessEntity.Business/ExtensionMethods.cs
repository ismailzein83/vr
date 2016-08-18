﻿using System;
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
            overlappedIndices.Add(currentElementIndex);
            
            if (currentElementIndex == -1)
            {
                overlappedIndices.Add(entities.FindIndex(item => item.BED >= effectiveOn));
                overlappedIndices.Add(entities.FindLastIndex(item => item.BED < effectiveOn));
              
            }

            List<int> changedEntitiesIndices = new List<int>();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].ChangedEntity != null)
                    changedEntitiesIndices.Add(i);
            }

            int? overlappedIndex = changedEntitiesIndices.Intersect<int>(overlappedIndices).FirstOrDefault();
           
            if (!overlappedIndex.HasValue)
                overlappedIndex = changedEntitiesIndices.Last();


            connectedEntities.Add(entities[overlappedIndex.Value]);
            connectedEntities.AddRange(GetConnectedEntitiesToTheRight(entities.GetRange(overlappedIndex.Value, entities.Count() - overlappedIndex.Value)));
            connectedEntities.AddRange(GetConnectedEntitiesToTheLeft(entities.GetRange(0, overlappedIndex.Value + 1)));

            return connectedEntities;
        }


        public static T GetSystemRate<T>(this List<T> entities, DateTime effectiveOn) where T : IDateEffectiveSettings
        {
            if (entities == null || entities.Count == 0)
                return default(T);

            T systemRate = default(T);

            for (int i = 0; i < entities.Count(); i++)
            {
                var currentElement = entities[i];

                if (currentElement.BED <= effectiveOn && Vanrise.Common.ExtensionMethods.VRGreaterThan(currentElement.EED, effectiveOn))
                    return currentElement;
            }

            return systemRate;
        }

        private static List<T> GetConnectedEntitiesToTheRight<T>(List<T> entities) where T : IDateEffectiveSettings
        {
            List<T> connectedEntities = new List<T>();

            for (int i = 0; i < entities.Count() - 1; i++)
            {
                var currentElement = entities[i];
                var nextElement = entities[i + 1];

                if (currentElement.EED.HasValue && currentElement.EED == nextElement.BED)
                    connectedEntities.Add(nextElement);
                else
                    break;
            }

            return connectedEntities;
        }

        private static List<T> GetConnectedEntitiesToTheLeft<T>(List<T> entities) where T : IDateEffectiveSettings
        {
            List<T> connectedEntities = new List<T>();

            for (int i = entities.Count() - 1; i > 0; i--)
            {
                var currentElement = entities[i];
                var previousElement = entities[i - 1];

                if (previousElement.EED.HasValue && currentElement.BED == previousElement.EED)
                    connectedEntities.Add(previousElement);
                else
                    break;
            }

            return connectedEntities;
        }

    }
}
