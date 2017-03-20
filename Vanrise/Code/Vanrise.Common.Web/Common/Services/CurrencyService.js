
app.service('VRCommon_CurrencyService', ['UtilsService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService',
    function (UtilsService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
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

        function getEntityUniqueName() {
            return "VR_Common_Currency";
        }

        function registerObjectTrackingDrillDownToCurrency() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, currencyItem) {
                currencyItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: currencyItem.Entity.CurrencyId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return currencyItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

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
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToCurrency: registerObjectTrackingDrillDownToCurrency
        });

    }]);
