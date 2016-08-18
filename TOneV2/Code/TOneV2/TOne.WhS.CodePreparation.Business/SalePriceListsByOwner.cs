using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
    public class SalePriceListsByOwner
    {
        private Dictionary<string, PriceListToAdd> _salePriceListsByOwner;
        public SalePriceListsByOwner()
        {
            _salePriceListsByOwner = new Dictionary<string, PriceListToAdd>();
        }
        public PriceListToAdd TryAddValue(PriceListToAdd salePriceList)
        {
            string owner = string.Join<int>(",", new List<int>() { (int)salePriceList.OwnerType, salePriceList.OwnerId });
            lock (_salePriceListsByOwner)
            {
                PriceListToAdd priceList;
                if(this._salePriceListsByOwner.TryGetValue(owner , out priceList))
                {
                    return priceList;
                }
                else
                {
                    this._salePriceListsByOwner.Add(owner, salePriceList);
                    return salePriceList;
                }
            }
        }
        public IEnumerable<PriceListToAdd> GetSalePriceLists()
        {
            return this._salePriceListsByOwner.Values;
        }
        public void ReserveIds()
        {
            lock (this._salePriceListsByOwner)
            {
                IEnumerable<PriceListToAdd> salePriceLists = this._salePriceListsByOwner.Values.FindAllRecords(item => item.PriceListId == 0);

                if (salePriceLists != null && salePriceLists.Count() > 0)
                {
                    SalePriceListManager salePriceListManager = new SalePriceListManager();
                    int salePriceListStartingId = (int)salePriceListManager.ReserveIdRange(salePriceLists.Count());
                    foreach (PriceListToAdd priceList in salePriceLists)
                    {
                        priceList.PriceListId = salePriceListStartingId++;
                    } 
                }
            }
        }
    }
}
