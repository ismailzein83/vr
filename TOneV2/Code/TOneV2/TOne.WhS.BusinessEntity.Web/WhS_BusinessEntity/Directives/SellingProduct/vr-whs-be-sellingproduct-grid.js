"use strict";

app.directive("vrWhsBeSellingproductGrid", ["VRCommon_ObjectTrackingService", "UtilsService", "VRNotificationService", "WhS_BE_SellingProductAPIService", "WhS_BE_SellingProductService", "VRUIUtilsService",
function (VRCommon_ObjectTrackingService, UtilsService, VRNotificationService, WhS_BE_SellingProductAPIService, WhS_BE_SellingProductService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var sellingProductGrid = new SellingProductGrid($scope, ctrl);
            sellingProductGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SellingProduct/Templates/SellingProductGridTemplate.html"
    };

    function SellingProductGrid($scope, ctrl) {

        this.initializeController = initializeController;

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();
        var gridDrillDownTabsObj;

        function initializeController() {
            $scope.sellingProducts = [];

            defineMenuActions();

            $scope.gridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = getDrillDownDefinitions();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                gridReadyDeferred.resolve();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SellingProductAPIService.GetFilteredSellingProducts(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            UtilsService.waitMultiplePromises([gridReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {
            var api = {};

            api.loadGrid = function (query) {
                return gridAPI.retrieveData(query);
            };

            api.onSellingProductAdded = function (sellingProductObject) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(sellingProductObject);
                gridAPI.itemAdded(sellingProductObject);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getDrillDownDefinitions() {
            var drillDownDefinitions = [];

            AddObjectTrackingDrillDownDefinition();

            function AddObjectTrackingDrillDownDefinition() {
                var objectTrackingDrillDownDefinition = {
                    title: VRCommon_ObjectTrackingService.getObjectTrackingGridTitle(),
                    directive: 'vr-common-objecttracking-grid',
                    loadDirective: function (directiveAPI, sellingProductItem) {
                        sellingProductItem.objectTrackingGridAPI = directiveAPI;
                        var query = {
                            ObjectId: sellingProductItem.Entity.SellingProductId,
                            EntityUniqueName: WhS_BE_SellingProductService.getEntityUniqueName()
                        };
                        return sellingProductItem.objectTrackingGridAPI.load(query);
                    }
                };
                drillDownDefinitions.push(objectTrackingDrillDownDefinition);
            }

            return drillDownDefinitions;
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editSellingProduct,
                haspermission: hasUpdateSellingProductPermission
            }];
        }
        function editSellingProduct(sellingProductObj) {
            var onSellingProductUpdated = function (sellingProduct) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(sellingProduct);
                gridAPI.itemUpdated(sellingProduct);
            };

            WhS_BE_SellingProductService.editSellingProduct(sellingProductObj.Entity, onSellingProductUpdated);
        }
        function hasUpdateSellingProductPermission() {
            return WhS_BE_SellingProductAPIService.HasUpdateSellingProductPermission();
        }
    }

    return directiveDefinitionObject;
}]);