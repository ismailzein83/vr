(function (app) {
    'use strict';
    switchInterconnectioncustomobjectsettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function switchInterconnectioncustomobjectsettingsDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SwitchInterconnectioncustomobjectsettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/RecordAnalysis/Directives/SwitchInterconnection/Templates/SwitchInterconnectionCustomObjectSettings.html"
        };
        function SwitchInterconnectioncustomobjectsettings($scope, ctrl, $attrs) {
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
                        $type: "RecordAnalysis.MainExtensions.SwitchInterconnection.SwitchInterconnectionSettingsCustomObjectTypeSettings, RecordAnalysis.MainExtensions"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }
    
    app.directive('recAnalSwitchinterconnectionCustomobjectsettings', switchInterconnectioncustomobjectsettingsDirective);

})(app);
