"use strict";

app.directive("vrWhsSalesRateTypeGrid", ["UtilsService", "VRNotificationService", "VRCommon_RateTypeAPIService",
function (UtilsService, VRNotificationService, VRCommon_RateTypeAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var rateTypeGrid = new RateTypeGrid($scope, ctrl, $attrs);
            rateTypeGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Sales/Directives/RateType/Templates/RateTypeGridTemplate.html"

    };

    function RateTypeGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.dataItem;
            $scope.rateTypes = [];
            $scope.onGridReady = function (api) {
                
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        $scope.dataItem = query.dataItem;
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onRateTypeAdded = function (rateTypeObject) {
                        gridAPI.itemAdded(rateTypeObject);
                    }
                    return directiveAPI;
                };

            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_RateTypeAPIService.GetFilteredRateTypes(dataRetrievalInput)
                    .then(function (response) {
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
