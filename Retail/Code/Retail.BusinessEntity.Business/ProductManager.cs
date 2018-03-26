using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class ProductManager : BaseBusinessEntityManager
    {
        #region Ctor/Fields

        static ProductFamilyManager _productFamilyManager;
        public ProductManager()
        {
            _productFamilyManager = new ProductFamilyManager();
        }

        #endregion
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<ProductDetail> GetFilteredProducts(Vanrise.Entities.DataRetrievalInput<ProductQuery> input)
        {
            var allProducts = GetCachedProducts();
            var allowedProduct = _productFamilyManager.GetViewAllowedProductFamilies();

            Func<Product, bool> filterExpression = (product) =>
                {
                    if (input.Query != null && input.Query.Name != null && !product.Name.ToLower().Contains(input.Query.Name.ToLower()))
                        return false;

                    if (input.Query != null && input.Query.ProductFamilyId.HasValue && product.Settings != null
                        && input.Query.ProductFamilyId.Value != product.Settings.ProductFamilyId)
                        return false;
                    if (allowedProduct.Count > 0 && !allowedProduct.Contains(product.Settings.ProductFamilyId))
                        return false;
                    return true;
                };

            var resultProcessingHandler = new ResultProcessingHandler<ProductDetail>()
            {
                ExportExcelHandler = new ProductDetailExportExcelHandler()
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allProducts.ToBigResult(input, filterExpression, ProductDetailMapper), resultProcessingHandler);
        }

        public IEnumerable<Product> GetAllProducts()
        {
            Dictionary<int, Product> cachedProducts = this.GetCachedProducts();
            return cachedProducts.Values;
        }
        public Product GetProduct(int productId, bool isViewedFromUI)
        {
            Dictionary<int, Product> cachedProducts = this.GetCachedProducts();
           var product=cachedProducts.GetRecord(productId);
            if (product != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(new ProductLoggableEntity(_productFamilyManager.GetProductFamilyAccountBEDefinitionId(product.Settings.ProductFamilyId)), product);
            return product;
        }

        public Product GetProduct(int productId)
        { 
            return GetProduct(productId,false);
        }
        public string GetProductName(int productId)
        {
            Product product = this.GetProduct(productId);
            if (product == null)
                return null;
            return product.Name;
        }
        public ProductEditorRuntime GetProductEditorRuntime(int productId)
        {
            //var packageNameByIds = new Dictionary<int,string>();
            //var product = GetProduct(productId);

            //PackageManager packageManager = new PackageManager();

            //string packageName;
            //if (product != null && product.Settings != null && product.Settings.Packages != null)
            //{
            //    foreach (var packageItem in product.Settings.Packages.Values)
            //    {
            //        if (!packageNameByIds.TryGetValue(packageItem.PackageId, out packageName))
            //            packageNameByIds.Add(packageItem.PackageId, packageManager.GetPackageName(packageItem.PackageId));
            //    }
            //}

            //ProductEditorRuntime editorRuntime = new ProductEditorRuntime();
            //editorRuntime.PackageNameByIds = packageNameByIds;
            //editorRuntime.Entity = product;

            ProductEditorRuntime editorRuntime = new ProductEditorRuntime();
            editorRuntime.Entity = GetProduct(productId,true);

            return editorRuntime;
        }

        public Vanrise.Entities.InsertOperationOutput<ProductDetail> AddProduct(Product productItem)
        {
            ProductFamily productfamily = _productFamilyManager.GetProductFamily(productItem.Settings.ProductFamilyId);
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<ProductDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int productId = -1;

            IProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IProductDataManager>();

            if (dataManager.Insert(productItem, out productId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                productItem.ProductId = productId;
                VRActionLogger.Current.TrackAndLogObjectAdded(new ProductLoggableEntity(_productFamilyManager.GetProductFamilyAccountBEDefinitionId(productItem.Settings.ProductFamilyId)), productItem);
                insertOperationOutput.InsertedObject = ProductDetailMapper(productItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<ProductDetail> UpdateProduct(Product productItem)
        {
            ProductFamily productfamily = _productFamilyManager.GetProductFamily(productItem.Settings.ProductFamilyId);
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ProductDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IProductDataManager>();

            if (dataManager.Update(productItem))
            {

                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(new ProductLoggableEntity(_productFamilyManager.GetProductFamilyAccountBEDefinitionId(productItem.Settings.ProductFamilyId)), productItem);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ProductDetailMapper(this.GetProduct(productItem.ProductId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<Entities.ProductInfo> GetProductsInfo(ProductInfoFilter filter)
        {
            Func<Product, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (product) =>
                {
                    if (filter.Filters != null && !CheckIfFilterIsMatch(product, filter.Filters))
                        return false;

                    return true;
                };
            }

            return this.GetCachedProducts().MapRecords(ProductInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public IEnumerable<int> GetProductPackageIds(int productId)
        {
            Product product = new ProductManager().GetProduct(productId);
            if (product == null)
                throw new NullReferenceException(string.Format("product of productId: {0}",productId));

            if (product.Settings == null)
                throw new NullReferenceException(string.Format("product.Settings of productId: {0}", productId));

            ProductFamily ProductFamily = new ProductFamilyManager().GetProductFamily(product.Settings.ProductFamilyId);

            if (ProductFamily == null || ProductFamily.Settings == null || ProductFamily.Settings.Packages == null || ProductFamily.Settings.Packages.Count == 0)
                return null;

            return ProductFamily.Settings.Packages.Values.Select(itm => itm.PackageId);
        }
        
        public int? GetProductFamilyId(int productId)
        {
            var product = GetProduct(productId);
            return product != null && product.Settings != null ? product.Settings.ProductFamilyId : (int?)null;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IProductDataManager _dataManager = BEDataManagerFactory.GetDataManager<IProductDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreProductUpdated(ref _updateHandle);
            }
        }
        private class ProductLoggableEntity : VRLoggableEntityBase
        {
           
            Guid _accountDefinitionId;
            static AccountBEDefinitionManager _accountBEDefintionManager = new AccountBEDefinitionManager();
            static ProductManager s_productManager = new ProductManager();
            
            public ProductLoggableEntity(Guid accountDefinitionId)
            {
                _accountDefinitionId = accountDefinitionId;
              
            }

            public override string EntityUniqueName
            {
                get { return String.Format("Retail_BusinessEntity_Product_{0}", _accountDefinitionId); }
            }

            public override string EntityDisplayName
            {
                get
                {
                    return String.Format(_accountBEDefintionManager.GetAccountBEDefinitionName(_accountDefinitionId),"_Products" ); 
                   
                }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "Retail_BusinessEntity_Product_ViewHistoryItem"; }
            }


            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                Product product = context.Object.CastWithValidate<Product>("context.Object");
                return product.ProductId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                Product product = context.Object.CastWithValidate<Product>("context.Object");
                return s_productManager.GetProductName(product.ProductId);
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }
        }

        private class ProductDetailExportExcelHandler : ExcelExportHandler<ProductDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<ProductDetail> context)
            {
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Products",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Name" });
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.ProductId });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Name });
                            sheet.Rows.Add(row);
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<int, Product> GetCachedProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetProducts",
               () =>
               {
                   IProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IProductDataManager>();
                   return dataManager.GetProducts().ToDictionary(x => x.ProductId, x => x);
               });
        }

        private bool CheckIfFilterIsMatch(Product product, List<IProductFilter> filters)
        {
            ProductFilterContext context = new ProductFilterContext { Product = product };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        #endregion

        #region Mappers

        public ProductDetail ProductDetailMapper(Product product)
        {
            
           
            ProductDetail productDetail = new ProductDetail()
            {
                AccountBEDefinitionId = _productFamilyManager.GetProductFamilyAccountBEDefinitionId(product.Settings.ProductFamilyId),
                Entity = product
            };
            bool allowEdit = true;
            if (product != null && product.Settings != null)
                allowEdit = _productFamilyManager.DoesUserHaveEditProductDefinitions(product.Settings.ProductFamilyId);
            productDetail.AllowEdit = allowEdit;
            return productDetail;
        }

        public Entities.ProductInfo ProductInfoMapper(Product product)
        {
            Entities.ProductInfo productInfo = new Entities.ProductInfo()
            {
                ProductId = product.ProductId,
                Name = product.Name
            };
            return productInfo;
        }

        #endregion

        #region IBusinessEntityManager

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetProductName(Convert.ToInt32(context.EntityId));
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetProduct(context.EntityId);
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var product = context.Entity as Product;
            return product.ProductId;
        }

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetAllProducts().Select(itm => itm as dynamic).ToList();
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
