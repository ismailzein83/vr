﻿(function (app) {

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

            var objectSelectorAPI;
            var objectSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var objectPropertySelectorAPI;
            var objectPropertySelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var objectPropertySelectorLoadDeferred = UtilsService.createPromiseDeferred();

            var directiveAPI;
            var directiveReadyDeferred;


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.objectVariables = [];
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedObjectVariable;
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onObjectSelectorReady = function (api) {
                    objectSelectorAPI = api;
                    objectSelectorReadyDeferred.resolve();
                    //defineAPI();
                };
                $scope.scopeModel.onObjectVariableSelectionChanged = function () {

                    if (objectPropertySelectorLoadDeferred != undefined) {
                        return;
                    }

                    objectPropertySelectorAPI.clearDataSource();
                    objectProperty = undefined;

                    if ($scope.scopeModel.selectedObjectVariable != undefined && $scope.scopeModel.selectedObjectVariable.ObjectType != undefined) {
                        var objectTypeConfigId = $scope.scopeModel.selectedObjectVariable.ObjectType.ConfigId;

                        $scope.scopeModel.isSelectorLoading = true;
                        loadObjectPropertySelector(objectTypeConfigId).finally(function () {
                            //$scope.scopeModel.isSelectorLoading = false;
                        });
                    }                
                }

                $scope.scopeModel.onObjectPropertySelectorReady = function (api) {
                    objectPropertySelectorAPI = api;
                    objectPropertySelectorReadyDeferred.resolve();
                    objectSelectorReadyDeferred.promise.then(function () {
                        defineAPI();
                    });
                };
             
                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    var directivePayload = {};
                    if ($scope.scopeModel.selectedObjectVariable.ObjectType.RecordTypeId != undefined)
                        directivePayload.dataRecordTypeId = $scope.scopeModel.selectedObjectVariable.ObjectType.RecordTypeId;
                    if (objectProperty != undefined)
                        directivePayload.valueEvaluator = objectProperty.propertyEvaluator;

                    var setLoader = function (value) {
                        $scope.scopeModel.isDirectiveLoading = value;
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

                    return UtilsService.waitMultiplePromises(promises).finally(function () {
                                                                            objectPropertySelectorLoadDeferred = undefined;
                                                                       });
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
                objectPropertySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                objectPropertySelectorReadyDeferred.promise.then(function () {
                    $scope.scopeModel.isSelectorLoading = true;
                    loadObjectTypeConfigs().then(function () {
                        var getObjectPropertyTemplateConfigsPromise = getObjectPropertySelectorTemplateConfigs(objectTypeConfigId);
                        objectPropertySelectorLoadDeferred.resolve();
                        //$scope.scopeModel.isSelectorLoading = false;
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
                        var str = objectTypeConfigs[0].PropertyEvaluatorExtensionType;
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
                                                $scope.scopeModel.isSelectorLoading = false;

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
                    if ($scope.scopeModel.selectedObjectVariable.ObjectType.RecordTypeId != undefined)
                        directivePayload.dataRecordTypeId = $scope.scopeModel.selectedObjectVariable.ObjectType.RecordTypeId;
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
