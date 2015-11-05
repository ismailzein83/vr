
app.service('VRCommon_CurrencyExchangeRateService', ['UtilsService', 'VRModalService', 'VRNotificationService',
    function (UtilsService, VRModalService, VRNotificationService) {

        function addExchangeRate(onCurrencyExchangeRateAdded) {
            var settings = {
                useModalTemplate: true

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForAddEditor("Currency Exchange Rate");
                modalScope.onCurrencyExchangeRateAdded = onCurrencyExchangeRateAdded; 
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/Common/Views/CurrencyExchangeRate/CurrencyExchangeRateEditor.html', parameters, settings);
        }


        return ({
            addExchangeRate: addExchangeRate
        });

    }]);
