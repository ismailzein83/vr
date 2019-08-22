(function (app) {

    'use strict';

    windowsDirective.$inject = ['UtilsService'];

    function windowsDirective(UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new windowsDirectiveCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/Product/Directives/MainExtensions/Templates/WindowsTemplate.html'
        };

        function windowsDirectiveCtor($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                
                $scope.scopeModel.isValid = function () {
                    var licenseKey = $scope.scopeModel.licenseKey;
                    if (licenseKey == undefined)
                        return null;

                    var errorMessage = "License Key must respect this format: xxxxx-xxxxx-xxxxx-xxxxx-xxxxx";

                    var keys = licenseKey.split('-');
                    if (keys.length != 5)
                        return errorMessage;

                    for (var i = 0; i < keys.length; i++) {
                        var key = keys[i].trim().split(' ');
                        if (key[0].length != 5)
                            return errorMessage;
                    }

                    return null;
                };

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
                        $scope.scopeModel.licenseKey = softwareOperatingSystem.LicenseKey;
                    }

                    return UtilsService.waitMultiplePromises([]);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.OperatingSystem.Windows,Demo.Module.MainExtension",
                        Version: $scope.scopeModel.version,
                        LicenseKey: $scope.scopeModel.licenseKey
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }

    }

    app.directive('demoModuleOperatingsystemWindows', windowsDirective);

})(app);