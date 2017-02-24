"use strict";

app.directive("vrNotificationNotificationVieweditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new NotificationViewEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Notification/Directives/VRNotificationViewDefinition/Templates/VRNotificationViewEditor.html"
        };
        function NotificationViewEditorCtor($scope, ctrl, $attrs) {
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

                    function loadNotificationTypeSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();

                        vrNotificationTypeSettingsSelectorReadyDeferred.promise.then(function () {

                            var selectorPayload;
                            if (payload != undefined && payload.Settings != undefined) {
                                selectorPayload = {
                                    selectedIds: buildSelectorIdsObj(payload.Settings)
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(vrNotificationTypeSettingsSelectorAPI, selectorPayload, selectorLoadDeferred);
                        });

                        function buildSelectorIdsObj(settings) {
                            var _seletedIds = [];
                            for (var index = 0; index < settings.length; index++) {
                                var current = settings[index];
                                _seletedIds.push(current.VRNotificationTypeId)
                            }
                            return _seletedIds;
                        }

                        return selectorLoadDeferred.promise;
                    }

                    promises.push(loadNotificationTypeSelector());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var obj = {
                        $type: "Vanrise.Notification.Entities.VRNotificationViewSettings, Vanrise.Notification.Entities",
                        Settings: buildNotificationViewSettingsObj()
                    };

                    function buildNotificationViewSettingsObj() {
                        var settings = [];
                        var notificationTypeIds = vrNotificationTypeSettingsSelectorAPI.getSelectedIds()
                        if (notificationTypeIds != undefined) {
                            for (var index = 0; index < notificationTypeIds.length; index++) {
                                settings.push({ VRNotificationTypeId: notificationTypeIds[index] })
                            }
                        }
                        return settings;
                    }
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);