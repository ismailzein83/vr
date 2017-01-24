'use strict';

app.directive('retailBeProductfamilyGrid', ['VRNotificationService', 'Retail_BE_ProductFamilyAPIService', 'Retail_BE_ProductFamilyService',
    function (VRNotificationService, Retail_BE_ProductFamilyAPIService, Retail_BE_ProductFamilyService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProductFamilyGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/ProductFamily/Templates/ProductFamilyGridTemplate.html'
        };

        function ProductFamilyGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.productFamily = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_ProductFamilyAPIService.GetFilteredProductFamilies(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onProductFamilyAdded = function (addedProductFamily) {
                    gridAPI.itemAdded(addedProductFamily);
                };

                api.onProductFamilyUpdated = function (updatedProductFamily) {
                    gridAPI.itemUpdated(updatedProductFamily);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editProductFamily
                    //haspermission: hasEditProductFamilyPermission
                });
            }
            function editProductFamily(productFamilyItem) {
                var onProductFamilyUpdated = function (updatedProductFamily) {
                    gridAPI.itemUpdated(updatedProductFamily);
                };

                Retail_BE_ProductFamilyService.editProductFamily(productFamilyItem.Entity.ProductFamilyId, onProductFamilyUpdated);
            }
            //function hasEditProductFamilyPermission() {
            //    return Retail_BE_ProductFamilyAPIService.HasUpdateProductFamilyPermission()
            //}
        }
    }]);
