'use strict';
(function (app) {

    VRLocalizationService.$inject = ['UtilsService', '$rootScope'];

    function VRLocalizationService(UtilsService, $rootScope) {
        var localizationEnabled;
        var isRTLLocalization;

        function isLocalizationEnabled() {
            return localizationEnabled;
        }

        function setLocalizationEnabled(isLocalizationEnabled) {
            localizationEnabled = isLocalizationEnabled;
        }

        function isLocalizationRTL() {
            return isRTLLocalization;
        }

        function setLocalizationRTL(isRTL) {
            isRTLLocalization = isRTL;
        }

        function getResourceValue(resourceKey, defaultValue, variables) {
            var resourceValue;
            if (localizationEnabled && resourceKey != undefined)
                resourceValue = resourceKey;
            else
                resourceValue = defaultValue;
            if (variables != undefined && resourceValue != undefined) {
                for (var variableName in variables) {
                    resourceValue = UtilsService.replaceAll(resourceValue, variableName, variables[variableName]);
                }
            }
            return resourceValue;
        }

        return ({
            getResourceValue: getResourceValue,
            setLocalizationEnabled: setLocalizationEnabled,
            isLocalizationEnabled: isLocalizationEnabled,
            setLocalizationRTL: setLocalizationRTL,
            isLocalizationRTL: isLocalizationRTL
        });
    }

    app.service('VRLocalizationService', VRLocalizationService);

})(app);