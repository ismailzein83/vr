"use strict";

app.directive("vrQmBeZoneGrid", ["UtilsService", "VRNotificationService", "QM_BE_ZoneAPIService",
function (UtilsService, VRNotificationService, QM_BE_ZoneAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new ZoneGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Qm_BusinessEntity/Directives/Zone/Templates/ZoneGridTemplate.html"

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

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                   
                    return directiveAPI;
                }
            };

          

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                console.log(dataRetrievalInput)
                return QM_BE_ZoneAPIService.GetFilteredZones(dataRetrievalInput)
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
