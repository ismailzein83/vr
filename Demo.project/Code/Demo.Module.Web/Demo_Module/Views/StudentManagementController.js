(function (appControllers) {
    "use strict";

    studentManagementController.$inject = ['$scope', 'Demo_Module_StudentService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function studentManagementController($scope, Demo_Module_StudentService, VRNotificationService, UtilsService, VRUIUtilsService) {

        var gridApi;
        var schoolDirectiveApi;
        var schoolReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var secondSchoolDirectiveApi;
        var secondSchoolReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        load();
        

        
        function defineScope() {


            var calculate = {
                add: function (a, b) {
                    return (a + b);
                },
                subtract: function (a, b) { return (a - b) },
                multiply: function (a, b)
                { return (a * b); }
            };

            function first(operator) {
                var second = function (object, a, b) {
                    if (a<b) return object.subtract(a, b);
                    else if (a>b) return object.add(a, b);
                    else return object.multiply(a, b);
                };

                third(second);
                
            };


            function third(func) {

                var result = func(calculate, 5, 5);
                console.log(result);
            };
            first()
            $scope.scopeModel = {};

          
            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                api.load(getFilter());
            };

            $scope.scopeModel.onSchoolDirectiveReady = function (api,secondapi) {
                schoolDirectiveApi = api;
                schoolReadyPromiseDeferred.resolve();
                secondSchoolDirectiveApi = secondapi;
                secondSchoolReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.search = function () {
                return gridApi.load(getFilter());
            };

            function showStudentName(student) {
                console.log(student.StudentId);
                console.log(student.Name);

            }

            $scope.scopeModel.addStudent = function () {
                var onStudentAdded = function (student) {
                    if (gridApi != undefined) {
                        gridApi.onStudentAdded(student);
                    }
                };
                var myFunction = function (student) {

                    showStudentName(student);
                }

                Demo_Module_StudentService.addStudent(onStudentAdded, myFunction);

            };
        };

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();

        };
  



        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSchoolSelector])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.scopeModel.isLoading = false;
             });
        };

        function loadSchoolSelector() {
            var schoolLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            schoolReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload = undefined;

                VRUIUtilsService.callDirectiveLoad(schoolDirectiveApi, directivePayload, schoolLoadPromiseDeferred);
            });
            secondSchoolReadyPromiseDeferred.promise
            .then(function () {
                secondSchoolDirectiveApi.consolelog();
                
            });
            return schoolLoadPromiseDeferred.promise;
        };


        function getFilter() {
            return {
                query: {
                    Name: $scope.scopeModel.name,
                    Age: $scope.scopeModel.age,
                    SchoolIds: schoolDirectiveApi.getSelectedIds(),
                    hideSchoolColumn: false

                }
            };
        };

    };

    appControllers.controller('Demo_Module_StudentManagementController', studentManagementController);
})(appControllers);