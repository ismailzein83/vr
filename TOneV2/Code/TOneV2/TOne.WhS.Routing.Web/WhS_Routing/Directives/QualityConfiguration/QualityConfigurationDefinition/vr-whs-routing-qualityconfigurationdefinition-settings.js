'use strict';

app.directive('vrWhsRoutingQualityconfigurationdefinitionSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new QualityConfigurationDefinitionSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Routing/Directives/QualityConfiguration/QualityConfigurationDefinition/Templates/QualityConfigurationDefinitionSettingsTemplate.html'
        };

        function QualityConfigurationDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var qcDefinitionExtendedSettingsDirectiveAPI;
            var qcDefinitionExtendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onQCDefinitionExtendedSettingsDirectiveReady = function (api) {
                    qcDefinitionExtendedSettingsDirectiveAPI = api;
                    qcDefinitionExtendedSettingsDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var qualityConfigurationDefinitionSettings;

                    if (payload != undefined) {
                        var qualityConfigurationDefinitionEntity = payload.componentType;

                        if (qualityConfigurationDefinitionEntity != undefined) {
                            $scope.scopeModel.name = qualityConfigurationDefinitionEntity.Name;
                            qualityConfigurationDefinitionSettings = qualityConfigurationDefinitionEntity.Settings;
                        }
                    }

                    //Loading qualityConfigurationDefinitionExtendedSettings Directive
                    var qcDefinitionExtendedSettingsDirectiveLoadPromise = getQCDefinitionExtendedSettingsDirectiveLoadPromise();
                    promises.push(qcDefinitionExtendedSettingsDirectiveLoadPromise);


                    function getQCDefinitionExtendedSettingsDirectiveLoadPromise() {
                        var qcDefinitionExtendedSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        qcDefinitionExtendedSettingsDirectiveReadyDeferred.promise.then(function () {

                            var qcDefinitionExtendedSettingsDirectivePayload;
                            if (qualityConfigurationDefinitionSettings != undefined) {
                                qcDefinitionExtendedSettingsDirectivePayload = { qualityConfigurationDefinitionExtendedSettings: qualityConfigurationDefinitionSettings.ExtendedSettings };
                            }
                            VRUIUtilsService.callDirectiveLoad(qcDefinitionExtendedSettingsDirectiveAPI, qcDefinitionExtendedSettingsDirectivePayload, qcDefinitionExtendedSettingsDirectiveLoadDeferred);
                        });

                        return qcDefinitionExtendedSettingsDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "TOne.WhS.Routing.Entities.QualityConfigurationDefinitionSettings, TOne.WhS.Routing.Entities",
                            ExtendedSettings: qcDefinitionExtendedSettingsDirectiveAPI.getData()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);