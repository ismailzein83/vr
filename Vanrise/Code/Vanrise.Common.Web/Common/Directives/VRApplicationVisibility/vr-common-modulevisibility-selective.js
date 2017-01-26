(function (app) {

    'use strict';

    VRModuleVisibilityDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRApplicationVisibilityAPIService'];

    function VRModuleVisibilityDirective(UtilsService, VRUIUtilsService, VRCommon_VRApplicationVisibilityAPIService) {
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
                var ctor = new VRModuleVisibilityCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRApplicationVisibility/Templates/VRModuleVisibilitySelectiveTemplate.html'
        };

        function VRModuleVisibilityCtor($scope, ctrl, $attrs) {
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
                    var directivePayload = {
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    console.log(payload);

                    var promises = [];
                    var vrModuleVisibility;
                    var vrModuleVisibilityEditorRuntime;

                    if (payload != undefined) {
                        vrModuleVisibility = payload.vrModuleVisibility;
                        vrModuleVisibilityEditorRuntime: payload.vrModuleVisibilityEditorRuntime;
                    }

                    var getVRModuleVisibilityExtensionConfigsPromise = getVRModuleVisibilityExtensionConfigs();
                    promises.push(getVRModuleVisibilityExtensionConfigsPromise);

                    if (vrModuleVisibility != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getVRModuleVisibilityExtensionConfigs() {
                        return VRCommon_VRApplicationVisibilityAPIService.GetVRModuleVisibilityExtensionConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (vrModuleVisibility != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, vrModuleVisibility.ConfigId, 'ExtensionConfigurationId');
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
                                vrModuleVisibility: vrModuleVisibility,
                                vrModuleVisibilityEditorRuntime: vrModuleVisibilityEditorRuntime
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
        }
    }

    app.directive('vrCommonModulevisibilitySelective', VRModuleVisibilityDirective);

})(app);