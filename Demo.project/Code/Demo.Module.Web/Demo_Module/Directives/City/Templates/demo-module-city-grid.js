"use strict";

app.directive("demoModuleCityGrid", ["UtilsService", "VRNotificationService", "Demo_Module_CityAPIService", "Demo_Module_CityService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Demo_Module_CityAPIService, Demo_Module_CityService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var cityGrid = new CityGrid($scope, ctrl, $attrs);
            cityGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/City/Templates/CityGridTemplate.html"

    };

    function CityGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.cities = [];
            $scope.onGridReady = function (api) {

                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                       
                        
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onCityAdded = function (cityObject) {
                        gridAPI.itemAdded(cityObject);
                    };
                    return directiveAPI;
                }
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
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editCity,
                
            }];
        }

       

        function editCity(cityObj) {
            var onCityUpdated = function (cityObj) {
                gridAPI.itemUpdated(cityObj);
            };

            Demo_Module_CityService.editCity(cityObj.Entity.Id, onCityUpdated);
        }

    }

    return directiveDefinitionObject;

}]);