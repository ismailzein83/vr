(function (app) {

    'use strict';

    RetailAccountFinancial.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function RetailAccountFinancial(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var retailAccountObjectType = new RetailAccountObjectType($scope, ctrl, $attrs);
                retailAccountObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/VRObjectType/PropertyEvaluator/Templates/FinancialRetailAccountEvaluatorTemplate.html"

        };
        function RetailAccountObjectType($scope, ctrl, $attrs) {
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
                        $type: "Retail.BusinessEntity.MainExtensions.VRObjectTypes.FinancialRetailAccountPropertyEvaluator, Retail.BusinessEntity.MainExtensions",
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeRetailaccountobjecttypeFinancial', RetailAccountFinancial);

})(app);