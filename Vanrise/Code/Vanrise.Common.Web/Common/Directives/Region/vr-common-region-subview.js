"use strict";

app.directive("vrCommonRegionSubview", ["UtilsService","VRCommon_RegionService",
    function (UtilsService, VRCommon_RegionService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var regionGrid = RegionGrid($scope, ctrl, $attrs);
            regionGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/Region/Templates/RegionSubviewTemplate.html"

    };

    function RegionGrid($scope, ctrl, $attrs) {

        var regionGridAPI;
        var regionGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var countryItem;
        return { initializeController: initializeController };
        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.onRegionGridDirectiveReady = function (api) {
                regionGridAPI = api;
                regionGridReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.addRegion= function () {
               
                var onRegionAdded = function (regionObj) {
                    if (regionGridAPI != undefined) {
                        regionGridAPI.onRegionAdded(regionObj);
                    }
                };
                VRCommon_RegionService.addRegion(onRegionAdded, countryItem.Entity.CountryId);
            }
            defineAPI();
        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    countryItem = payload.countryItem;
                    var loadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadPromiseDeferred.promise);
                    regionGridReadyPromiseDeferred.promise.then(function () {
                        regionGridAPI.loadGrid(payload.query).then(function () {
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