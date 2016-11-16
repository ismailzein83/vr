"use strict";

app.directive('vrWhsSalesSellingrulesGrid', ['VRNotificationService', 'WhS_Sales_SellingRuleAPIService', 'WhS_Sales_SellingRuleService',
function (VRNotificationService, WhS_Sales_SellingRuleAPIService, WhS_Sales_SellingRuleService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var sellingRuleGridCtor = new sellingRuleGrid($scope, ctrl, $attrs);
            sellingRuleGridCtor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Sales/Directives/SellingRules/Templates/SellingRulesGridTemplate.html"

    };

    function sellingRuleGrid($scope, ctrl, $attrs) {
        var gridAPI;

        function initializeController() {
            $scope.sellingRules = [];
            $scope.hideCustomerColumn = true;
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (payload) {
                        var query = payload;
                        if (query.loadedFromSellingProduct) {
                            $scope.hideCustomerColumn = false;
                            query = payload.query;
                        }
                        return gridAPI.retrieveData(query);
                    };

                    directiveAPI.onSellingRuleAdded = function (sellingRuleObject) {
                        gridAPI.itemAdded(sellingRuleObject);
                    };

                    directiveAPI.onSellingRuleUpdated = function (sellingRuleObject) {
                        gridAPI.itemUpdated(sellingRuleObject);
                    };

                    return directiveAPI;
                }

            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Sales_SellingRuleAPIService.GetFilteredSellingRules(dataRetrievalInput)
                   .then(function (response) {
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editSellingRule,
                haspermission: hasUpdateRulePermission
            },
            {
                name: "Delete",
                clicked: deleteSellingRule,
                haspermission: hasDeleteRulePermission
            }
            ];
        }

        function hasUpdateRulePermission() {
            return WhS_Sales_SellingRuleAPIService.HasUpdateRulePermission();
        }

        function hasDeleteRulePermission() {
            return WhS_Sales_SellingRuleAPIService.HasDeleteRulePermission();
        }

        function editSellingRule(sellingRule) {
            var onSellingRuleUpdated = function (updatedItem) {
                gridAPI.itemUpdated(updatedItem);
            };
            WhS_Sales_SellingRuleService.editSellingRule(sellingRule.Entity.RuleId, onSellingRuleUpdated);
        }

        function deleteSellingRule(sellingRule) {

            var onSellingRuleDeleted = function (deletedItem) {
                gridAPI.itemDeleted(deletedItem);
            };

            WhS_Sales_SellingRuleService.deleteSellingRule($scope, sellingRule, onSellingRuleDeleted);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
