"use strict";
app.directive("vrGenericdataGenericbusinessentityConditiongroupSelectorcondition", ["UtilsService", "VRUIUtilsService", 'VR_GenericData_GenericBusinessEntityAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBusinessEntitySelectorFilterCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/SelectorCondition/Templates/ConditionGroupSelectorConditionTemplate.html"
        };

        function GenericBusinessEntitySelectorFilterCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var logicalOperatorDirectiveAPI;
            var logicalOperatorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var beDefinitionId;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.conditions = [];

                $scope.scopeModel.onLogicalOperatorReady = function (api) {
                    logicalOperatorDirectiveAPI = api;
                    logicalOperatorDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.conditions.length == 0)
                        return 'At least one item should be added';
                    return null;
                };

                $scope.scopeModel.addCondition = function () {
                    var condition = {
                        onSelectorConditionDirectiveReady: function (api) {
                            condition.api = api;

                            var payload = {
                                beDefinitionId: beDefinitionId
                            };

                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingCondition = value;
                            };

                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, payload, setLoader, undefined);
                        }
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
                    var promises = [];
                    var genericBESelectorConditionList;

                    if (payload != undefined) {
                        beDefinitionId = payload.beDefinitionId;
                        if (payload.genericBESelectorCondition != undefined)
                            genericBESelectorConditionList = payload.genericBESelectorCondition.GenericBESelectorConditionList;
                    }

                    var loadLogicalOperatorDirectivePromise = loadLogicalOperatorDirective();
                    promises.push(loadLogicalOperatorDirectivePromise);

                    function loadLogicalOperatorDirective() {
                        var loadLogicalOperatorDirectiveDeferred = UtilsService.createPromiseDeferred();

                        logicalOperatorDirectiveReadyDeferred.promise.then(function () {
                            var logicalOperatorPayload;
                            if (payload.genericBESelectorCondition != undefined)
                                logicalOperatorPayload = { LogicalOperator: payload.genericBESelectorCondition.Operator };

                            VRUIUtilsService.callDirectiveLoad(logicalOperatorDirectiveAPI, logicalOperatorPayload, loadLogicalOperatorDirectiveDeferred);
                        });

                        return loadLogicalOperatorDirectiveDeferred.promise;
                    }

                    if (genericBESelectorConditionList != undefined) {
                        for (var i = 0; i < genericBESelectorConditionList.length; i++) {

                            var selectorConditionDirectivePayload = {
                                beDefinitionId: beDefinitionId,
                                genericBESelectorCondition: genericBESelectorConditionList[i]
                            };

                            var selectorCondition = {
                                payload: selectorConditionDirectivePayload,
                                readySelectorConditionDirectivePromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadSelectorConditionDirectivePromiseDeferred: UtilsService.createPromiseDeferred()
                            };

                            promises.push(selectorCondition.loadSelectorConditionDirectivePromiseDeferred.promise);

                            extendSelectorCondition(selectorCondition);
                            $scope.scopeModel.conditions.push(selectorCondition);
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var selectorConditionList = [];
                    for (var i = 0; i < $scope.scopeModel.conditions.length; i++) {
                        selectorConditionList.push($scope.scopeModel.conditions[i].api.getData());
                    }

                    return {
                        $type: "Vanrise.GenericData.Business.GenericBESelectorConditionGroup,Vanrise.GenericData.Business",
                        Operator: logicalOperatorDirectiveAPI.getData(),
                        GenericBESelectorConditionList: selectorConditionList
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendSelectorCondition(selectorCondition) {
                selectorCondition.onSelectorConditionDirectiveReady = function (api) {
                    selectorCondition.api = api;
                    selectorCondition.readySelectorConditionDirectivePromiseDeferred.resolve();
                };

                selectorCondition.readySelectorConditionDirectivePromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(selectorCondition.api, selectorCondition.payload, selectorCondition.loadSelectorConditionDirectivePromiseDeferred);
                });
            }
        }
        return directiveDefinitionObject;
    }
]);