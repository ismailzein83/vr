(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericRuleDefinitionAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, VR_GenericData_GenericRuleDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                onselectionchanged: '=',
                ismultipleselection: '@',
                isrequired: '=',
                normalColNum: '@',
                selectedvalues: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined && $attrs.ismultipleselection != null)
                    ctrl.selectedvalues = [];
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
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function OverriddenSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var overriddenCriteriaDefinition;
            var overriddenObjects;
            var overriddenSettingsDefinition;
            var overriddenSecurity;
            var filter;
            var extendedSettings;
            var genericRuleDefinitionCriteriaDefinition;
            var genericRuleDefinitionObjects;
            var genericRuleDefinitionSettingsDefinition;
            var genericRuleDefinitionSecurity;
            var selectedIds;
            var genericRuleEntityDefinitionEntity;
            var genericRuleDefinitionSelectorApi;
            var genericRuleDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var settingsAPI;
            var settingReadyPromiseDeferred;
            var selectedPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.loadSettings = function () {
                    loadOverriddenSettingsEditor();
                };
                $scope.scopeModel.changeFlag = function () {
                    if (selectedIds != undefined) {
                        if (selectedPromiseDeferred != undefined) {
                            selectedPromiseDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.genericRuleDefinitionTitle = "";
                            $scope.scopeModel.showSettings = false;
                            overriddenCriteriaDefinition = undefined;
                            overriddenObjects = undefined;
                            overriddenSettingsDefinition = undefined;
                            overriddenSecurity = undefined;
                            $scope.scopeModel.showDirectiveSettings = false;
                            loadOverriddenSettingsEditor();
                        }
                    }
                       
                };
                $scope.scopeModel.showSettings = false;
                $scope.scopeModel.onGenericRuleDefinfitionSettingsReady = function (api) {
                    settingsAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingsAPI, undefined, setLoader, settingReadyPromiseDeferred);
                };
                $scope.scopeModel.onGenericRuleDefinitionSelectorReady = function (api) {
                    genericRuleDefinitionSelectorApi = api;
                    genericRuleDefinitionSelectorPromiseDeferred.resolve();
                };
                genericRuleDefinitionSelectorPromiseDeferred.promise.then(function () {
                    if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                });
            }

            function getDirectiveAPI() {
                var directiveAPI = {};

                directiveAPI.load = function (payload) {
                    if (payload) {
                        extendedSettings = payload.extendedSettings;
                        selectedIds = extendedSettings.RuleDefinitionId;
                        overriddenCriteriaDefinition = extendedSettings.OverriddenCriteriaDefinition;
                        overriddenObjects = extendedSettings.OverriddenObjects;
                        overriddenSettingsDefinition = extendedSettings.OverriddenSettingsDefinition;
                        overriddenSecurity = extendedSettings.OverriddenSecurity;
                        $scope.scopeModel.genericRuleDefinitionTitle = extendedSettings.OverriddenTitle;
                        $scope.scopeModel.showSettings = (overriddenCriteriaDefinition != undefined || overriddenObjects != undefined || overriddenSettingsDefinition != undefined || overriddenSecurity!=undefined) ? true : false;
                        loadOverriddenSettingsEditor();
                    }
                    var promises = [];

                    promises.push(loadGenericRuleDefinitionSelector());

                    function loadGenericRuleDefinitionSelector() {
                        var payloadSelector = {
                            selectedIds: selectedIds,
                            filter: filter
                        };
                        return genericRuleDefinitionSelectorApi.load(payloadSelector);
                    }

                    selectedPromiseDeferred.promise.then(function () {
                        selectedPromiseDeferred = undefined
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };

                directiveAPI.getSelectedIds = function () {
                    return genericRuleDefinitionSelectorApi.getSelectedIds();
                };

                directiveAPI.getData = function () {
                    var settings;
                    if (settingsAPI != undefined)
                    { settings = settingsAPI.getData(); }
                    var genericRuleDefinitionOverriddenConfiguration = {};
                    genericRuleDefinitionOverriddenConfiguration.$type = "Vanrise.GenericData.Business.GenericRuleDefinitionOverriddenConfiguration ,Vanrise.GenericData.Business";
                    genericRuleDefinitionOverriddenConfiguration.ConfigId = '';
                    genericRuleDefinitionOverriddenConfiguration.RuleDefinitionId = genericRuleDefinitionSelectorApi.getSelectedIds();
                    genericRuleDefinitionOverriddenConfiguration.OverriddenTitle = $scope.scopeModel.genericRuleDefinitionTitle;
                    if (settings != undefined)
                    {
                        genericRuleDefinitionOverriddenConfiguration.OverriddenCriteriaDefinition = settings.criteriaDefinition;
                        genericRuleDefinitionOverriddenConfiguration.OverriddenObjects = settings.objects;
                        genericRuleDefinitionOverriddenConfiguration.OverriddenSettingsDefinition = settings.settingsDefinition;
                        genericRuleDefinitionOverriddenConfiguration.OverriddenSecurity = settings.security;
                    }
                   
                    return genericRuleDefinitionOverriddenConfiguration;
                }

                return directiveAPI;
            }

            function loadOverriddenSettingsEditor() {
                if ($scope.scopeModel.showSettings == true) {

                    settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    if (overriddenCriteriaDefinition == undefined && overriddenObjects == undefined && overriddenSettingsDefinition == undefined && overriddenSecurity == undefined)
                    {
                       
                        getGenericRuleDefinition().then(function () {
                            genericRuleDefinitionCriteriaDefinition = genericRuleEntityDefinitionEntity.CriteriaDefinition;
                            genericRuleDefinitionObjects = genericRuleEntityDefinitionEntity.Objects;
                            genericRuleDefinitionSettingsDefinition = genericRuleEntityDefinitionEntity.SettingsDefinition;
                            genericRuleDefinitionSecurity = genericRuleEntityDefinitionEntity.Security;
                            loadSettings();
                        });
                    }
                    else {
                        genericRuleDefinitionCriteriaDefinition = overriddenCriteriaDefinition;
                        genericRuleDefinitionObjects = overriddenObjects;
                        genericRuleDefinitionSettingsDefinition = overriddenSettingsDefinition;
                        genericRuleDefinitionSecurity = overriddenSecurity;
                            loadSettings();
                    }
                }
                else {
                    settingsAPI = undefined;
                    $scope.scopeModel.showDirectiveSettings = false;
                }
                function loadSettings() {

                    if (genericRuleDefinitionCriteriaDefinition != undefined || genericRuleDefinitionObjects != undefined || genericRuleDefinitionSettingsDefinition != undefined || genericRuleDefinitionSecurity != undefined) {

                        $scope.scopeModel.showDirectiveSettings = true;
                        settingReadyPromiseDeferred.promise
                            .then(function () {
                                var directivePayload = {

                                    objects: genericRuleDefinitionObjects,
                                    criteriaDefinition: genericRuleDefinitionCriteriaDefinition,
                                    settingsDefinition: genericRuleDefinitionSettingsDefinition,
                                    security: genericRuleDefinitionSecurity
                                };
                                VRUIUtilsService.callDirectiveLoad(settingsAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                            });

                        return loadSettingDirectivePromiseDeferred.promise;
                    }
                }
            }
            function getGenericRuleDefinition() {
                return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(genericRuleDefinitionSelectorApi.getSelectedIds()).then(function (genericRuleEntityDefinition) {
                    genericRuleEntityDefinitionEntity = genericRuleEntityDefinition;
                });
            }
        }

        function getDirectiveTemplate(attrs) {

            var ismultipleselection = '';
            var labelTab = "'RuleDefinitionSettings'";
            var label = 'RuleDefinitionSettings';
            if (attrs.ismultipleselection != undefined && attrs.ismultipleselection != null) {
                ismultipleselection = ' ismultipleselection';
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;
            return ' <vr-tab header="' + labelTab + '">\
                <vr-row>\
                <vr-columns colnum="{{ctrl.normalColNum}}">\
                     <vr-genericdata-genericruledefinition-selector on-ready="scopeModel.onGenericRuleDefinitionSelectorReady" isrequired="true"    \
                      selectedvalues="ctrl.selectedvalues" \
                      customlabel="' + label + '"\
                      ' + ismultipleselection +
                     ' onselectionchanged="scopeModel.changeFlag">\
                    </<vr-genericdata-genericruledefinition-selector>\
                    </vr-columns>\
                    </vr-row>\
                    <vr-row>\
                        <vr-columns width="1/2row">\
                            <vr-textbox value="scopeModel.genericRuleDefinitionTitle" isrequired="true" label="Title" customvalidate="scopeModel.validate()"></vr-textbox>\
                        </vr-columns>\
                    </vr-row>\
                    <vr-row>\
                    <vr-columns width="1/2row">\
                            <vr-switch value="scopeModel.showSettings" label="Use Record Type" onvaluechanged="scopeModel.loadSettings" ></vr-switch>\
                    </vr-columns>\
                     </vr-row>\
                        </vr-tab> \
<div ng-if="scopeModel.showDirectiveSettings">\
<vr-genericdata-genericruledefinition-settings on-ready="scopeModel.onGenericRuleDefinfitionSettingsReady"></vr-genericdata-genericruledefinition-settings>\
</div>\
';
        }

        return directiveDefinitionObject;
    }

    app.directive('vrCommonOverriddenconfigurationGenericruledefinition', OverriddenSettings);

})(app);
