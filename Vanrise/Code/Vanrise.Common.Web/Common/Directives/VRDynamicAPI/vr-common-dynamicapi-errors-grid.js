"use strict";
app.directive("vrCommonDynamicapiErrorsGrid", ["UtilsService", "VRNotificationService",  "VRUIUtilsService", "VRCommon_ObjectTrackingService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var vrDynamicAPIGrid = new VRDynamicAPIGrid($scope, ctrl, $attrs);
            vrDynamicAPIGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Common/Directives/VRDynamicAPI/Templates/DynamicAPIErrorsGridTemplate.html"
    };

    function VRDynamicAPIGrid($scope, ctrl) {

        var gridApi;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.vrDynamicAPIErrors = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (payload) {
                        if (payload != undefined && payload.errors != undefined) {
                            for (var j = 0; j < payload.errors.length; j++) {
                                $scope.scopeModel.vrDynamicAPIErrors.push({ error: payload.errors[j] });
                            }
                        }
                    };

                    return directiveApi;
                }
            };
           
        }
    }

    return directiveDefinitionObject;
}]);
