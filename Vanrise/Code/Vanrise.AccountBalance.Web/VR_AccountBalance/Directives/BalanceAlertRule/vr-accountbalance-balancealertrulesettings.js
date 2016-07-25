'use strict';
app.directive('vrAccountbalanceBalancealertrulesettings', ['UtilsService','VR_AccountBalance_BalanceAlertService',
function (UtilsService, VR_AccountBalance_BalanceAlertService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new BalanceAlartruleSettings(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_AccountBalance/Directives/BalanceAlertRule/Templates/BalanceAlertRuleSettingsTemplate.html"

    };


    function BalanceAlartruleSettings(ctrl, $scope, $attrs) {

        function initializeController() {

            ctrl.datasource = [];

            ctrl.isValid = function () {

                if (ctrl.datasource.length > 0)
                    return null;
                return "You Should Select at least one threshold action.";
            }

            ctrl.addThresholdAction = function () {
                var onBalanceAlertThresholdAdded = function (balanceAlertThreshold)
                {
                    ctrl.datasource.push({ Entity: balanceAlertThreshold });
                }
                VR_AccountBalance_BalanceAlertService.addBalanceAlertThreshold(onBalanceAlertThresholdAdded);
            };

            ctrl.removeThresholdAction = function (dataItem) {
                var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.id, 'id');
                ctrl.datasource.splice(index, 1);
            };

            defineAPI();
            defineMenuActions();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var data = [];
                if(ctrl.datasource.length >0)
                {
                    for(var i=0;i<ctrl.datasource.length;i++)
                    {
                        data.push(ctrl.datasource[i].Entity);
                    }
                }
                var result = {
                   ThresholdActions: data
                }
                return result;
            }
          
            api.load = function (payload) {
                if(payload !=undefined && payload.settings !=undefined)
                {
                    if(payload.settings.ThresholdActions !=undefined)
                    {
                        for(var i=0;i<payload.settings.ThresholdActions.length;i++)
                        {
                            ctrl.datasource.push({ Entity: payload.settings.ThresholdActions[i] });
                        }
                    }
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {

            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editThresholdAction,
            }];
        }

        function editThresholdAction(dataItem) {
            var onThresholdActionUpdated = function (thresholdActionObj) {
                ctrl.datasource[ctrl.datasource.indexOf(dataItem)] = { Entity: thresholdActionObj };
            }
            VR_AccountBalance_BalanceAlertService.editBalanceAlertThreshold(dataItem.Entity, onThresholdActionUpdated);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);