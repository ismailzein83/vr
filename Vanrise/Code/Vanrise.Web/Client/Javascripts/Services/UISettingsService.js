(function (app) {

    "use strict";
    UISettingsService.$inject = ['VRNotificationService', 'UtilsService', 'VRCommon_UISettingsAPIService'];

    function UISettingsService(VRNotificationService, UtilsService, VRCommon_UISettingsAPIService) {

        var uiSettings;
      
        function loadUISettings() {
            uiSettings = undefined;
            return VRCommon_UISettingsAPIService.GetUIParameters().then(function (response) {
                uiSettings = response;
            });
        }
        function isUISettingsHasValue() {
            return uiSettings != undefined;
        }

        function getDefaultPageURl() {
            var url;
            if (uiSettings != undefined && uiSettings.Parameters.length > 0) {
                var param = UtilsService.getItemByVal(uiSettings.Parameters, "DefaultURL", "Name");
                if(param!=null )
                    url = UtilsService.getBaseUrlPrefix() + param.Value;
            }
            return url;
        }
        function getNormalPrecision() {
            var normalPrecision;
            if (uiSettings != undefined && uiSettings.Parameters.length > 0) {
                var param = UtilsService.getItemByVal(uiSettings.Parameters, "NormalPrecision", "Name");
                if (param != null)
                    normalPrecision  = param.Value;
            }
            return normalPrecision;
        }
        function getUIParameterValue(name) {
            var val;
            if (uiSettings != undefined && uiSettings.Parameters.length > 0) {
                var param = UtilsService.getItemByVal(uiSettings.Parameters, name, "Name");
                if (param != null)
                    val = param.Value;
            }
            return val;
        }
        return ({
            loadUISettings: loadUISettings,
            getDefaultPageURl: getDefaultPageURl,
            getNormalPrecision: getNormalPrecision,
            getUIParameterValue: getUIParameterValue,
            isUISettingsHasValue: isUISettingsHasValue
        });
    }

    app.service('UISettingsService', UISettingsService);
})(app);


