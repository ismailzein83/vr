"use strict"
app.directive("demoModuleStudentGrid", ["UtilsService", "VRNotificationService", "Demo_Module_StudentAPIService", "Demo_Module_StudentService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_StudentAPIService, Demo_Module_StudentService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var studentGrid = new StudentGrid($scope, ctrl, $attrs);
            studentGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/Student/Templates/StudentGridTemplate.html"
    };

    function StudentGrid($scope, ctrl) {

        var gridApi;
        var schoolId;
        var demoCountryId;
        var demoCityId;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.students = [];
            $scope.scopeModel.hideSchoolColumn = false;

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (payload) {
                        var query = payload.query;
                        schoolId = payload.schoolId;
                        demoCountryId = payload.demoCountryId;
                         demoCityId = payload.demoCityId;


                        $scope.scopeModel.hideSchoolColumn = query.hideSchoolColumn;
                        return gridApi.retrieveData(query);
                    };

                    directiveApi.onStudentAdded = function (student) {
                        gridApi.itemAdded(student);
                    };
                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_StudentAPIService.GetFilteredStudents(dataRetrievalInput)
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
                clicked: editStudent,

            }];
        };
        function editStudent(student) {
            var onStudentUpdated = function (student) {
                gridApi.itemUpdated(student);
            };
            

            Demo_Module_StudentService.editStudent(student.StudentId, onStudentUpdated);
        };


    };
    return directiveDefinitionObject;
}]);
