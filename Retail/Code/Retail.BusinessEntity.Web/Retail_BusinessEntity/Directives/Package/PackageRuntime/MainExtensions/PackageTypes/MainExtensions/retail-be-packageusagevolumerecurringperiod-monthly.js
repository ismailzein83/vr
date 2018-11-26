(function (app) {

    'use strict';

    RecurchargeEvaluatorPeriodicDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_PeriodicRecurringChargePeriodTypeEnum'];

    function RecurchargeEvaluatorPeriodicDirective(UtilsService, VRNotificationService, Retail_BE_PeriodicRecurringChargePeriodTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new RecurChargePackageSettings($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageRuntime/MainExtensions/PackageTypes/MainExtensions/Templates/MonthlyPackageUsageVolumeRecurringPeriodTemplate.html'
        };

        function RecurChargePackageSettings($scope, ctrl) {
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
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.PackageUsageVolumeRecurringPeriods.MonthlyPackageUsageVolumeRecurringPeriod, Retail.BusinessEntity.MainExtensions"
                    };
                    
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBePackagesettingsRecurchargeEvaluatorPeriodic', RecurchargeEvaluatorPeriodicDirective);

})(app);