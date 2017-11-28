"use strict";

app.directive("vrCpZoneroutingproductpreviewGrid", ["WhS_CP_CodePreparationPreviewAPIService", 'UtilsService', "VRNotificationService","WhS_CP_ZoneRoutingProductChangeTypeEnum","VRUIUtilsService",
function (WhS_CP_CodePreparationPreviewAPIService, UtilsService, VRNotificationService, WhS_CP_ZoneRoutingProductChangeTypeEnum, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var zoneRoutingProdcutPreviewGrid = new ZoneRoutingProdcutPreviewGrid($scope, ctrl, $attrs);
            zoneRoutingProdcutPreviewGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_CodePreparation/Directives/ZoneRoutingProductPreview/Templates/CodePreparationZoneRoutingProductPreviewGrid.html"

    };

    function ZoneRoutingProdcutPreviewGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.zonesRoutingProductsPreview = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.load = function (payload) {
                        $scope.isSubGrid = payload.isSubGrid;
                        var query={
                        ProcessInstanceId:payload.ProcessInstanceId,
                        ZoneName:payload.ZoneName,
                        OnlyModified:payload.OnlyModified,
};
                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };


            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_CP_CodePreparationPreviewAPIService.GetFilteredZonesRoutingProductsPreview(dataRetrievalInput)
                    .then(function (response) {
                        for (var i = 0; i < response.Data.length; i++) {
                            mapDataNeeded(response.Data[i]);
                            setRPService(response.Data[i]);
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            function mapDataNeeded(dataItem) {

                switch (dataItem.Entity.ChangeType) {
                    case WhS_CP_ZoneRoutingProductChangeTypeEnum.New.value:
                        dataItem.codeStatusIconUrl = WhS_CP_ZoneRoutingProductChangeTypeEnum.New.icon;
                        dataItem.codeStatusIconTooltip = WhS_CP_ZoneRoutingProductChangeTypeEnum.New.label;
                        break;
                }
            }

            function setRPService(item) {
                item.RPserviceViewerLoadDeferred = UtilsService.createPromiseDeferred();
                item.onRPServiceViewerReady = function (api) {
                    item.serviceViewerAPI = api;
                    var routingProductserviceViewerPayload = { selectedIds: item.RoutingProductServicesIds };
                    VRUIUtilsService.callDirectiveLoad(item.serviceViewerAPI, routingProductserviceViewerPayload, item.RPserviceViewerLoadDeferred);
                };
            }
        }
    }
    return directiveDefinitionObject;

}]);
