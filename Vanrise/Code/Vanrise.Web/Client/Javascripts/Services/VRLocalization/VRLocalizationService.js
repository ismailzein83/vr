'use strict';
(function (app) {

    VRLocalizationService.$inject = ['UtilsService', '$rootScope','$cookies'];

    function VRLocalizationService(UtilsService, $rootScope, $cookies) {
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
            if (localizationEnabled && resourceKey != undefined && resourceKey != '')
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

        function getLanguageCookieName() {
            
            return "VR_Common_LocalizationLangauge_" + location.origin;
        }

        function createOrUpdateLanguageCookie(languageId) {
            $cookies.put(getLanguageCookieName(), languageId, { path: '/', domain: location.hostname, expires: '', secure: false });
        }

        function getLanguageCookie() {
            return $cookies.get(getLanguageCookieName());
        }
        return ({
            getResourceValue: getResourceValue,
            setLocalizationEnabled: setLocalizationEnabled,
            isLocalizationEnabled: isLocalizationEnabled,
            setLocalizationRTL: setLocalizationRTL,
            isLocalizationRTL: isLocalizationRTL,
            getLanguageCookieName: getLanguageCookieName,
            createOrUpdateLanguageCookie: createOrUpdateLanguageCookie,
            getLanguageCookie: getLanguageCookie
        });
    }

    app.service('VRLocalizationService', VRLocalizationService);

})(app);
