using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Business
{
    public class ProductServiceManager
    {
        public static Guid _definitionId = new Guid("6222BD42-668C-4574-86B5-A71A1F21B623");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public List<ProductService> GetAllProductServices()
        {
            return GetCachedProductServices().Values.ToList();
        }


        private Dictionary<Guid, ProductService> GetCachedProductServices()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedProductServices", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, ProductService> result = new Dictionary<Guid, ProductService>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        ProductService productService = new ProductService()
                        {
                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

                        };
                        result.Add(productService.ID, productService);
                    }
                }

                return result;
            });
        }
        public ProductService GetProductServiceById(Guid productServiceId)
        {
            var products = GetCachedProductServices();
            return products.GetRecord(productServiceId);
        }
        public IEnumerable<ProductServiceDetail> GetProductServicesInfo(ProductServiceInfoFilter filter)
        {
            var productServices = GetCachedProductServices();
            Func<ProductService, bool> filterFunc = (productService) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new ProductServiceFilterContext
                        {
                            ProductService = productService
                        };
                        foreach (var productServiceFilter in filter.Filters)
                        {
                            if (!productServiceFilter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return productServices.MapRecords((record) =>
            {
                return ProductServiceInfoMapper(record);
            }, filterFunc);

        }
        private ProductServiceDetail ProductServiceInfoMapper(ProductService productService)
        {
            return new ProductServiceDetail
            {
                ID = productService.ID,
                Name = productService.Name
            };
        }


    }
}
