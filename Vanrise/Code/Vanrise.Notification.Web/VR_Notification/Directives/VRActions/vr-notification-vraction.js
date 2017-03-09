(function (app) {

    'use strict';

    VRActionDirective.$inject = ['VR_Notification_VRActionAPIService', 'UtilsService', 'VRUIUtilsService', 'VR_Notification_VRActionDefinitionAPIService'];

    function VRActionDirective(VR_Notification_VRActionAPIService, UtilsService, VRUIUtilsService, VR_Notification_VRActionDefinitionAPIService) {
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
                var ctor = new VRAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function VRAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            var context;
            var vrActionEntity;

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
                    VR_Notification_VRActionDefinitionAPIService.GetVRActionDefinition($scope.scopeModel.selectedTemplateConfig.VRActionDefinitionId).then(function (response) {
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingDirective = value;
                        };
                        var directivePayload = {
                            context: getContext(),
                            selectedVRActionDefinition: response
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                    });
                };

                $scope.scopeModel.onActionDefinitionChanged = function (selectedItem) {
                    if (directiveAPI != undefined) {
                        VR_Notification_VRActionDefinitionAPIService.GetVRActionDefinition(selectedItem.VRActionDefinitionId).then(function (response) {
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingDirective = value;
                            };
                            var directivePayload = {
                                context: getContext(),
                                selectedVRActionDefinition: response
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                        });
                    }
                };
            }

            function defineAPI() {
                var api = {};
                var serviceSettings;

                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var promises = [];

                    var extensionType;
                    if (payload != undefined) {
                        context = payload.context;
                        vrActionEntity = payload.vrActionEntity;
                        extensionType = payload.extensionType;
                    }

                    if (vrActionEntity != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }
                    var filter = {};
                    if (payload.vrActionTargetType != undefined) {
                        filter.VRActionTargetType = payload.vrActionTargetType;
                    }
                    var getVRActionTemplateConfigsPromise = getVRActionTemplateConfigs(filter);
                    promises.push(getVRActionTemplateConfigsPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.Settings.DefinitionId = $scope.scopeModel.selectedTemplateConfig.VRActionDefinitionId;
                        }
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            };

            function getVRActionTemplateConfigs(filter) {
                return VR_Notification_VRActionDefinitionAPIService.GetVRActionDefinitionsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModel.templateConfigs.push(response[i]);
                        }

                        if (vrActionEntity != undefined) {
                            $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, vrActionEntity.Settings.DefinitionId, 'VRActionDefinitionId');
                        }
                    }
                });
            };

            function loadDirective() {
                directiveReadyDeferred = UtilsService.createPromiseDeferred();

                var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                VR_Notification_VRActionDefinitionAPIService.GetVRActionDefinition(vrActionEntity.Settings.DefinitionId).then(function (response) {
                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = undefined;
                        var directivePayload = { context: getContext(), vrActionEntity: vrActionEntity, selectedVRActionDefinition: response };
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                    });
                });

                return directiveLoadDeferred.promise;
            };
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        function getTamplate(attrs) {

            var template =
                '<vr-row>'
                    + '<vr-columns colnum="{{ctrl.normalColNum}}">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="VRActionDefinitionId"'
                            + ' datatextfield="Name"'
                            + 'label="Action Definition"'
                            + ' isrequired="true" onselectionchanged="scopeModel.onActionDefinitionChanged"'
                            + 'hideremoveicon>'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.RuntimeEditor"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>';
            return template;
        }
    }

    app.directive('vrNotificationVraction', VRActionDirective);

})(app);