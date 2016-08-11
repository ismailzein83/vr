(function (app) {

    'use strict';

    excelconversionListmapping.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_MappingFieldTypeEnum', 'VR_GenericData_DataRecordTypeService','VR_GenericData_RecordFilterService'];

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

            function initializeController() {
                ctrl.fieldMappings = [];
                ctrl.updateLastRowIndexRange = function () {
                    if (context != undefined) {
                        var range = context.getSelectedCell();
                        if (range != undefined) {
                            ctrl.lastRowIndex = {
                                row: range[0],
                                col: range[1],
                                sheet: context.getSelectedSheet()
                            }
                        }

                    }

                }
                ctrl.selectLastRowIndex = function () {
                    if (context != undefined) {
                        context.setSelectedCell(ctrl.lastRowIndex.row, ctrl.lastRowIndex.col, ctrl.lastRowIndex.sheet);
                    }
                }
                ctrl.updateFirstRowIndexRange = function () {
                    if (context != undefined) {
                        var range = context.getSelectedCell();
                        if (range != undefined) {
                            ctrl.firstRowIndex = {
                                row: range[0],
                                col: range[1],
                                sheet: context.getSelectedSheet(),
                            }
                        }

                    }

                }
                ctrl.selectFirstRowIndex = function () {
                    if (context != undefined) {
                        context.setSelectedCell(ctrl.firstRowIndex.row, ctrl.firstRowIndex.col, ctrl.firstRowIndex.sheet);
                    }
                }

                ctrl.onRecordFilterReady = function(api)
                {
                    recordFilterAPI = api;
                    recordFilterReadyPromiseDeferred.resolve();
                }

                ctrl.filterFieldsMappings = [];
                ctrl.addField = function () {
                    var dataItem =
                        {
                            readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                            loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                            selectedDataTypes : VR_GenericData_MappingFieldTypeEnum.Text
                        };
                    addFilterAPIExtension(dataItem);
                }
                ctrl.dataTypes = UtilsService.getArrayEnum(VR_GenericData_MappingFieldTypeEnum);
                ctrl.addFilterValidate = function()
                {
                    if (ctrl.filterFieldsMappings.length == 0)
                        return true;
                    for(var i=0 ;i<ctrl.filterFieldsMappings.length ;i++)
                    {
                        var filterFieldsMapping = ctrl.filterFieldsMappings[i];
                        if (filterFieldsMapping.FieldName == undefined || filterFieldsMapping.selectedDataTypes == undefined)
                            return true;
                    }
                    return false;
                }
                ctrl.removeFilter = function(dataItem)
                {
                    var index = ctrl.filterFieldsMappings.indexOf(dataItem);
                    ctrl.filterFieldsMappings.splice(index, 1);
                }
                ctrl.isFilterFilled = function()
                {
                    var recordFilterData = recordFilterAPI.getData();
                    if (ctrl.filterFieldsMappings.length > 0 && recordFilterData.filterObj == undefined)
                        return "Filter does not edited yet.";
                    if (ctrl.checkIfFieldNameChanges(recordFilterData))
                        return "Fields are not matched."
                    return null;
                }
                ctrl.checkIfFieldNameChanges = function (recordFilterData)
                {
                    if (recordFilterData != undefined && recordFilterData.filterObj != undefined)
                    {
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
                }
                defineAPI();
            }

            function addFilterAPIExtension(dataItem, payloadEntity) {
                var payload = {
                    context: getContext()
                };
                if (payloadEntity != undefined) {
                    payload.fieldMapping = payloadEntity.FieldMapping;
                    dataItem.FieldName = payloadEntity.FieldName;
                    dataItem.selectedDataTypes = UtilsService.getItemByVal(ctrl.dataTypes, payloadEntity.FieldType.ConfigId,"value.ConfigId")
                }
                dataItem.normalColNum = ctrl.normalColNum;

                dataItem.onFieldMappingReady = function (api) {
                    dataItem.fieldMappingAPI = api;
                    dataItem.readyPromiseDeferred.resolve();
                }
                dataItem.readyPromiseDeferred.promise
              .then(function () {
                  
                  VRUIUtilsService.callDirectiveLoad(dataItem.fieldMappingAPI, payload, dataItem.loadPromiseDeferred);
              });
                ctrl.filterFieldsMappings.push(dataItem);
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        var promises = [];
                        context = payload.context;
                        listName = payload.listName;
                        listMappingData = payload.listMappingData;
                        ctrl.lastRowIndex = undefined;
                        ctrl.firstRowIndex = undefined;
                        if (listMappingData != undefined)
                        {
                           
                            if (listMappingData.FirstRowIndex !=undefined)
                            {
                                ctrl.firstRowIndex = {
                                    row: listMappingData.FirstRowIndex,
                                    col: listMappingData.FirstRowIndex,
                                    sheet: listMappingData.SheetIndex
                                }
                            }
                            if (listMappingData.LastRowIndex != undefined)
                            {
                                ctrl.lastRowIndex = {
                                    row: listMappingData.LastRowIndex,
                                    col: listMappingData.LastRowIndex,
                                    sheet: listMappingData.SheetIndex
                                }
                            }
                            if(listMappingData.Filter != undefined && listMappingData.Filter.Fields != undefined)
                            {
                                for (var j = 0; j < listMappingData.Filter.Fields.length; j++) {
                                    var filterItem = {
                                        readyPromiseDeferred : UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred : UtilsService.createPromiseDeferred()
                                    }
                                    var filterFieldsMappingEntity = listMappingData.Filter.Fields[j];
                                    promises.push(filterItem.loadPromiseDeferred.promise);
                                    addFilterAPIExtension(filterItem, filterFieldsMappingEntity);
                                }
                            }

                        }
                        ctrl.fieldMappings = payload.fieldMappings;
                        for (var i = 0; i < ctrl.fieldMappings.length; i++) {
                            var item = ctrl.fieldMappings[i];
                            item.readyPromiseDeferred = UtilsService.createPromiseDeferred(),
                            item.loadPromiseDeferred = UtilsService.createPromiseDeferred()
                            promises.push(item.loadPromiseDeferred.promise);
                            addAPIExtension(ctrl.fieldMappings[i]);
                        }

                      

                    }
                    promises.push(loadRecordFilterDirective());
                    function loadRecordFilterDirective() {
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                        recordFilterReadyPromiseDeferred.promise.then(function () {
                            var directivePayload = { context: getRecordFilterContext() };
                            if (listMappingData.Filter != undefined)
                            {
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
                        }
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
                    if (ctrl.filterFieldsMappings.length > 0)
                    {
                        filter = {
                            Fields: []
                        };
                        if (recordFilterAPI != undefined) {
                            var filterObj = recordFilterAPI.getData();

                            filter.ConditionExpression = filterObj.expression;

                            filter.FilterGroup = filterObj.filterObj;
                        }
                        
                        for(var i=0;i<ctrl.filterFieldsMappings.length;i++)
                        {
                            var filterFieldsMapping = ctrl.filterFieldsMappings[i];
                            filter.Fields.push({
                                FieldName: filterFieldsMapping.FieldName,
                                FieldMapping: filterFieldsMapping.fieldMappingAPI !=undefined? filterFieldsMapping.fieldMappingAPI.getData():undefined,
                                FieldType: filterFieldsMapping.selectedDataTypes != undefined ? filterFieldsMapping.selectedDataTypes.value : undefined,
                            });
                        }
                    }

                    var data = {
                        ListName: listName,
                        SheetIndex: ctrl.firstRowIndex != undefined ? ctrl.firstRowIndex.sheet : undefined,
                        FirstRowIndex: ctrl.firstRowIndex != undefined ? ctrl.firstRowIndex.row : undefined,
                        LastRowIndex: ctrl.lastRowIndex != undefined ? ctrl.lastRowIndex.row : undefined,
                        FieldMappings: fieldMappings,
                        Filter: filter
                    };
                    return data;
                }
                
            }
            function getContext() {

                if (context != undefined) {
                    var currentContext = UtilsService.cloneObject(context);
                    if (currentContext == undefined)
                        currentContext = {};
                    currentContext.getFirstRowIndex = function () {
                        return ctrl.firstRowIndex;
                    }
                    return currentContext;
                }
            }

            function getRecordFilterContext()
            {
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
        }
    }

    app.directive('vrExcelconversionListmapping', excelconversionListmapping);

})(app);