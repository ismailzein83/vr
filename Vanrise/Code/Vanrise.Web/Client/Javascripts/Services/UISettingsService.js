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
                if (param != null)
                    url = UtilsService.getBaseUrlPrefix() + param.Value;
            }
            return url;
        }

        function getGoogleTrackingStatus() {
            var status;
            if (uiSettings != undefined && uiSettings.Parameters.length > 0) {
                var param = UtilsService.getItemByVal(uiSettings.Parameters, "GoogleAnalyticsEnabled", "Name");
                if (param != null)
                    status = param.Value;
            }
            return status;
        }

        function getGoogleTrackingAccount() {
            var account;
            if (uiSettings != undefined && uiSettings.Parameters.length > 0) {
                var param = UtilsService.getItemByVal(uiSettings.Parameters, "GoogleAnalyticsAccount", "Name");
                if (param != null)
                    account = param.Value;
            }
            return account;
        }

        function getNormalPrecision() {
            var normalPrecision;
            if (uiSettings != undefined && uiSettings.Parameters.length > 0) {
                var param = UtilsService.getItemByVal(uiSettings.Parameters, "NormalPrecision", "Name");
                if (param != null)
                    normalPrecision = param.Value;
            }
            return normalPrecision;
        }

        function getLongPrecision() {
            var longPrecision;
            if (uiSettings != undefined && uiSettings.Parameters.length > 0) {
                var param = UtilsService.getItemByVal(uiSettings.Parameters, "LongPrecision", "Name");
                if (param != null)
                    longPrecision = param.Value;
            }
            return longPrecision;
        }

        function getGridLayoutOptions() {
            var layoutOption = {
                alternativeColor: true
            };
            if (uiSettings != undefined && uiSettings.Parameters.length > 0) {

                var horizontalLine = UtilsService.getItemByVal(uiSettings.Parameters, "HorizontalLine", "Name");
                layoutOption.horizontalLine = horizontalLine.Value;

                var alternativeColor = UtilsService.getItemByVal(uiSettings.Parameters, "AlternativeColor", "Name");
                layoutOption.alternativeColor = alternativeColor.Value;


                var verticalLine = UtilsService.getItemByVal(uiSettings.Parameters, "VerticalLine", "Name");
                layoutOption.verticalLine = verticalLine.Value;
            }
            return layoutOption;
        }

        function getMaxSearchRecordCount() {
            var recordCount;
            if (uiSettings != undefined && uiSettings.Parameters.length > 0) {
                var param = UtilsService.getItemByVal(uiSettings.Parameters, "MaxSearchRecordCount", "Name");
                if (param != null)
                    recordCount = param.Value;
            }
            return recordCount;
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

        function getServerUtcOffsetInMinutes() {
            var val;
            if (uiSettings != undefined && uiSettings.Parameters.length > 0) {
                var param = UtilsService.getItemByVal(uiSettings.Parameters, "ServerUtcOffsetInMinutes", "Name");
                if (param != null)
                    val = param.Value;
            }
            return val;
        }


        return ({
            loadUISettings: loadUISettings,
            getDefaultPageURl: getDefaultPageURl,
            getNormalPrecision: getNormalPrecision,
            getLongPrecision: getLongPrecision,
            getUIParameterValue: getUIParameterValue,
            isUISettingsHasValue: isUISettingsHasValue,
            getGoogleTrackingStatus: getGoogleTrackingStatus,
            getGoogleTrackingAccount: getGoogleTrackingAccount,
            getMaxSearchRecordCount: getMaxSearchRecordCount,
            getServerUtcOffsetInMinutes: getServerUtcOffsetInMinutes,
            getGridLayoutOptions: getGridLayoutOptions

        });
    }

    app.service('UISettingsService', UISettingsService);

})(app);


