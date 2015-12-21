"use strict";

app.directive("vrSplRatepreviewGrid", ["WhS_SupPL_SupplierPriceListPreviewPIService", "UtilsService", "VRUIUtilsService", "VRNotificationService", "VRValidationService",
function (WhS_SupPL_SupplierPriceListPreviewPIService, UtilsService, VRUIUtilsService, VRNotificationService, VRValidationService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ratePreviewGrid = new RatePreviewGrid($scope, ctrl, $attrs);
                ratePreviewGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/RatePreview/Templates/SupplierPriceListRatePreviewGridTemplate.html"

        };

        function RatePreviewGrid($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
              
                $scope.changedRates = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());
                    function getDirectiveAPI() {

                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {

                            return gridAPI.retrieveData(query);
                        }
                        
                        return directiveAPI;
                    }
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_SupPL_SupplierPriceListPreviewPIService.GetFilteredRatePreview(dataRetrievalInput)
                        .then(function (response) {
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };
            }
        }

        return directiveDefinitionObject;
        
}]);
