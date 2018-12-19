(function (app) {

    'use strict';

    CompositeRecordConditionGroupDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_CompositeRecordConditionAPIService', 'VR_GenericData_RecordQueryLogicalOperatorEnum'];

    function CompositeRecordConditionGroupDirective(UtilsService, VRUIUtilsService, VR_GenericData_CompositeRecordConditionAPIService, VR_GenericData_RecordQueryLogicalOperatorEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CompositeRecordConditionGroupDirective($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/CompositeRecordCondition/Runtime/Templates/CompositeRecordConditionGroupTemplate.html'
        };

        function CompositeRecordConditionGroupDirective($scope, ctrl) {
            this.initializeController = initializeController;

            var compositeRecordCondition;
            var compositeRecordConditionResolvedDataList;
            var compositeRecordConditionDefinitionGroup;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.conditions = [];
                $scope.scopeModel.compositeRecordConditionConfigs = [];
                $scope.scopeModel.logicalOperators = UtilsService.getArrayEnum(VR_GenericData_RecordQueryLogicalOperatorEnum);

                $scope.scopeModel.validateCompositeRecordCondition = function () {
                    if ($scope.scopeModel.conditions.length == 0)
                        return 'At least one condition should be added';
                    return null;
                };

                $scope.scopeModel.addCondition = function (selectedCondition) {
                    var condition = {
                        onConditionFilterReady: function (api) {
                            condition.api = api;

                            var payload = {
                                compositeRecordConditionResolvedDataList: compositeRecordConditionResolvedDataList,
                                compositeRecordConditionDefinitionGroup: compositeRecordConditionDefinitionGroup
                            };
                            api.load(payload);
                        },
                        editor: selectedCondition.Editor
                    };

                    $scope.scopeModel.conditions.push(condition);
                };

                $scope.scopeModel.removeCondition = function (condition) {
                    var index = $scope.scopeModel.conditions.indexOf(condition);
                    $scope.scopeModel.conditions.splice(index, 1);
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var recordConditions;

                    if (payload != undefined) {
                        compositeRecordCondition = payload.compositeRecordCondition;
                        compositeRecordConditionResolvedDataList = payload.compositeRecordConditionResolvedDataList;
                        compositeRecordConditionDefinitionGroup = payload.compositeRecordConditionDefinitionGroup;

                        if (compositeRecordCondition != undefined) {
                            $scope.scopeModel.logicalOperator = compositeRecordCondition.LogicalOperator;
                            recordConditions = compositeRecordCondition.RecordConditions;
                        }
                    }

                    var promises = [];

                    var getCompositeRecordConditionConfigsPromise = getCompositeRecordConditionConfigs();
                    promises.push(getCompositeRecordConditionConfigsPromise);

                    if (recordConditions != undefined) {
                        var loadCompositeRecordConditionsDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadCompositeRecordConditionsDeferred.promise);

                        getCompositeRecordConditionConfigsPromise.then(function () {
                            buildCompositeRecordConditions(recordConditions, loadCompositeRecordConditionsDeferred);
                        });
                    }

                    function getCompositeRecordConditionConfigs() {
                        return VR_GenericData_CompositeRecordConditionAPIService.GetCompositeRecordConditionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.compositeRecordConditionConfigs.push(response[i]);
                                }
                            }
                        });
                    }

                    function buildCompositeRecordConditions(recordConditions, loadCompositeRecordConditionsDeferred) {
                        var _promises = [];

                        for (var i = 0; i < recordConditions.length; i++) {
                            var currentRecordCondition = recordConditions[i];

                            var compositeRecordConditionPayload = {
                                compositeRecordCondition: currentRecordCondition,
                                compositeRecordConditionDefinitionGroup: compositeRecordConditionDefinitionGroup,
                                compositeRecordConditionResolvedDataList: compositeRecordConditionResolvedDataList
                            };

                            var compositeRecordConditionDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                            _promises.push(compositeRecordConditionDirectiveLoadDeferred.promise);

                            buildCompositeRecordCondition(compositeRecordConditionPayload, compositeRecordConditionDirectiveLoadDeferred);
                        }

                        UtilsService.waitMultiplePromises(_promises).then(function () {
                            loadCompositeRecordConditionsDeferred.resolve();
                        });
                    }

                    function buildCompositeRecordCondition(compositeRecordConditionPayload, compositeRecordConditionDirectiveLoadDeferred) {
                        var condition = {};

                        var compositeRecordCondition = compositeRecordConditionPayload.compositeRecordCondition;

                        if (compositeRecordCondition != undefined) {
                            var conditionConfigId = compositeRecordCondition.ConfigId.toLowerCase();
                            var compositeRecordConditionConfig = UtilsService.getItemByVal($scope.scopeModel.compositeRecordConditionConfigs, conditionConfigId, 'ExtensionConfigurationId');
                            if (compositeRecordConditionConfig != undefined)
                                condition.editor = compositeRecordConditionConfig.Editor;
                        }

                        condition.onConditionFilterReady = function (api) {
                            condition.api = api;
                            VRUIUtilsService.callDirectiveLoad(condition.api, compositeRecordConditionPayload, compositeRecordConditionDirectiveLoadDeferred);
                        };

                        $scope.scopeModel.conditions.push(condition);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.CompositeRecordCondition.Runtime.CompositeRecordConditionGroup, Vanrise.GenericData.MainExtensions",
                        ConfigId: "9ADCEA46-87C2-4747-8B88-20796AA99CA0",
                        RecordConditions: getRecordConditions(),
                        LogicalOperator: $scope.scopeModel.logicalOperator
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function getRecordConditions() {
                var recordConditions = [];
                for (var i = 0; i < $scope.scopeModel.conditions.length; i++) {
                    var currentCondition = $scope.scopeModel.conditions[i];
                    recordConditions.push(currentCondition.api.getData());
                }
                return recordConditions;
            }
        }
    }

    app.directive('vrGenericdataCompositerecordconditiongroup', CompositeRecordConditionGroupDirective);
})(app);