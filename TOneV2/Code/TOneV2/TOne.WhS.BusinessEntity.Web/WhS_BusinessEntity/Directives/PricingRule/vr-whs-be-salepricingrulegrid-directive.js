"use strict";

app.directive("vrWhsBeSalepricingrulegrid", ["VRNotificationService", "WhS_BE_SalePricingRuleAPIService", "WhS_BE_MainService",
function ( VRNotificationService, WhS_BE_SalePricingRuleAPIService, WhS_BE_MainService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var salePricingRuleGrid = new SalePricingRuleGrid($scope, ctrl, $attrs);
            salePricingRuleGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Templates/SalePricingRuleGrid.html"

    };

    function SalePricingRuleGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.salePricingRules = [];
            $scope.gridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onPricingRuleAdded = function (salePricingRuleObj) {
                        gridAPI.itemAdded(salePricingRuleObj);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SalePricingRuleAPIService.GetFilteredSalePricingRules(dataRetrievalInput)
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
                clicked: editSalePricingRule,
            },
           {
               name: "Delete",
               clicked: deleteSalePricingRule
           }
            ];
        }

        function editSalePricingRule(salePricingRuleObj) {
           
            var onPricingRuleUpdated = function (salePricingRule) {

                gridAPI.itemUpdated(salePricingRule);
            }
            var obj = {
                RuleId:salePricingRuleObj.Entity.RuleId,
                PricingType: salePricingRuleObj.Entity.Criteria.CriteriaType
            }
            WhS_BE_MainService.editSalePricingRule(obj, onPricingRuleUpdated);
        }
        function deleteSalePricingRule(salePricingRuleObj) {
            var onSalePricingRuleDeleted = function (salePricingRuleObj) {
                gridAPI.itemDeleted(salePricingRuleObj);
            };
            WhS_BE_MainService.deleteSalePricingRule($scope, salePricingRuleObj, onSalePricingRuleDeleted);
        }
    }

    return directiveDefinitionObject;

}]);
