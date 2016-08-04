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
                var objectPropertySelector = new ObjectPropertySelector($scope, ctrl, $attrs);
                objectPropertySelector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRObjectProperty/Templates/VRObjectPropertySelectorTemplate.html'
        };

        function ObjectPropertySelector($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var objectTypeConfigs;

            var objectSelectorAPI;

            var objectPropertySelectorAPI;
            var objectPropertySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var directiveAPI;
            var directiveReadyDeferred;
            //var directivePayload;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isLoadingDirective = true;

                $scope.scopeModel.objectVariables = [];
                $scope.scopeModel.templateConfigs = [];
                //$scope.scopeModel.selectedObjectVariable = {};
                //$scope.scopeModel.selectedTemplateConfig = {};

                $scope.scopeModel.onObjectSelectorReady = function (api) {
                    objectSelectorAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.onObjectVariableSelectionChanged = function () {

                    if ($scope.scopeModel.selectedObjectVariable != undefined && $scope.scopeModel.selectedObjectVariable.ObjectType != undefined) {
                        var configId = $scope.scopeModel.selectedObjectVariable.ObjectType.ConfigId;

                        objectPropertySelectorReadyDeferred.promise.then(function () {
                                loadObjectTypeConfigs().then(function () {
                                    $scope.scopeModel.isLoadingSelector = true;
                                    getObjectPropertySelectorTemplateConfigs(configId).then(function () {
                                        $scope.scopeModel.isLoadingSelector = false;
                                    });
                                });
                        });
                    }
                }

                $scope.scopeModel.onObjectPropertySelectorReady = function (api) {
                    objectPropertySelectorAPI = api;
                    objectPropertySelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onObjectPropertySelectionChengaed = function () {
                    if ($scope.scopeModel.selectedTemplateConfig != undefined)
                        var idzz = $scope.scopeModel.selectedTemplateConfig.Editor;
                }
             
                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var directivePayload = {};
                    directivePayload.dataRecordTypeId = $scope.scopeModel.selectedObjectVariable.ObjectType.RecordTypeId;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    objectSelectorAPI.clearDataSource();
                    objectPropertySelectorAPI.clearDataSource();

                    var promises = [];

                    //Loading ObjectSelector
                    if (payload != undefined && payload.objectVariables != undefined) {
                        for (var key in payload.objectVariables)
                            $scope.scopeModel.objectVariables.push(payload.objectVariables[key]);
                    }


                    //Loading ObjectPropertySelector
                    //if ($scope.scopeModel.selectedObjectVariable != undefined && $scope.scopeModel.selectedObjectVariable.ObjectType) {
                    //    var configId = $scope.scopeModel.selectedObjectVariables.ObjectType.ConfigId;

                    //    objectPropertySelectorReadyDeferred.promise.then(function () {
                    //        loadObjectTypeConfigs().then(function () {
                    //            var getObjectPropertyTemplateConfigsPromise = getObjectPropertySelectorTemplateConfigs(configId);
                    //            promises.push(getObjectPropertyTemplateConfigsPromise);
                    //        });
                    //    });
                    //}

                    //Loading DirectiveWrapper
                    //if ($scope.scopeModel.selectedTemplateConfig != undefined) {
                    //    var loadDirectivePromise = loadDirective();
                    //    promises.push(loadDirectivePromise);
                    //}



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

            function loadObjectTypeConfigs() {
                return VRCommon_VRObjectTypeAPIService.GetObjectTypeExtensionConfigs().then(function (response) {
                    if (response != null) {
                        objectTypeConfigs = [];
                        for (var i = 0; i < response.length; i++) {
                            objectTypeConfigs.push(response[i]);
                        }
                        var str = objectTypeConfigs[0].PropertyEvaluatorExtensionType;
                    }
                });
            }
            function getObjectPropertySelectorTemplateConfigs(configId) {

                var propertyEvaluatorExtensionType;
                for (var index = 0; index < objectTypeConfigs.length ; index++)
                    if (objectTypeConfigs[index].ExtensionConfigurationId == configId)
                        propertyEvaluatorExtensionType = objectTypeConfigs[index].PropertyEvaluatorExtensionType;

                return VRCommon_VRObjectPropertyAPIService.GetObjectPropertyExtensionConfigs(propertyEvaluatorExtensionType)
                                        .then(function (response) {
                                            if (response != null) {
                                                for (var i = 0; i < response.length; i++) {
                                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                                }
                                                //if (objectProperty != undefined && objectProperty.ConfigId != undefined) {
                                                //    $scope.scopeModel.selectedTemplateConfig =
                                                //        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, objectProperty.ConfigId, 'ExtensionConfigurationId');
                                                //}
                                            }
                                        });

            }
            function loadDirective() {
                directiveReadyDeferred = UtilsService.createPromiseDeferred();
                var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                directiveReadyDeferred.promise.then(function () {
                    directiveReadyDeferred = undefined;
                    var directivePayload;
                    //directivePayload.dataRecordTypeId = $scope.scopeModel.selectedObjectVariables.ObjectType.RecordTypeId;
                    //if (objectProperty != undefined && objectProperty.RecordTypeId != undefined) {
                    //    directivePayload = { recordTypeId: objectProperty.RecordTypeId }
                    //}
                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });

                return directiveLoadDeferred.promise;
            }
        }
    }

    app.directive('vrCommonObjectpropertySelector', VRObjectPropertySelector);

})(app);