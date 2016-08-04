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
            var criteriaField;

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
                    objectPropertySelectorAPI.clearDataSource();

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
                //$scope.scopeModel.onObjectPropertySelectionChengaed = function () {
                //    if ($scope.scopeModel.selectedTemplateConfig != undefined)
                //        var idzz = $scope.scopeModel.selectedTemplateConfig.Editor;
                //}
             
                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    var directivePayload = {};
                    directivePayload.dataRecordTypeId = $scope.scopeModel.selectedObjectVariable.ObjectType.RecordTypeId;
                    if (criteriaField != undefined)
                        directivePayload.valueEvaluator = criteriaField.ValueEvaluator

                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                    //VRUIUtilsService.callDirectiveLoad($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    objectSelectorAPI.clearDataSource();
                    objectPropertySelectorAPI.clearDataSource();

                    //var promises = [];

                    if (payload != undefined && payload.criteriaField != undefined)
                        criteriaField = payload.criteriaField;

                    //Loading ObjectSelector
                    if (payload != undefined && payload.objectVariables != undefined) {
                        for (var key in payload.objectVariables)
                            $scope.scopeModel.objectVariables.push(payload.objectVariables[key]);

                        if (criteriaField != undefined) {
                            $scope.scopeModel.selectedObjectVariable = UtilsService.getItemByVal($scope.scopeModel.objectVariables, criteriaField.ValueObjectName, 'ObjectName');
                        }
                    }

                    ////Loading ObjectPropertySelector
                    //if ($scope.scopeModel.selectedObjectVariable != undefined && $scope.scopeModel.selectedObjectVariable.ObjectType != undefined) {
                    //    var configId = $scope.scopeModel.selectedObjectVariable.ObjectType.ConfigId;

                    //    objectPropertySelectorReadyDeferred.promise.then(function () {
                    //        loadObjectTypeConfigs().then(function () {
                    //                getObjectPropertySelectorTemplateConfigs(configId);
                    //        });
                    //    });
                    //}

                    //return UtilsService.waitMultiplePromises(promises);

                    //Loading ObjectPropertySelector
                    //if ($scope.scopeModel.selectedObjectVariable != undefined && $scope.scopeModel.selectedObjectVariable.ObjectType != undefined) {
                    //    var configId = $scope.scopeModel.selectedObjectVariables.ObjectType.ConfigId;

                    //    objectPropertySelectorReadyDeferred.promise.then(function () {
                    //        loadObjectTypeConfigs().then(function () {
                    //            var getObjectPropertyTemplateConfigsPromise = getObjectPropertySelectorTemplateConfigs(configId);
                    //            promises.push(getObjectPropertyTemplateConfigsPromise);
                    //        });
                    //    });
                    //}

                    //Loading DirectiveWrapper
                    //if ($scope.scopemodel.selectedtemplateconfig != undefined) {
                    //    var loaddirectivepromise = loaddirective();
                    //    promises.push(loaddirectivepromise);
                    //}
                };

                api.getData = function () {

                    if ($scope.scopeModel.selectedObjectVariable != undefined) {
                        var valueObjectName = $scope.scopeModel.selectedObjectVariable.ObjectName;
                    }
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
                        var valueEvaluator = directiveAPI.getData();
                        if (valueEvaluator != undefined) 
                            valueEvaluator.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;                     
                    }

                    var data = {
                        ValueObjectName: valueObjectName,
                        ValueEvaluator: valueEvaluator
                    };

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
                                                if (criteriaField != undefined && criteriaField.ValueEvaluator.ConfigId != undefined) {
                                                    $scope.scopeModel.selectedTemplateConfig =
                                                            UtilsService.getItemByVal($scope.scopeModel.templateConfigs, criteriaField.ValueEvaluator.ConfigId, 'ExtensionConfigurationId');
                                                }
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