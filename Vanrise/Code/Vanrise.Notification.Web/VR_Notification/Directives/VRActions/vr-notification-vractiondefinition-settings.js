'use strict';

app.directive('vrNotificationVractiondefinitionSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new actionDefinitionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/VR_Notification/Directives/VRActions/Templates/VRActionDefinitionSettingsTemplate.html"
        };

        function actionDefinitionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            $scope.scopeModel = {};

            var actionDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var actionDefinitionSelectorDirectiveApi;

            var directiveReadyPromiseDeferred;
            var directiveApi;

            function initializeController() {
                $scope.scopeModel.onActionDefinitionConfigSelectorReady = function (api) {
                    actionDefinitionSelectorDirectiveApi = api;
                    actionDefinitionSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveApi = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isDirectiveLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveApi, undefined, setLoader, directiveReadyPromiseDeferred);
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var vrActionDefinitionEntity;
                    if (payload != undefined) {
                        vrActionDefinitionEntity = payload.componentType;

                        if (vrActionDefinitionEntity != undefined) {
                            $scope.scopeModel.name = vrActionDefinitionEntity.Name;
                        }
                    }

                    var promises = [];
                    var loadactionDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();

                    actionDefinitionSelectorReadyPromiseDeferred.promise.then(function () {
                        var actionDefinitionPayload;
                        if (vrActionDefinitionEntity != undefined) {
                            actionDefinitionPayload = { selectedIds: vrActionDefinitionEntity.Settings.ExtendedSettings.ConfigId };
                        }
                        VRUIUtilsService.callDirectiveLoad(actionDefinitionSelectorDirectiveApi, actionDefinitionPayload, loadactionDefinitionPromiseDeferred);
                    });
                    promises.push(loadactionDefinitionPromiseDeferred.promise);

                    if (vrActionDefinitionEntity != undefined) {
                        directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyPromiseDeferred.promise.then(function () {
                            directiveReadyPromiseDeferred = undefined;
                            VRUIUtilsService.callDirectiveLoad(directiveApi, vrActionDefinitionEntity, directiveLoadDeferred);
                        });

                        promises.push(directiveLoadDeferred.promise);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Vanrise.Notification.Entities.VRActionDefinitionSettings, Vanrise.Notification.Entities",
                            ExtendedSettings: directiveApi.getData()
                        }
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);