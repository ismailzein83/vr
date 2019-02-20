(function (app) {
    'use strict';
    probecustomobjectsettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function probecustomobjectsettingsDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new Probecustomobjectsettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/RecordAnalysis/Directives/C4Probe/Templates/C4ProbeCustomObjectSettings.html"
        };
        function Probecustomobjectsettings($scope, ctrl, $attrs) {
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
                        $type: "RecordAnalysis.MainExtensions.C4Probe.C4ProbeSettingsCustomObjectTypeSettings, RecordAnalysis.MainExtensions"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }
    
    app.directive('recAnalC4probeCustomobjectsettings', probecustomobjectsettingsDirective);

})(app);
