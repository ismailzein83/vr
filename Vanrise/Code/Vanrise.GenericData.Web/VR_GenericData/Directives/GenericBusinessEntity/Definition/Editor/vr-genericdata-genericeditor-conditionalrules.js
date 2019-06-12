//(function (app) {

//    'use strict';

//    genericEditorConditionalRulesDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBEDefinitionAPIService'];

//    function genericEditorConditionalRulesDirective(UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//                normalColNum: '@',
//                label: '@',
//                customvalidate: '=',
//                showremoveicon: '@',
//                customlabel: '@'
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new ConditionalRulesCtor($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "genericEditorConditionalRuleCtrl",
//            bindToController: true,
//            template: function (element, attrs) {
//                return getTamplate(attrs);
//            }
//        };

//        function ConditionalRulesCtor($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var selectorAPI;

//            var directiveAPI;
//            var directiveReadyDeferred;
//            var context;

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.templateConfigs = [];
//                $scope.scopeModel.selectedTemplateConfig;

//                $scope.scopeModel.onSelectorReady = function (api) {
//                    selectorAPI = api;
//                    defineAPI();
//                };

//                $scope.scopeModel.onDirectiveReady = function (api) {
//                    directiveAPI = api;
//                    var setLoader = function (value) {
//                        $scope.scopeModel.isLoadingDirective = value;
//                    };
//                    var directivepPayload = {
//                        context: getContext()
//                    };
//                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivepPayload, setLoader, directiveReadyDeferred);
//                };
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    selectorAPI.clearDataSource();

//                    var promises = [];
//                    var genericEditorContainerRule;

//                    if (payload != undefined) {
//                        genericEditorContainerRule = payload.genericEditorContainerRule;
//                        context = payload.context;
//                    }

//                    if (genericEditorContainerRule != undefined) {
//                        var loadDirectivePromise = loadDirective();
//                        promises.push(loadDirectivePromise);
//                    }

//                    var getGenericEditorConditionalRulesConfigsPromise = getGenericEditorConditionalRulesConfigs();
//                    promises.push(getGenericEditorConditionalRulesConfigsPromise);

//                    function getGenericEditorConditionalRulesConfigs() {
//                        return VR_GenericData_GenericBEDefinitionAPIService.GetGenericEditorConditionalRulesConfigs().then(function (response) {
//                            if (response != null) {
//                                for (var i = 0; i < response.length; i++) {
//                                    $scope.scopeModel.templateConfigs.push(response[i]);
//                                }
//                                if (genericEditorContainerRule != undefined) {
//                                    $scope.scopeModel.selectedTemplateConfig =
//                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, genericEditorContainerRule.ConfigId, 'ExtensionConfigurationId');
//                                }
//                            }
//                        });
//                    }

//                    function loadDirective() {
//                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

//                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

//                        directiveReadyDeferred.promise.then(function () {
//                            directiveReadyDeferred = undefined;
//                            var directivePayload = {
//                                context: getContext()
//                            };

//                            if (genericEditorContainerRule != undefined) {
//                                directivePayload.genericEditorContainerRule = genericEditorContainerRule;
//                            }

//                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
//                        });

//                        return directiveLoadDeferred.promise;
//                    }

//                    return UtilsService.waitMultiplePromises(promises);
//                };

//                api.getData = function () {
//                    var data;

//                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
//                        data = directiveAPI.getData();
//                        if (data != undefined) {
//                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
//                        }
//                    }
//                    return data;
//                };

//                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
//                    ctrl.onReady(api);
//            }

//            function getContext() {
//                var currentContext = context;
//                if (currentContext == undefined)
//                    currentContext = {};
//                return currentContext;
//            }
//        }

//        function getTamplate(attrs) {
//            var label = "Conditional Rule";
//            if (attrs.customlabel != undefined)
//                label = attrs.customlabel;
//            var isrequired = attrs.isnotrequired == undefined ? ' isrequired="true"' : ' ';
//            var hideremoveicon = attrs.showremoveicon == undefined ? 'hideremoveicon' : ' ';
//            var template =
//                '<vr-row>'
//                + '<vr-columns colnum="{{genericEditorConditionalRuleCtrl.normalColNum}}">'
//                + ' <vr-select on-ready="scopeModel.onSelectorReady"'
//                + ' datasource="scopeModel.templateConfigs"'
//                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
//                + ' datavaluefield="ExtensionConfigurationId"'
//                + ' datatextfield="Title"'
//                + ' label="' + label + '"'
//                + isrequired
//                + hideremoveicon
//                + '>'
//                + '</vr-select>'
//                + ' </vr-columns>'
//                + '</vr-row>'
//                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
//                + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{genericEditorConditionalRuleCtrl.normalColNum}}" isrequired="genericEditorConditionalRuleCtrl.isrequired" customvalidate="genericEditorConditionalRuleCtrl.customvalidate">'
//                + '</vr-directivewrapper>';
//            return template;
//        }
//    }

//    app.directive('vrGenericdataGenericeditorConditionalrules', genericEditorConditionalRulesDirective);

//})(app);