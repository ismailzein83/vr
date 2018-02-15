"use strict";

app.directive("whsSplOtherratespreviewGrid", ["WhS_SupPL_SupplierPriceListPreviewPIService", "WhS_SupPL_RateChangeTypeEnum", "VRUIUtilsService", "VRNotificationService",
function (WhS_SupPL_SupplierPriceListPreviewPIService, WhS_SupPL_RateChangeTypeEnum, VRUIUtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var otherRatePreviewGrid = new OtherRatePreviewGrid($scope, ctrl, $attrs);
            otherRatePreviewGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/OtherRatesPreview/Templates/SupplierPriceListOtherRatesPreviewGridTemplate.html"

    };

    function OtherRatePreviewGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var onlyModified;
        var processInstanceId;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.otherRatesPreview = [];
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
                    };

                    return directiveAPI;
                }
            };


            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_SupPL_SupplierPriceListPreviewPIService.GetFilteredOtherRatePreview(dataRetrievalInput)
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

            switch (dataItem.Entity.ChangeTypeRate) {
                case WhS_SupPL_RateChangeTypeEnum.New.value:
                    dataItem.RateStatusIconUrl = WhS_SupPL_RateChangeTypeEnum.New.icon;
                    dataItem.RateStatusIconTooltip = WhS_SupPL_RateChangeTypeEnum.New.description;
                    dataItem.RateChangeTypeIconType = WhS_SupPL_RateChangeTypeEnum.New.iconType;
                    break;

                case WhS_SupPL_RateChangeTypeEnum.Deleted.value:
                    dataItem.RateStatusIconUrl = WhS_SupPL_RateChangeTypeEnum.Deleted.icon;
                    dataItem.RateStatusIconTooltip = WhS_SupPL_RateChangeTypeEnum.Deleted.description;
                    dataItem.RateChangeTypeIconType = WhS_SupPL_RateChangeTypeEnum.Deleted.iconType;
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
        }


    }

    return directiveDefinitionObject;

}]);
