"use strict";

app.directive("vrWhsCdrprocessingSetcustomerruleGrid", ["UtilsService", "VRNotificationService", "WhS_CDRProcessing_MainService", "WhS_CDRProcessing_SetCustomerRuleAPIService",
function (UtilsService, VRNotificationService, WhS_CDRProcessing_MainService, WhS_CDRProcessing_SetCustomerRuleAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var customerRuleGrid = new CustomerRuleGrid($scope, ctrl, $attrs);
            customerRuleGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_CDRProcessing/Directives/CustomerRule/Templates/SetCustomerRuleGrid.html"

    };

    function CustomerRuleGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.customerRules = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onSetCustomerRuleAdded = function (customerRuleObj) {
                        gridAPI.itemAdded(customerRuleObj);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_CDRProcessing_SetCustomerRuleAPIService.GetFilteredSetCustomerRules(dataRetrievalInput)
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
            var defaultMenuActions = [
                        {
                            name: "Edit",
                            clicked: editSetCustomerRule,
                        },
                         {
                             name: "Delete",
                             clicked: deleteSetCustomerRule,
                         }
            ];

            $scope.gridMenuActions = function (dataItem) {
                return defaultMenuActions;
            }
        }

        function editSetCustomerRule(customerRuleObj) {
            var onSetCustomerRuleUpdated = function (customerRule) {
                gridAPI.itemUpdated(customerRule);
            }

            WhS_CDRProcessing_MainService.editSetCustomerRule(customerRuleObj.Entity, onSetCustomerRuleUpdated);
        }
        function deleteSetCustomerRule(customerRuleObj) {
            var onSetCustomerRuleObjDeleted = function (customerRule) {
                gridAPI.itemDeleted(customerRule);
            };

            WhS_CDRProcessing_MainService.deleteSetCustomerRule($scope,customerRuleObj, onSetCustomerRuleObjDeleted);
        }
    }

    return directiveDefinitionObject;

}]);
