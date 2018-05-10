'use strict';

app.directive('vrGenericdataDataproviderextendedsettingsSelector', ['VR_GenericData_BusinessObjectDataRecordStorageAPIService',  'UtilsService', 'VRUIUtilsService',
function (VR_GenericData_BusinessObjectDataRecordStorageAPIService, UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            enableadd: '=',
            normalColNum: '@',
            label: '@',
            customvalidate: '=',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var dataProviderSelector = new DataProviderSelector(ctrl, $scope, $attrs);
            dataProviderSelector.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
        },
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function DataProviderSelector(ctrl, $scope, attrs) {

        this.initializeController = initializeController;

        var selectorAPI;
        var directiveAPI;
        var directiveReadyDeferred;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.templateConfigs = [];
            $scope.scopeModel.selectedTemplateConfig;

            $scope.scopeModel.onDataProviderSettingsSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
            };
        }
        function defineAPI() {

            var api = {};
            api.load = function (payload) {

                selectorAPI.clearDataSource();
                var promises = [];
                var extendedSettings;
                if (payload != undefined) {
                    extendedSettings = payload.extendedSettings;
                }
                if (extendedSettings != undefined) {
                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);
                }

                var getDataProviderExtendedSettingsTemplateConfigsPromise = getDataProviderExtendedSettingsTemplateConfigs();
                promises.push(getDataProviderExtendedSettingsTemplateConfigsPromise);

                function getDataProviderExtendedSettingsTemplateConfigs() {
                    return VR_GenericData_BusinessObjectDataRecordStorageAPIService.GetDataRecordStorageTemplateConfigs().then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.templateConfigs.push(response[i]);
                                
                            }
                            if (extendedSettings != undefined) {
                                $scope.scopeModel.selectedTemplateConfig =
                                    UtilsService.getItemByVal($scope.scopeModel.templateConfigs, extendedSettings.ConfigId, 'ExtensionConfigurationId');
                            }
                        }
                    });
                }


                function loadDirective() {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = undefined;
                        var directivePayload = { extendedSettings: extendedSettings };
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                    });

                    return directiveLoadDeferred.promise;
                }
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var data;
                if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
                    data = directiveAPI.getData();
                    if (data != undefined) {
                        data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                    }
                }
                return data;
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);

            }
        }
    }

    function getTemplate(attrs) {
        var hideremoveicon = '';
        if (attrs.hideremoveicon != undefined) {
            hideremoveicon = 'hideremoveicon';
        }
        var template =
            '<vr-row>'
                + '<vr-columns colnum="{{ctrl.normalColNum}}">'
                    + ' <vr-select on-ready="scopeModel.onDataProviderSettingsSelectorReady"'
                        + ' datasource="scopeModel.templateConfigs"'
                        + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                        + ' datavaluefield="ExtensionConfigurationId"'
                        + ' datatextfield="Title"'
                        + 'label="Type"  entityName="Type" '
                        + ' ' + hideremoveicon + ' '
                       + 'isrequired >'
                    + '</vr-select>'
                + ' </vr-columns>'
            + '</vr-row>'
            + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                    + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
            + '</vr-directivewrapper>';
        return template;
    }
}]);