"use strict";

app.directive("vrInterconnectBeOperatoraccountGrid", ["UtilsService", "VRNotificationService", "InterConnect_BE_OperatorAccountAPIService", "InterConnect_BE_OperatorAccountService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, InterConnect_BE_OperatorAccountAPIService, InterConnect_BE_OperatorAccountService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var operatorAccountGrid = new OperatorAccountGrid($scope, ctrl, $attrs);
            operatorAccountGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/InterConnect_BusinessEntity/Directives/OperatorAccount/Templates/OperatorAccountGridTemplate.html"

    };

    function OperatorAccountGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.operatorAccounts = [];
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
                    directiveAPI.onOperatorAccountAdded = function (operatorAccountObject) {
                        gridAPI.itemAdded(operatorAccountObject);
                    }
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return InterConnect_BE_OperatorAccountAPIService.GetFilteredOperatorAccounts(dataRetrievalInput)
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
                clicked: editOperatorAccount,
            }];
        }

        function editOperatorAccount(operatorAccountObj) {
            var onOperatorAccountUpdated = function (operatorAccount) {
                gridAPI.itemUpdated(operatorAccount);
            }
            InterConnect_BE_OperatorAccountService.editOperatorAccount(operatorAccountObj, onOperatorAccountUpdated);
        }
    }

    return directiveDefinitionObject;

}]);