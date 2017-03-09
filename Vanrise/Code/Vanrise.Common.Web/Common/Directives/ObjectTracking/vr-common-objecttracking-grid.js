"use strict";

app.directive("vrCommonObjecttrackingGrid", ["UtilsService", "VRNotificationService","VRUIUtilsService","VRCommon_ObjectTrackingAPIService",
function (UtilsService, VRNotificationService, VRUIUtilsService,VRCommon_ObjectTrackingAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var objecttrackingGrid = new objectTrackingGrid($scope, ctrl, $attrs);
            objecttrackingGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/ObjectTracking/Templates/ObjectTrackingGridTemplate.html"

    };

    function objectTrackingGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.objects = [];
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
                return VRCommon_ObjectTrackingAPIService.GetFilteredObjectTracking(dataRetrievalInput)
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