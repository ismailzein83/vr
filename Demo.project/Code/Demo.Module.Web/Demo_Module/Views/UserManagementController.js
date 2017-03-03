(function (appControllers) {

    "use strict";

    userManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_UserAPIService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_UserService'];

    function userManagementController($scope, VRNotificationService, Demo_Module_UserAPIService, UtilsService, VRUIUtilsService, Demo_Module_UserService) {



        var CityDirectiveApi;
        var CityReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var gridAPI;
        var query = {};
        defineScope();
        load();





        function defineScope() {
            $scope.users = [];

            $scope.searchClicked = function () {
               
                getfilterdobject()
                getfilterdobject()
               
                gridAPI.loadGrid(query);
            };
            $scope.onCityDirectiveReady = function (api) {
                CityDirectiveApi = api;
                CityReadyPromiseDeferred.resolve();
            };
            function getfilterdobject() {
                query = {
                    Name: $scope.name,
                    CityId: CityDirectiveApi.getSelectedIds()
                };
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(query);
            };
            $scope.addNewUser = addNewUser;
        }
        function load() {

            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCitySelector])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoadingFilters = false;
             });

        }
        function addNewUser() {
            var onUserAdded = function (UserObj) {
                gridAPI.onUserAdded(UserObj);
            };

            Demo_Module_UserService.addUser(onUserAdded);
        }
        function loadCitySelector() {
            var CityLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            CityReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(CityDirectiveApi, undefined, CityLoadPromiseDeferred);
                });
            return CityLoadPromiseDeferred.promise;
        }

    }




    appControllers.controller('Demo_Module_UserManagementController', userManagementController);
})(appControllers);