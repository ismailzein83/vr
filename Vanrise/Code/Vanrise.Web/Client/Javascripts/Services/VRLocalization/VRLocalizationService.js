'use strict';
(function (app) {

    VRLocalizationService.$inject = ['UtilsService', '$rootScope'];

    function VRLocalizationService(UtilsService, $rootScope) {
        var localizationEnabled;

        function isLocalizationEnabled()
        {
            if (localizationEnabled != undefined)
                return localizationEnabled;
            else
            {
                localizationEnabled = $rootScope.isLocalizationEnabled;
                return localizationEnabled;
            }
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
        });
    }

    app.service('VRLocalizationService', VRLocalizationService);

})(app);