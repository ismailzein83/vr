'use strict';
            
app.directive('vrWhsSmsbusinessentitySmsimportSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new smsImportEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_SMSBusinessEntity/Directives/SMSImportSettings/Templates/SMSImportSettingsTemplate.html"
        };

        function smsImportEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var smsImportMappingConfigurationDirectiveAPI;
            var smsImportMappingConfigurationDirectiveDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSMSImportMappingConfigurationReady = function (api) {
                    smsImportMappingConfigurationDirectiveAPI = api;
                    smsImportMappingConfigurationDirectiveDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([smsImportMappingConfigurationDirectiveDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var smsImportMappingConfiguration;

                    if (payload != undefined && payload.data != undefined) {
                        smsImportMappingConfiguration = payload.data.SMSImportMappingConfiguration;
                    }

                    var promises = [];

                    //Loading SMSImportMappingConfiguration Directive
                    var smsImportMappingConfigurationLoadPromise = getSMSImportMappingConfigurationLoadPromise();
                    promises.push(smsImportMappingConfigurationLoadPromise);

                    function getSMSImportMappingConfigurationLoadPromise() {
                        var smsImportMappingConfigurationLoadDeferred = UtilsService.createPromiseDeferred();

                        var smsImportMappingConfigurationPayload = {
                            smsImportMappingConfiguration: smsImportMappingConfiguration
                        };
                        VRUIUtilsService.callDirectiveLoad(smsImportMappingConfigurationDirectiveAPI, smsImportMappingConfigurationPayload, smsImportMappingConfigurationLoadDeferred);

                        return smsImportMappingConfigurationLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "TOne.WhS.SMSBusinessEntity.Entities.SMSImportSettings, TOne.WhS.SMSBusinessEntity.Entities",
                        SMSImportMappingConfiguration: smsImportMappingConfigurationDirectiveAPI.getData()
                    };

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);