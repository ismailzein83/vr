﻿(function (appControllers) {

    "use strict";

    cityManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_CityAPIService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_CityService'];

    function cityManagementController($scope, VRNotificationService, Demo_Module_CityAPIService, UtilsService, VRUIUtilsService, Demo_Module_CityService) {




        var gridAPI;
        var query = {};
        defineScope();
        load();


        
        
       
        function defineScope() {
            $scope.cities = [];

            $scope.searchClicked = function () {
                getfilterdobject()
                gridAPI.loadGrid(query);
            };

            function getfilterdobject() {
                query = {
                    Name: $scope.name
                };
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(query);
            };
            $scope.addNewCity = addNewCity;
        }
        function load() {
           
            loadAllControls();
        }

        function loadAllControls() {
           
        }
        function addNewCity() {
           var onCityAdded = function (cityObj) {
               gridAPI.onCityAdded(cityObj);
            };

            Demo_Module_CityService.addCity(onCityAdded);
        }
    }
       
    
       

    appControllers.controller('Demo_Module_CityManagementController', cityManagementController);
})(appControllers);