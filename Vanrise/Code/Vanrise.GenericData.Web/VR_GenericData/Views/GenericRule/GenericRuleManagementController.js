(function (appControllers) {
    'use strict';

    genericRuleManagementController.$inject = ['$scope', 'VRNavigationService', 'VR_GenericData_GenericRule', 'VR_GenericData_GenericRuleDefinitionAPIService', 'VR_GenericData_DataRecordFieldTypeConfigAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function genericRuleManagementController($scope, VRNavigationService, VR_GenericData_GenericRule, VR_GenericData_GenericRuleDefinitionAPIService, VR_GenericData_DataRecordFieldTypeConfigAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;

        loadParameters();
        defineScope();
        load();

        var ruleDefinitionId

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null) {
                ruleDefinitionId = parameters.ruleDefinitionId;
            }
        }

        function defineScope() {
            $scope.filters = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var defFilter = {
                    RuleDefinitionId: ruleDefinitionId
                };
                gridAPI.loadGrid(defFilter);
            };

            $scope.search = function () {
                var filter = getFilterObject();
                console.log(filter);
                return gridAPI.loadGrid(filter);
            };

            $scope.addGenericRule = function () {
                var onGenericRuleAdded = function (ruleObj) {
                    gridAPI.onGenericRuleAdded(ruleObj);
                };

                VR_GenericData_GenericRule.addGenericRule(ruleDefinitionId, onGenericRuleAdded);
            };

            function getFilterObject() {
                var gridQuery = { RuleDefinitionId: ruleDefinitionId };

                gridQuery.CriteriaFieldValues = {};
                for (var i = 0; i < $scope.filters.length; i++) {
                    gridQuery.CriteriaFieldValues[$scope.filters[i].criteriaFieldName] = $scope.filters[i].directiveAPI.getData();
                }

                return gridQuery;
            }
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilters]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function loadFilters() {
                var promises = [];
                var ruleDefinition;
                var fieldTypes;

                var getRuleDefinitionAndFieldTypesDeferred = UtilsService.createPromiseDeferred();
                promises.push(getRuleDefinitionAndFieldTypesDeferred.promise);

                UtilsService.waitMultipleAsyncOperations([getRuleDefinition, getFieldTypes]).then(function () {
                    for (var i = 0; i < ruleDefinition.CriteriaDefinition.Fields.length; i++) {
                        var fieldTypeConfigId = ruleDefinition.CriteriaDefinition.Fields[i];
                        var filter = getFilter(fieldTypeConfigId);
                        if (filter != undefined) {
                            promises.push(filter.directiveLoadDeferred.promise);
                            $scope.filters.push(filter);
                        }
                    }

                    getRuleDefinitionAndFieldTypesDeferred.resolve();
                }).catch(function (error) {
                    getRuleDefinitionAndFieldTypesDeferred.reject(error);
                });

                return UtilsService.waitMultiplePromises(promises);

                function getRuleDefinition() {
                    return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(ruleDefinitionId).then(function (response) {
                        ruleDefinition = response;
                    });
                }
                function getFieldTypes() {
                    return VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypes().then(function (response) {
                        fieldTypes = [];
                        for (var i = 0; i < response.length; i++) {
                            fieldTypes.push(response[i]);
                        }
                    });
                }
                function getFilter(criteriaField) {
                    var filter;
                    var filterEditor = UtilsService.getItemByVal(fieldTypes, criteriaField.FieldType.ConfigId, 'DataRecordFieldTypeConfigId').FilterEditor;

                    if (filterEditor == null) return filter;

                    filter = {};
                    filter.criteriaFieldName = criteriaField.FieldName;
                    filter.directiveEditor = filterEditor;
                    filter.directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    filter.onDirectiveReady = function (api) {
                        filter.directiveAPI = api;
                        var directivePayload = {
                            fieldTitle: criteriaField.Title,
                            fieldType: criteriaField.FieldType
                        };
                        VRUIUtilsService.callDirectiveLoad(api, directivePayload, filter.directiveLoadDeferred);
                    };

                    return filter;
                }
            }
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleManagementController', genericRuleManagementController);

})(appControllers);
