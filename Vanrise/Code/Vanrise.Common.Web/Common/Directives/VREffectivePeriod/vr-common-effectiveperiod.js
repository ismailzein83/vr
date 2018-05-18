(function (app) {

    'use strict';

    EffectivePeriodDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function EffectivePeriodDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new EffectivePeriod($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,

            templateUrl: "/Client/Modules/Common/Directives/VREffectivePeriod/Templates/EffectivePeriodDirectiveTemplate.html"
        };

        function EffectivePeriod($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            
            function initializeController() {

                $scope.scopeModel.validateDates = function (date) {
                    return UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
                };
            
                $scope.scopeModel.validateEEDDate = function (date) {
                    return UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    
                    if (payload != undefined) {
                        $scope.scopeModel.beginEffectiveDate = payload.BED;
                        $scope.scopeModel.endEffectiveDate = payload.EED;
                    }
                };

                api.getData = function () {
                    return {
                        BED: $scope.scopeModel.beginEffectiveDate,
                        EED: $scope.scopeModel.endEffectiveDate
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }
    app.directive('vrCommonEffectiveperiod', EffectivePeriodDirective);
})(app);