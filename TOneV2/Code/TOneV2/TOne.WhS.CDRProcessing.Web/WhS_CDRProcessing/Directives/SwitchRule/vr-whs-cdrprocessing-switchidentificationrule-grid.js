"use strict";

app.directive("vrWhsCdrprocessingSwitchidentificationruleGrid", ["UtilsService", "VRNotificationService", "WhS_CDRProcessing_MainService", "WhS_CDRProcessing_SwitchIdentificationRuleAPIService",
function (UtilsService, VRNotificationService, WhS_CDRProcessing_MainService, WhS_CDRProcessing_SwitchIdentificationRuleAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var switchRuleGrid = new SwitchRuleGrid($scope, ctrl, $attrs);
            switchRuleGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_CDRProcessing/Directives/SwitchRule/Templates/SwitchIdentificationRuleGrid.html"

    };

    function SwitchRuleGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.switchRules = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onSwitchIdentificationRuleAdded = function (switchRuleObj) {
                        gridAPI.itemAdded(switchRuleObj);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_CDRProcessing_SwitchIdentificationRuleAPIService.GetFilteredSwitchIdentificationRules(dataRetrievalInput)
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
                            clicked: editSwitchIdentificationRule,
                        },
                         {
                             name: "Delete",
                             clicked: deleteSwitchIdentificationRule,
                         }
            ];

            $scope.gridMenuActions = function (dataItem) {
                return defaultMenuActions;
            }
        }

        function editSwitchIdentificationRule(switchRuleObj) {
            var onSwitchIdentificationRuleUpdated = function (switchRule) {
                gridAPI.itemUpdated(switchRule);
            }

            WhS_CDRProcessing_MainService.editSwitchIdentificationRule(switchRuleObj.Entity, onSwitchIdentificationRuleUpdated);
        }
        function deleteSwitchIdentificationRule(switchRuleObj) {
            var onSwitchIdentificationRuleObjDeleted = function (switchRule) {
                gridAPI.itemDeleted(switchRule);
            };

            WhS_CDRProcessing_MainService.deleteSwitchIdentificationRule($scope, switchRuleObj, onSwitchIdentificationRuleObjDeleted);
        }
    }

    return directiveDefinitionObject;

}]);
