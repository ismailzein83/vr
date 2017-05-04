(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_BusinessEntityDefinitionAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, VR_GenericData_BusinessEntityDefinitionAPIService) {

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
            var overriddenSettings;
            var filter;
            var beDefinitionSettings;
            var selectedIds;
            var businessEntityDefinitionEntity;
            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var settingsAPI;
            var settingReadyPromiseDeferred;
            var selectedPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.loadSettings = function () {
                    loadOverriddenSettingsEditor();
                };
                $scope.scopeModel.changeFlag = function () {
                    if (selectedIds != undefined)
                    {
                        if (selectedPromiseDeferred != undefined) {
                            selectedPromiseDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.businessEntityTitle = "";
                            $scope.scopeModel.showSettings = false;
                            overriddenSettings = undefined;
                            loadOverriddenSettingsEditor();
                        }
                    }
                };
                $scope.scopeModel.showSettings = false;
                $scope.scopeModel.onSettingsEditorReady = function (api) {
                    settingsAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingsAPI, undefined, setLoader, settingReadyPromiseDeferred);
                };
                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };
                beDefinitionSelectorPromiseDeferred.promise.then(function () {
                    if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                });
            }

            function getDirectiveAPI() {
                var directiveAPI = {};

                directiveAPI.load = function (payload) {
                    
                    if (payload) {
                         
                         var extendedSettings = payload.extendedSettings;
                         selectedIds = extendedSettings.BusinessEntityDefinitionId;
                        overriddenSettings = extendedSettings.OverriddenSettings;
                        $scope.scopeModel.businessEntityTitle = extendedSettings.OverriddenTitle;
                        $scope.scopeModel.showSettings = (extendedSettings.OverriddenSettings != undefined) ? true : false;
                        loadOverriddenSettingsEditor();
                    }
                    var promises = [];

                    promises.push(loadBusinessEntityDefinitionSelector());

                    function loadBusinessEntityDefinitionSelector() {
                            var payloadSelector = {
                                selectedIds: selectedIds,
                                filter: filter
                            };
                            return beDefinitionSelectorApi.load(payloadSelector);
                    }
                    selectedPromiseDeferred.promise.then(function () {
                        selectedPromiseDeferred = undefined
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };

                directiveAPI.getSelectedIds = function () {
                    return beDefinitionSelectorApi.getSelectedIds();
                };

                directiveAPI.getData = function () {
                    var settings;
                    if (settingsAPI != undefined)
                    { settings = settingsAPI.getData(); }
                    var beDefinitionOverriddenConfiguration = {};
                    beDefinitionOverriddenConfiguration.$type = "Vanrise.GenericData.Business.BEDefinitionOverriddenConfiguration ,Vanrise.GenericData.Business";
                    beDefinitionOverriddenConfiguration.ConfigId = '';
                    beDefinitionOverriddenConfiguration.BusinessEntityDefinitionId = beDefinitionSelectorApi.getSelectedIds();
                    beDefinitionOverriddenConfiguration.OverriddenTitle = $scope.scopeModel.businessEntityTitle;
                    beDefinitionOverriddenConfiguration.OverriddenSettings = settings;
                    return beDefinitionOverriddenConfiguration;
                }

                return directiveAPI;
            }
            
            function loadOverriddenSettingsEditor() {
                if ($scope.scopeModel.showSettings == true) {

                    settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    if (overriddenSettings == undefined) {
                        getBEDefinitionSettingConfigs().then(
                            function () {
                                getBusinessEntityDefinition().then(function () {

                                    beDefinitionSettings = businessEntityDefinitionEntity.Settings;
                                    loadSettings();
                                });
                            }
                            )
                    }
                    else {
                        getBEDefinitionSettingConfigs().then(function () {
                            beDefinitionSettings = overriddenSettings;
                            loadSettings();
                        });

                    }
                }
                else {
                    $scope.scopeModel.selectedSetingsTypeConfig = undefined;
                    settingsAPI = undefined;
                }
                function loadSettings() {

                    if (beDefinitionSettings != undefined) {

                        $scope.scopeModel.selectedSetingsTypeConfig = UtilsService.getItemByVal($scope.scopeModel.bEDefinitionSettingConfigs, beDefinitionSettings.ConfigId, "ExtensionConfigurationId");
                        settingReadyPromiseDeferred.promise
                            .then(function () {
                                var directivePayload = {
                                    businessEntityDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
                                    businessEntityDefinitionSettings: beDefinitionSettings,
                                };
                                VRUIUtilsService.callDirectiveLoad(settingsAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                            });

                        return loadSettingDirectivePromiseDeferred.promise;
                    }
                }
            }
            function getBEDefinitionSettingConfigs() {
                return VR_GenericData_BusinessEntityDefinitionAPIService.GetBEDefinitionSettingConfigs().then(function (response) {
                    if (response) {

                        $scope.scopeModel.bEDefinitionSettingConfigs = response;
                    }
                });
            }
            function getBusinessEntityDefinition() {
                return VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(beDefinitionSelectorApi.getSelectedIds()).then(function (businessEntityDefinition) {
                    businessEntityDefinitionEntity = businessEntityDefinition;
                });
            }
        }

        function getDirectiveTemplate(attrs) {

            var ismultipleselection = '';
            var labelTab = "'BEDefinitionSettings'";
            var label = 'BEDefinitionSettings';
            if (attrs.ismultipleselection != undefined && attrs.ismultipleselection != null) {
                ismultipleselection = ' ismultipleselection';
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;
            return ' <vr-tab header="'+labelTab+'">\
                <vr-row>\
                <vr-columns colnum="{{ctrl.normalColNum}}">\
                     <vr-genericdata-businessentitydefinition-selector on-ready="scopeModel.onBusinessEntityDefinitionSelectorReady"    \
                      isrequired="ctrl.isrequired" \
                      selectedvalues="ctrl.selectedvalues" \
                      customlabel="' + label + '"\
                      ' + ismultipleselection +
                     ' onselectionchanged="scopeModel.changeFlag">\
                    </vr-genericdata-businessentitydefinition-selector>\
                    </vr-columns>\
                    </vr-row>\
                    <vr-row>\
                        <vr-columns width="1/2row">\
                            <vr-textbox value="scopeModel.businessEntityTitle" isrequired="true" label="Title" customvalidate="scopeModel.validate()"></vr-textbox>\
                        </vr-columns>\
                    </vr-row>\
                    <vr-row>\
                    <vr-columns width="1/2row">\
                            <vr-switch value="scopeModel.showSettings" label="Use Record Type" onvaluechanged="scopeModel.loadSettings" ></vr-switch>\
                    </vr-columns>\
                     </vr-row>\
                        </vr-tab> \
  <vr-directivewrapper vr-loader="scopeModel.isLoadingDirective"\
            ng-if="scopeModel.selectedSetingsTypeConfig!=undefined"\
            directive="scopeModel.selectedSetingsTypeConfig.Editor"\
            on-ready="scopeModel.onSettingsEditorReady"\
            normal-col-num="4"\
            isrequired="true">\
</vr-directivewrapper>';
        }

        return directiveDefinitionObject;
    }

    app.directive('vrCommonOverriddenconfigurationBedefinition', OverriddenSettings);

})(app);
