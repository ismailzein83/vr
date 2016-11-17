"use strict";

app.directive("vrCommonCurrencyGrid", ["UtilsService", "VRNotificationService", "VRCommon_CurrencyAPIService","VRCommon_CurrencyService","VRCommon_CurrencyExchangeRateService", 'VRUIUtilsService',
function (UtilsService, VRNotificationService, VRCommon_CurrencyAPIService, VRCommon_CurrencyService, VRCommon_CurrencyExchangeRateService, VRUIUtilsService) {
  
    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
           
            var ctrl = this;
            var currencyGrid = new CurrencyGrid($scope, ctrl, $attrs);
            currencyGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/Currency/Templates/CurrencyGridTemplate.html"

    };

    function CurrencyGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;

        function initializeController() {
           
            $scope.currencies = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = VRCommon_CurrencyService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onCurrencyAdded = function (currencyObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(currencyObject);
                        gridAPI.itemAdded(currencyObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_CurrencyAPIService.GetFilteredCurrencies(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
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
            defineMenuActions();
        }                

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
               clicked: editCurrency
            }
            ];
        }

        function editCurrency(currencyObj) {
            var onCurrencyUpdated = function (currencyObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(currencyObj);
                gridAPI.itemUpdated(currencyObj);
            };
            VRCommon_CurrencyService.editCurrency(currencyObj.Entity.CurrencyId, onCurrencyUpdated);
        }
        
        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
