using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class RAPeriodDefinitionOnAfterSaveHandler : GenericBEOnAfterSaveHandler
    {
        public override Guid ConfigId { get { return new Guid("db7ea63c-0407-4712-a95c-7fc253db8fdf"); } }
        public override void Execute(IGenericBEOnAfterSaveHandlerContext context)
        {
            PeriodDefinitionManager periodDefinitionManager = new PeriodDefinitionManager();
            bool monthlyRepeat = false;
            bool dailyRepeat = false;
            List<GenericBusinessEntityToAdd> periodDefinitionsToAdd = new List<GenericBusinessEntityToAdd>();
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            context.ThrowIfNull("context");
            context.NewEntity.ThrowIfNull("context.NewEntity");
            context.NewEntity.FieldValues.ThrowIfNull("context.NewEntity.FieldValues");
            if (context.BusinessEntityDefinitionId == null)
                throw new NullReferenceException("BusinessEntityDefinitionId");
            var from = (DateTime)context.NewEntity.FieldValues.GetRecord("From");
            if (from == null)
                throw new NullReferenceException("from");
            var to = (DateTime)context.NewEntity.FieldValues.GetRecord("To");
            if (to == null)
                throw new NullReferenceException("to");
            var repeat = context.NewEntity.FieldValues.GetRecord("Repeat");
            var timeSpan = to.Subtract(from);
            if (from.Month == to.Month)
            {
                var daysInMonth = DateTime.DaysInMonth(from.Year, from.Month);

                if (daysInMonth == timeSpan.Days + 1)
                {
                    monthlyRepeat = true;
                }
                else dailyRepeat = true;
            }
            else
            {
                int daysInAllMonths = 0;
                var currentDate = from;
                while (currentDate.Month < to.Month || currentDate.Year < to.Year)
                {
                    daysInAllMonths += DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                    currentDate = currentDate.AddMonths(1);
                }
                if (to.Day == DateTime.DaysInMonth(to.Year, to.Month))
                {
                    daysInAllMonths += DateTime.DaysInMonth(to.Year, to.Month);
                }
                if (daysInAllMonths == timeSpan.Days + 1)
                    monthlyRepeat = true;
                else
                    dailyRepeat = true;
            }

            if (repeat != null)
            {
                var newFrom = from;
                var monthsDifference = (to.Year - from.Year) * 12 + (to.Month - from.Month) + 1;
                var newTo = to;
                for (var i = 0; i < Convert.ToInt32(repeat); i++)
                {
                    var genericBusinessEntityToAdd = new GenericBusinessEntityToAdd
                    {
                        BusinessEntityDefinitionId = context.BusinessEntityDefinitionId,
                        FieldValues = new Dictionary<string, object>()
                    };
                    if (monthlyRepeat)
                    {
                        newFrom = newTo.AddDays(1);
                        newTo = AddMonths(newTo, monthsDifference);
                        genericBusinessEntityToAdd.FieldValues.Add("From", newFrom);
                        genericBusinessEntityToAdd.FieldValues.Add("To", newTo);
                    }
                    else if (dailyRepeat)
                    {
                        newFrom = newFrom.AddDays(timeSpan.Days + 1);
                        newTo = newTo.AddDays(timeSpan.Days + 1);
                        genericBusinessEntityToAdd.FieldValues.Add("From", newFrom);
                        genericBusinessEntityToAdd.FieldValues.Add("To", newTo);
                    }
                    periodDefinitionsToAdd.Add(genericBusinessEntityToAdd);

                }

            }

            if (periodDefinitionsToAdd.Count() > 0)
            {
                foreach (var periodDefinition in periodDefinitionsToAdd)
                {
                    genericBusinessEntityManager.AddGenericBusinessEntity(periodDefinition);
                }
            }

        }
        private DateTime AddMonths(DateTime date, int numberOfMonths)
        {
            if (date.Day != DateTime.DaysInMonth(date.Year, date.Month))
                return date.AddMonths(numberOfMonths);
            else
                return date.AddDays(1).AddMonths(numberOfMonths).AddDays(-1);
        }

    }
}

