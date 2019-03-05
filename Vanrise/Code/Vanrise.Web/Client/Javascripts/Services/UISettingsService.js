(function (app) {

    "use strict";

    UISettingsService.$inject = ['VRNotificationService', 'UtilsService', 'VRCommon_UISettingsAPIService'];

    function UISettingsService(VRNotificationService, UtilsService, VRCommon_UISettingsAPIService) {

        var uiSettings;

        function getUIParameterValue(parameterName) {
            if (uiSettings != undefined && uiSettings.Parameters != undefined) {
                return uiSettings.Parameters[parameterName];
            }
        }

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
            var defaultURL = getUIParameterValue("DefaultURL");
            if (defaultURL != undefined) {
                url = UtilsService.getBaseUrlPrefix() + defaultURL;
            }
            return url;
        }
        function getDefaultPageTitle() {
            return getUIParameterValue("DefaultURLTitle");
        }

        function getDefaultPath() {
          return  getUIParameterValue("DefaultURL");
        }
        function getGoogleTrackingStatus() {
            return getUIParameterValue("GoogleAnalyticsEnabled");
        }

        function getGoogleTrackingAccount() {
            return getUIParameterValue("GoogleAnalyticsAccount");
        }

        function getMasterLayoutSettings() {
            return getUIParameterValue("MasterLayoutSetting");
        }

        function getMasterLayoutDefaultViewTileState() {
            var tilesMode = false;
            var param = getMasterLayoutSettings();
            if (param != undefined)
                tilesMode = param["TilesMode"];
            return tilesMode;
        }

        function getMasterLayoutModuleDefaultViewTileState() {
            var tilesMode = false;
            var param = getMasterLayoutSettings();
            if (param != undefined)
                tilesMode = param["ModuleTilesMode"];
            return tilesMode;
        }
        function getMasterLayoutMenuToogledState() {
            var expandedMenu = false;
            var param = getMasterLayoutSettings();
            if (param != undefined)
                expandedMenu = param["ExpandedMenu"];
            return expandedMenu;
        }

        function getMasterLayoutShowApplicationTilesState() {
            var state = false;
            var param = getMasterLayoutSettings();
            if (param != undefined)
                state = param["ShowApplicationTiles"];
            return state;
        }

        function getMasterLayoutBreadcrumbVisibility() {
            var isBreadcrumbVisible = false;
            var param = getMasterLayoutSettings();
            if (param != undefined)
                isBreadcrumbVisible = param["IsBreadcrumbVisible"];
            return isBreadcrumbVisible;
        }
        function getNormalPrecision() {
            return getUIParameterValue("NormalPrecision");
        }

        function getLongPrecision() {
            return getUIParameterValue("LongPrecision");
        }

        function getGridLayoutOptions() {
            var alternativeColor = getUIParameterValue("AlternativeColor");
            var layoutOption = {
                horizontalLine: getUIParameterValue("HorizontalLine"),
                alternativeColor: alternativeColor != undefined ? alternativeColor : true,
                verticalLine: getUIParameterValue("VerticalLine")
            };
            return layoutOption;
        }

        function getMaxSearchRecordCount() {
            return getUIParameterValue("MaxSearchRecordCount");
        }
        function getServerUtcOffsetInMinutes() {
            return getUIParameterValue("ServerUtcOffsetInMinutes");
        }


        return ({
            loadUISettings: loadUISettings,
            getDefaultPageURl: getDefaultPageURl,
            getDefaultPageTitle:getDefaultPageTitle,
            getDefaultPath: getDefaultPath,
            getNormalPrecision: getNormalPrecision,
            getLongPrecision: getLongPrecision,
            getUIParameterValue: getUIParameterValue,
            isUISettingsHasValue: isUISettingsHasValue,
            getGoogleTrackingStatus: getGoogleTrackingStatus,
            getGoogleTrackingAccount: getGoogleTrackingAccount,
            getMaxSearchRecordCount: getMaxSearchRecordCount,
            getServerUtcOffsetInMinutes: getServerUtcOffsetInMinutes,
            getGridLayoutOptions: getGridLayoutOptions,
            getMasterLayoutSettings: getMasterLayoutSettings,
            getMasterLayoutDefaultViewTileState: getMasterLayoutDefaultViewTileState,
            getMasterLayoutModuleDefaultViewTileState: getMasterLayoutModuleDefaultViewTileState,
            getMasterLayoutMenuToogledState: getMasterLayoutMenuToogledState,
            getMasterLayoutShowApplicationTilesState: getMasterLayoutShowApplicationTilesState,
            getMasterLayoutBreadcrumbVisibility: getMasterLayoutBreadcrumbVisibility
        });
    }

    app.service('UISettingsService', UISettingsService);

})(app);


