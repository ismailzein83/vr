(function (app) {

    'use strict';

    ProductFamilyPackageItemManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_ProductFamilyService'];

    function ProductFamilyPackageItemManagementDirective(UtilsService, VRNotificationService, Retail_BE_ProductFamilyService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProductFamilyPackageItemManagementCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/ProductFamily/Templates/ProductFamilyPackageItemManagementTemplate.html'
        };

        function ProductFamilyPackageItemManagementCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var productDefinitionId;
            var packageNameByIds;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.productFamilyPackageItems = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddProductFamilyPackageItem = function () {
                    var onProductFamilyPackageItemAdded = function (addedProductFamilyPackageItem) {
                        $scope.scopeModel.productFamilyPackageItems.push({ Entity: addedProductFamilyPackageItem });
                    };

                    Retail_BE_ProductFamilyService.addProductFamilyPackageItem(productDefinitionId, getExcludedPackageIds(), onProductFamilyPackageItemAdded);
                };
                $scope.scopeModel.onDeleteProductFamilyPackageItem = function (deletedProductFamilyPackageItem) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.productFamilyPackageItems, deletedProductFamilyPackageItem.Entity.PackageName, 'Entity.PackageName');
                            $scope.scopeModel.productFamilyPackageItems.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var packages;

                    if (payload != undefined) {
                        productDefinitionId = payload.productDefinitionId;
                        packageNameByIds = payload.packageNameByIds;
                        packages = payload.packages;
                    }

                    //Loading Grid
                    var productFamilyPackageItemsByPriority = {};
                    if (packages != undefined) {
                        for (var key in packages) {
                            if (key != "$type") {
                                var currentPackageItem = packages[key];
                                currentPackageItem = extendProductFamilyPackageItemObj(currentPackageItem);
                                productFamilyPackageItemsByPriority[currentPackageItem.Priority] = currentPackageItem;
                            }
                        }
                    }
                    for (var priority in productFamilyPackageItemsByPriority) {
                        $scope.scopeModel.productFamilyPackageItems.push({ Entity: productFamilyPackageItemsByPriority[priority] });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var productFamilyPackageItems;
                    if ($scope.scopeModel.productFamilyPackageItems.length > 0) {
                        productFamilyPackageItems = {};
                        var priority = 1;
                        for (var index = 0; index < $scope.scopeModel.productFamilyPackageItems.length; index++) {
                            var packageItem = $scope.scopeModel.productFamilyPackageItems[index].Entity;
                            packageItem.Priority = priority++;
                            productFamilyPackageItems[packageItem.PackageId] = packageItem;
                        }
                    }
                    return productFamilyPackageItems;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editPackageItem
                }];
            }
            function editPackageItem(productFamilyPackageItem) {
                var onProductFamilyPackageItemUpdated = function (updatedProductFamilyPackageItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.productFamilyPackageItems, updatedProductFamilyPackageItem.PackageName, 'Entity.PackageName');
                    $scope.scopeModel.productFamilyPackageItems[index] = { Entity: updatedProductFamilyPackageItem };
                };

                Retail_BE_ProductFamilyService.editProductFamilyPackageItem(productFamilyPackageItem.Entity, productDefinitionId, getExcludedPackageIds(), onProductFamilyPackageItemUpdated);
            }

            function extendProductFamilyPackageItemObj(productFamilyPackageItem) {
                if (productFamilyPackageItem == undefined)
                    return;

                productFamilyPackageItem.PackageName = packageNameByIds[productFamilyPackageItem.PackageId];
                return productFamilyPackageItem;
            }
            function getExcludedPackageIds() {
                var productFamilyPackageItems = $scope.scopeModel.productFamilyPackageItems;
                if (productFamilyPackageItems.length == 0)
                    return;

                var excludedPackageIds = [];
                for (var i = 0; i < productFamilyPackageItems.length; i++) {
                    var currentPackageItem = productFamilyPackageItems[i].Entity;
                    excludedPackageIds.push(currentPackageItem.PackageId);
                }
                return excludedPackageIds;
            }
        }
    }

    app.directive('retailBeProductfamilypackageitemManagement', ProductFamilyPackageItemManagementDirective);

})(app);