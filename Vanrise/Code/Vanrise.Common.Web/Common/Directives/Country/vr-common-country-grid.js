"use strict";

app.directive("vrCommonCountryGrid", ["UtilsService", "VRNotificationService", "VRCommon_CountryAPIService", "VRCommon_CountryService",
function (UtilsService, VRNotificationService, VRCommon_CountryAPIService, VRCommon_CountryService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var countryGrid = new CountryGrid($scope, ctrl, $attrs);
            countryGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/Country/Templates/CountryGridTemplate.html"

    };

    function CountryGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
           
            $scope.countries = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                       
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onCountryAdded = function (countryObject) {
                        gridAPI.itemAdded(countryObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_CountryAPIService.GetFilteredCountries(dataRetrievalInput)
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
               clicked: editCountry
            }];
        }

        function editCountry(countryObj) {
            var onCountryUpdated = function (countryObj) {
                gridAPI.itemUpdated(countryObj);
            }

            VRCommon_CountryService.editCountry(countryObj, onCountryUpdated);
        }
              
    }

    return directiveDefinitionObject;

}]);
