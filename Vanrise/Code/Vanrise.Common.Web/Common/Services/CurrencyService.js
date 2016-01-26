
app.service('VRCommon_CurrencyService', ['UtilsService','VRModalService', 'VRNotificationService',
    function (UtilsService, VRModalService, VRNotificationService) {
        var drillDownDefinitions = [];
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

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return ({
            addCurrency: addCurrency,
            editCurrency: editCurrency,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        });

    }]);
