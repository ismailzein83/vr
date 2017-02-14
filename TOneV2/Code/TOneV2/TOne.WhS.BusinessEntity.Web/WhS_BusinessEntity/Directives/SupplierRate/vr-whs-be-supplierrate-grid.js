"use strict";

app.directive("vrWhsBeSupplierrateGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SupplierRateAPIService", "VRUIUtilsService", "FileAPIService",
function (UtilsService, VRNotificationService, WhS_BE_SupplierRateAPIService, VRUIUtilsService, fileApiService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SupplierRateGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SupplierRate/Templates/SupplierRateGridTemplate.html"

    };

    function SupplierRateGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var drillDownManager;
        var effectiveOn;
        var supplierId;
        var isExpandable;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.supplierrates = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(getDirectiveTabs(), gridAPI);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (input) {
                        isExpandable = input.Query.IsChild;
                        effectiveOn = input.Query.EffectiveOn;
                        supplierId = input.Query.SupplierId;
                        return gridAPI.retrieveData(input);
                    };

                    return directiveAPI;
                }
            };
            $scope.isExpandable = function () {
                return !isExpandable;
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SupplierRateAPIService.GetFilteredSupplierRates(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++)
                                drillDownManager.setDrillDownExtensionObject(response.Data[i]);

                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Download",
                clicked: downloadPriceList
            }];
        }
        function downloadPriceList(priceListObj) {
            fileApiService.DownloadFile(priceListObj.Entity.PriceListFileId)
                    .then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });
        }
        function getDirectiveTabs() {
            var directiveTabs = [];

            var otherRatesTab = {
                title: "Other Rates",
                directive: "vr-whs-be-supplierotherrate-grid",
                loadDirective: function (directiveAPI, rateDataItem) {
                    rateDataItem.otherRateGridAPI = directiveAPI;

                    var otherRateGridPayload = {
                        ZoneId: rateDataItem.Entity.ZoneId,
                        EffectiveOn: effectiveOn
                    };
                    return rateDataItem.otherRateGridAPI.loadGrid(otherRateGridPayload);
                }
            };

            directiveTabs.push(otherRatesTab);

            var historyRatesTab = {
                title: "History",
                directive: "vr-whs-be-supplierrate-grid",
                loadDirective: function (directiveApi, rateDataItem) {
                    rateDataItem.historygRateGridAPI = directiveApi;
                    var historyRateGridPayload = {
                        $type: "TOne.WhS.BusinessEntity.Business.SupplierRateHistoryQueryHandler,TOne.WhS.BusinessEntity.Business",
                        Query: {
                            SupplierZoneName: rateDataItem.SupplierZoneName,
                            SupplierId: supplierId,
                            IsChild: true
                        }
                    };
                    return rateDataItem.historygRateGridAPI.loadGrid(historyRateGridPayload);
                }
            };
            directiveTabs.push(historyRatesTab);

            return directiveTabs;
        }
    }

    return directiveDefinitionObject;

}]);
