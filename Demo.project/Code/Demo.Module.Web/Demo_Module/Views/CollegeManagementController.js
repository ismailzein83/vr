(function (appControllers) {

    "use strict";

    collegeManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_CollegeAPIService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_CollegeService', 'VRNavigationService'];

    function collegeManagementController($scope, VRNotificationService, Demo_Module_CollegeAPIService, UtilsService, VRUIUtilsService, Demo_Module_CollegeService, VRNavigationService) {

        var gridAPI;
        var universityDirectiveApi;
        var universityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.colleges = [];

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(setFilterdObject());
            };

            function setFilterdObject() {
               return {
                    Name: $scope.name,
                    UniversityIds: universityDirectiveApi.getSelectedIds()
                };
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(setFilterdObject());
            };

            $scope.onUniversityDirectiveReady = function (api) {
                universityDirectiveApi = api;
                universityReadyPromiseDeferred.resolve();
            };

            $scope.addNewCollege = addNewCollege;
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadUniversitySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function loadUniversitySelector() {
            var universityLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            universityReadyPromiseDeferred.promise.then(function () {
                var directivePayload = undefined;
                VRUIUtilsService.callDirectiveLoad(universityDirectiveApi, directivePayload, universityLoadPromiseDeferred);
            });
            return universityLoadPromiseDeferred.promise;
        }
    
        function addNewCollege() {
            var onCollegeAdded = function (college) {
                if (gridAPI != undefined)
                    gridAPI.onCollegeAdded(college);
            };

            Demo_Module_CollegeService.addCollege(onCollegeAdded);
        }
    }




    appControllers.controller('Demo_Module_CollegeManagementController', collegeManagementController);
})(appControllers);