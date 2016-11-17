"use strict";

app.directive("vrCommonCurrencyexchangerateGrid", ["UtilsService", "VRNotificationService", "VRCommon_CurrencyExchangeRateAPIService", "VRCommon_CurrencyService",
function (UtilsService, VRNotificationService, VRCommon_CurrencyExchangeRateAPIService, VRCommon_CurrencyService) {
    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            $scope.hidecurrencycolumn ;
            var ctrl = this;
            var grid = new CurrencyExhangeRateGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/CurrencyExchangeRate/Templates/CurrencyExchangeRateGridTemplate.html"

    };

    function CurrencyExhangeRateGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var disableCurrency;
        this.initializeController = initializeController;

        function initializeController() {
          
            if ($attrs.hidecurrencycolumn != undefined) {
                $scope.hidecurrencycolumn = disableCurrency = true;
            }
            $scope.currenciesExchangeRate = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query, isChildgrid) {
                        if (isChildgrid != undefined)
                            $scope.hidecurrencycolumn = true;
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onExchangeRateAdded = function (exchangeRateObject) {
                        gridAPI.itemAdded(exchangeRateObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_CurrencyExchangeRateAPIService.GetFilteredExchangeRateCurrencies(dataRetrievalInput)
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
            $scope.gridMenuActions = [ ];
        }

        
        
        
    }

    return directiveDefinitionObject;

}]);
