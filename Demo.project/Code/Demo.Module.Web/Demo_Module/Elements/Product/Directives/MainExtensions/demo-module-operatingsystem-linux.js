(function (app) {

    'use strict';

    linuxDirective.$inject = ['UtilsService'];

    function linuxDirective(UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new linuxDirectiveCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/Product/Directives/MainExtensions/Templates/LinuxTemplate.html'
        };

        function linuxDirectiveCtor($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var softwareOperatingSystem;

                    if (payload != undefined) {
                        softwareOperatingSystem = payload.softwareOperatingSystem;
                    }

                    if (softwareOperatingSystem != undefined) {
                        $scope.scopeModel.version = softwareOperatingSystem.Version;
                        $scope.scopeModel.hasGUI = softwareOperatingSystem.HasGUI;
                    }

                    return UtilsService.waitMultiplePromises([]);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.OperatingSystem.Linux,Demo.Module.MainExtension",
                        Version: $scope.scopeModel.version,
                        HasGUI: $scope.scopeModel.hasGUI
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }

    }

    app.directive('demoModuleOperatingsystemLinux', linuxDirective);

})(app);