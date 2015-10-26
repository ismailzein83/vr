"use strict";

app.directive("vrWhsCdrprocessingSetsupplierruleGrid", ["UtilsService", "VRNotificationService", "WhS_CDRProcessing_MainService","WhS_CDRProcessing_SetSupplierRuleAPIService",
function (UtilsService, VRNotificationService, WhS_CDRProcessing_MainService,WhS_CDRProcessing_SetSupplierRuleAPIService) {

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
        templateUrl: "/Client/Modules/WhS_CDRProcessing/Directives/SupplierRule/Templates/SetSupplierRuleGrid.html"

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
                    directiveAPI.onSetSupplierRuleAdded = function (supplierRuleObj) {
                        gridAPI.itemAdded(supplierRuleObj);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_CDRProcessing_SetSupplierRuleAPIService.GetFilteredSetSupplierRules(dataRetrievalInput)
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
                            clicked: editSetSupplierRule,
                        },
                         {
                             name: "Delete",
                             clicked: deleteSetSupplierRule,
                         }
            ];

            $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
            }
        }

        function editSetSupplierRule(supplierRuleObj) {
            var onSetSupplierRuleUpdated = function (supplierRule) {
                gridAPI.itemUpdated(supplierRule);
            }

            WhS_CDRProcessing_MainService.editSetSupplierRule(supplierRuleObj.Entity, onSetSupplierRuleUpdated);
        }
        function deleteSetSupplierRule(supplierRuleObj) {
            var onSetSupplierRuleDeleted = function (supplierRule) {
                gridAPI.itemDeleted(supplierRule);
            };

            WhS_CDRProcessing_MainService.deleteSetSupplierRule($scope,supplierRuleObj, onSetSupplierRuleDeleted);
        }
    }

    return directiveDefinitionObject;

}]);
