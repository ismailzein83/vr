﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class PriceListManager
    {
        IPriceListDataManager _dataManager;
        public PriceListManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<IPriceListDataManager>();
        }
        public List<PriceList> GetPriceList()
        {
            return _dataManager.GetPriceList();
        }

        public PriceList GetPriceListById(int priceListId)
        {
            return _dataManager.GetPriceListById(priceListId);
        }

        public bool SavePriceList(PriceList pricelist, out int priceListId)
        {
            return _dataManager.SavePriceList(pricelist, out priceListId);
        }

        public bool SavePricelistData(byte[] data, int priceList)
        {
            return _dataManager.SavePriceListData(data, priceList);
        }
    }
}
