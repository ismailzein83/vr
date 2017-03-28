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
            var businessEntityDefinitionSelectorAPI;
            var businessEntityDefinitionSelectorReadyDeffered = UtilsService.createPromiseDeferred();
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
                   
                    return UtilsService.waitMultipleAsyncOperations([loadNotificationTypeSelector,loadBusinessEntityDefinitionSelector]).catch(function (error) {
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
                            VRAlertLevelDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
