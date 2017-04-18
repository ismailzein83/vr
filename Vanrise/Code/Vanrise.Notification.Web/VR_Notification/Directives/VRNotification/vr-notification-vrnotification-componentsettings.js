'use strict';

app.directive('vrNotificationVrnotificationComponentsettings', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrNotificationTypeSettings = new VrNotificationTypeSettings($scope, ctrl, $attrs);
                vrNotificationTypeSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Notification/Directives/VRNotification/Templates/VRNotificationComponentTypeSettingsTemplate.html'
        };

        function VrNotificationTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var vrNotificationTypeSettingsSelectorAPI;
            var vrNotificationTypeSettingsSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var businessEntityDefinitionSelectorAPI;
            var businessEntityDefinitionSelectorReadyDeffered = UtilsService.createPromiseDeferred();


            var viewPermissionAPI;
            var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var hidePermissionAPI;
            var hidePermissionReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onVRNotificationTypeSettingsSelectorReady = function (api) {
                    vrNotificationTypeSettingsSelectorAPI = api;
                    vrNotificationTypeSettingsSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    businessEntityDefinitionSelectorAPI = api;
                    businessEntityDefinitionSelectorReadyDeffered.resolve();
                };

                $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                    viewPermissionAPI = api;
                    viewPermissionReadyDeferred.resolve();
                };

                $scope.scopeModel.onHideRequiredPermissionReady = function (api) {
                    hidePermissionAPI = api;
                    hidePermissionReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var settings;
                    if (payload != undefined && payload.componentType != undefined) {
                        $scope.scopeModel.name = payload.componentType.Name;
                        settings = payload.componentType.Settings;

                    }

                    function loadNotificationTypeSelector() {

                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();

                        vrNotificationTypeSettingsSelectorReadyDeferred.promise.then(function () {

                            var selectorPayload;
                            if (settings != undefined && settings.ExtendedSettings != undefined) {
                                selectorPayload = {
                                    extendedSettings: settings.ExtendedSettings
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(vrNotificationTypeSettingsSelectorAPI, selectorPayload, selectorLoadDeferred);
                        });

                        return selectorLoadDeferred.promise;
                    }

                    function loadBusinessEntityDefinitionSelector() {

                        var selectorLoadBEDeferred = UtilsService.createPromiseDeferred();

                        businessEntityDefinitionSelectorReadyDeffered.promise.then(function () {

                            var payloadSelector = {
                                selectedIds: settings != undefined ? settings.VRAlertLevelDefinitionId : undefined,
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.Notification.Business.VRAlertLevelBEDefinitionFilter, Vanrise.Notification.Business"
                                    }]
                                }
                            };
                            VRUIUtilsService.callDirectiveLoad(businessEntityDefinitionSelectorAPI, payloadSelector, selectorLoadBEDeferred);
                        });

                        return selectorLoadBEDeferred.promise;
                    }

                    function loadViewRequiredPermission() {
                        var viewSettingPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        viewPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: settings && settings.Security && settings.Security.ViewRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, dataPayload, viewSettingPermissionLoadDeferred);
                        });
                        return viewSettingPermissionLoadDeferred.promise;
                    }


                    function loadHideRequiredPermission() {
                        var hidePermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        hidePermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: settings && settings.Security && settings.Security.HideRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(hidePermissionAPI, dataPayload, hidePermissionLoadDeferred);
                        });
                        return hidePermissionLoadDeferred.promise;
                    }


                    return UtilsService.waitMultipleAsyncOperations([loadNotificationTypeSelector, loadBusinessEntityDefinitionSelector, loadViewRequiredPermission, loadHideRequiredPermission]).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {

                    });

                };

                api.getData = function () {

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Vanrise.Notification.Entities.VRNotificationTypeSettings, Vanrise.Notification.Entities",
                            ExtendedSettings: vrNotificationTypeSettingsSelectorAPI.getData(),
                            VRAlertLevelDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds(),
                            Security: {
                                ViewRequiredPermission:viewPermissionAPI.getData(),
                                HideRequiredPermission: hidePermissionAPI.getData()
                            }
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
