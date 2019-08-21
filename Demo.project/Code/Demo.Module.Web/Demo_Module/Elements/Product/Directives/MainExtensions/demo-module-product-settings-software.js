(function (app) {

    'use strict';

    softwareSettings.$inject = ['UtilsService', 'Demo_Module_ProductAPIService'];

    function softwareSettings(UtilsService, Demo_Module_ProductAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope) {
                var ctrl = this;
                var ctor = new softwareSettingsCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/Product/Directives/MainExtensions/Templates/SoftwareTemplate.html'
        };

        function softwareSettingsCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var operatingSystemGridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onOperatingSystemGridReady = function (api) {
                    operatingSystemGridAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var settings;

                    if (payload != undefined) {
                        settings = payload.Settings;
                    }

                    if (settings != undefined) {
                        $scope.scopeModel.size = settings.Size;
                        $scope.scopeModel.language = settings.Language;
                    }

                    var loadSoftwareOperatingSystemGridPromise = loadSoftwareOperatingSystemGrid();
                    promises.push(loadSoftwareOperatingSystemGridPromise);

                    function loadSoftwareOperatingSystemGrid() {
                        var payload;

                        if (settings != undefined) {
                            payload = {
                                softwareOperatingSystems: settings.SoftwareOperatingSystems
                            };
                        }

                        return operatingSystemGridAPI.load(payload);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Demo.Module.MainExtension.Product.Software,Demo.Module.MainExtension",
                        Size: $scope.scopeModel.size,
                        Language: $scope.scopeModel.language,
                        SoftwareOperatingSystems: operatingSystemGridAPI.getData()
                    };

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('demoModuleProductSettingsSoftware', softwareSettings);

})(app);