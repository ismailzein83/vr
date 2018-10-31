"use strict";

app.directive("demoModuleStudentSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'Demo_Module_StudentService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, Demo_Module_StudentService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new StudentSearch($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/Student/Templates/StudentSearchTemplate.html"

    };

    function StudentSearch($scope, ctrl, $attrs) {
        
        var gridAPI;
        var schoolId;

        this.initializeController = initializeController;
        var context;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.addStudent = function () {
                var schoolIdItem = { SchoolId: schoolId };
                var onStudentAdded = function (obj) {
                    gridAPI.onStudentAdded(obj);
                };
                Demo_Module_StudentService.addStudent(onStudentAdded, schoolIdItem);
            };

            $scope.scopeModel.search = function () {
                return gridAPI.load(getGridQuery());
            };


            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    schoolId = payload.schoolId;
                }
                return gridAPI.load(getGridQuery());
            };
            api.onStudentAdded = function (studentObject) {
                gridAPI.onStudentAdded(studentObject);
            };


            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function getGridQuery() {
            var payload = {
                query: {
                    Name: $scope.scopeModel.name,
                    Age: $scope.scopeModel.age,
                    SchoolIds: [schoolId],
                    hideSchoolColumn: true
                },
                schoolId: schoolId,
            };
            return payload;
        }
    }

    return directiveDefinitionObject;

}]);
