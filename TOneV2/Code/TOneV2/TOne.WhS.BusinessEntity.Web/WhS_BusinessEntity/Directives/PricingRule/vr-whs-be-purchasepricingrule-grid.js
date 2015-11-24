﻿"use strict";

app.directive("vrWhsBePurchasepricingruleGrid", ["VRNotificationService", "WhS_BE_PurchasePricingRuleAPIService", "WhS_BE_PurchasePricingRuleService",
function (VRNotificationService, WhS_BE_PurchasePricingRuleAPIService, WhS_BE_PurchasePricingRuleService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var purchasePricingRuleGrid = new PurchasePricingRuleGrid($scope, ctrl, $attrs);
            purchasePricingRuleGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Templates/PurchasePricingRuleGrid.html"

    };

    function PurchasePricingRuleGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.purchasePricingRules = [];
            $scope.gridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onPricingRuleAdded = function (purchasePricingRuleObj) {
                        gridAPI.itemAdded(purchasePricingRuleObj);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_PurchasePricingRuleAPIService.GetFilteredPurchasePricingRules(dataRetrievalInput)
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
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editPurchasePricingRule,
            },
           {
               name: "Delete",
               clicked: deletePurchasePricingRule
           }
            ];
        }

        function editPurchasePricingRule(purchasePricingRuleObj) {
            var onPricingRuleUpdated = function (purchasePricingRule) {
                gridAPI.itemUpdated(purchasePricingRule);
            }
            var obj = {
                RuleId: purchasePricingRuleObj.Entity.RuleId,
                PricingType: purchasePricingRuleObj.Entity.Criteria.CriteriaType
            }
            WhS_BE_PurchasePricingRuleService.editPurchasePricingRule(obj, onPricingRuleUpdated);
        }
        function deletePurchasePricingRule(purchasePricingRuleObj) {
            var onPurchasePricingRuleDeleted = function (purchasePricingRuleObj) {
                gridAPI.itemDeleted(purchasePricingRuleObj);
            };
            WhS_BE_PurchasePricingRuleService.deletePurchasePricingRule($scope, purchasePricingRuleObj, onPurchasePricingRuleDeleted);
        }
    }

    return directiveDefinitionObject;

}]);
