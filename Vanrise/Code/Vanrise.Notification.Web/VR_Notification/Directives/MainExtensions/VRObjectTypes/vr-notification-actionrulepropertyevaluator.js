(function (app) {

    'use strict';

    ActionRulePropertyEvaluator.$inject = ['VR_Notification_ActionRulePropertyEnum', 'UtilsService', 'VRUIUtilsService'];

    function ActionRulePropertyEvaluator(VR_Notification_ActionRulePropertyEnum, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var selector = new ActionRuleSelectorPropertyEvaluator($scope, ctrl, $attrs);
                selector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Notification/Directives/MainExtensions/VRObjectTypes/Templates/ActionRulePropertyEvaluatorTemplate.html'
        };

        function ActionRuleSelectorPropertyEvaluator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.actionRuleFields = UtilsService.getArrayEnum(VR_Notification_ActionRulePropertyEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                        $scope.scopeModel.selectedActionRuleField = UtilsService.getItemByVal($scope.scopeModel.actionRuleFields, payload.objectPropertyEvaluator.ActionRuleField, "value");
                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Notification.MainExtensions.ActionRulePropertyEvaluator, Vanrise.Notification.MainExtensions",
                        ActionRuleField: $scope.scopeModel.selectedActionRuleField.value
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrNotificationActionrulepropertyevaluator', ActionRulePropertyEvaluator);

})(app);
