"use strict";

app.directive("whsSplSupplierpricelistpreviewSummary", ["WhS_SupPL_PreviewChangeTypeEnum", "WhS_SupPL_PreviewGroupedBy", "UtilsService", "VRUIUtilsService", "VRNotificationService", "WhS_SupPL_SupplierPriceListPreviewPIService",
function (WhS_SupPL_PreviewChangeTypeEnum, WhS_SupPL_PreviewGroupedBy, UtilsService, VRUIUtilsService, VRNotificationService, WhS_SupPL_SupplierPriceListPreviewPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var supplierPriceListPreviewSummary = new SupplierPriceListPreviewSummary($scope, ctrl, $attrs);
            supplierPriceListPreviewSummary.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
        },
        templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/SupplierPriceListPreviewSummary/Templates/SupplierPriceListPreviewSummary.html"
    };

    function SupplierPriceListPreviewSummary($scope, ctrl, $attrs) {

        var processInstanceId;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                if (payload != null)
                    processInstanceId = payload.processInstanceId;
                return UtilsService.waitMultipleAsyncOperations([loadSupplierPricelistPreviewSummary])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadSupplierPricelistPreviewSummary() {

            return WhS_SupPL_SupplierPriceListPreviewPIService.GetSupplierPricelistPreviewSummary(processInstanceId).then(function (previewSummary) {
                $scope.scopeModel.numberOfNewRates = previewSummary.NumberOfNewRates;
                $scope.scopeModel.numberOfIncreasedRates = previewSummary.NumberOfIncreasedRates;
                $scope.scopeModel.numberOfDecreasedRates = previewSummary.NumberOfDecreasedRates;
                $scope.scopeModel.numberOfNewOtherRates = previewSummary.NumberOfNewOtherRates;
                $scope.scopeModel.numberOfIncreasedOtherRates = previewSummary.NumberOfIncreasedOtherRates;
                $scope.scopeModel.numberOfDecreasedOtherRates = previewSummary.NumberOfDecreasedOtherRates;
                $scope.scopeModel.numberOfNewZones = previewSummary.NumberOfNewZones;
                $scope.scopeModel.numberOfRenamedZones = previewSummary.NumberOfRenamedZones;
                $scope.scopeModel.numberOfClosedZones = previewSummary.NumberOfClosedZones;
                $scope.scopeModel.numberOfNewCodes = previewSummary.NumberOfNewCodes;
                $scope.scopeModel.numberOfMovedCodes = previewSummary.NumberOfMovedCodes;
                $scope.scopeModel.numberOfClosedCodes = previewSummary.NumberOfClosedCodes;
                $scope.scopeModel.numberOfZonesWithChangeServices = previewSummary.NumberOfZonesWithChangedServices;
            });
        }
    }

    return directiveDefinitionObject;
}]);
