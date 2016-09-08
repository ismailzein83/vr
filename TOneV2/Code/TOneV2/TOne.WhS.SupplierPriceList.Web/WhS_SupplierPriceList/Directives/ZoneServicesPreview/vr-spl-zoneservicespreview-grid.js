"use strict";

app.directive("vrSplZoneservicespreviewGrid", ["WhS_SupPL_SupplierPriceListPreviewPIService", "WhS_SupPL_ZoneServiceChangeTypeEnum", "VRUIUtilsService", "VRNotificationService",
function (WhS_SupPL_SupplierPriceListPreviewPIService, WhS_SupPL_ZoneServiceChangeTypeEnum, VRUIUtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var zoneServicesPreviewGrid = new ZoneServicesPreviewGrid($scope, ctrl, $attrs);
            zoneServicesPreviewGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/ZoneServicesPreview/Templates/SupplierPriceListZoneServicesPreviewGridTemplate.html"

    };

    function ZoneServicesPreviewGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var onlyModified;
        var processInstanceId;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.zoneServicesPreview = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

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
                return WhS_SupPL_SupplierPriceListPreviewPIService.GetFilteredZoneServicesPreview(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                mapDataNeeded(response.Data[i]);
                            }

                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        }

        function mapDataNeeded(dataItem) {

            dataItem.onSystemServicseReady = function (api) {
                dataItem.SystemServicesApi = api
                dataItem.SystemServicesApi.load({ selectedIds: dataItem.SystemServiceIds });
            }

            dataItem.onImportedServicesReady = function (api) {
                dataItem.ImportedServicesApi = api
                dataItem.ImportedServicesApi.load({ selectedIds: dataItem.ImportedServiceIds });
            }

            switch (dataItem.ZoneServicesChangeType) {
                case WhS_SupPL_ZoneServiceChangeTypeEnum.New.value:
                    dataItem.ZoneServicesStatusIconUrl = WhS_SupPL_ZoneServiceChangeTypeEnum.New.icon;
                    dataItem.ZoneServicesStatusIconTooltip = WhS_SupPL_ZoneServiceChangeTypeEnum.New.description;
                    break;

                case WhS_SupPL_ZoneServiceChangeTypeEnum.Deleted.value:
                    dataItem.ZoneServicesStatusIconUrl = WhS_SupPL_ZoneServiceChangeTypeEnum.Deleted.icon;
                    dataItem.ZoneServicesStatusIconTooltip = WhS_SupPL_ZoneServiceChangeTypeEnum.Deleted.description;
                    break;
            }
        }


    }

    return directiveDefinitionObject;

}]);
