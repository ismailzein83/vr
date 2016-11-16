'use strict';

app.directive('vrWhsBeCdrimportSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new cdrImportEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CDRImportSettings/Templates/CDRImportSettingsTemplate.html"
        };

        function cdrImportEditorCtor(ctrl, $scope, $attrs) {

            var switchCDRMappingConfigurationDirectiveAPI;
            var switchCDRMappingConfigurationDirectiveDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSwitchCDRMappingConfiguration = function (api) {
                    switchCDRMappingConfigurationDirectiveAPI = api;
                    switchCDRMappingConfigurationDirectiveDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([switchCDRMappingConfigurationDirectiveDeferred.promise]).then(function () {
                    defineAPI();
                });

            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var switchCDRMappingConfiguration;

                    if (payload != undefined && payload.data != undefined) {
                        switchCDRMappingConfiguration = payload.data.SwitchCDRMappingConfiguration;
                    }

                    var promises = [];

                    //Loading SwitchCDRMappingConfiguration Directive
                    var switchCDRMAppingConfigurationLoadDeferred = UtilsService.createPromiseDeferred();
                    var switchCDRMappingConfigurationPayload = {
                        switchCDRMappingConfiguration: switchCDRMappingConfiguration
                    };
                    VRUIUtilsService.callDirectiveLoad(switchCDRMappingConfigurationDirectiveAPI, switchCDRMappingConfigurationPayload, switchCDRMAppingConfigurationLoadDeferred);
                    promises.push(switchCDRMAppingConfigurationLoadDeferred.promise);


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "TOne.WhS.BusinessEntity.Entities.CDRMappingSettings, TOne.WhS.BusinessEntity.Entities",
                        SwitchCDRMappingConfiguration: switchCDRMappingConfigurationDirectiveAPI.getData()
                    };

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);