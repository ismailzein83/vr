(function (app) {

    'use strict';

    gridViewConditionDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBEDefinitionAPIService'];

    function gridViewConditionDirective(UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {
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
                var ctor = new ConditionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "conditionCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function ConditionCtor($scope, ctrl, $attrs) {
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

                    var condition = [];
                    var initialPromises = [];

                    if (payload != undefined) {
                        condition = payload.condition;
                        context = payload.context;
                    }

                    function getGenericBEGridConditionConfigs() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEGridConditionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (condition != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, condition.ConfigId, 'ExtensionConfigurationId');
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
                                condition: condition
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];
                            directivePromises.push(getGenericBEGridConditionConfigs());

                            if (condition != undefined) 
                                directivePromises.push(loadDirective());
                            
                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
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
                + '<vr-columns colnum="{{conditionCtrl.normalColNum}}">'
                + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                + ' datasource="scopeModel.templateConfigs"'
                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                + ' datavaluefield="ExtensionConfigurationId"'
                + ' datatextfield="Title"'
                + ' label="' + label + '"'
                + ' isrequired="conditionCtrl.isrequired"'
                + ' ' + hideremoveicon + ' >'
                + '</vr-select>'
                + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{conditionCtrl.normalColNum}}" isrequired="conditionCtrl.isrequired" customvalidate="conditionCtrl.customvalidate">'
                + '</vr-directivewrapper>';
            return template;
        }
    }

    app.directive('vrGenericdataGenericbeGridviewcondition', gridViewConditionDirective);

})(app);