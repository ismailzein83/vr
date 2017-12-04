using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class ProductZoneRoutingProductHistoryLocator
    {
        #region Fields
        private ProductZoneRoutingProductHistoryReader _reader;
        private IEnumerable<SaleEntityZoneRoutingProductSource> _orderedRoutingProductSources;
        #endregion

        #region Constructors
        public ProductZoneRoutingProductHistoryLocator(ProductZoneRoutingProductHistoryReader reader)
        {
            InitializeFields(reader);
        }
        #endregion

        public IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetProductZoneRoutingProductHistory(int sellingProductId, string zoneName)
        {
            var routingProductHistoryBySource = new RoutingProductHistoryBySource();

            if (_orderedRoutingProductSources.Contains(SaleEntityZoneRoutingProductSource.ProductDefault))
                AddProductDefaultRoutingProductHistory(routingProductHistoryBySource, sellingProductId);

            if (_orderedRoutingProductSources.Contains(SaleEntityZoneRoutingProductSource.ProductZone))
                AddProductZoneRoutingProductHistory(routingProductHistoryBySource, sellingProductId, zoneName);

            return ZoneRoutingProductHistoryUtilities.GetZoneRoutingProductHistory(routingProductHistoryBySource, _orderedRoutingProductSources);
        }

        #region Private Methods
        private void InitializeFields(ProductZoneRoutingProductHistoryReader reader)
        {
            _reader = reader;

            _orderedRoutingProductSources = new List<SaleEntityZoneRoutingProductSource>()
            {
                SaleEntityZoneRoutingProductSource.ProductDefault,
                SaleEntityZoneRoutingProductSource.ProductZone
            };
        }
        public void AddProductDefaultRoutingProductHistory(RoutingProductHistoryBySource routingProductHistoryBySource, int sellingProductId)
        {
            IEnumerable<DefaultRoutingProduct> productDefaultRoutingProducts = _reader.GetDefaultRoutingProducts(sellingProductId);

            if (productDefaultRoutingProducts != null && productDefaultRoutingProducts.Count() > 0)
            {
                IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> productDefaultRoutingProductHistory = productDefaultRoutingProducts.MapRecords(ZoneRoutingProductHistoryUtilities.DefaultRoutingProductMapperFunc);
                routingProductHistoryBySource.AddRoutingProductHistoryRange(SaleEntityZoneRoutingProductSource.ProductDefault, productDefaultRoutingProductHistory);
            }
        }
        public void AddProductZoneRoutingProductHistory(RoutingProductHistoryBySource routingProductHistoryBySource, int sellingProductId, string zoneName)
        {
            IEnumerable<SaleZoneRoutingProduct> productZoneRoutingProducts = _reader.GetZoneRoutingProducts(sellingProductId, zoneName);

            if (productZoneRoutingProducts != null && productZoneRoutingProducts.Count() > 0)
            {
                IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> productZoneRoutingProductHistory = productZoneRoutingProducts.MapRecords(ZoneRoutingProductHistoryUtilities.ZoneRoutingProductMapperFunc);
                routingProductHistoryBySource.AddRoutingProductHistoryRange(SaleEntityZoneRoutingProductSource.ProductZone, productZoneRoutingProductHistory);
            }
        }
        #endregion
    }
}
