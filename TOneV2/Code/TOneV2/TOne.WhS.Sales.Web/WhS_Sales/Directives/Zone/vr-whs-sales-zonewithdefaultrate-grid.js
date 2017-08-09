
"use strict";

app.directive("vrWhsSalesZonewithdefaultrateGrid", ["UtilsService", "VRNotificationService", "WhS_Sales_ZoneWithDefaultRateAPIService", "VRCommon_CountryService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, WhS_Sales_ZoneWithDefaultRateAPIService, VRCommon_CountryService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var zoneGrid = new ZoneGrid($scope, ctrl, $attrs);
            zoneGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Zone/Templates/ZoneWithDefaultRateGridTemplate.html'

    };

    function ZoneGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.zones = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

               

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.load = function (query) {

                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Sales_ZoneWithDefaultRateAPIService.GetFilteredZones(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        }

    }

    return directiveDefinitionObject;

}]);
