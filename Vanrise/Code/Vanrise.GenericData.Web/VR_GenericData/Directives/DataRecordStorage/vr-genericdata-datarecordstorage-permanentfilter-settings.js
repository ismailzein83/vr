(function (app) {

    'use strict';

    DataRecordStoragePermanentfilterSettings.$inject = ['UtilsService', 'VRUIUtilsService',  'VR_GenericData_DataRecordStorageAPIService'];

    function DataRecordStoragePermanentfilterSettings(UtilsService, VRUIUtilsService, VR_GenericData_DataRecordStorageAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PermanentFilterSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "permanentFilterCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function PermanentFilterSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            var context;
            var dataRecordTypeId;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    }; 
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, { dataRecordTypeId: dataRecordTypeId }, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var settings;
                    if (payload != undefined) {
                        settings = payload.settings;
                        context = payload.context;
                        dataRecordTypeId = payload.dataRecordTypeId;
                    }
                    if (settings != undefined ) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getParameterSettingsConfigsPromise = getPermanentFilterSettingsConfigs();
                    promises.push(getParameterSettingsConfigsPromise);

                    function getPermanentFilterSettingsConfigs() {
                        return VR_GenericData_DataRecordStorageAPIService.GetPermanentFilterSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (settings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                    UtilsService.getItemByVal($scope.scopeModel.templateConfigs, settings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = {
                                context: getContext()
                            };
                            if (settings != undefined) {
                                directivePayload.settings = settings;
                            }; 
                            directivePayload.dataRecordTypeId = dataRecordTypeId;
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
            function getContext() {
                return context;
            }
        }

        function getTamplate(attrs) {
            var label = "Permanent Filter Settings";
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;
            var template =
                '<vr-row>'
                + '<vr-columns colnum="{{permanentFilterCtrl.normalColNum}}">'
                + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                + ' datasource="scopeModel.templateConfigs"'
                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                + ' datavaluefield="ExtensionConfigurationId"'
                + ' datatextfield="Title"'
                + 'label="' + label + '"'
                + ' isrequired="true"'
                + '</vr-select>'
                + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{permanentFilterCtrl.normalColNum}}" isrequired="permanentFilterCtrl.isrequired" customvalidate="permanentFilterCtrl.customvalidate">'
                + '</vr-directivewrapper>';
            return template;
        }
    }

    app.directive('vrGenericdataDatarecordstoragePermanentfilterSettings', DataRecordStoragePermanentfilterSettings);

})(app);
