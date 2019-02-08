(function (app) {
    'use strict';
    c4switchcustomobjectsettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function c4switchcustomobjectsettingsDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new C4switchcustomobjectsettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_MobileNetwork/Directives/MobileNetwork/Templates/MobileNetworkSettingsCustomObjectSettingsTemplate.html"
        };
        function C4switchcustomobjectsettings($scope, ctrl, $attrs) {
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
                        $type: "RecordAnalysis.MainExtensions.C4Switch.C4SwitchSettingsCustomObjectTypeSettings, RecordAnalysis.MainExtensions"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }
    
    app.directive('recAnalC4switchCustomobjectsettings', c4switchcustomobjectsettingsDirective);

})(app);
