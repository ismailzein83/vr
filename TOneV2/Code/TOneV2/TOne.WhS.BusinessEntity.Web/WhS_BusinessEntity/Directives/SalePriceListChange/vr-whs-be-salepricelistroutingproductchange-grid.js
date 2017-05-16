"use strict";

app.directive("vrWhsBeSalepricelistroutingproductchangeGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SalePriceListChangeAPIService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, WhS_BE_SalePriceListChangeAPIService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new RPChangeGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListChange/Templates/SalePriceListRoutingProductChangeTemplate.html"
    };

    function RPChangeGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.RPChange = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SalePriceListChangeAPIService.GetFilteredSalePriceListRPChanges(dataRetrievalInput)
                    .then(function (response) {
                        if (response != undefined && response.Data != null) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var item = response.Data[i];
                                setRPService(item);
                                setRecentPService(item);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };
        }
        function setRPService(item) {
            item.RPserviceViewerLoadDeferred = UtilsService.createPromiseDeferred();
            item.onRPServiceViewerReady = function (api) {
                item.serviceViewerAPI = api;
                var routingProductserviceViewerPayload = { selectedIds: item.RoutingProductServicesId };
                VRUIUtilsService.callDirectiveLoad(item.serviceViewerAPI, routingProductserviceViewerPayload, item.RPserviceViewerLoadDeferred);
            };
        }
        function setRecentPService(item) {
            item.RecentRPserviceViewerLoadDeferred = UtilsService.createPromiseDeferred();
            item.onRecentRPServiceViewerReady = function (api) {
                item.serviceViewerAPI = api;
                var routingProductserviceViewerPayload = { selectedIds: item.RecentRouringProductServicesId };
                VRUIUtilsService.callDirectiveLoad(item.serviceViewerAPI, routingProductserviceViewerPayload, item.RecentRPserviceViewerLoadDeferred);
            };
        }
    }
    return directiveDefinitionObject;
}]);
