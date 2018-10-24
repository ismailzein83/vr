(function (app) {

    'use strict';

    ActionRuleObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function ActionRuleObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var actionRuleObjectType = new actionRuleDierctiveObjectType($scope, ctrl, $attrs);
                actionRuleObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/ActionRuleObjectTypeTemplate.html'


        };
        function actionRuleDierctiveObjectType($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.ActionRuleObjectType, TOne.WhS.BusinessEntity.MainExtensions"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsBeActionruleobjecttype', ActionRuleObjectType);

})(app);




