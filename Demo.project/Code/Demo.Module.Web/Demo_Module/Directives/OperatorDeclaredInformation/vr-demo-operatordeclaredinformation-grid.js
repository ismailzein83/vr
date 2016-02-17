"use strict";

app.directive("vrDemoOperatordeclaredinformationGrid", ["UtilsService", "VRNotificationService", "Demo_OperatorDeclaredInformationAPIService", "Demo_OperatorDeclaredInformationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Demo_OperatorDeclaredInformationAPIService, Demo_OperatorDeclaredInformationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var operatorDeclaredInformationGrid = new OperatorDeclaredInformationGrid($scope, ctrl, $attrs);
            operatorDeclaredInformationGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/OperatorDeclaredInformation/Templates/OperatorDeclaredInformationGridTemplate.html"

    };

    function OperatorDeclaredInformationGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.operatorDeclaredInformations = [];
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
                    directiveAPI.onOperatorDeclaredInformationAdded = function (operatorDeclaredInformationObject) {
                        gridAPI.itemAdded(operatorDeclaredInformationObject);
                    }
                    return directiveAPI;
                }


            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Demo_OperatorDeclaredInformationAPIService.GetFilteredOperatorDeclaredInformations(dataRetrievalInput)
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
                clicked: editOperatorDeclaredInformation,
            }
            ];
        }

        function editOperatorDeclaredInformation(operatorDeclaredInformationObj) {
            var onOperatorDeclaredInformationUpdated = function (operatorDeclaredInformation) {
                gridAPI.itemUpdated(operatorDeclaredInformation);

            }
            Demo_OperatorDeclaredInformationService.editOperatorDeclaredInformation(operatorDeclaredInformationObj, onOperatorDeclaredInformationUpdated);
        }

    }

    return directiveDefinitionObject;

}]);