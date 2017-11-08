(function (app) {

    'use strict';

    excelconversionListmapping.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_MappingFieldTypeEnum', 'VR_GenericData_DataRecordTypeService', 'VR_GenericData_RecordFilterService'];

    function excelconversionListmapping(UtilsService, VRUIUtilsService, VR_GenericData_MappingFieldTypeEnum, VR_GenericData_DataRecordTypeService, VR_GenericData_RecordFilterService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var listmapping = new Listmapping($scope, ctrl, $attrs);
                listmapping.initializeController();
            },
            controllerAs: "listMappingCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/ExcelConversion/Directives/ListMapping/Templates/ListMappingTemplate.html"
        };

        function Listmapping($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            var listName;
            var listMappingData;
            var recordFilterAPI;
            var recordFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            //var filterFieldMappingAPI;
            //var filterFieldMappingReadyPromiseDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                //$scope.scopeModel.onFilterFieldMappingReady = function (api) {
                //    filterFieldMappingAPI = api;
                //    var setLoader = function (value) {
                //        $scope.isLoadingDirective = value;
                //    };
                //    var directivePayload = { context: getContext() };

                //    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterFieldMappingAPI, directivePayload, setLoader, filterFieldMappingReadyPromiseDeferred);

                //};
                ctrl.fieldMappings = [];
                ctrl.updateLastRowIndexRange = function () {
                    if (context != undefined) {
                        var range = context.getSelectedCell();
                        if (range != undefined) {
                            ctrl.lastRowIndex = {
                                row: range[0],
                                col: range[1],
                                sheet: context.getSelectedSheet()
                            };
                        }

                    }

                };
                $scope.scopeModel.filterTypes = [{ value: 0, description: "Basic" }, { value: 1, description: "Advanced" }];
                ctrl.selectLastRowIndex = function () {
                    if (context != undefined) {
                        context.setSelectedCell(ctrl.lastRowIndex.row, ctrl.lastRowIndex.col, ctrl.lastRowIndex.sheet);
                    }
                };
                ctrl.updateFirstRowIndexRange = function () {
                    if (context != undefined) {
                        var range = context.getSelectedCell();
                        if (range != undefined) {
                            ctrl.firstRowIndex = {
                                row: range[0],
                                col: range[1],
                                sheet: context.getSelectedSheet()
                            };
                        }

                    }

                };
                ctrl.selectFirstRowIndex = function () {
                    if (context != undefined) {
                        context.setSelectedCell(ctrl.firstRowIndex.row, ctrl.firstRowIndex.col, ctrl.firstRowIndex.sheet);
                    }
                };

                ctrl.onRecordFilterReady = function (api) {
                    recordFilterAPI = api;
                    recordFilterReadyPromiseDeferred.resolve();
                };

                //$scope.scopeModel.onFilterTypeSelectionChanged = function (value) {
                //    if(value != undefined)
                //    {
                //        if($scope.scopeModel.selectedFilterType.value ==  $scope.scopeModel.filterTypes[1].value)
                //        {
                //            var directivePayload = { context: getRecordFilterContext() };
                //            var setLoader = function (value) { ctrl.isLoadingDirective = value };
                //            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, recordFilterAPI, directivePayload, setLoader);
                //        }
                //    }
                //};
                ctrl.filterFieldsMappings = [];
                ctrl.addField = function () {
                    var dataItem =
                        {
                            readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                            loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                            selectedDataTypes: VR_GenericData_MappingFieldTypeEnum.Text
                        };
                    addFilterAPIExtension(dataItem);
                };
                ctrl.dataTypes = UtilsService.getArrayEnum(VR_GenericData_MappingFieldTypeEnum);
                ctrl.addFilterValidate = function () {
                    if (ctrl.filterFieldsMappings.length == 0)
                        return true;
                    for (var i = 0 ; i < ctrl.filterFieldsMappings.length ; i++) {
                        var filterFieldsMapping = ctrl.filterFieldsMappings[i];
                        if (filterFieldsMapping.FieldName == undefined || filterFieldsMapping.selectedDataTypes == undefined)
                            return true;
                    }
                    return false;
                };
                ctrl.removeFilter = function (dataItem) {
                    var index = ctrl.filterFieldsMappings.indexOf(dataItem);
                    ctrl.filterFieldsMappings.splice(index, 1);
                };
                ctrl.isFilterFilled = function () {
                    var recordFilterData = recordFilterAPI.getData();
                    if (ctrl.filterFieldsMappings.length > 0 && recordFilterData.filterObj == undefined)
                        return "Filter does not edited yet.";
                    if (ctrl.checkIfFieldNameChanges(recordFilterData))
                        return "Fields are not matched.";
                    return null;
                };
                ctrl.checkIfFieldNameChanges = function (recordFilterData) {
                    if (recordFilterData != undefined && recordFilterData.filterObj != undefined) {
                        var fieldNames = VR_GenericData_RecordFilterService.getFilterGroupFieldNames(recordFilterData.filterObj);
                        if (fieldNames != undefined) {
                            for (var i = 0; i < fieldNames.length ; i++) {
                                var fieldName = fieldNames[i];
                                if (UtilsService.getItemByVal(ctrl.filterFieldsMappings, fieldName, "FieldName") == undefined)
                                    return true;
                            }
                        }
                    }
                    return false;
                };
                //$scope.scopeModel.conditions = UtilsService.getArrayEnum(VR_GenericData_ConditionEnum);
                //$scope.scopeModel.filterFieldMappings = [];
                //$scope.scopeModel.addConditionalCell = function () {
                //    var dataItem = {
                //        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                //        loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                //        selectedCondition: $scope.scopeModel.conditions[0],
                //        selectedDataTypes: ctrl.dataTypes[1]
                //    };
                //    addConditionalCellFieldMapping(dataItem);
                //};
                defineAPI();
            }
      
            function addFilterAPIExtension(dataItem, payloadEntity) {
                var payload = {
                    context: getContext()
                };
                if (payloadEntity != undefined) {
                    payload.fieldMapping = payloadEntity.FieldMapping;
                    payload.hideCell = true;
                    dataItem.FieldName = payloadEntity.FieldName;
                    dataItem.selectedDataTypes = UtilsService.getItemByVal(ctrl.dataTypes, payloadEntity.FieldType.ConfigId, "value.ConfigId");
                }
                dataItem.normalColNum = ctrl.normalColNum;
                dataItem.onFieldMappingReady = function (api) {
                    dataItem.fieldMappingAPI = api;
                    dataItem.readyPromiseDeferred.resolve();
                };
                dataItem.readyPromiseDeferred.promise
              .then(function () {

                  VRUIUtilsService.callDirectiveLoad(dataItem.fieldMappingAPI, payload, dataItem.loadPromiseDeferred);
              });
                ctrl.filterFieldsMappings.push(dataItem);
            }
            //function addConditionalCellFieldMapping(dataItem, payloadEntity) {
            //    var payload = {
            //        context: getContext()
            //    };
            //    if (payloadEntity != undefined) {
            //        payload.fieldMapping = payloadEntity.FieldMapping;
            //        payload.hideCell = true;
            //        dataItem.selectedDataTypePromiseDeferred = UtilsService.createPromiseDeferred();
            //        dataItem.selectedDataTypes = UtilsService.getItemByVal(ctrl.dataTypes, payloadEntity.FieldType.ConfigId, "value.ConfigId");
            //        fillOperations(dataItem);
            //        if (payloadEntity.RecordFilter != undefined)
            //        {
            //            dataItem.selectedConditionPromiseDeferred = UtilsService.createPromiseDeferred();

            //            dataItem.selectedCondition = UtilsService.getItemByVal($scope.scopeModel.conditions, payloadEntity.RecordFilter.$type, 'type');
            //            if (!dataItem.selectedCondition != undefined) {
            //                dataItem.selectedCondition = UtilsService.getItemByVal($scope.scopeModel.conditions, '', 'type');
            //            }

            //            dataItem.selectedConditionPromiseDeferred.promise.then(function () {
            //                dataItem.selectedConditionPromiseDeferred = undefined;
            //                if (!dataItem.selectedCondition != undefined) {
            //                    dataItem.selectedOperation = UtilsService.getItemByVal(dataItem.operations, payloadEntity.RecordFilter.CompareOperator, "value");
            //                    dataItem.showOperators = true;
            //                }
            //            });
                       
            //            dataItem.value = payloadEntity.RecordFilter.Value;
            //        }
            //    }
            //    dataItem.onFilterFieldMappingReady = function (api) {
            //        dataItem.fieldMappingAPI = api;
            //        dataItem.readyPromiseDeferred.resolve();
            //    };
            //    dataItem.onDataRecordTypeFieldSelectionChanged = function (dataType) {
            //        if (dataType != undefined) {
            //            if (dataItem.selectedDataTypePromiseDeferred != undefined)
            //            {
            //                dataItem.selectedDataTypePromiseDeferred.resolve();
            //                dataItem.selectedDataTypePromiseDeferred = undefined;
            //            }
            //            else
            //            {
            //                dataItem.value = undefined;
            //                fillOperations(dataItem);
            //            }
            //        }
            //    };
            //    dataItem.onConditionSelectionChanged = function (selectedCondition) {
            //        if (dataItem.selectedCondition != undefined) {
            //            if (dataItem.selectedConditionPromiseDeferred != undefined)
            //                dataItem.selectedConditionPromiseDeferred.resolve();
            //            else
            //            {
            //                switch (dataItem.selectedCondition.description) {
            //                    case VR_GenericData_ConditionEnum.Condition.description:
            //                        dataItem.showOperators = true;
            //                        break;
            //                    default:
            //                        dataItem.showOperators = false;
            //                        break;
            //                }
            //                dataItem.selectedOperation = undefined;
            //            }
                        
            //        }

            //    };
            //    dataItem.readyPromiseDeferred.promise
            //  .then(function () {

            //      VRUIUtilsService.callDirectiveLoad(dataItem.fieldMappingAPI, payload, dataItem.loadPromiseDeferred);
            //  });
            //    function  fillOperations(dataItem)
            //    {
            //        switch (dataItem.selectedDataTypes.value.ConfigId) {
            //            case VR_GenericData_MappingFieldTypeEnum.DateTime.value.ConfigId:
            //                dataItem.operations = UtilsService.getArrayEnum(VR_GenericData_DateTimeRecordFilterOperatorEnum);
            //                dataItem.showDateTime = true;
            //                dataItem.showTextBox = false;
            //                dataItem.showNumber = false;

            //                break;
            //            case VR_GenericData_MappingFieldTypeEnum.Text.value.ConfigId:
            //                dataItem.operations = UtilsService.getArrayEnum(VR_GenericData_StringRecordFilterOperatorEnum);
            //                dataItem.showDateTime = false;
            //                dataItem.showTextBox = true;
            //                dataItem.showNumber = false;
            //                break;
            //            case VR_GenericData_MappingFieldTypeEnum.Number.value.ConfigId:
            //                dataItem.operations = UtilsService.getArrayEnum(VR_GenericData_NumberRecordFilterOperatorEnum);
            //                dataItem.showDateTime = false;
            //                dataItem.showTextBox = false;
            //                dataItem.showNumber = true;
            //                break;
            //        }
            //    }
            //    $scope.scopeModel.filterFieldMappings.push(dataItem);
            //}
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        var promises = [];
                        context = payload.context;
                        listName = payload.listName;
                        listMappingData = payload.listMappingData;
                        ctrl.filterFieldsMappings.length = 0;
                        ctrl.lastRowIndex = undefined;
                        ctrl.firstRowIndex = undefined;
                        $scope.scopeModel.labelName = (payload.listTitle!=undefined)?payload.listTitle + " Date Time Format":undefined;
                        $scope.showDateFormat = payload.showDateFormat;
                        $scope.scopeModel.dateTimeFormat = (payload.listMappingData != undefined && payload.showDateFormat==true) ? payload.listMappingData.DateTimeFormat : undefined;
                        //$scope.scopeModel.filterFieldMappings.length = 0;
                        if (listMappingData != undefined) {

                            if (listMappingData.FirstRowIndex != undefined) {
                                ctrl.firstRowIndex = {
                                    row: listMappingData.FirstRowIndex,
                                    col: listMappingData.FirstRowIndex,
                                    sheet: listMappingData.SheetIndex
                                };
                            }
                            if (listMappingData.LastRowIndex != undefined) {
                                ctrl.lastRowIndex = {
                                    row: listMappingData.LastRowIndex,
                                    col: listMappingData.LastRowIndex,
                                    sheet: listMappingData.SheetIndex
                                };
                            }
                            if (listMappingData.Filter != undefined)
                            {
                                $scope.scopeModel.selectedFilterType = $scope.scopeModel.filterTypes[1];
                                
                                if (listMappingData.Filter.Fields != undefined) {
                                    for (var j = 0; j < listMappingData.Filter.Fields.length; j++) {
                                        var filterItem = {
                                            readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                        };
                                        var filterFieldsMappingEntity = listMappingData.Filter.Fields[j];
                                        promises.push(filterItem.loadPromiseDeferred.promise);
                                        addFilterAPIExtension(filterItem, filterFieldsMappingEntity);
                                    }
                                }
                            }

                            //if (listMappingData.RowFilters != undefined) {
                            //    $scope.scopeModel.selectedFilterType = $scope.scopeModel.filterTypes[0];
                            //    for (var j = 0; j < listMappingData.RowFilters.length; j++) {
                            //        var rowFilterItem = {
                            //            readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                            //            loadPromiseDeferred: UtilsService.createPromiseDeferred()
                            //        };
                            //        var rowFilter = listMappingData.RowFilters[j];
                            //        promises.push(rowFilterItem.loadPromiseDeferred.promise);
                            //        addConditionalCellFieldMapping(rowFilterItem, rowFilter);
                            //    }

                            //}

                        }
                        ctrl.fieldMappings = payload.fieldMappings;
                        for (var i = 0; i < ctrl.fieldMappings.length; i++) {
                            var item = ctrl.fieldMappings[i];
                            item.readyPromiseDeferred = UtilsService.createPromiseDeferred();
                            item.loadPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(item.loadPromiseDeferred.promise);
                            addAPIExtension(ctrl.fieldMappings[i]);
                        }


                        
                    }
                    if ($scope.scopeModel.selectedFilterType == undefined) {
                        $scope.scopeModel.selectedFilterType = $scope.scopeModel.filterTypes[1];
                    }
                    promises.push(loadRecordFilterDirective());
               //     promises.push(loadDataRecordFieldTypeConfig());
                    function loadRecordFilterDirective() {
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                        recordFilterReadyPromiseDeferred.promise.then(function () {
                            var directivePayload = { context: getRecordFilterContext() };
                            if (listMappingData != undefined && listMappingData.Filter != undefined) {
                                directivePayload.FilterGroup = listMappingData.Filter.FilterGroup;
                                directivePayload.ConditionExpression = listMappingData.Filter.ConditionExpression;

                            }
                            VRUIUtilsService.callDirectiveLoad(recordFilterAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }
                    function addAPIExtension(dataItem) {
                        var payload = {
                            context: getContext()
                        };

                        dataItem.normalColNum = ctrl.normalColNum;
                        dataItem.onFieldMappingReady = function (api) {
                            dataItem.fieldMappingAPI = api;
                            dataItem.readyPromiseDeferred.resolve();
                        };
                        dataItem.readyPromiseDeferred.promise
                      .then(function () {
                          if (listMappingData != undefined && listMappingData.FieldMappings != undefined && listMappingData.FieldMappings.length > 0) {
                              var fieldMapping = UtilsService.getItemByVal(listMappingData.FieldMappings, dataItem.FieldName, "FieldName");
                              if (fieldMapping != undefined) {
                                  payload.fieldMapping = fieldMapping;

                              }

                          }
                          VRUIUtilsService.callDirectiveLoad(dataItem.fieldMappingAPI, payload, dataItem.loadPromiseDeferred);
                      });
                    }
                    //function loadFieldMappings()
                    //{
                    //    ctrl.fieldMappings = payload.fieldMappings;
                    //    for (var i = 0; i < ctrl.fieldMappings.length; i++) {
                    //        var item = ctrl.fieldMappings[i];
                    //        item.readyPromiseDeferred = UtilsService.createPromiseDeferred();
                    //        item.loadPromiseDeferred = UtilsService.createPromiseDeferred();
                    //        promises.push(item.loadPromiseDeferred.promise);
                    //        addAPIExtension(ctrl.fieldMappings[i]);
                    //    }
                    //}

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var fieldMappings;
                    if (ctrl.fieldMappings.length > 0) {
                        fieldMappings = [];
                        for (var i = 0; i < ctrl.fieldMappings.length; i++) {
                            var fieldMapping = ctrl.fieldMappings[i];
                            if (fieldMapping.fieldMappingAPI != undefined) {
                                var fieldMappingData = fieldMapping.fieldMappingAPI.getData();
                                if (fieldMappingData != undefined) {
                                    fieldMappingData.FieldName = fieldMapping.FieldName;
                                    fieldMappingData.FieldType = fieldMapping.FieldType;
                                    fieldMappings.push(fieldMappingData);
                                }

                            }

                        }

                    }
                    var filter;
                    if (ctrl.filterFieldsMappings.length > 0) {
                        filter = {
                            Fields: []
                        };
                        if (recordFilterAPI != undefined) {
                            var filterObj = recordFilterAPI.getData();

                            filter.ConditionExpression = filterObj.expression;

                            filter.FilterGroup = filterObj.filterObj;
                        }

                        for (var i = 0; i < ctrl.filterFieldsMappings.length; i++) {
                            var filterFieldsMapping = ctrl.filterFieldsMappings[i];
                            filter.Fields.push({
                                FieldName: filterFieldsMapping.FieldName,
                                FieldMapping: filterFieldsMapping.fieldMappingAPI != undefined ? filterFieldsMapping.fieldMappingAPI.getData() : undefined,
                                FieldType: filterFieldsMapping.selectedDataTypes != undefined ? filterFieldsMapping.selectedDataTypes.value : undefined,
                            });
                        }
                    }

                    //var rowFilters;
                    //if ($scope.scopeModel.filterFieldMappings.length > 0) {
                    //    rowFilters = [];
                    //    for (var i = 0; i < $scope.scopeModel.filterFieldMappings.length; i++) {
                    //        var filterField = $scope.scopeModel.filterFieldMappings[i];
                    //        rowFilters.push({
                    //            FieldMapping: filterField.fieldMappingAPI != undefined ? filterField.fieldMappingAPI.getData() : undefined,
                    //            FieldType: filterField.selectedDataTypes != undefined ? filterField.selectedDataTypes.value : undefined,
                    //            RecordFilter: getRecordFilter(filterField)
                    //        });
                    //    }
                    //}
                    var data = {
                        ListName: listName,
                        SheetIndex: ctrl.firstRowIndex != undefined ? ctrl.firstRowIndex.sheet : undefined,
                        FirstRowIndex: ctrl.firstRowIndex != undefined ? ctrl.firstRowIndex.row : undefined,
                        LastRowIndex: ctrl.lastRowIndex != undefined ? ctrl.lastRowIndex.row : undefined,
                        FieldMappings: fieldMappings,
                        Filter: filter,
                       // RowFilters:rowFilters,
                        DateTimeFormat: ($scope.scopeModel.dateTimeFormat != undefined && $scope.scopeModel.showDateFormat==true) ? $scope.scopeModel.dateTimeFormat : undefined
                    };
                    return data;
                }
                //function getRecordFilter(dataItem)
                //{
                //    var recordFilter;
                //    if (dataItem.selectedCondition != undefined)
                //    {
                //        switch (dataItem.selectedCondition.description) {
                //            case VR_GenericData_ConditionEnum.Condition.description:
                //                if (dataItem.selectedDataTypes != undefined && dataItem.selectedOperation != undefined)
                //                {
                //                    switch (dataItem.selectedDataTypes.value.ConfigId) {
                //                        case VR_GenericData_MappingFieldTypeEnum.DateTime.value.ConfigId:
                //                            recordFilter = {
                //                                $type: "Vanrise.GenericData.Entities.DateTimeRecordFilter,Vanrise.GenericData.Entities",
                //                                CompareOperator: dataItem.selectedOperation.value,
                //                                Value: dataItem.value
                //                            };
                //                            break;
                //                        case VR_GenericData_MappingFieldTypeEnum.Text.value.ConfigId:
                //                            recordFilter = {
                //                                $type: "Vanrise.GenericData.Entities.StringRecordFilter,Vanrise.GenericData.Entities",
                //                                CompareOperator: dataItem.selectedOperation.value,
                //                                Value: dataItem.value
                //                            };
                //                            break;
                //                        case VR_GenericData_MappingFieldTypeEnum.Number.value.ConfigId:
                //                            recordFilter = {
                //                                $type: "Vanrise.GenericData.Entities.NumberRecordFilter,Vanrise.GenericData.Entities",
                //                                CompareOperator: dataItem.selectedOperation.value,
                //                                Value: dataItem.value
                //                            };
                //                            break;

                //                    }
                //                }

                //                break;
                //            case VR_GenericData_ConditionEnum.Empty.description:
                //                recordFilter = {
                //                    $type: VR_GenericData_ConditionEnum.Empty.type,
                //                };
                //                break;
                //            case VR_GenericData_ConditionEnum.NonEmpty.description:
                //                recordFilter = {
                //                    $type: VR_GenericData_ConditionEnum.NonEmpty.type,
                //                };
                //                break;
                //        }
                //    }
                   
                  
                //    return recordFilter;
                //}

            }
            function getContext() {

                if (context != undefined) {
                    var currentContext = UtilsService.cloneObject(context);
                    if (currentContext == undefined)
                        currentContext = {};
                    currentContext.getFirstRowIndex = function () {
                        return ctrl.firstRowIndex;
                    };
                    currentContext.getLastRowIndex = function () {
                        return  ctrl.lastRowIndex;
                    };
                    currentContext.getFilterFieldsMappings = function () {
                        return ctrl.filterFieldsMappings;
                    };

                    currentContext.getFilterCellFieldMapping = function (fieldName) {
                        if (fieldName != undefined)
                        {
                            var fieldMapping = UtilsService.getItemByVal(ctrl.filterFieldsMappings, fieldName, "FieldName");
                            if (fieldMapping != undefined && fieldMapping.fieldMappingAPI != undefined)
                                return fieldMapping.fieldMappingAPI.getData();
                        }
                    };
                    return currentContext;
                }
            }

            function getRecordFilterContext() {
                var context =
                    {
                        getFields: function () {
                            var fields = [];
                            for (var i = 0; i < ctrl.filterFieldsMappings.length; i++) {
                                var filterFieldsMapping = ctrl.filterFieldsMappings[i];

                                fields.push({
                                    FieldName: filterFieldsMapping.FieldName,
                                    FieldTitle: filterFieldsMapping.FieldName,
                                    Type: filterFieldsMapping.selectedDataTypes.value,
                                });
                            }
                            return fields;
                        }
                    };
                return context;
            }
            //function getRuleFilterEditorByFieldType(configId) {
            //    var dataRecordFieldTypeConfig = UtilsService.getItemByVal($scope.scopeModel.dataRecordFieldTypesConfig, configId, 'ExtensionConfigurationId');
            //    if (dataRecordFieldTypeConfig != undefined) {
            //        return dataRecordFieldTypeConfig.RuleFilterEditor;
            //    }
            //}
            //function loadDataRecordFieldTypeConfig() {
            //    $scope.scopeModel.dataRecordFieldTypesConfig = [];
            //    return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
            //        if (response) {
            //            for (var i = 0; i < response.length; i++) {
            //                $scope.scopeModel.dataRecordFieldTypesConfig.push(response[i]);
            //            }
            //        }
            //    });
            //}
        }
    }

    app.directive('vrExcelconversionListmapping', excelconversionListmapping);

})(app);