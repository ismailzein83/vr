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

            var objectProperty;
            var objectTypeConfigs;

            var objectVariableSelectorAPI;
            var objectVariableSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var onObjectVariableSelectionChangedDeferred;

            var objectPropertySelectorAPI;
            var objectPropertySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var directiveAPI;
            var directiveReadyDeferred;


            function initializeController() {
                $scope.scopeModel = {};
                var _promises = [objectVariableSelectorReadyDeferred.promise, objectPropertySelectorReadyDeferred.promise];

                $scope.scopeModel.objectVariables = [];
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedObjectVariable;
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onObjectSelectorReady = function (api) {
                    objectVariableSelectorAPI = api;
                    objectVariableSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onObjectVariableSelectionChanged = function () {

                    if ($scope.scopeModel.selectedObjectVariable != undefined) {

                        if (onObjectVariableSelectionChangedDeferred != undefined) {
                            onObjectVariableSelectionChangedDeferred.resolve();
                        }
                        else {
                            loadObjectPropertySelector();
                        }
                    }
                }

                $scope.scopeModel.onObjectPropertySelectorReady = function (api) {
                    objectPropertySelectorAPI = api;
                    objectPropertySelectorReadyDeferred.resolve();
                };
             
                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    var directivePayload = {};
                    if ($scope.scopeModel.selectedObjectVariable.ObjectType != undefined)
                        directivePayload.objectType = $scope.scopeModel.selectedObjectVariable.ObjectType;

                    var setLoader = function (value) {
                        $scope.scopeModel.isDirectiveLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };


                UtilsService.waitMultiplePromises(_promises).then(function () {
                    defineAPI();
                });

                function loadObjectPropertySelector() {
                    objectPropertySelectorAPI.clearDataSource();
                    objectProperty = undefined;

                    $scope.scopeModel.isSelectorLoading = true;
                    var objectTypeConfigId = $scope.scopeModel.selectedObjectVariable.ObjectType.ConfigId;

                    VRCommon_VRObjectTypeAPIService.GetObjectTypeExtensionConfigs().then(function (response) {
                        if (response != null) {
                            objectTypeConfigs = [];
                            for (var i = 0; i < response.length; i++)
                                objectTypeConfigs.push(response[i]);

                            var propertyEvaluatorExtensionType;
                            for (var index = 0; index < objectTypeConfigs.length ; index++)
                                if (objectTypeConfigs[index].ExtensionConfigurationId == objectTypeConfigId)
                                    propertyEvaluatorExtensionType = objectTypeConfigs[index].PropertyEvaluatorExtensionType;

                            VRCommon_VRObjectPropertyAPIService.GetObjectPropertyExtensionConfigs(propertyEvaluatorExtensionType)
                                                    .then(function (response) {
                                                        if (response != null) {
                                                            for (var i = 0; i < response.length; i++) {
                                                                $scope.scopeModel.templateConfigs.push(response[i]);
                                                            }

                                                            $scope.scopeModel.isSelectorLoading = false;
                                                        }
                                                    });
                        }
                    });
                }
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    objectVariableSelectorAPI.clearDataSource();
                    objectPropertySelectorAPI.clearDataSource();

                    var promises = [];

                    if (payload != undefined && payload.objectProperty != undefined)
                        objectProperty = payload.objectProperty;

                    //Loading ObjectSelector
                    if (payload != undefined && payload.objectVariables != undefined) {
                        for (var key in payload.objectVariables)
                            $scope.scopeModel.objectVariables.push(payload.objectVariables[key]);

                        if (objectProperty != undefined) {
                            $scope.scopeModel.selectedObjectVariable = UtilsService.getItemByVal($scope.scopeModel.objectVariables, objectProperty.objectName, 'ObjectName');

                            //In Case we have deleted the objectVariable
                            if ($scope.scopeModel.selectedObjectVariable == undefined) {
                                objectProperty = undefined;
                            }
                        }
                    }

                    //Loading ObjectPropertySelector
                    if ($scope.scopeModel.selectedObjectVariable != undefined && $scope.scopeModel.selectedObjectVariable.ObjectType != undefined) {
                        var objectTypeConfigId = $scope.scopeModel.selectedObjectVariable.ObjectType.ConfigId;

                        var loadObjectPropertySelectorPromise = loadObjectPropertySelector(objectTypeConfigId)
                        promises.push(loadObjectPropertySelectorPromise);
                    }

                    //Loading DirrectiveWrapper               
                    var objectVariable = $scope.scopeModel.selectedObjectVariable;
                    if (objectVariable != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    if ($scope.scopeModel.selectedObjectVariable != undefined) {
                        var objectName = $scope.scopeModel.selectedObjectVariable.ObjectName;
                    }
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
                        var propertyEvaluator = directiveAPI.getData();
                        if (propertyEvaluator != undefined) 
                            propertyEvaluator.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;                     
                    }

                    var data;
                    if (objectName != undefined && propertyEvaluator != undefined)
                        data = { objectName: objectName, propertyEvaluator: propertyEvaluator };

                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function loadObjectPropertySelector(objectTypeConfigId) {

                if (onObjectVariableSelectionChangedDeferred == undefined)
                    onObjectVariableSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                var objectPropertySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                onObjectVariableSelectionChangedDeferred.promise.then(function () {
                    onObjectVariableSelectionChangedDeferred = undefined;
                    loadObjectTypeConfigs().then(function () {
                        var getObjectPropertyTemplateConfigsPromise = getObjectPropertySelectorTemplateConfigs(objectTypeConfigId);
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

                                                if (objectProperty != undefined && objectProperty.propertyEvaluator.ConfigId != undefined) {
                                                    $scope.scopeModel.selectedTemplateConfig =
                                                            UtilsService.getItemByVal($scope.scopeModel.templateConfigs, objectProperty.propertyEvaluator.ConfigId, 'ExtensionConfigurationId');
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
                    if ($scope.scopeModel.selectedObjectVariable.ObjectType != undefined)
                        directivePayload.objectType = $scope.scopeModel.selectedObjectVariable.ObjectType;
                    if (objectProperty != undefined)
                        directivePayload.valueEvaluator = objectProperty.propertyEvaluator;

                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });

                return directiveLoadDeferred.promise;
            }
        }
    }

    app.directive('vrCommonObjectpropertySelector', VRObjectPropertySelector);

})(app);
