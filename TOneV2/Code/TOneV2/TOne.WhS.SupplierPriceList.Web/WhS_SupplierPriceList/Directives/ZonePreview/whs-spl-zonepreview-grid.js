"use strict";

app.directive("whsSplZonepreviewGrid", ["WhS_SupPL_SupplierPriceListPreviewPIService", "WhS_SupPL_ZoneChangeTypeEnum", "WhS_SupPL_RateChangeTypeEnum", "WhS_SupPL_ZoneServiceChangeTypeEnum", "VRUIUtilsService", "VRNotificationService",'VRDateTimeService', 
function (WhS_SupPL_SupplierPriceListPreviewPIService, WhS_SupPL_ZoneChangeTypeEnum, WhS_SupPL_RateChangeTypeEnum, WhS_SupPL_ZoneServiceChangeTypeEnum, VRUIUtilsService, VRNotificationService, VRDateTimeService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var zonePreviewGrid = new ZonePreviewGrid($scope, ctrl, $attrs);
            zonePreviewGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/ZonePreview/Templates/SupplierPriceListZonePreviewGridTemplate.html"

    };

    function ZonePreviewGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var drillDownManager;
        var onlyModified;
        var processInstanceId;
        var countryId;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.changedZones = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(getDirectiveTabs(), gridAPI);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.load = function (query) {
                        processInstanceId = query.ProcessInstanceId;
                        onlyModified = query.OnlyModified;
                        countryId = query.CountryId;
                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };


            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_SupPL_SupplierPriceListPreviewPIService.GetFilteredZonePreview(dataRetrievalInput)
                    .then(function (response) {

                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                mapDataNeeded(response.Data[i]);
                                drillDownManager.setDrillDownExtensionObject(response.Data[i]);
                            }

                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        }

        function getDirectiveTabs() {
            var directiveTabs = [];

            var codeTab = {
                title: "Codes",
                directive: "whs-spl-codepreview-grid",
                loadDirective: function (directiveAPI, zoneDataItem) {
                    zoneDataItem.codeGridAPI = directiveAPI;

                    var codeGridPayload = {
                        ProcessInstanceId: processInstanceId,
                        ZoneName: zoneDataItem.ZoneName,
                        OnlyModified: onlyModified,
                        CountryId: countryId
                    };

                    return zoneDataItem.codeGridAPI.load(codeGridPayload);
                }
            };

            var otherRatesTab = {
                title: "Other Rates",
                directive: "whs-spl-otherratespreview-grid",
                loadDirective: function (directiveAPI, zoneDataItem) {
                    zoneDataItem.otherRatesGridAPI = directiveAPI;

                    var otherRatesGridPayload = {
                        ProcessInstanceId: processInstanceId,
                        ZoneName: zoneDataItem.ZoneName,
                        OnlyModified: onlyModified
                    };

                    return zoneDataItem.otherRatesGridAPI.load(otherRatesGridPayload);
                }
            };

            var zoneServicesTab = {
                title: "Zone Services",
                directive: "whs-spl-zoneservicespreview-grid",
                loadDirective: function (directiveAPI, zoneDataItem) {
                    zoneDataItem.zoneServicesGridAPI = directiveAPI;

                    var zoneServicesGridPayload = {
                        ProcessInstanceId: processInstanceId,
                        ZoneName: zoneDataItem.ZoneName,
                        OnlyModified: onlyModified
                    };

                    return zoneDataItem.zoneServicesGridAPI.load(zoneServicesGridPayload);
                }
            };

            directiveTabs.push(codeTab);
            directiveTabs.push(otherRatesTab);
            directiveTabs.push(zoneServicesTab);

            return directiveTabs;
        }


        function mapDataNeeded(dataItem) {
            if (!onlyModified) {
                if (dataItem.NewCodes > 0 || dataItem.DeletedCodes > 0 || dataItem.CodesMovedTo > 0 || dataItem.CodesMovedFrom > 0 || dataItem.ChangeTypeRate != WhS_SupPL_RateChangeTypeEnum.NotChanged.value) {
                    dataItem.ZoneStatusIconUrl = "Client/Modules/WhS_BusinessEntity/Images/Modified.png";
                    dataItem.ZoneStatusIconTooltip = "Modified";
                }
            }

            dataItem.onImportedServicesReady = function (api) {
                dataItem.ImportedServicesApi = api;
                dataItem.ImportedServicesApi.load({ selectedIds: dataItem.ImportedServiceIds });
            };

            var today = VRDateTimeService.getNowDateTime();
            var importedRateBED = new Date(Date.parse(dataItem.ImportedRateBED));
            if (importedRateBED > today)
                dataItem.ImportedRate += ' ( Future )';

            switch (dataItem.ChangeTypeZone) {

                case WhS_SupPL_ZoneChangeTypeEnum.New.value:
                    dataItem.ZoneStatusIconUrl = WhS_SupPL_ZoneChangeTypeEnum.New.icon;
                    dataItem.ZoneStatusIconTooltip = WhS_SupPL_ZoneChangeTypeEnum.New.description;
                    break;

                case WhS_SupPL_ZoneChangeTypeEnum.Deleted.value:
                    dataItem.ZoneStatusIconUrl = WhS_SupPL_ZoneChangeTypeEnum.Deleted.icon;
                    dataItem.ZoneStatusIconTooltip = WhS_SupPL_ZoneChangeTypeEnum.Deleted.description;
                    break;

                case WhS_SupPL_ZoneChangeTypeEnum.ReOpened.value:
                    dataItem.ZoneStatusIconUrl = WhS_SupPL_ZoneChangeTypeEnum.ReOpened.icon;
                    dataItem.ZoneStatusIconTooltip = WhS_SupPL_ZoneChangeTypeEnum.ReOpened.description;
                    break;

                case WhS_SupPL_ZoneChangeTypeEnum.Renamed.value:
                    dataItem.ZoneStatusIconUrl = WhS_SupPL_ZoneChangeTypeEnum.Renamed.icon;
                    dataItem.ZoneStatusIconTooltip = WhS_SupPL_ZoneChangeTypeEnum.Renamed.description;
                    break;
            }


            switch (dataItem.ChangeTypeRate) {
                case WhS_SupPL_RateChangeTypeEnum.New.value:
                    dataItem.RateStatusIconUrl = WhS_SupPL_RateChangeTypeEnum.New.icon;
                    dataItem.RateStatusIconTooltip = WhS_SupPL_RateChangeTypeEnum.New.description;
                    dataItem.RateChangeTypeIconType = WhS_SupPL_RateChangeTypeEnum.New.iconType;
                    break;

                case WhS_SupPL_RateChangeTypeEnum.Increase.value:
                    dataItem.RateStatusIconUrl = WhS_SupPL_RateChangeTypeEnum.Increase.icon;
                    dataItem.RateStatusIconTooltip = WhS_SupPL_RateChangeTypeEnum.Increase.description;
                    dataItem.RateChangeTypeIconType = WhS_SupPL_RateChangeTypeEnum.Increase.iconType;
                    break;

                case WhS_SupPL_RateChangeTypeEnum.Decrease.value:
                    dataItem.RateStatusIconUrl = WhS_SupPL_RateChangeTypeEnum.Decrease.icon;
                    dataItem.RateStatusIconTooltip = WhS_SupPL_RateChangeTypeEnum.Decrease.description;
                    dataItem.RateChangeTypeIconType = WhS_SupPL_RateChangeTypeEnum.Decrease.iconType;
                    break;

            }

            if (dataItem.ZoneServicesChangeType != WhS_SupPL_ZoneServiceChangeTypeEnum.NotChanged.value)
            {
                dataItem.showServiceChanges = true;
                dataItem.serviceChanges = 1;
            }

        }

    }

    return directiveDefinitionObject;

}]);
