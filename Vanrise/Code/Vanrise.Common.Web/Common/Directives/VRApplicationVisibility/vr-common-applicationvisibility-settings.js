(function (app) {

    'use strict';

    VRApplicationVisibilitySettingsDirective.$inject = ['UtilsService', 'VRNotificationService', 'VRCommon_VRApplicationVisibilityService', 'VRCommon_VRApplicationVisibilityAPIService'];

    function VRApplicationVisibilitySettingsDirective(UtilsService, VRNotificationService, VRCommon_VRApplicationVisibilityService, VRCommon_VRApplicationVisibilityAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var objectTypePropertyDefinition = new VRApplicationVisibilitySetting($scope, ctrl);
                objectTypePropertyDefinition.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Common/Directives/VRApplicationVisibility/Templates/VRApplicationVisibilitySettingsTemplate.html'
        };

        function VRApplicationVisibilitySetting($scope, ctrl) {
            this.initializeController = initializeController;

            var vrModuleVisibilityExtensionConfigs = [];
            var modulesVisibilityEditorRuntime;

            var gridAPI;
            var context;

            function initializeController() {
                ctrl.vrModuleVisibilities = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.onAddVRModuleVisibility = function () {
                    var onVRModuleVisibilityAdded = function (addedVRModuleVisibility) {
                        extendVRModuleVisibility(addedVRModuleVisibility);
                        ctrl.vrModuleVisibilities.push({ Entity: addedVRModuleVisibility });
                    };

                    VRCommon_VRApplicationVisibilityService.addVRModuleVisibility(getExcludedVRModuleVisibilityConfigTitles(), onVRModuleVisibilityAdded);
                };
                ctrl.onDeleteVRModuleVisibility = function (vrModuleVisibility) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal(ctrl.vrModuleVisibilities, vrModuleVisibility.Entity.Name, 'Entity.Name');
                            ctrl.vrModuleVisibilities.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.vrModuleVisibilities = [];

                    var modulesVisibility;

                    if (payload != undefined) {
                        modulesVisibilityEditorRuntime = payload.ModulesVisibilityEditorRuntime;

                        if (payload.Settings != undefined) {
                            modulesVisibility = payload.Settings.ModulesVisibility;
                        }
                    }

                    var loadPromiseDeferred = UtilsService.createPromiseDeferred();

                    getVRModuleVisibilityExtensionConfigs().then(function () {

                        if (modulesVisibility != undefined) {
                            for (var index in modulesVisibility) {
                                if (index != "$type") {
                                    var moduleVisibility = modulesVisibility[index];
                                    extendVRModuleVisibility(moduleVisibility);
                                    ctrl.vrModuleVisibilities.push({ Entity: moduleVisibility });
                                }
                            }
                        }

                        loadPromiseDeferred.resolve();
                    });

                    function getVRModuleVisibilityExtensionConfigs() {
                        return VRCommon_VRApplicationVisibilityAPIService.GetVRModuleVisibilityExtensionConfigs().then(function (response) {
                            if (response != undefined) {
                                vrModuleVisibilityExtensionConfigs = response;
                            }
                        });
                    }

                    return loadPromiseDeferred.promise;
                };

                api.getData = function () {

                    var vrModuleVisibilities;
                    if (ctrl.vrModuleVisibilities.length > 0) {
                        vrModuleVisibilities = {};
                        for (var i = 0; i < ctrl.vrModuleVisibilities.length; i++) {
                            var vrModuleVisibility = ctrl.vrModuleVisibilities[i].Entity;
                            vrModuleVisibilities[vrModuleVisibility.ConfigId] = vrModuleVisibility;
                        }
                    }
                    return {
                        ModulesVisibility: vrModuleVisibilities
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editVRModuleVisibility
                }];
            }
            function editVRModuleVisibility(vrModuleVisibility) {
                var onVRModuleVisibilityUpdated = function (updatedVRModuleVisibility) {
                    var index = UtilsService.getItemIndexByVal(ctrl.vrModuleVisibilities, vrModuleVisibility.Entity.Name, 'Entity.Name');
                    extendVRModuleVisibility(updatedVRModuleVisibility);
                    ctrl.vrModuleVisibilities[index] = { Entity: updatedVRModuleVisibility };
                };

                VRCommon_VRApplicationVisibilityService.editVRModuleVisibility(vrModuleVisibility.Entity, modulesVisibilityEditorRuntime[vrModuleVisibility.Entity.ConfigId], getExcludedVRModuleVisibilityConfigTitles(), onVRModuleVisibilityUpdated);
            }

            function extendVRModuleVisibility(extendModuleVisibility) {

                for (var index = 0 ; index < vrModuleVisibilityExtensionConfigs.length; index++) {
                    var currentVRModuleVisibilityExtensionConfigs = vrModuleVisibilityExtensionConfigs[index];
                    if (extendModuleVisibility.ConfigId == currentVRModuleVisibilityExtensionConfigs.ExtensionConfigurationId)
                        extendModuleVisibility.Title = currentVRModuleVisibilityExtensionConfigs.Title;
                }
            }
            function getExcludedVRModuleVisibilityConfigTitles() {
                if (ctrl.vrModuleVisibilities == undefined)
                    return;

                var titles = [];
                for (var i = 0; i < ctrl.vrModuleVisibilities.length; i++) {
                    var entity = ctrl.vrModuleVisibilities[i].Entity;
                    if (entity != undefined) {
                        titles.push(entity.Title);
                    }
                }
                return titles;
            }
        }
    }

    app.directive('vrCommonApplicationvisibilitySettings', VRApplicationVisibilitySettingsDirective);

})(app);