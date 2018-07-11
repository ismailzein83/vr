"use strict"
app.directive("demoModuleServiceGrid", ["UtilsService","LabelColorsEnum", "VRNotificationService", "Demo_Module_ServiceAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService,LabelColorsEnum, VRNotificationService, Demo_Module_ServiceAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var serviceGrid = new ServiceGrid($scope, ctrl);
            serviceGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Elements/CallCenterCustomer/Directives/Templates/ServiceGridTemplate.html"
    };

    function ServiceGrid($scope, ctrl) {

        var gridApi;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.services = [];

            $scope.scopeModel.getStatusColor = function (dataItem) {
                if (dataItem.Status == "Terminated") return LabelColorsEnum.Error.color;
                if (dataItem.Status == "Suspended") return LabelColorsEnum.Error.color;
                if (dataItem.Status == "Actif") return LabelColorsEnum.Success.color;
                return;
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function () {
                        return gridApi.retrieveData();
                    };

                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_ServiceAPIService.GetServices()
                .then(function (response) {
                    if (response != null) {
                        $scope.scopeModel.services = response;
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            defineMenuActions();
        };

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [];
        };


    };
    return directiveDefinitionObject;
}]);
