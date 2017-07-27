"use strict"
app.directive("demoModuleStudentGrid", ["UtilsService", "VRNotificationService", "Demo_Module_StudentAPIService", "Demo_Module_StudentService", "VRUIUtilsService",
    function (UtilsService,VRNotificationService, Demo_Module_StudentAPIService, Demo_Module_StudentService, VRUIUtilsService) {
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
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/Student/templates/StudentGridTemplate.html"
        };
        function StudentGrid($scope, ctrl, $attrs) {
            var gridAPI;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.students = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveAPI());
                    }
                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onStudentAdded = function (student) {
                            gridAPI.itemAdded(student);
                        };
                        return directiveAPI;
                    }
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_StudentAPIService.GetFilteredStudents(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };
                defineMenuActions();
            }
            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: "Edit",
                    clicked: editStudent,

                }, {
                    name: "Delete",
                    clicked: deleteStudent,
                }];
            }

            function editStudent(student) {
                var onStudentUpdated = function (student) {
                    gridAPI.itemUpdated(student);
                }
                Demo_Module_StudentService.editStudent(student.Entity.StudentId, onStudentUpdated);
            }

            function deleteStudent(student) {
                var onStudentDeleted = function (student) {
                    gridAPI.itemDeleted(student);
                };
                Demo_Module_StudentService.deleteStudent($scope, student, onStudentDeleted)
            }


        }
        return directiveDefinitionObject;
    }]
    );