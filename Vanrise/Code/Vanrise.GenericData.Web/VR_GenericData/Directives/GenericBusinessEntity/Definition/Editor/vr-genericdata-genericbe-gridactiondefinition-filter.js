(function (app) {

    'use strict';

    gridactiondefinitionFilterDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBEDefinitionAPIService'];

    function gridactiondefinitionFilterDirective(UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {
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
                var ctor = new FilterCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "filterCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function FilterCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            var context;
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
                    var directivepPayload = {
                        context: getContext()
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivepPayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var filtercondition=[];

                    if (payload != undefined) {
                        filtercondition = payload.filterCondition;
                        context = payload.context; 
                    }

                    if (filtercondition != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getParameterSettingsConfigsPromise = getGenericBEFilterConditionConfigs();
                    promises.push(getParameterSettingsConfigsPromise);

                    function getGenericBEFilterConditionConfigs() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GenericBEActionFilterConditionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (filtercondition != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, filtercondition.ConfigId, 'ExtensionConfigurationId');
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
                                FilterGroup: filtercondition != undefined?filtercondition.FilterGroup:undefined
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

        function getTamplate(attrs) {
            var label = "Filter Condition";
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;
            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";
            var template =
                '<vr-row>'
                    + '<vr-columns colnum="{{filterCtrl.normalColNum}}">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + ' label="' + label + '"'
                            + ' isrequired="filterCtrl.isrequired"'
                            + ' ' + hideremoveicon + ' >'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{filterCtrl.normalColNum}}" isrequired="filterCtrl.isrequired" customvalidate="filterCtrl.customvalidate">'
                + '</vr-directivewrapper>';
            return template;
        }
    }

    app.directive('vrGenericdataGenericbeGridactiondefinitionFilter', gridactiondefinitionFilterDirective);

})(app);