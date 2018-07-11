"use strict"
app.directive("demoModuleComplaintGrid", ["UtilsService","LabelColorsEnum", "VRNotificationService", "Demo_Module_ComplaintAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService,LabelColorsEnum, VRNotificationService, Demo_Module_ComplaintAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var complaintsGrid = new ComplaintsGrid($scope, ctrl, $attrs);
            complaintsGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Elements/CallCenterCustomer/Directives/Templates/ComplaintGridTemplate.html"
    };

    function ComplaintsGrid($scope, ctrl) {

        var gridApi;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.complaints = [];

            $scope.scopeModel.getStatusColor = function (dataItem) {
                if (dataItem.Status == "New") return LabelColorsEnum.Success.color;
                if (dataItem.Status == "Pending") return LabelColorsEnum.Warning.color;
                if (dataItem.Status == "Closed") return LabelColorsEnum.Error.color;
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

                return Demo_Module_ComplaintAPIService.GetComplaints()
                .then(function (response) {
                    if (response != null) {
                        $scope.scopeModel.complaints = response;
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
