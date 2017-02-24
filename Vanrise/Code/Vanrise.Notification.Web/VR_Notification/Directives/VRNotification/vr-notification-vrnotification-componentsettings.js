'use strict';

app.directive('vrNotificationVrnotificationComponentsettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
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

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onVRNotificationTypeSettingsSelectorReady = function (api) {
                    vrNotificationTypeSettingsSelectorAPI = api;
                    vrNotificationTypeSettingsSelectorReadyDeferred.resolve();
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

                    promises.push(loadNotificationTypeSelector());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Vanrise.Notification.Entities.VRNotificationTypeSettings, Vanrise.Notification.Entities",
                            ExtendedSettings: vrNotificationTypeSettingsSelectorAPI.getData()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
