(function (app) {

    'use strict';

    whsJazzAmountCalculationCustomObjectStaticEditor.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function whsJazzAmountCalculationCustomObjectStaticEditor(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                onselectionchanged: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Jazz/Elements/Directives/AmountCalculation/Templates/AmountCalculationCustomObjectStaticEditor.html"

        };
        function SettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var rateCalculationTypeSelectorAPI;
            var rateCalculationTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onRateCalculationTypeSelectorReady = function (api) {
                    rateCalculationTypeSelectorAPI = api;
                    rateCalculationTypeSelectorReadyPromiseDeferred.resolve();
                };


                defineAPI();
            }
            function loadRateCalculationTypeSelector() {
                var rateCalculationTypeSelectorLoadromiseDeferred = UtilsService.createPromiseDeferred();
                rateCalculationTypeSelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(rateCalculationTypeSelectorAPI, undefined, rateCalculationTypeSelectorLoadromiseDeferred);
                });
                return rateCalculationTypeSelectorLoadromiseDeferred.promise;
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {console.log(payload)

                    return loadRateCalculationTypeSelector();

                };

                api.setData = function (payload) {
                    payload.AmountCalculation = rateCalculationTypeSelectorAPI.getSelectedIds()
                    return payload;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsJazzAmountcalculationCustomobjectStaticeditor', whsJazzAmountCalculationCustomObjectStaticEditor);

})(app);
