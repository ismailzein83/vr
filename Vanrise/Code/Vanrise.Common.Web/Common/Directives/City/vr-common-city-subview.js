"use strict";

app.directive("vrCommonCitySubview", ["UtilsService","VRCommon_CityService",
    function (UtilsService, VRCommon_CityService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var cityGrid = CityGrid($scope, ctrl, $attrs);
            cityGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/City/Templates/CitySubviewTemplate.html"

    };

        function CityGrid($scope, ctrl, $attrs) {

        var cityGridAPI;
        var cityGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var countryItem;
        return { initializeController: initializeController };
        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.onCityGridDirectiveReady = function (api) {
                cityGridAPI = api;
                cityGridReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.addCity= function () {
               
                var onCityAdded = function (cityObj) {
                    if (cityGridAPI != undefined) {
                        cityGridAPI.onCityAdded(cityObj);
                    }
                };
                VRCommon_CityService.addCity(onCityAdded, countryItem.Entity.CountryId);
            }
            defineAPI()
        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    countryItem = payload.countryItem;
                    var loadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadPromiseDeferred.promise);
                    cityGridReadyPromiseDeferred.promise.then(function () {
                        cityGridAPI.loadGrid(payload.query).then(function () {
                            loadPromiseDeferred.resolve();
                        });
                    });
                }
                var rootPromiseNode = { promises: promises };
                return UtilsService.waitPromiseNode(rootPromiseNode);
            };

       
            if (ctrl.onReady != null)
                ctrl.onReady(api);
            return api;
        }

    }

    return directiveDefinitionObject;

}]);