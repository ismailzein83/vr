(function (appControllers) {

    "use strict";

    cityManagementController.$inject = ['$scope', 'Demo_Module_CityAPIService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_CityService'];

    function cityManagementController($scope, Demo_Module_CityAPIService, UtilsService, VRUIUtilsService, Demo_Module_CityService) {




        var gridAPI;
        var query = {};
        defineScope();
        


        function getfilterdobject() {
            query = {
                Name: $scope.name
            };
        }
        function editCity(cityObj) {
            var onCityUpdated = function (cityObj) {
                gridAPI.itemUpdated(cityObj);
            };

            Demo_Module_CityService.editCity(cityObj.Entity.Id, onCityUpdated);
        }
        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editCity,

            }];
        }
       
        function defineScope() {
            $scope.cities = [];

            $scope.searchClicked = function () {
                getfilterdobject()
                $scope.onGridReady(gridAPI);
            };

            $scope.onGridReady = function (api) {
                console.log(api);
                gridAPI = api;
                gridAPI.retrieveData(query);

            };
           
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Demo_Module_CityAPIService.GetFilteredCities(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
               
            };
            defineMenuActions();
            $scope.addNewCity = addNewCity;
        }
      
        function addNewCity() {
           var onCityAdded = function (cityObj) {
                gridAPI.itemAdded(cityObj);
            };

            Demo_Module_CityService.addCity(onCityAdded);
        }
    }
       
   
       

    appControllers.controller('Demo_Module_CityManagementController', cityManagementController);
})(appControllers);