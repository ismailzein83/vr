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

                    if (packages != undefined) {
                        for (var index = 0; index < packages.length; index++) {
                            var currentPackageItem = packages[index];
                            currentPackageItem = extendPackageItemObj(currentPackageItem)
                            $scope.scopeModel.packageItems.push({ Entity: currentPackageItem });
                        }
                    }

                    //var loadAccountFieldsPromise = loadAccountFields();
                    //promises.push(loadAccountFieldsPromise);

                    //loadAccountFieldsPromise.then(function () {

                    //    //Loading ColumnDefinitions Grid
                    //    if (accountGridDefinition != undefined && accountGridDefinition.ColumnDefinitions != undefined) {
                    //        for (var index in accountGridDefinition.ColumnDefinitions) {
                    //            if (index != "$type") {
                    //                var columnDefinition = accountGridDefinition.ColumnDefinitions[index];
                    //                extendColumnDefinitionObj(columnDefinition);
                    //                $scope.scopeModel.packageItems.push({ Entity: columnDefinition });
                    //            }
                    //        }
                    //    }
                    //});

                    //function loadAccountFields() {

                    //    return Retail_BE_AccountTypeAPIService.GetGenericFieldDefinitionsInfo().then(function (response) {
                    //        accountFields = response;
                    //    });
                    //}

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var packageItems;
                    if ($scope.scopeModel.packageItems.length > 0) {
                        packageItems = [];
                        for (var index = 0; index < $scope.scopeModel.packageItems.length; index++) {
                            var packageItem = $scope.scopeModel.packageItems[index].Entity;
                            packageItems.push(packageItem);
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
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.packageItems, updatedPackageItem.Entity.PackageName, 'Entity.PackageName');
                    $scope.scopeModel.packageItems[index] = { Entity: updatedPackageItem };
                };

                Retail_BE_ProductService.editProductPackageItem(packageItem.Entity, productDefinitionId, getExcludedPackageIds(), onPackageItemUpdated);
            }

            function extendPackageItemObj(packageItem) {
                if (packageItem == undefined)
                    return;

                return {
                    PackageId: packageItem.PackageId,
                    PackageName: packageNameByIds[packageItem.PackageId]
                }
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