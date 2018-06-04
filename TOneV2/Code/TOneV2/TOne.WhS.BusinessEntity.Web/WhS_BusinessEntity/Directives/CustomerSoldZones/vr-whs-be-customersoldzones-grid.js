"use strict";

app.directive("vrWhsBeCustomersoledzonesGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "WhS_BE_CustomerSoldZones", "UISettingsService", function (UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_CustomerSoldZones, UISettingsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var saleZoneGrid = new CustomerSoldZonesGrid($scope, ctrl, $attrs);
            saleZoneGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CustomerSoldZones/Templates/CustomerSoldZonesGridTemplate.html"
    };

    function CustomerSoldZonesGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var gridAPI;
        var gridDrillDownTabsObj;
        function initializeController() {

            $scope.customerSoldZones = [];
            $scope.longPrecision = UISettingsService.getLongPrecision();
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = getDrillDownDefinitions();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                defineAPI();
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_CustomerSoldZones.GetFilteredCustomerSoldZones(dataRetrievalInput).then(function (response) {
                    if (response && response.Data) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var dataItem = response.Data[i]
                            gridDrillDownTabsObj.setDrillDownExtensionObject(dataItem);
                            for (var j = 0; j < dataItem.CustomerZones.length; j++) {
                                setService(dataItem.CustomerZones[j]);
                            }
                        }
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            defineMenuActions();
        }

        function defineAPI() {
            var api = {};

            api.loadGrid = function (query) {
                return gridAPI.retrieveData(query);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getDrillDownDefinitions() {

            var drillDownDefinitions = [];

            var saleCodeDrillDownDefinition = {
                title: "Sale Codes",
                directive: 'vr-whs-be-salecode-grid',
                loadDirective: function (saleCodeGridAPI, dataItem) {
                    dataItem.saleCodeGridAPI = saleCodeGridAPI;
                    var queryHandler = {
                        $type: "TOne.WhS.BusinessEntity.Business.SaleCodeQueryHandler, TOne.WhS.BusinessEntity.Business"
                    };
                    queryHandler.Query = {
                        ZonesIds: [dataItem.ZoneId],
                        EffectiveOn: dataItem.EffectiveOn
                    };
                    var saleCodeGridPayload = {
                        queryHandler: queryHandler,
                        hidesalezonecolumn: true
                    };
                    return dataItem.saleCodeGridAPI.loadGrid(saleCodeGridPayload);
                }
            };

            drillDownDefinitions.push(saleCodeDrillDownDefinition);
            return drillDownDefinitions;
        }

        function defineMenuActions() {

            $scope.gridMenuActions = [];
        }

        function setService(item) {
            item.serviceViewerLoadDeferred = UtilsService.createPromiseDeferred();
            item.onServiceViewerReady = function (api) {
                item.serviceViewerAPI = api;
                var serviceViewerPayload = { selectedIds: item !=undefined ? item.Services : undefined };
                VRUIUtilsService.callDirectiveLoad(item.serviceViewerAPI, serviceViewerPayload, item.serviceViewerLoadDeferred);
            };
        }
    }
}]);
