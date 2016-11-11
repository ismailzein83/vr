'use strict';
app.directive('vrGenericdataBelookuprulestep', ['UtilsService', 'VRUIUtilsService','VR_GenericData_BELookupRuleDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_BELookupRuleDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new BELookupRuleStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/BELookupRule/Templates/BELookupRuleStepTemplate.html';
            }

        };

        function BELookupRuleStepCtor(ctrl, $scope) {
            var beLookupRuleDefinitionSelectorDirectiveReadyAPI;
            var beLookupRuleDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var businessEntityMappingApi;
            var businessEntityMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var mainPayload;
            function initializeController() {
                $scope.criteriaFieldsMappings = [];
                $scope.onBusinessEntityMappingReady = function (api) {


                    businessEntityMappingApi = api;
                    businessEntityMappingReadyPromiseDeferred.resolve();
                };

                $scope.onBELookupRuleDefinitionSelectorReady = function (api) {
                    beLookupRuleDefinitionSelectorDirectiveReadyAPI = api;

                    beLookupRuleDefinitionSelectorReadyPromiseDeferred.resolve();
                };

                $scope.onBELookupRuleDefinitionSelectionChanged = function () {

                    if (beLookupRuleDefinitionSelectorDirectiveReadyAPI != undefined && beLookupRuleDefinitionSelectorDirectiveReadyAPI.getSelectedIds() != undefined) {
                        $scope.isLoadingMappingData = true;
                        GetBELookupRuleDefinitionById(beLookupRuleDefinitionSelectorDirectiveReadyAPI.getSelectedIds()).then(function (response) {
                            if (response && response.CriteriaFields) {
                                $scope.criteriaFieldsMappings.length = 0;
                                for (var i = 0; i < response.CriteriaFields.length; i++) {
                                    var criteriaField = response.CriteriaFields[i];
                                    var filterItem = {
                                        CriteriaField: criteriaField,
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    var payload;
                                    if (mainPayload != undefined && mainPayload.stepDetails.CriteriaFieldsMappings != undefined) {
                                        var data = UtilsService.getItemByVal(mainPayload.stepDetails.CriteriaFieldsMappings, criteriaField.FieldPath, "FieldPath");
                                        if (data != undefined) {
                                            payload = data.Value;
                                        }
                                    }
                                    addFilterItemToGrid(filterItem, payload);
                                }
                            }
                        }).finally(function () {
                            $scope.isLoadingMappingData = false;
                        });
                    }
                };


                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined)
                        mainPayload = payload;
                    var promises = [];
                    var loadBeLookupRuleDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                    beLookupRuleDefinitionSelectorReadyPromiseDeferred.promise.then(function () {
                        var payloadSelector;
                        if (payload != undefined && payload.stepDetails != undefined)
                            payloadSelector = {
                                selectedIds: payload.stepDetails.BELookupRuleDefinitionId
                            }
                        VRUIUtilsService.callDirectiveLoad(beLookupRuleDefinitionSelectorDirectiveReadyAPI, payloadSelector, loadBeLookupRuleDefinitionSelectorPromiseDeferred);
                    });
                    promises.push(loadBeLookupRuleDefinitionSelectorPromiseDeferred.promise);

                    if (payload != undefined && payload.stepDetails != undefined && payload.stepDetails.BELookupRuleDefinitionId != undefined) {
                        GetBELookupRuleDefinitionById(payload.stepDetails.BELookupRuleDefinitionId).then(function (response) {
                            if (response && response.CriteriaFields) {
                                $scope.criteriaFieldsMappings.length = 0;
                                for (var i = 0; i < response.CriteriaFields.length; i++) {
                                    var criteriaField = response.CriteriaFields[i];
                                    var filterItem = {
                                        CriteriaField: criteriaField,
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    promises.push(filterItem.loadPromiseDeferred.promise);

                                    var payloadData;
                                    if (payload.stepDetails.CriteriaFieldsMappings != undefined) {
                                        var data = UtilsService.getItemByVal(payload.stepDetails.CriteriaFieldsMappings, criteriaField.FieldPath, "FieldPath");
                                        if (data != undefined) {
                                            payloadData = data.Value;
                                        }
                                    }
                                    addFilterItemToGrid(filterItem, payloadData);
                                }
                            }
                        });
                    }
                    var loadbusinessEntityMappingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                    businessEntityMappingReadyPromiseDeferred.promise.then(function () {
                        var payloadBusinessEntityMapping;
                        if (payload != undefined) {
                            payloadBusinessEntityMapping = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadBusinessEntityMapping.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadBusinessEntityMapping.selectedRecords = payload.stepDetails.BusinessEntity;
                        }
                        VRUIUtilsService.callDirectiveLoad(businessEntityMappingApi, payloadBusinessEntityMapping, loadbusinessEntityMappingDirectivePromiseDeferred);
                    });
                    promises.push(loadbusinessEntityMappingDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var criteriaFieldsMappings = [];
                    if ($scope.criteriaFieldsMappings.length > 0) {
                        for (var i = 0; i < $scope.criteriaFieldsMappings.length; i++) {
                            var criteriaFieldsMapping = $scope.criteriaFieldsMappings[i];
                            if (criteriaFieldsMapping.directiveAPI != undefined && criteriaFieldsMapping.directiveAPI.getData() != undefined) {
                                criteriaFieldsMappings.push({
                                    FieldPath: criteriaFieldsMapping.FieldName,
                                    Value: criteriaFieldsMapping.directiveAPI != undefined ? criteriaFieldsMapping.directiveAPI.getData() : undefined
                                });
                            }

                        }
                    }
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.MappingSteps.BELookupRuleMappingStep, Vanrise.GenericData.MainExtensions",
                        BELookupRuleDefinitionId: beLookupRuleDefinitionSelectorDirectiveReadyAPI != undefined ? beLookupRuleDefinitionSelectorDirectiveReadyAPI.getSelectedIds() : undefined,
                        CriteriaFieldsMappings: criteriaFieldsMappings,
                        BusinessEntity: businessEntityMappingApi != undefined ? businessEntityMappingApi.getData() : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function addFilterItemToGrid(filterItem, payload) {
                var dataItem = {
                    FieldName: filterItem.CriteriaField.FieldPath,
                    Title: filterItem.CriteriaField.Title
                };
                var dataItemPayload = {};

                if (mainPayload != undefined) {
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

                $scope.criteriaFieldsMappings.push(dataItem);
            }
            function GetBELookupRuleDefinitionById(beLookupRuleDefinitionId)
            {
                return VR_GenericData_BELookupRuleDefinitionAPIService.GetBELookupRuleDefinition(beLookupRuleDefinitionId);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);