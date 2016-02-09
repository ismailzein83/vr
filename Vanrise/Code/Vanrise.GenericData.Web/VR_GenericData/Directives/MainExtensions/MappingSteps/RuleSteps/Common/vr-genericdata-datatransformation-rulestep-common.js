'use strict';
app.directive('vrGenericdataDatatransformationRulestepCommon', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService','VR_GenericData_GenericRuleDefinitionAPIService','VR_GenericData_GenericRuleDefinitionService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService, VR_GenericData_GenericRuleDefinitionAPIService, VR_GenericData_GenericRuleDefinitionService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new rulestepCommonCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/Common/Templates/RuleStepCommonTemplate.html';
            }

        };

        function rulestepCommonCtor(ctrl, $scope) {
            var ruleTypeName;
            var ruleTypeEntity;
            var isSecondSelection = false;
            var firstTimeload = true;
            var mainPayload;

            var ruleDefinitionDirectiveAPI;
            var ruleDefinitionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var effectiveTimeDirectiveAPI;
            var effectiveTimeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.ruleFieldsMappings = [];

                $scope.onRuleDefinitionReady = function (api) {
                    ruleDefinitionDirectiveAPI = api;
                    ruleDefinitionDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onEffectiveTimeDirectiveReady = function (api) {
                    effectiveTimeDirectiveAPI = api; 
                    effectiveTimeDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onRuleSelectionChanged = function () {
                    if ($scope.selectedRuleDefinition == undefined)
                    {
                        $scope.ruleFieldsMappings.length = 0;
                    }
                    else
                    {
                        if (isSecondSelection && firstTimeload)
                        {
                            $scope.onRuleSelectionItem($scope.selectedRuleDefinition);
                        }
                        isSecondSelection = true;
                    }
                }

                $scope.onRuleSelectionItem = function (selectedItem) {
                
                        $scope.isLoadingMappingData = true;
                        $scope.ruleFieldsMappings.length = 0;
                        loadRuleDefinition(selectedItem.GenericRuleDefinitionId).then(function (response) {
                            if (response.CriteriaDefinition.Fields) {
                                for (var i = 0; i < response.CriteriaDefinition.Fields.length; i++) {
                                    var filterItem = {
                                        RuleFields: response.CriteriaDefinition.Fields[i],
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    var payload;
                                    if (mainPayload != undefined && mainPayload.ruleFieldsMappings != undefined && mainPayload.ruleFieldsMappings.length>0 && firstTimeload)
                                        payload = mainPayload.ruleFieldsMappings[i].Value;
                                    addFilterItemToGrid(filterItem, payload);
                                }
                            }
                        }).finally(function () {
                            $scope.isLoadingMappingData = false;
                            isSecondSelection = false;
                            firstTimeload = false;
                        });
                    
                }

                function addFilterItemToGrid(filterItem, payload) {

                    var dataItem = {
                        FieldName: filterItem.RuleFields.FieldName,
                        Title: filterItem.RuleFields.Title
                    };
                    var dataItemPayload = {};

                    if (mainPayload != undefined)
                    {
                        dataItemPayload.context = mainPayload.context;

                    }
                    if (payload != undefined)
                        dataItemPayload.selectedRecords = payload;
                    dataItem.onSourceMappingReady = function (api) {
                        dataItem.directiveAPI = api;
                        filterItem.readyPromiseDeferred.resolve();
                    };

                    filterItem.readyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, filterItem.loadPromiseDeferred);
                        });

                    $scope.ruleFieldsMappings.push(dataItem);
                }


                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    mainPayload = payload;
                    if (payload != undefined)
                    {
                        var promises = [];
                        var loadRuleDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        ruleTypeName = payload.ruleTypeName;
                        loadRuleType(payload.ruleTypeName).then(function () {
                            ruleDefinitionDirectiveReadyPromiseDeferred.promise.then(function () {
                                var payloadRuleDefinition = { filter: { RuleTypeId: ruleTypeEntity.GenericRuleTypeConfigId } };
                                if (payload.ruleDefinitionId != undefined)
                                {
                                    payloadRuleDefinition.selectedIds = payload.ruleDefinitionId;
                                }
                                payloadRuleDefinition.showaddbutton = true;
                                payloadRuleDefinition.specificTypeName = ruleTypeName;
                                VRUIUtilsService.callDirectiveLoad(ruleDefinitionDirectiveAPI, payloadRuleDefinition, loadRuleDefinitionDirectivePromiseDeferred);
                            });
                        })
                        promises.push(loadRuleDefinitionDirectivePromiseDeferred.promise);

                        var loadEffectiveTimeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        effectiveTimeDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadEffectiveTime = {};
                            if (payload.context != undefined)
                                payloadEffectiveTime.context = payload.context;
                            if (payload.effectiveTime != undefined)
                                payloadEffectiveTime.selectedRecords = payload.effectiveTime;
                            VRUIUtilsService.callDirectiveLoad(effectiveTimeDirectiveAPI, payloadEffectiveTime, loadEffectiveTimeDirectivePromiseDeferred);
                        });
                        promises.push(loadEffectiveTimeDirectivePromiseDeferred.promise);
                        return UtilsService.waitMultiplePromises(promises);
                    }
                   
                }

                function loadRuleType(ruleTypeName) {
                    return VR_GenericData_GenericRuleTypeConfigAPIService.GetGenericRuleTypeByName(ruleTypeName).then(function (response) {
                        ruleTypeEntity = response;
                    });
                }

                api.setData =function(obj)
                {
                    var ruleFieldsMappings = [];
                    for (var i = 0; i < $scope.ruleFieldsMappings.length; i++) {
                        ruleFieldsMappings.push({
                            RuleCriteriaFieldName: $scope.ruleFieldsMappings[i].FieldName,
                            Value: $scope.ruleFieldsMappings[i].directiveAPI.getData()
                        });
                    }
                    obj.ConfigId = ruleTypeEntity !=undefined? ruleTypeEntity.GenericRuleTypeConfigId:undefined;
                    obj.RuleDefinitionId = $scope.selectedRuleDefinition!=undefined? $scope.selectedRuleDefinition.GenericRuleDefinitionId:undefined;
                    obj.EffectiveTime = effectiveTimeDirectiveAPI != undefined ? effectiveTimeDirectiveAPI.getData() : undefined;
                    obj.RuleFieldsMappings = ruleFieldsMappings;

                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadRuleDefinition(ruleDefinitionId) {
               return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(ruleDefinitionId);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);