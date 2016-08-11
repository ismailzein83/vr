(function (app) {

    'use strict';

    VRObjectPropertySelector.$inject = ['VRCommon_VRObjectTypeAPIService', 'VRCommon_VRObjectPropertyAPIService', 'UtilsService', 'VRUIUtilsService'];

    function VRObjectPropertySelector(VRCommon_VRObjectTypeAPIService, VRCommon_VRObjectPropertyAPIService, UtilsService, VRUIUtilsService) {
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
                var objectPropertyEvaluatorSelector = new ObjectPropertySelector($scope, ctrl, $attrs);
                objectPropertyEvaluatorSelector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRObjectProperty/Templates/VRObjectPropertySelector2Template.html'
        };

        function ObjectPropertySelector($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var objectType;
            var objectTypeConfigs;
            var objectPropertyEvaluator;

            var objectPropertyEvaluatorSelectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onObjectPropertySelectorReady = function (api) {
                    objectPropertyEvaluatorSelectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    var directivePayload = {};
                    if (objectType != undefined)
                        directivePayload.objectType = objectType;

                    var setLoader = function (value) {
                        $scope.scopeModel.isDirectiveLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    objectPropertyEvaluatorSelectorAPI.clearDataSource();

                    var promises = [];

                    if (payload != undefined && payload.objectType != undefined)
                        objectType = payload.objectType;

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                        objectPropertyEvaluator = payload.objectPropertyEvaluator;


                    //Loading ObjectPropertySelector
                    if (objectType != undefined) {
                        var loadObjectPropertySelectorPromise = loadObjectPropertySelector(objectType.ConfigId)
                        promises.push(loadObjectPropertySelectorPromise);
                    }

                    //Loading DirrectiveWrapper               
                    if (objectPropertyEvaluator != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
                        var propertyEvaluator = directiveAPI.getData();
                        if (propertyEvaluator != undefined)
                            propertyEvaluator.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                    }

                    return propertyEvaluator;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function loadObjectPropertySelector(objectTypeConfigId) {

                var objectPropertySelectorLoadDeferred = UtilsService.createPromiseDeferred();
                loadObjectTypeConfigs().then(function () {
                    getObjectPropertySelectorTemplateConfigs(objectTypeConfigId).then(function () {
                        objectPropertySelectorLoadDeferred.resolve();
                    });
                });

                return objectPropertySelectorLoadDeferred.promise;
            }
            function loadObjectTypeConfigs() {
                return VRCommon_VRObjectTypeAPIService.GetObjectTypeExtensionConfigs().then(function (response) {
                    if (response != null) {
                        objectTypeConfigs = [];
                        for (var i = 0; i < response.length; i++) {
                            objectTypeConfigs.push(response[i]);
                        }
                    }
                });
            }
            function getObjectPropertySelectorTemplateConfigs(objectTypeConfigId) {

                var propertyEvaluatorExtensionType;
                for (var index = 0; index < objectTypeConfigs.length ; index++)
                    if (objectTypeConfigs[index].ExtensionConfigurationId == objectTypeConfigId)
                        propertyEvaluatorExtensionType = objectTypeConfigs[index].PropertyEvaluatorExtensionType;

                return VRCommon_VRObjectPropertyAPIService.GetObjectPropertyExtensionConfigs(propertyEvaluatorExtensionType)
                                        .then(function (response) {
                                            if (response != null) {
                                                for (var i = 0; i < response.length; i++) {
                                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                                }

                                                if (objectPropertyEvaluator != undefined && objectPropertyEvaluator.ConfigId != undefined) {
                                                    $scope.scopeModel.selectedTemplateConfig =
                                                            UtilsService.getItemByVal($scope.scopeModel.templateConfigs, objectPropertyEvaluator.ConfigId, 'ExtensionConfigurationId');
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
                    directivePayload.objectType = objectType;
                    directivePayload.valueEvaluator = objectPropertyEvaluator;

                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });

                return directiveLoadDeferred.promise;
            }
        }
    }

    app.directive('vrCommonObjectproperty2Selector', VRObjectPropertySelector);

})(app);






//function loadObjectPropertySelector() {

//    return VRCommon_VRObjectPropertyAPIService.GetObjectPropertyExtensionConfigs(objectType.Settings.PropertyEvaluatorExtensionType)
//                            .then(function (response) {
//                                if (response != null) {
//                                    for (var i = 0; i < response.length; i++) {
//                                        $scope.scopeModel.templateConfigs.push(response[i]);
//                                    }

//                                    if (objectPropertyEvaluator != undefined && objectPropertyEvaluator.ConfigId != undefined) {
//                                        $scope.scopeModel.selectedTemplateConfig =
//                                                UtilsService.getItemByVal($scope.scopeModel.templateConfigs, objectPropertyEvaluator.ConfigId, 'ExtensionConfigurationId');
//                                    }
//                                }
//                            });
//}