
app.service('VRCommon_CurrencyExchangeRateService', ['UtilsService', 'VRModalService', 'VRNotificationService', 'VRCommon_CurrencyService',
    function (UtilsService, VRModalService, VRNotificationService, VRCommon_CurrencyService) {

        function addExchangeRate(onCurrencyExchangeRateAdded, currencyId) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForAddEditor("Currency Exchange Rate");
                modalScope.onCurrencyExchangeRateAdded = onCurrencyExchangeRateAdded; 
            };
            var parameters = {};

            if (currencyId != undefined)
                parameters.CurrencyId = currencyId;
            VRModalService.showModal('/Client/Modules/Common/Views/CurrencyExchangeRate/CurrencyExchangeRateEditor.html', parameters, settings);
        }
        function editExchangeRate(currencyExchangeRateId, onCurrencyExchangeRateUpdated) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCurrencyExchangeRateUpdated = onCurrencyExchangeRateUpdated;
            };
            var parameters = {
                currencyExchangeRateId: currencyExchangeRateId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/CurrencyExchangeRate/CurrencyExchangeRateEditor.html', parameters, settings);
        }
        function registerDrillDownToCurrency() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Exchange Rates";
            drillDownDefinition.directive = "vr-common-currencyexchangerate-grid";
            //drillDownDefinition.parentMenuActions = [{
            //    name: "New Exchange Rate",
            //    clicked: function (currencyItem) {
            //        if (drillDownDefinition.setTabSelected != undefined)
            //            drillDownDefinition.setTabSelected(currencyItem);
                    
            //        var onExchangeRateAdded = function (exchangeRateObj) {
            //            if (currencyItem.exchangeGridAPI != undefined) {
            //                currencyItem.exchangeGridAPI.onExchangeRateAdded(exchangeRateObj);
            //            }
            //        };
            //        addExchangeRate(onExchangeRateAdded, currencyItem.CurrencyId);
            //    }
            //}];

            drillDownDefinition.loadDirective = function (directiveAPI, currencyItem) {
                currencyItem.exchangeGridAPI = directiveAPI;
                var query = {
                    Currencies: [currencyItem.Entity.CurrencyId],
                };

                return currencyItem.exchangeGridAPI.loadGrid(query,true);
            };

            VRCommon_CurrencyService.addDrillDownDefinition(drillDownDefinition);
        }


        return ({
            addExchangeRate: addExchangeRate,
            editExchangeRate:editExchangeRate,
            registerDrillDownToCurrency: registerDrillDownToCurrency
        });

    }]);
