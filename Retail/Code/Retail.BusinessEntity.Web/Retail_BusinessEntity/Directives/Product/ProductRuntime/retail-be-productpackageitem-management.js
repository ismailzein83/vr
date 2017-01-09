(function (app) {

    'use strict';

    ProductPackageItemManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_ProductService'];

    function ProductPackageItemManagementDirective(UtilsService, VRNotificationService, Retail_BE_ProductService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PackageItemManagementCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Product/ProductRuntime/Templates/ProductPackageItemManagementTemplate.html'
        };

        function PackageItemManagementCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var productDefinitionId;
            var packageNameByIds;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.packageItems = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddPackageItem = function () {
                    var onPackageItemAdded = function (addedPackageItem) {
                        $scope.scopeModel.packageItems.push({ Entity: addedPackageItem });
                    };

                    Retail_BE_ProductService.addProductPackageItem(productDefinitionId, getExcludedPackageIds(), onPackageItemAdded);
                };
                $scope.scopeModel.onDeletePackageItem = function (deletedPackageItem) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.packageItems, deletedPackageItem.Entity.PackageName, 'Entity.PackageName');
                            $scope.scopeModel.packageItems.splice(index, 1);
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
                    var packageItemsByPriority = {};
                    if (packages != undefined) {
                        for (var key in packages) {
                            if (key != "$type") {
                                var currentPackageItem = packages[key];
                                currentPackageItem = extendPackageItemObj(currentPackageItem);
                                packageItemsByPriority[currentPackageItem.Priority] = currentPackageItem;
                            }
                        }
                    }
                    for (var priority in packageItemsByPriority) {
                        $scope.scopeModel.packageItems.push({ Entity: packageItemsByPriority[priority] });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var packageItems;
                    if ($scope.scopeModel.packageItems.length > 0) {
                        packageItems = {};
                        var priority = 1;
                        for (var index = 0; index < $scope.scopeModel.packageItems.length; index++) {
                            var packageItem = $scope.scopeModel.packageItems[index].Entity;
                            packageItem.Priority = priority++;
                            packageItems[packageItem.PackageId] = packageItem;
                        }
                    }
                    return packageItems;
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
            function editPackageItem(packageItem) {
                var onPackageItemUpdated = function (updatedPackageItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.packageItems, updatedPackageItem.PackageName, 'Entity.PackageName');
                    $scope.scopeModel.packageItems[index] = { Entity: updatedPackageItem };
                };

                Retail_BE_ProductService.editProductPackageItem(packageItem.Entity, productDefinitionId, getExcludedPackageIds(), onPackageItemUpdated);
            }

            function extendPackageItemObj(packageItem) {
                if (packageItem == undefined)
                    return;

                packageItem.PackageName = packageNameByIds[packageItem.PackageId];
                return packageItem;
            }
            function getExcludedPackageIds() {
                var packageItems = $scope.scopeModel.packageItems;
                if (packageItems.length == 0)
                    return;

                var excludedPackageIds = [];
                for (var i = 0; i < packageItems.length; i++) {
                    var currentPackageItem = packageItems[i].Entity;
                    excludedPackageIds.push(currentPackageItem.PackageId);
                }
                return excludedPackageIds;
            }
        }
    }

    app.directive('retailBeProductpackageitemManagement', ProductPackageItemManagementDirective);

})(app);