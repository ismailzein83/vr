(function (app) {

    'use strict';

    ObjectTypeSelectiveSelective.$inject = ['VRCommon_VRObjectTypeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ObjectTypeSelectiveSelective(VRCommon_VRObjectTypeAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                onselectionchanged: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var objectTypeSelective = new ObjectTypeSelective($scope, ctrl, $attrs);
                objectTypeSelective.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRObjectVariable/Templates/VRObjectTypeSelectiveTemplate.html'
        };

        function ObjectTypeSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig = {};

                $scope.scopeModel.onObjectTypeSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.onObjectTypeSelectionChanged = function () {

                    if ($scope.scopeModel.selectedTemplateConfig != undefined && context != undefined)
                        context.canDefineProperties(true);

                    if (ctrl.onselectionchanged != null && typeof (ctrl.onselectionchanged) == "function")
                        ctrl.onselectionchanged();
                }

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    directivePayload = {};
                    directivePayload.context = context;

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var objectType;

                    if (payload.context != undefined) {
                        context = payload.context;
                    }

                    if (payload != undefined && payload.objectType != undefined) {
                        objectType = payload.objectType;
                    }

                    var getObjectTypeExtensionConfigsPromise = getObjectTypeExtensionConfigs();
                    promises.push(getObjectTypeExtensionConfigsPromise);

                    if (objectType != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getObjectTypeExtensionConfigs() {
                        return VRCommon_VRObjectTypeAPIService.GetObjectTypeExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (objectType != undefined && objectType.ConfigId != undefined) {
                                   $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, objectType.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload = {};
                            directivePayload.context = context;
                            if (objectType != undefined) {
                                directivePayload.objectType = objectType;
                            }
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data;
                    if (directiveAPI != undefined && $scope.scopeModel.selectedTemplateConfig != undefined) {
                        
                        data = directiveAPI.getData();
                        if(data != undefined)
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                    }
                    
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrCommonObjecttypeSelective', ObjectTypeSelectiveSelective);

})(app);