"use strict";

app.directive("vrDemoDestinationgroupGrid", ["UtilsService", "VRNotificationService", "Demo_DestinationGroupAPIService", "Demo_DestinationGroupService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Demo_DestinationGroupAPIService, Demo_DestinationGroupService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var destinationGroupGrid = new DestinationGroupGrid($scope, ctrl, $attrs);
            destinationGroupGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/DestinationGroup/Templates/DestinationGroupGridTemplate.html"

    };

    function DestinationGroupGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.destinationGroups = [];
            defineMenuActions();
            $scope.onGridReady = function (api) {
                gridAPI = api;


                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());


                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onDestinationGroupAdded = function (destinationGroupObject) {
                        gridAPI.itemAdded(destinationGroupObject);
                    }
                    return directiveAPI;
                }


            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Demo_DestinationGroupAPIService.GetFilteredDestinationGroups(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editDestinationGroup,
            }
            ];
        }

        function editDestinationGroup(destinationGroupObj) {
            var onDestinationGroupUpdated = function (destinationGroup) {
                gridAPI.itemUpdated(destinationGroup);

            }
            Demo_DestinationGroupService.editDestinationGroup(destinationGroupObj, onDestinationGroupUpdated);
        }

    }

    return directiveDefinitionObject;

}]);