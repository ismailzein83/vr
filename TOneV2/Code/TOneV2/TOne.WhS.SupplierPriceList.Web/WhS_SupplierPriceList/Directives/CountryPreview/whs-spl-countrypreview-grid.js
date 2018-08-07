"use strict";

app.directive("whsSplCountrypreviewGrid", ["WhS_SupPL_SupplierPriceListPreviewPIService", "VRUIUtilsService", "VRNotificationService",
function (WhS_SupPL_SupplierPriceListPreviewPIService, VRUIUtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var countryPreviewGrid = new CountryPreviewGrid($scope, ctrl, $attrs);
            countryPreviewGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/CountryPreview/Templates/SupplierPriceListCountryPreviewGridTemplate.html"

    };

    function CountryPreviewGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var drillDownManager;
        var onlyModified;
        var processInstanceId;
        var isExcluded;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.changedCodes = [];
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
                        isExcluded = query.IsExcluded;
                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_SupPL_SupplierPriceListPreviewPIService.GetFilteredCountryPreview(dataRetrievalInput)
                    .then(function (response) {

                        if (response && response.Data) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    if (!onlyModified)
                                        mapDataNeeded(response.Data[i]);
                                    else
                                        $scope.showStatusIcon = false;
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
                title: "Zones",
                directive: "whs-spl-zonepreview-grid",
                loadDirective: function (directiveAPI, countryDataItem) {
                    countryDataItem.zoneRateGridAPI = directiveAPI;

                    var zoneRateGridPayload = {
                        ProcessInstanceId: processInstanceId,
                        CountryId: countryDataItem.Entity.CountryId,
                        OnlyModified: onlyModified,
                        IsExcluded : isExcluded
                    };

                    return countryDataItem.zoneRateGridAPI.load(zoneRateGridPayload);
                }
            };

            directiveTabs.push(directiveTab);

            return directiveTabs;
        }


        function mapDataNeeded(dataItem) {
            if (dataItem.Entity.NewCodes > 0 || dataItem.Entity.MovedCodes > 0 || dataItem.Entity.DeletedCodes > 0) {
                $scope.showStatusIcon = true;
                dataItem.StatusIconUrl = "Client/Modules/WhS_BusinessEntity/Images/Modified.png";
            }

        }

    }

    return directiveDefinitionObject;

}]);
