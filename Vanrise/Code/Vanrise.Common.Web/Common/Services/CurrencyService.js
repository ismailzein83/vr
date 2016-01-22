
app.service('VRCommon_CurrencyService', ['UtilsService','VRModalService', 'VRNotificationService',
    function (UtilsService, VRModalService, VRNotificationService) {

        function addCurrency(onCurrencyAdded) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCurrencyAdded = onCurrencyAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/Common/Views/Currency/CurrencyEditor.html', parameters, settings);
        }

        function editCurrency(currencyId, onCurrencyUpdated) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCurrencyUpdated = onCurrencyUpdated;
            };
            var parameters = {
                CurrencyId:currencyId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/Currency/CurrencyEditor.html', parameters, settings);
        }

        return ({
            addCurrency: addCurrency,
            editCurrency: editCurrency
        });

    }]);
