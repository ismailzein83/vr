(function (app) {

    'use strict';

    ChildEntityFilterRuntimeSettingsDirective.$inject = ['UtilsService', 'VR_GenericData_StringRecordFilterOperatorEnum', 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_GenericBusinessEntityAPIService', 'VRUIUtilsService', 'VR_GenericData_RecordQueryLogicalOperatorEnum'];

    function ChildEntityFilterRuntimeSettingsDirective(UtilsService, VR_GenericData_StringRecordFilterOperatorEnum, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_GenericBusinessEntityAPIService, VRUIUtilsService, VR_GenericData_RecordQueryLogicalOperatorEnum) {

        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ChildEntityFilterRuntimeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/GenericBEFilterDefinition/Templates/ChildEntityFilterRuntimeTemplate.html"
        };

        function ChildEntityFilterRuntimeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var definitionSettings;
            var childInputFieldName;
            var childOutputFieldName;
            var dataRecordTypeId;
            var fieldTypeConfigs;
            var isFromFilterSection;

            var childValuesArray = [];
            var parentValuesArray = [];

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    var allFieldValuesByName;
                    var genericContext;

                    if (payload != undefined) {
                        dataRecordTypeId = payload.dataRecordTypeId;
                        isFromFilterSection = payload.isFromFilterSection;
                        allFieldValuesByName = payload.allFieldValuesByName;
                        genericContext = payload.genericContext;
                        definitionSettings = payload.settings;

                        childInputFieldName = definitionSettings.ChildInputFieldName;
                        childOutputFieldName = definitionSettings.ChildOutputFieldName;
                    }

                    var loadDataRecordTypeFieldConfigsPromise = loadDataRecordTypeFieldConfigs();
                    initialPromises.push(loadDataRecordTypeFieldConfigsPromise);

                    var rootNodePromise = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var filterEditor;

                            var fieldType = definitionSettings.FieldType;
                            if (fieldType != undefined) {
                                var fieldTypeConfig = UtilsService.getItemByVal(fieldTypeConfigs, fieldType.ConfigId, 'ExtensionConfigurationId');
                                if (fieldTypeConfig != undefined) {
                                    filterEditor = fieldTypeConfig.FilterEditor;
                                }
                            }

                            if (filterEditor == null) {
                                return;
                            }

                            var filter = {};
                            filter.fieldName = childInputFieldName;
                            filter.isRequired = definitionSettings.IsRequired;
                            filter.directiveEditor = filterEditor;
                            filter.directiveLoadDeferred = UtilsService.createPromiseDeferred();
                            filter.onDirectiveReady = function (api) {
                                filter.directiveAPI = api;
                                var directivePayload = {
                                    fieldTitle: definitionSettings.FieldTitle,
                                    fieldType: fieldType,
                                    fieldName: childInputFieldName,
                                    fieldValue: definitionSettings.ChildInputDefaultFieldValues,
                                    genericContext: genericContext,
                                    allFieldValuesByName: allFieldValuesByName,
                                    //dataRecordTypeId: dataRecordTypeId
                                    isFromFilterSection: isFromFilterSection,
                                    childEntityFilterContext: getContext()
                                };
                                VRUIUtilsService.callDirectiveLoad(filter.directiveAPI, directivePayload, filter.directiveLoadDeferred);
                            };

                            $scope.scopeModel.filter = filter;

                            return {
                                promises: [filter.directiveLoadDeferred.promise],
                                getChildNode: function () {
                                    return {
                                        promises: [loadParentFilterFieldValues()]
                                    };
                                }
                            };
                        }
                    };

                    function loadDataRecordTypeFieldConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                            fieldTypeConfigs = response;
                        });
                    }

                    return UtilsService.waitPromiseNode(rootNodePromise);
                };

                api.getData = function () {
                    if (childValuesArray.length == 0)
                        return;

                    if (parentValuesArray != undefined && parentValuesArray.length > 0) {
                        return {
                            Filters: [{
                                FieldName: definitionSettings.ParentMappedFieldName,
                                FilterValues: parentValuesArray
                            }]
                        };
                    }
                    else {
                        return {
                            RecordFilter: { $type: "Vanrise.GenericData.Entities.AlwaysFalseRecordFilter, Vanrise.GenericData.Entities" }
                        };
                    }
                };

                api.hasFilters = function () {
                    return $scope.scopeModel.filter.directiveAPI != undefined;
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                    var _promises = [];
                    var directiveAPI = $scope.scopeModel.filter.directiveAPI;

                    if (directiveAPI != undefined && directiveAPI.onFieldValueChanged != undefined && typeof (directiveAPI.onFieldValueChanged) == "function") {
                        var onFieldValueChangedPromise = directiveAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                        if (onFieldValueChangedPromise != undefined)
                            _promises.push(onFieldValueChangedPromise);
                    }
                    return UtilsService.waitMultiplePromises(_promises);
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function loadParentFilterFieldValues() {
                childValuesArray.length = 0;
                parentValuesArray.length = 0;

                var values;
                if ($scope.scopeModel.filter.directiveAPI != undefined) {
                    values = $scope.scopeModel.filter.directiveAPI.getValuesAsArray();
                }

                if (values == undefined || values == "" || values.length == 0)
                    return UtilsService.waitMultiplePromises([]);

                for (var i = 0; i < values.length; i++) {
                    var value = values[i];
                    if (value != undefined) {
                        childValuesArray.push(value);
                    }
                }

                var recordFilter = {
                    $type: "Vanrise.GenericData.Entities.StringRecordFilter, Vanrise.GenericData.Entities",
                    FieldName: childInputFieldName,
                    Value: childValuesArray[0],
                    CompareOperator: VR_GenericData_StringRecordFilterOperatorEnum.Contains.value
                };

                var filterGroup = {
                    $type: "Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities",
                    LogicalOperator: VR_GenericData_RecordQueryLogicalOperatorEnum.And.value,
                    Filters: [recordFilter]
                };

                var input = {
                    BusinessEntityDefinitionId: definitionSettings.ChildBusinessEntityDefinitionId,
                    ColumnsNeeded: [childInputFieldName, childOutputFieldName],
                    FilterGroup: filterGroup
                };

                var getParentFilterFieldValuesDeferred = UtilsService.createPromiseDeferred();

                VR_GenericData_GenericBusinessEntityAPIService.GetAllGenericBusinessEntities(input).then(function (response) {
                    if (response && response.length > 0) {
                        for (var i = 0; i < response.length; i++) {
                            var dataRecord = response[i];

                            for (var prop in dataRecord.FieldValues) {
                                if (prop != childOutputFieldName)
                                    continue;

                                var currentdataRecordFieldValues = dataRecord.FieldValues[prop];
                                if (parentValuesArray.indexOf(currentdataRecordFieldValues) == -1)
                                    parentValuesArray.push(currentdataRecordFieldValues);
                            }
                        }
                    }

                    getParentFilterFieldValuesDeferred.resolve();
                });

                return getParentFilterFieldValuesDeferred.promise;
            }

            function getContext() {
                return {
                    loadParentFilterFieldValues: function () {
                        return loadParentFilterFieldValues();
                    }
                };
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterruntimeChildentity', ChildEntityFilterRuntimeSettingsDirective);
})(app);