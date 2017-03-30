"use strict";

app.directive("vrCommonCurrencyexchangerateGrid", ["UtilsService", "VRNotificationService", "VRCommon_CurrencyExchangeRateAPIService", "VRCommon_CurrencyExchangeRateService",
function (UtilsService, VRNotificationService, VRCommon_CurrencyExchangeRateAPIService, VRCommon_CurrencyExchangeRateService) {
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
                        if (isChildgrid != undefined) {
                            $scope.hidecurrencycolumn = true;
                        }
                           
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onExchangeRateAdded = function (exchangeRateObject) {
                        gridAPI.itemAdded(exchangeRateObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                if (dataRetrievalInput.Query.ExchangeDate == undefined || dataRetrievalInput.Query.ExchangeDate == null) {
                    dataRetrievalInput.IsSortDescending = true;
                    dataRetrievalInput.SortByColumnName = "Entity.ExchangeDate";
                }
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
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editCurrencyExchangeRate,
                haspermission: hasEditCurrencyExchangeRateId
            }];
        }

        function editCurrencyExchangeRate(dataItem) {
            var onCurrencyExchangeRateUpdated = function (updatedObj) {
                gridAPI.itemUpdated(updatedObj);
            };
            VRCommon_CurrencyExchangeRateService.editExchangeRate(dataItem.Entity.CurrencyExchangeRateId, onCurrencyExchangeRateUpdated);
        }

        function hasEditCurrencyExchangeRateId() {
            return VRCommon_CurrencyExchangeRateAPIService.HasEditCurrencyExchangeRatePermission();
        }
    }

    return directiveDefinitionObject;

}]);
