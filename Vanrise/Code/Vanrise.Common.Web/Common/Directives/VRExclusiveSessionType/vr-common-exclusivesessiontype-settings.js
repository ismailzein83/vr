'use strict';

app.directive('vrCommonExclusivesessiontypeSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRExclusiveSessionTypeSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRExclusiveSessionType/Templates/VRExclusiveSessionTypeSettingsTemplate.html'
        };

        function VRExclusiveSessionTypeSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var vrExclusiveSessionTypeExtendedSettingsDirectiveAPI;
            var vrExclusiveSessionTypeExtendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onVRExclusiveSessionTypeExtendedSettingsDirectiveReady = function (api) {
                    vrExclusiveSessionTypeExtendedSettingsDirectiveAPI = api;
                    vrExclusiveSessionTypeExtendedSettingsDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var vrExclusiveSessionTypeSettings, vrExclusiveSessionTypeExtendedSettings;

                    if (payload != undefined) {
                        var vrExclusiveSessionTypeEntity = payload.componentType;

                        if (vrExclusiveSessionTypeEntity != undefined) {
                            $scope.scopeModel.name = vrExclusiveSessionTypeEntity.Name;
                            vrExclusiveSessionTypeSettings = vrExclusiveSessionTypeEntity.Settings;
                        }

                        if (vrExclusiveSessionTypeSettings != undefined) {
                            vrExclusiveSessionTypeExtendedSettings = vrExclusiveSessionTypeSettings.ExtendedSettings;
                        }
                    }

                    //Loading ParentBEDefinitionRuntimeSelectorSettings selector
                    var vrExclusiveSessionTypeExtendedSettingsDirectiveLoadPromise = getVRExclusiveSessionTypeExtendedSettingsDirectiveLoadPromise();
                    promises.push(vrExclusiveSessionTypeExtendedSettingsDirectiveLoadPromise);


                    function getVRExclusiveSessionTypeExtendedSettingsDirectiveLoadPromise() {
                        var vrExclusiveSessionTypeExtendedSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        vrExclusiveSessionTypeExtendedSettingsDirectiveReadyDeferred.promise.then(function () {
                            var vrExclusiveSessionTypeExtendedSettingsDirectivePayload = {
                                vrExclusiveSessionTypeExtendedSettings: vrExclusiveSessionTypeExtendedSettings
                            };
                            VRUIUtilsService.callDirectiveLoad(vrExclusiveSessionTypeExtendedSettingsDirectiveAPI, vrExclusiveSessionTypeExtendedSettingsDirectivePayload, vrExclusiveSessionTypeExtendedSettingsDirectiveLoadDeferred);
                        });

                        return vrExclusiveSessionTypeExtendedSettingsDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Vanrise.Entities.VRExclusiveSessionTypeSettings, Vanrise.Entities",
                            ExtendedSettings: vrExclusiveSessionTypeExtendedSettingsDirectiveAPI.getData()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
