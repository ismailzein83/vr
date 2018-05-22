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
            this.initializeController = initializeController;

            var switchCDRMappingConfigurationDirectiveAPI;
            var switchCDRMappingConfigurationDirectiveDeferred = UtilsService.createPromiseDeferred();

            var cdrImportZoneIdentificationDirectiveAPI;
            var cdrImportZoneIdentificationDirectiveDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSwitchCDRMappingConfigurationReady = function (api) {
                    switchCDRMappingConfigurationDirectiveAPI = api;
                    switchCDRMappingConfigurationDirectiveDeferred.resolve();
                };

                $scope.scopeModel.onCDRImportZoneIdentificationReady = function (api) {
                    cdrImportZoneIdentificationDirectiveAPI = api;
                    cdrImportZoneIdentificationDirectiveDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([switchCDRMappingConfigurationDirectiveDeferred.promise, cdrImportZoneIdentificationDirectiveDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var switchCDRMappingConfiguration;
                    var cdrImportZoneIdentification;

                    if (payload != undefined && payload.data != undefined) {
                        switchCDRMappingConfiguration = payload.data.SwitchCDRMappingConfiguration;
                        cdrImportZoneIdentification = payload.data.CDRImportZoneIdentification;
                    }

                    var promises = [];

                    //Loading SwitchCDRMappingConfiguration Directive
                    var switchCDRMappingConfigurationLoadPromise = getSwitchCDRMappingConfigurationLoadPromise();
                    promises.push(switchCDRMappingConfigurationLoadPromise);

                    //Loading CDRImportZoneIdentificationn Directive
                    var cdrImportZoneIdentificationLoadPromise = getCDRImportZoneIdentificationLoadPromise();
                    promises.push(cdrImportZoneIdentificationLoadPromise);


                    function getSwitchCDRMappingConfigurationLoadPromise() {
                        var switchCDRMappingConfigurationLoadDeferred = UtilsService.createPromiseDeferred();

                        var switchCDRMappingConfigurationPayload = {
                            switchCDRMappingConfiguration: switchCDRMappingConfiguration
                        };
                        VRUIUtilsService.callDirectiveLoad(switchCDRMappingConfigurationDirectiveAPI, switchCDRMappingConfigurationPayload, switchCDRMappingConfigurationLoadDeferred);

                        return switchCDRMappingConfigurationLoadDeferred.promise;
                    }
                    function getCDRImportZoneIdentificationLoadPromise() {
                        var cdrImportZoneIdentificationLoadDeferred = UtilsService.createPromiseDeferred();

                        var cdrImportZoneIdentificationPayload = {
                            cdrImportZoneIdentification: cdrImportZoneIdentification
                        };
                        VRUIUtilsService.callDirectiveLoad(cdrImportZoneIdentificationDirectiveAPI, cdrImportZoneIdentificationPayload, cdrImportZoneIdentificationLoadDeferred);

                        return cdrImportZoneIdentificationLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "TOne.WhS.BusinessEntity.Entities.CDRImportSettings, TOne.WhS.BusinessEntity.Entities",
                        SwitchCDRMappingConfiguration: switchCDRMappingConfigurationDirectiveAPI.getData(),
                        CDRImportZoneIdentification: cdrImportZoneIdentificationDirectiveAPI.getData()
                    };

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);