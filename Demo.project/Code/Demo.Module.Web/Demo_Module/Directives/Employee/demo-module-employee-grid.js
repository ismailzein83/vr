"use strict"
app.directive("demoModuleEmployeeGrid", ["UtilsService", "VRNotificationService", "Demo_Module_EmployeeAPIService", "Demo_Module_EmployeeService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_EmployeeAPIService, Demo_Module_EmployeeService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var employeeGrid = new EmployeeGrid($scope, ctrl, $attrs);
            employeeGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/Employee/Templates/EmployeeGridTemplate.html"
    };

    function EmployeeGrid($scope, ctrl) {

        var gridApi;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.employees = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (payload) {
                        var query = payload.query;

                        return gridApi.retrieveData(query);
                    };

                    directiveApi.onEmployeeAdded = function (employee) {
                        gridApi.itemAdded(employee);
                    };
                    return directiveApi;
                };
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_EmployeeAPIService.GetFilteredEmployees(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            defineMenuActions();
        };

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editEmployee,

            }];
        }

        function editEmployee(employee) {
            var onEmployeeUpdated = function (employee) {
                gridApi.itemUpdated(employee); 
            };


            Demo_Module_EmployeeService.editEmployee(employee.EmployeeId, onEmployeeUpdated);
        };


    };
    return directiveDefinitionObject;
}]);
