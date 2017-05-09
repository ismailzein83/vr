(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_BE_ServiceTypeAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, Retail_BE_ServiceTypeAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var overriddenSettingsDirective = new OverriddenSettingsDirective(ctrl, $scope, $attrs);
                overriddenSettingsDirective.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/ServiceType/Templates/OverriddenConfigurationServiceType.html'
        };

        function OverriddenSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var overriddenSettings;
            var filter;
            var extendedSettings;
            var serviceTypeSettings;
            var selectedIds;
            var serviceTypeEntity;
            var serviceTypeSelectorApi;
            var serviceTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var settingsAPI;
            var settingReadyPromiseDeferred;
            var selectedPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.isSettingsOverriddenValuechanged = function () {
                    if ($scope.scopeModel.isSettingsOverridden == true) {
                        loadOverriddenSettingsEditor();
                    }
                    else {
                        hideOverriddenSettingsEditor();
                    }
                };
                $scope.scopeModel.serviceTypeSelectorSelectionChanged = function () {
                    if (selectedIds != undefined) {
                        if (selectedPromiseDeferred != undefined) {
                            selectedPromiseDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.title = "";
                            $scope.scopeModel.isSettingsOverridden = false;
                            overriddenSettings = undefined;
                            settingsAPI = undefined;
                            $scope.scopeModel.showDirectiveSettings = false;
                        }
                    }

                };
                $scope.scopeModel.isSettingsOverridden = false;
                $scope.scopeModel.onServiceTypeSettingsReady = function (api) {
                    settingsAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingsAPI, undefined, setLoader, settingReadyPromiseDeferred);
                };
                $scope.scopeModel.onServiceTypeSelectorReady = function (api) {
                    serviceTypeSelectorApi = api;
                    serviceTypeSelectorPromiseDeferred.resolve();
                };
                serviceTypeSelectorPromiseDeferred.promise.then(function () {
                    if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                });
            }

            function getDirectiveAPI() {
                var directiveAPI = {};

                directiveAPI.load = function (payload) {
                    var promises = [];

                    if (payload) {
                        extendedSettings = payload.extendedSettings;
                        selectedIds = extendedSettings.ServiceTypeId;
                        overriddenSettings = extendedSettings.OverriddenSettings;
                        $scope.scopeModel.title = extendedSettings.OverriddenTitle;
                        $scope.scopeModel.isSettingsOverridden = (overriddenSettings != undefined) ? true : false;

                    }
                    var loadServiceTypeSelectorPromise = loadServiceTypeSelector();


                    function loadServiceTypeSelector() {
                        var payloadSelector = {
                            selectedIds: selectedIds,
                            filter: filter
                        };
                        return serviceTypeSelectorApi.load(payloadSelector);
                    }

                    selectedPromiseDeferred.promise.then(function () {
                        selectedPromiseDeferred = undefined
                    });

                    if ($scope.scopeModel.isSettingsOverridden) {
                        var serviceTypeSettingsloadPromiseDeferred = UtilsService.createPromiseDeferred();
                        loadServiceTypeSelectorPromise.then(function () {
                            loadOverriddenSettingsEditor().then(function () { serviceTypeSettingsloadPromiseDeferred.resolve(); });
                        });
                        promises.push(serviceTypeSettingsloadPromiseDeferred.promise);
                    }
                    else {

                        promises.push(loadServiceTypeSelectorPromise);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };
                directiveAPI.getData = function () {
                    var settings;
                    if (settingsAPI != undefined)
                    { settings = settingsAPI.getData(); }
                    var serviceTypeOverriddenConfiguration = {};
                    serviceTypeOverriddenConfiguration.$type = " Retail.BusinessEntity.Business.ServiceTypeOverriddenConfiguration , Retail.BusinessEntity.Business";
                    serviceTypeOverriddenConfiguration.ConfigId = '';
                    serviceTypeOverriddenConfiguration.ServiceTypeId = serviceTypeSelectorApi.getSelectedIds();
                    serviceTypeOverriddenConfiguration.OverriddenTitle = $scope.scopeModel.title;
                    serviceTypeOverriddenConfiguration.OverriddenSettings = settings;

                    return serviceTypeOverriddenConfiguration;
                };

                return directiveAPI;
            }

            function loadOverriddenSettingsEditor() {
                var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                if (overriddenSettings == undefined) {

                    getServiceType().then(function () {
                        serviceTypeSettings = serviceTypeEntity.Settings;
                        loadSettings();
                    }).catch(function (error) {
                        loadSettingDirectivePromiseDeferred.reject();
                    });
                }
                else {
                    serviceTypeSettings = overriddenSettings;
                    loadSettings();
                }

                function loadSettings() {

                    $scope.scopeModel.showDirectiveSettings = true;
                        settingReadyPromiseDeferred.promise
                       .then(function () {
                           var directivePayload = {
                               serviceTypeSettings: serviceTypeSettings
                           };
                           VRUIUtilsService.callDirectiveLoad(settingsAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                       }).catch(function (error) {
                           loadSettingDirectivePromiseDeferred.reject();
                       });

                }
                return loadSettingDirectivePromiseDeferred.promise;
            }
            function hideOverriddenSettingsEditor() {
                $scope.scopeModel.showDirectiveSettings = false;
                settingsAPI = undefined;
            }
            function getServiceType() {
                return Retail_BE_ServiceTypeAPIService.GetServiceType(serviceTypeSelectorApi.getSelectedIds()).then(function (response) {
                    serviceTypeEntity = response;
                });
            }
        }


        return directiveDefinitionObject;
    }
   
    app.directive('retailBeOverriddenconfigurationServicetype', OverriddenSettings);

})(app);
