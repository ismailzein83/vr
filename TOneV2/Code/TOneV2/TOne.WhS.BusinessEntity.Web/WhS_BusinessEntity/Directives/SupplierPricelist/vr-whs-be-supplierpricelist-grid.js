"use strict";

app.directive("vrWhsBeSupplierpricelistGrid", ["UtilsService", "VRNotificationService", "FileAPIService", "WhS_BE_SupplierPriceListAPIService", "WhS_BE_SupplierPriceListService",
function (UtilsService, VRNotificationService, FileAPIService, WhS_BE_SupplierPriceListAPIService, WhS_BE_SupplierPriceListService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var supplierPriceListGrid = new SupplierPriceListGrid($scope, ctrl, $attrs);
            supplierPriceListGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SupplierPricelist/Templates/SupplierPricelistGridTemplate.html"

    };

    function SupplierPriceListGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var disabCountry;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.supplierPriceLists = [];
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
                return WhS_BE_SupplierPriceListAPIService.GetFilteredSupplierPricelist(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }



        function defineMenuActions() {
            $scope.gridMenuActions = function (dataItem) {
                var menuActions = [];
                var processInstanceId = dataItem.Entity.ProcessInstanceId;

                if (dataItem.Entity.FileId !== 0 && dataItem.Entity.FileId!=null) {
                    var downloadPricelistAction = {
                        name: "Download",
                        clicked: downloadPriceList
                    };
                    menuActions.push(downloadPricelistAction);
                }

                if (processInstanceId != null) {
                    var additionalMenuActions = WhS_BE_SupplierPriceListService.getAdditionalActionOfSupplierPricelistGrid();
                    for (var i = 0, length = additionalMenuActions.length; i < length; i++) {
                        var additionalMenuAction = additionalMenuActions[i];
                        var menuAction = {
                                name: additionalMenuAction.name,
                                clicked: function (dataItem) {
                                var payload = {
                                    processInstanceId: dataItem.Entity.ProcessInstanceId,
                                    fileId: dataItem.Entity.FileId,
                                    supplierPricelistType: dataItem.Entity.PricelistType,
                                    pricelistDate: dataItem.Entity.EffectiveOn,
                                    currencyId: dataItem.Entity.CurrencyId
                                };
                                additionalMenuAction.clicked(payload);
                        }
                    };
                        menuActions.push(menuAction);
                }
            }

                return menuActions;
        };
    }

        function downloadPriceList(priceListObj) {
            FileAPIService.DownloadFile(priceListObj.Entity.FileId)
                    .then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
        });
    }

}

    return directiveDefinitionObject;

}]);
