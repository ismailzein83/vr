'use strict';

app.directive('networkprovisionHandlertypeSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HandlerTypeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/NetworkProvision/Elements/HandlerType/Directives/Templates/NetworkProvisionHandlerTypeSettingsTemplate.html'
        };

        function HandlerTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var handlerTypeExtendedSettingsDirectiveAPI;
            var handlerTypeExtendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onHandlerTypeExtendedSettingsDirectiveReady = function (api) {
                    handlerTypeExtendedSettingsDirectiveAPI = api;
                    handlerTypeExtendedSettingsDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var handlerTypeEntity;

                    if (payload != undefined) {
                        handlerTypeEntity = payload.componentType;
                    }

                    if (handlerTypeEntity != undefined) {
                        $scope.scopeModel.name = handlerTypeEntity.Name;
                    }

                    var loadHandlerTypeExtendedSettingsDirectivePromise = loadHandlerTypeExtendedSettingsDirective();
                    promises.push(loadHandlerTypeExtendedSettingsDirectivePromise);

                    function loadHandlerTypeExtendedSettingsDirective() {
                        var handlerTypeExtendedSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        handlerTypeExtendedSettingsDirectiveReadyDeferred.promise.then(function () {
                            var directivePayload;

                            if (handlerTypeEntity != undefined && handlerTypeEntity.Settings != undefined) {
                                directivePayload = {
                                    extendedSettings: handlerTypeEntity.Settings.ExtendedSettings
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(handlerTypeExtendedSettingsDirectiveAPI, directivePayload, handlerTypeExtendedSettingsDirectiveLoadDeferred);
                        });

                        return handlerTypeExtendedSettingsDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var settings = {
                        $type: "NetworkProvision.Entities.NetworkProvisionHandlerTypeSettings, NetworkProvision.Entities",
                        ExtendedSettings: handlerTypeExtendedSettingsDirectiveAPI.getData(),
                    };

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: settings
                    };
                };

                if (ctrl.onReady != undefined)
                    ctrl.onReady(api);
            }
        }
    }
]);