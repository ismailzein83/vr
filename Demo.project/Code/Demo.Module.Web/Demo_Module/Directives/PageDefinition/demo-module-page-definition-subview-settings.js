(function (app) {

    'use strict';

    SubviewSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'Demo_Module_PageDefinitionAPIService'];

    function SubviewSettingsDirective(UtilsService, VRUIUtilsService, Demo_Module_PageDefinitionAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },

            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new SubviewSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function SubviewSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

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
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var subviewSettingsEntity;
                    if (payload != undefined) {
                        subviewSettingsEntity = payload.subviewSettingsEntity;
                        
                    }

                    if (subviewSettingsEntity != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getSubviewSettingsConfigsPromise = getSubviewSettingsConfigs();
                    promises.push(getSubviewSettingsConfigsPromise);

                    function getSubviewSettingsConfigs() {
                        return Demo_Module_PageDefinitionAPIService.GetSubviewConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (subviewSettingsEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, subviewSettingsEntity.ConfigId, 'ExtensionConfigurationId');

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
                                subviewSettingsEntity: subviewSettingsEntity
                            }; 
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
                        if (data != undefined && data.ConfigId==undefined) {
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

        function getTamplate(attrs) {
            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = 'hideremoveicon';
            }
            var template =
                '<vr-row>'
                    + '<vr-columns colnum="{{ctrl.normalColNum}}" width="1/2row">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + 'label="Subview Settings" '
                            + ' ' + hideremoveicon + ' '
                             + 'isrequired ="ctrl.isrequired"'
                           + ' >'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + '</vr-row>'

                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>';

            return template;
        }
    }

    app.directive('demoModulePageDefinitionSubviewSettings', SubviewSettingsDirective);

})(app);

