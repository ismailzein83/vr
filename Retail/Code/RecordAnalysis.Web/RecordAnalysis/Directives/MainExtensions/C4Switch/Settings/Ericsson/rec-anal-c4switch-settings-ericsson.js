(function (app) {

    'use strict';

    RecordAnalysisC4SwitchSettingsEricssonDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function RecordAnalysisC4SwitchSettingsEricssonDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope:
            {
                onReady: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new EricssonSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ericssonSettingsCtrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/RecordAnalysis/Directives/MainExtensions/C4Switch/Settings/Ericsson/Templates/EricssonC4SwitchSettingsTemplate.html"
        };

        function EricssonSettings($scope, ctrl, $attrs) {
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
                        $scope.host = settings.Host;
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'RecordAnalysis.MainExtensions.C4Switch.EricssonC4SwitchSettings, RecordAnalysis.MainExtensions',
                        Host: $scope.host
                    };
                };

                return api;
            }
        }
    }

    app.directive('recAnalC4switchSettingsEricsson', RecordAnalysisC4SwitchSettingsEricssonDirective);

})(app);