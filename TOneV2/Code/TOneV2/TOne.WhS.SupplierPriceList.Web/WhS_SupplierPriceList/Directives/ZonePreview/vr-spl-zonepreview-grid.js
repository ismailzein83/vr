"use strict";

app.directive("vrSplZonepreviewGrid", ["WhS_SupPL_SupplierPriceListPreviewPIService", "WhS_SupPL_ZoneChangeTypeEnum", "WhS_SupPL_RateChangeTypeEnum", "VRUIUtilsService", "VRNotificationService",
function (WhS_SupPL_SupplierPriceListPreviewPIService, WhS_SupPL_ZoneChangeTypeEnum, WhS_SupPL_RateChangeTypeEnum, VRUIUtilsService, VRNotificationService) {

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
                        return gridAPI.retrieveData(query);
                    }

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

            var directiveTab = {
                title: "Codes",
                directive: "vr-spl-codepreview-grid",
                loadDirective: function (directiveAPI, zoneDataItem) {
                    zoneDataItem.codeGridAPI = directiveAPI;

                    var codeGridPayload = {
                        ProcessInstanceId: processInstanceId,
                        ZoneName: zoneDataItem.ZoneName,
                        OnlyModified: onlyModified
                    };

                    return zoneDataItem.codeGridAPI.load(codeGridPayload);
                }
            };

            directiveTabs.push(directiveTab);

            return directiveTabs;
        }


        function mapDataNeeded(dataItem) {
            switch (dataItem.ChangeTypeZone) {

                case WhS_SupPL_ZoneChangeTypeEnum.New.value:
                    dataItem.ZoneStatusIconUrl = "Client/Modules/WhS_SupplierPriceList/Images/NewZone.png";
                    dataItem.ZoneStatusIconTooltip = WhS_SupPL_ZoneChangeTypeEnum.New.description;
                    break;

                case WhS_SupPL_ZoneChangeTypeEnum.Deleted.value:
                    dataItem.ZoneStatusIconUrl = "Client/Modules/WhS_SupplierPriceList/Images/NewZone.png";
                    dataItem.ZoneStatusIconTooltip = WhS_SupPL_ZoneChangeTypeEnum.Deleted.description;
                    break;

                case WhS_SupPL_ZoneChangeTypeEnum.ReOpened.value:
                    dataItem.ZoneStatusIconUrl = "Client/Modules/WhS_SupplierPriceList/Images/NewZone.png";
                    dataItem.ZoneStatusIconTooltip = WhS_SupPL_ZoneChangeTypeEnum.ReOpened.description;
                    break;

                case WhS_SupPL_ZoneChangeTypeEnum.Renamed.value:
                    dataItem.ZoneStatusIconUrl = "Client/Modules/WhS_SupplierPriceList/Images/Renamed.png";
                    dataItem.ZoneStatusIconTooltip = WhS_SupPL_ZoneChangeTypeEnum.Renamed.description;
                    break;
            }


            switch (dataItem.ChangeTypeRate) {
                case WhS_SupPL_RateChangeTypeEnum.New.value:
                    dataItem.RateStatusIconUrl = "Client/Modules/WhS_SupplierPriceList/Images/NewZone.png";
                    dataItem.RateStatusIconTooltip = WhS_SupPL_RateChangeTypeEnum.New.description;
                    break;

                case WhS_SupPL_RateChangeTypeEnum.Increase.value:
                    dataItem.RateStatusIconUrl = "Client/Modules/WhS_SupplierPriceList/Images/increase.jpg";
                    dataItem.RateStatusIconTooltip = WhS_SupPL_RateChangeTypeEnum.Increase.description;
                    break;

                case WhS_SupPL_RateChangeTypeEnum.Decrease.value:
                    dataItem.RateStatusIconUrl = "Client/Modules/WhS_SupplierPriceList/Images/decrease.jpg";
                    dataItem.RateStatusIconTooltip = WhS_SupPL_RateChangeTypeEnum.Decrease.description;
                    break;

            }

        }

    }

    return directiveDefinitionObject;

}]);
