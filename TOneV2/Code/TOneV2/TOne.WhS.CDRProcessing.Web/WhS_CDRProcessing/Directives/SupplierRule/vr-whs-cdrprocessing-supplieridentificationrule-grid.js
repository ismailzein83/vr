"use strict";

app.directive("vrWhsCdrprocessingSupplieridentificationruleGrid", ["UtilsService", "VRNotificationService", "WhS_CDRProcessing_MainService", "WhS_CDRProcessing_SupplierIdentificationRuleAPIService",
function (UtilsService, VRNotificationService, WhS_CDRProcessing_MainService,WhS_CDRProcessing_SupplierIdentificationRuleAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var supplierRuleGrid = new SupplierRuleGrid($scope, ctrl, $attrs);
            supplierRuleGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_CDRProcessing/Directives/SupplierRule/Templates/SupplierIdentificationRuleGrid.html"

    };

    function SupplierRuleGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.supplierRules = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onSupplierIdentificationRuleAdded = function (supplierRuleObj) {
                        gridAPI.itemAdded(supplierRuleObj);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_CDRProcessing_SupplierIdentificationRuleAPIService.GetFilteredSupplierIdentificationRules(dataRetrievalInput)
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
                            clicked: editSupplierIdentificationRule,
                        },
                         {
                             name: "Delete",
                             clicked: deleteSupplierIdentificationRule,
                         }
            ];

            $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
            }
        }

        function editSupplierIdentificationRule(supplierRuleObj) {
            var onSupplierIdentificationRuleUpdated = function (supplierRule) {
                gridAPI.itemUpdated(supplierRule);
            }

            WhS_CDRProcessing_MainService.editSupplierIdentificationRule(supplierRuleObj.Entity, onSupplierIdentificationRuleUpdated);
        }
        function deleteSupplierIdentificationRule(supplierRuleObj) {
            var onSupplierIdentificationRuleDeleted = function (supplierRule) {
                gridAPI.itemDeleted(supplierRule);
            };

            WhS_CDRProcessing_MainService.deleteSupplierIdentificationRule($scope,supplierRuleObj, onSupplierIdentificationRuleDeleted);
        }
    }

    return directiveDefinitionObject;

}]);
