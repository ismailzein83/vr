(function (app) {

    'use strict';

    AssignableToPackageConditionDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function AssignableToPackageConditionDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AssignableToPackageConditionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountCondition/MainExtensions/Templates/AssignableToPackageConditionTemplate.html"
        };

        function AssignableToPackageConditionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;

            function initializeController() {
                $scope.scopeModel = {}; 
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var accountCondition;
                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        accountCondition = payload.accountCondition;
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountConditions.AssignableToPackageCondition, Retail.BusinessEntity.MainExtensions",
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('retailBeAccountconditionAssignabletopackage', AssignableToPackageConditionDirective);

})(app);