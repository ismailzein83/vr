(function (appControllers) {

    "use strict";
    MeasureExternalSourceExtendedSettingsSelective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService'];
    function MeasureExternalSourceExtendedSettingsSelective(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var extendedSettingsSelective = new ExtendedSettingsSelective($scope, ctrl, $attrs);
                extendedSettingsSelective.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function ($attrs) {
                return getTemplate($attrs);
            }
        };
        function ExtendedSettingsSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

           
            var context;
            var tableId;
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
                    var payload = {
                        context: getContext(),
                        entity: undefined,
                        tableId: tableId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, payload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                 
                    selectorAPI.clearDataSource();
                    

                    var promises = [];
                    var entity;                    
                    
                    if (payload != undefined) {

                        entity = payload.entity;
                        context = payload.context;
                        tableId = payload.tableId;
                    }
                    if (entity != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);

                    }
                    var getTemplateConfigsPromise = getTemplateConfigs();
                    promises.push(getTemplateConfigsPromise);

                    function getTemplateConfigs() {
                        return VR_Analytic_AnalyticConfigurationAPIService.GetMeasureExternalSourceExtendedSettingConfigs().then(function (response) {
                            if (response != null) {
                                
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);                                    
                                }
                                if (entity != undefined) {
                                    
                                    $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, entity.ConfigId, 'ExtensionConfigurationId');
                                   
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
                                context: getContext(),
                                entity: entity,
                                tableId: tableId
                            }
                            
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
                var currentContext = context;
                if (currentContext == undefined) 
                    currentContext = {};
               
                return currentContext;
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
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + 'label="Extended Settings"  entityName="Extended Settings" '
                            + ' ' + hideremoveicon + ' '
                            + 'isrequired ="ctrl.isrequired"'
                           + ' >'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                        + ' vr-loader="scopeModel.isLoadingDirective"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>';
            return template;
        }
        return directiveDefinitionObject;
    }
    app.directive("vrAnalyticMeasureexternalsourceSelective", MeasureExternalSourceExtendedSettingsSelective);
})(appControllers);