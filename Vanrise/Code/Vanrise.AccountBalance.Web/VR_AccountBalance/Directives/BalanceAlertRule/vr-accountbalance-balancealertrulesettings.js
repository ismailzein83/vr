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
                    ctrl.datasource.push({ Entity: onBalanceAlertThresholdAdded });
                }
                VR_AccountBalance_BalanceAlertService.addBalanceAlertThreshold(onBalanceAlertThresholdAdded);
            };

            ctrl.removeThresholdAction = function (dataItem) {
                var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.id, 'id');
                ctrl.datasource.splice(index, 1);
            };

            defineAPI();

        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
            }
          
            api.load = function (payload) {
               
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);