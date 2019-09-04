//    Copyright 2019 EPAM Systems, Inc.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

namespace Wooli.Foundation.Connect.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Sitecore.Commerce.Engine.Connect.Entities;
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Commerce.Entities.Prices;
    using Sitecore.Commerce.Services.Prices;
    using Sitecore.Data.Items;

    using Wooli.Foundation.Connect.Models;
    using Wooli.Foundation.DependencyInjection;

    [Service(typeof(ICatalogManager))]
    public class CatalogManager : ICatalogManager
    {
        private readonly IPricingManager pricingManager;

        private readonly IInventoryManager inventoryManager;

        public CatalogManager(
            IPricingManager pricingManager,
            IInventoryManager inventoryManager)
        {
            this.pricingManager = pricingManager;
            this.inventoryManager = inventoryManager;
        }

        public ManagerResponse<GetProductBulkPricesResult, bool> GetProductBulkPrices(List<Product> products)
        {
            if (products == null || !products.Any())
            {
                return new ManagerResponse<GetProductBulkPricesResult, bool>(new GetProductBulkPricesResult(), true);
            }
                
            string catalogName = products.Select((p => p.CatalogName)).First().ToString();
            var productIds = products.Select((p => p.ProductId));
            var productBulkPrices = this.pricingManager.GetProductBulkPrices(catalogName, productIds, null);
            IDictionary<string, Price> source = productBulkPrices == null || productBulkPrices.Result == null ? new Dictionary<string, Price>() : productBulkPrices.Result;
            foreach (Product product in products)
            {
                Price price;
                if (source.Any() && source.TryGetValue(product.ProductId, out price))
                {
                    CommercePrice commercePrice = (CommercePrice)price;
                    product.CurrencyCode = price.CurrencyCode;
                    product.ListPrice = new Decimal?(commercePrice.Amount);
                    product.AdjustedPrice = new Decimal?(commercePrice.ListPrice);
                }
            }
            return new ManagerResponse<GetProductBulkPricesResult, bool>(new GetProductBulkPricesResult(), true);
        }

        public void GetProductPrice(Product product)
        {
            if (product == null)
                return;

            bool includeVariants = product.Variants != null && product.Variants.Count > 0;
            ManagerResponse<GetProductPricesResult, IDictionary<string, Price>> productPrices = this.pricingManager.GetProductPrices(product.CatalogName, product.ProductId, includeVariants, null);
            if (productPrices == null || !productPrices.ServiceProviderResult.Success || productPrices.Result == null)
                return;

            if (productPrices.Result.TryGetValue(product.ProductId, out Price price))
            {
                var commercePrice = (CommercePrice)price;
                product.CurrencyCode = price.CurrencyCode;
                product.ListPrice = price.Amount;
                product.AdjustedPrice = commercePrice.ListPrice;
            }

            if (!includeVariants)
                return;

            foreach (Variant variant in product.Variants)
            {
                if (productPrices.Result.TryGetValue(variant.VariantId, out price))
                {
                    CommercePrice commercePrice = (CommercePrice)price;
                    variant.CurrencyCode = commercePrice.CurrencyCode;
                    variant.ListPrice = commercePrice.Amount;
                    variant.AdjustedPrice = commercePrice.ListPrice;
                }
            }
        }

        public decimal? GetProductRating(Item productItem)
        {
            if (decimal.TryParse(productItem["Rating"], out decimal result))
            {
                return result;
            }
                
            return null;
        }

        public void GetStockInfo(Product product, string shopName)
        {
            if (product == null)
            {
                return;
            }

            var inventortyProducts = new List<CommerceInventoryProduct>
            {
                new CommerceInventoryProduct
                {
                    CatalogName = product.CatalogName,
                    ProductId = product.ProductId
                }
            };

            if (product.Variants != null)
            {
                foreach (Variant productVariant in product.Variants)
                {
                    inventortyProducts.Add(new CommerceInventoryProduct
                    {
                        CatalogName = product.CatalogName,
                        ProductId = product.ProductId,
                        VariantId = productVariant.VariantId
                    });
                }
            }

            var stockInformationResponse = this.inventoryManager.GetStockInformation(shopName, inventortyProducts, StockDetailsLevel.StatusAndAvailability);
            if (!stockInformationResponse.ServiceProviderResult.Success || stockInformationResponse.Result == null)
            {
                return;
            }

            var stockInformationItems = stockInformationResponse.Result;
            foreach (StockInformation stockInformationItem in stockInformationItems)
            {
                if (stockInformationItem == null || stockInformationItem.Status == null)
                {
                    return;
                }

                var commerceInvertaryProduct = stockInformationItem.Product as CommerceInventoryProduct;
                string variantId = commerceInvertaryProduct?.VariantId;
                if (string.IsNullOrEmpty(variantId))
                {
                    product.StockStatus = stockInformationItem.Status;
                    product.StockStatusName = stockInformationItem.Status.Name;
                }
                else
                {
                    Variant variant = product.Variants?.FirstOrDefault(x => x.VariantId == variantId);
                    if (variant != null)
                    {
                        variant.StockStatus = stockInformationItem.Status;
                        variant.StockStatusName = stockInformationItem.Status.Name;
                    }
                }
            }
        }
    }
}
