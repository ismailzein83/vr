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

            var criteriaFieldEntity;
            var objectTypeConfigs;

            var objectSelectorAPI;

            var objectPropertySelectorAPI;
            var objectPropertySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var directiveAPI;
            var directiveReadyDeferred;


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isDirectiveLoading = true;

                $scope.scopeModel.objectVariables = [];
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedObjectVariable;
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onObjectSelectorReady = function (api) {
                    objectSelectorAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.onObjectVariableSelectionChanged = function () {
                    objectPropertySelectorAPI.clearDataSource();

                    if ($scope.scopeModel.selectedObjectVariable != undefined && $scope.scopeModel.selectedObjectVariable.ObjectType != undefined) {
                        var objectTypeConfigId = $scope.scopeModel.selectedObjectVariable.ObjectType.ConfigId;

                        objectPropertySelectorReadyDeferred.promise.then(function () {
                                loadObjectTypeConfigs().then(function () {
                                    $scope.scopeModel.isSelectorLoading = true;
                                    getObjectPropertySelectorTemplateConfigs(objectTypeConfigId != undefined ? objectTypeConfigId : null).then(function () {
                                        $scope.scopeModel.isSelectorLoading = false;
                                    });
                                });
                        });
                    }
                }

                $scope.scopeModel.onObjectPropertySelectorReady = function (api) {
                    objectPropertySelectorAPI = api;
                    objectPropertySelectorReadyDeferred.resolve();
                };
             
                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    var directivePayload = {};
                    if ($scope.scopeModel.selectedObjectVariable.ObjectType.RecordTypeId != undefined)
                        directivePayload.dataRecordTypeId = $scope.scopeModel.selectedObjectVariable.ObjectType.RecordTypeId;
                    if (criteriaFieldEntity != undefined)
                        directivePayload.valueEvaluator = criteriaFieldEntity.ValueEvaluator

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

                    if (payload != undefined && payload.criteriaField != undefined)
                        criteriaFieldEntity = payload.criteriaField;

                    //Loading ObjectSelector
                    if (payload != undefined && payload.objectVariables != undefined) {
                        for (var key in payload.objectVariables)
                            $scope.scopeModel.objectVariables.push(payload.objectVariables[key]);

                        if (criteriaFieldEntity != undefined) {
                            $scope.scopeModel.selectedObjectVariable = UtilsService.getItemByVal($scope.scopeModel.objectVariables, criteriaFieldEntity.ValueObjectName, 'ObjectName');

                            //In Case we have deleted the objectVariable
                            if ($scope.scopeModel.selectedObjectVariable == undefined) {
                                criteriaFieldEntity = undefined;
                            }
                        }
                    }
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
                                                if (criteriaFieldEntity != undefined && criteriaFieldEntity.ValueEvaluator.ConfigId != undefined) {
                                                    $scope.scopeModel.selectedTemplateConfig =
                                                            UtilsService.getItemByVal($scope.scopeModel.templateConfigs, criteriaFieldEntity.ValueEvaluator.ConfigId, 'ExtensionConfigurationId');
                                                }
                                            }
                                        });

            }
        }
    }

    app.directive('vrCommonObjectpropertySelector', VRObjectPropertySelector);

})(app);