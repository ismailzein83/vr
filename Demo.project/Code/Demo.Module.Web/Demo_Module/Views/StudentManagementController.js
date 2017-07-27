(function (appControllers) {

    "use strict";

    studentManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_StudentAPIService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_StudentService','VRNavigationService'];

    function studentManagementController($scope, VRNotificationService, Demo_Module_StudentAPIService, UtilsService, VRUIUtilsService, Demo_Module_StudentService, VRNavigationService) {

        var gridAPI;
        var filter = {};
        var roomDirectiveApi;
        var roomReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {

            $scope.students = [];

            $scope.searchClicked = function () {
                setfilterdobject()
                return gridAPI.loadGrid(filter);
            };

            function setfilterdobject() {
                filter = {
                    Name: $scope.name,
                    RoomIds: roomDirectiveApi.getSelectedIds()
                };
            }

            $scope.onRoomDirectiveReady = function (api) {
                roomDirectiveApi = api;
                roomReadyPromiseDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            };
            $scope.addNewStudent = addNewStudent;
        }
 
        function load() {
            $scope.isLoadingFilters = true;

            function loadRoomSelector() {
                var roomLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                roomReadyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(roomDirectiveApi, undefined, roomLoadPromiseDeferred);
                    });
                return roomLoadPromiseDeferred.promise;
            }

            loadRoomSelector().catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });;
        }

        function addNewStudent() {
            var onStudentAdded = function (student) {
                if (gridAPI != undefined)
                    gridAPI.onStudentAdded(student);
            };

            Demo_Module_StudentService.addStudent(onStudentAdded);
        }
    }




    appControllers.controller('Demo_Module_StudentManagementController', studentManagementController);
})(appControllers);