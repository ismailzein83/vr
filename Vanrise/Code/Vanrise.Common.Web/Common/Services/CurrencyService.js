
app.service('VRCommon_CurrencyService', ['UtilsService','VRModalService', 'VRNotificationService',
    function (UtilsService, VRModalService, VRNotificationService) {

        function addCurrency(onCurrencyAdded) {
            var settings = {
                useModalTemplate: true

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForAddEditor("Currency");
                modalScope.onCurrencyAdded = onCurrencyAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/Common/Views/Currency/CurrencyEditor.html', parameters, settings);
        }

        function editCurrency(obj, onCurrencyUpdated) {
            var settings = {
                useModalTemplate: true

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForUpdateEditor(obj.Name, "Currency");
                modalScope.onCurrencyUpdated = onCurrencyUpdated;
            };
            var parameters = {
                CurrencyId: obj.CurrencyId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/Currency/CurrencyEditor.html', parameters, settings);
        }

        return ({
            addCurrency: addCurrency,
            editCurrency: editCurrency
        });

    }]);
