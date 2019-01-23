﻿(function (app) {

    'use strict';

    whsJazzAmountCalculationCustomObjectSettings.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function whsJazzAmountCalculationCustomObjectSettings(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Jazz/Elements/Directives/AmountCalculation/Templates/AmountCalculationCustomObjectSettings.html"

        };
        function SettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;

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
                        $type: "TOne.WhS.Jazz.Business.WhSJazzAmountCalculationCustomObject,TOne.WhS.Jazz.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsJazzAmountcalculationCustomobjectsettings', whsJazzAmountCalculationCustomObjectSettings);

})(app);