(function (app) {

    'use strict';

    hardwareSettings.$inject = ['UtilsService', 'MaterialEnum', 'VRUIUtilsService'];

    function hardwareSettings(UtilsService, MaterialEnum, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new hardwareSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/Product/Directives/MainExtensions/Templates/HardwareTemplate.html'
        };

        function hardwareSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var materialSelectorAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onMaterialSelectorReady = function (api) {
                    materialSelectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                ctrl.datasource = UtilsService.getArrayEnum(MaterialEnum);
                ctrl.selectedvalues = [];

                var api = {};

                api.load = function (payload) {
                    var settings;

                    if (payload != undefined) {
                        settings = payload.Settings;
                    }

                    if (settings != undefined) {
                        $scope.scopeModel.weight = settings.Weight;
                        $scope.scopeModel.volume = settings.Volume;
                        if (settings.Materials != undefined && settings.Materials != null) {
                            for (var i = 0; i < settings.Materials.length; i++) {
                                var selectedValue = UtilsService.getItemByVal(ctrl.datasource, settings.Materials[i], 'value');
                                ctrl.selectedvalues.push(selectedValue);
                            }

                            //$attrs.ismultipleselection = 'ismultipleselection';
                            //VRUIUtilsService.setSelectedValues(settings.Materials, 'value', $attrs, ctrl);
                        }
                    }

                    return UtilsService.waitMultiplePromises([]);
                };

                api.getData = function () {
                    var materials = UtilsService.getPropValuesFromArray(ctrl.selectedvalues, 'value');

                    var data = {
                        $type: "Demo.Module.MainExtension.Product.Hardware,Demo.Module.MainExtension",
                        Weight: $scope.scopeModel.weight,
                        Volume: $scope.scopeModel.volume,
                        Materials: materials
                    };

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('demoModuleProductSettingsHardware', hardwareSettings);

})(app);