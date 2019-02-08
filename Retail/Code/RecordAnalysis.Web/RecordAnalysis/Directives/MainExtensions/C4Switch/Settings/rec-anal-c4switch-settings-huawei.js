(function (app) {

    'use strict';

    RecordAnalysisC4SwitchSettingsHuaweiDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function RecordAnalysisC4SwitchSettingsHuaweiDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope:
            {
                onReady: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HuaweiSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "huaweiSettingsCtrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/RecordAnalysis/Directives/MainExtensions/C4Switch/Settings/Templates/HuaweiC4SwitchSettingsTemplate.html"
        };

        function HuaweiSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(getDirectiveAPI());
                }
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var settings;
                    if (payload != undefined) {
                        settings = payload.settings;
                    }

                    if (settings != undefined) {
                        $scope.ip = settings.IP;
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'RecordAnalysis.MainExtensions.C4Switch.HuaweiC4SwitchSettings, RecordAnalysis.MainExtensions',
                        IP: $scope.ip
                    };
                };

                return api;
            }
        }
    }

    app.directive('recAnalC4switchSettingsHuawei', RecordAnalysisC4SwitchSettingsHuaweiDirective);

})(app);