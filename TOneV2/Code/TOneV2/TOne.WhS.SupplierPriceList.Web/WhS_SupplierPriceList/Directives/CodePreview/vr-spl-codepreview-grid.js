"use strict";

app.directive("vrSplCodepreviewGrid", ["WhS_SupPL_SupplierPriceListPreviewPIService", "UtilsService", "VRUIUtilsService", "VRNotificationService", "VRValidationService",
function (WhS_SupPL_SupplierPriceListPreviewPIService, UtilsService, VRUIUtilsService, VRNotificationService, VRValidationService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var codePreviewGrid = new CodePreviewGrid($scope, ctrl, $attrs);
                codePreviewGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/CodePreview/Templates/SupplierPriceListCodePreviewGridTemplate.html"

        };

        function CodePreviewGrid($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
              
                $scope.changedCodes = [];
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
                    return WhS_SupPL_SupplierPriceListPreviewPIService.GetFilteredCodePreview(dataRetrievalInput)
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
