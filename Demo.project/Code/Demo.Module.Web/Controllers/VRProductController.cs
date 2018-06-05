using Demo.Module.Business;
using Demo.Module.Entities.Product;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;


namespace Demo.Module.Web.Controllers
{
    
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRProduct")]
    public class VRProductController : BaseAPIController
    {
        [HttpPost]
        [Route("AddProduct")]
        public InsertOperationOutput<ProductDetails> AddProduct(Product product)
        {
            ProductManager productManager = new ProductManager();
            return productManager.AddProduct(product);

        }

        [HttpPost]
        [Route("UpdateProduct")]
        public UpdateOperationOutput<ProductDetails> UpdateProduct(Product product)
        {
            ProductManager universityManager = new ProductManager();
            return universityManager.UpdateProduct(product);
        }

        [HttpPost]
        [Route("GetFilteredProducts")]
        public object GetFilteredProducts(DataRetrievalInput<ProductQuery> input)
        {
            ProductManager productManager = new ProductManager();
            return GetWebResponse(input, productManager.GetFilteredProducts(input));
        }

        [HttpGet]
        [Route("DeleteProduct")]
        public DeleteOperationOutput<ProductDetails> DeleteProduct(int productId)
        {
            ProductManager productManager = new ProductManager();
            return productManager.Delete(productId);
        }

        [HttpGet]
        [Route("GetProductsInfo")]
        public IEnumerable<Demo.Module.Entities.Product.ProductInfo> GetProductsInfo()
        {
            ProductManager productManager = new ProductManager();
            return productManager.GetProductsInfo();
        }

        [HttpGet]
        [Route("GetProductById")]
        public Product GetProductById(int productId)
        {
            ProductManager productManager = new ProductManager();
            return productManager.GetProductById(productId);
        }
       
    }
}