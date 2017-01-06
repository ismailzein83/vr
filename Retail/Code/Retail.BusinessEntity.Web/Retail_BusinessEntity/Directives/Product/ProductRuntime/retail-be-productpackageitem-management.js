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

            var context;
            //var accountFields;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.packageItems = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddPackageItem = function () {
                    var onPackageAdded = function (addedPackageItem) {
                        //extendColumnDefinitionObj(addedColumnDefinition);
                        $scope.scopeModel.packageItems.push({ Entity: addedPackageItem });
                    };

                    Retail_BE_ProductService.addProductPackageItem(getContext(), onPackageAdded);
                };
                $scope.scopeModel.onDeletePackageItem = function (columnDefinition) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.packageItems, columnDefinition.Entity.FieldName, 'Entity.FieldName');
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
                        packages = payload.Packages;
                        context = payload.context;
                    }

                    if (packages != undefined)
                        $scope.scopeModel.packageItems = packages;

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
                        for (var i = 0; i < $scope.scopeModel.packageItems.length; i++) {
                            var columnDefinition = $scope.scopeModel.packageItems[i].Entity;
                            packageItems.push(columnDefinition);
                        }
                    }

                    return {
                        ColumnDefinitions: packageItems
                    };
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
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.packageItems, columnDefinition.Entity.FieldName, 'Entity.FieldName');
                    //extendColumnDefinitionObj(updatedPackageItem);
                    $scope.scopeModel.packageItems[index] = { Entity: updatedPackageItem };
                };

                Retail_BE_ProductService.editProductPackageItem(packageItem.Entity, getContext(), onPackageItemUpdated);
            }

            function getContext(){
                return context;
            }

            //function extendColumnDefinitionObj(columnDefinition) {
            //    if (accountFields == undefined)
            //        return;

            //    for (var index = 0; index < accountFields.length; index++) {
            //        var currentAccountField = accountFields[index];
            //        if (columnDefinition.FieldName == currentAccountField.Name) {
            //            columnDefinition.FieldTitle = currentAccountField.Title;
            //            return;
            //        }
            //    }
            //}
        }
    }

    app.directive('retailBeProductpackageitemManagement', ProductPackageItemManagementDirective);

})(app);