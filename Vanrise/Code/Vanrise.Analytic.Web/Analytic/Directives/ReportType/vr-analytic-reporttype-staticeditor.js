//(function (app) {

//    'use strict';

//    AnalyticReportTypeStaticEditor.$inject = ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRComponentTypeAPIService', 'VR_GenericData_DataRecordStorageAPIService', 'VR_GenericData_DataRecordFieldAPIService'];

//    function AnalyticReportTypeStaticEditor(UtilsService, VRUIUtilsService, VRCommon_VRComponentTypeAPIService, VR_GenericData_DataRecordStorageAPIService, VR_GenericData_DataRecordFieldAPIService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new AnalyticReportTypeStaticEditorCtrol($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/Analytic/Directives/ReportType/Templates/ReportTypeStaticEditorTemplate.html"
//        };

//        function AnalyticReportTypeStaticEditorCtrol($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var reportTypeEntity;
//            var timePeriod;
//            var reportTypeId;
//            var reportAction;
//            var reportHandler;
//            var filterFields;
//            var genericContext;
//            var context;
//            var dataRecordTypeFieldsInfo = [];
//            var dataRecordFieldTypes = [];
//            var previousFieldTypeRuntimeDirective;

//            // Definition Section 
//            var periodSelectorAPI;
//            var periodSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            var reportTypeInfoSelectorAPI;
//            var reportTypeInfoReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            // Action Section 
//            var actionTypeSelectiveAPI;
//            var actionTypeSelectiveReadyDeferred = UtilsService.createPromiseDeferred();
//            var actionTypeSelectionChanged = UtilsService.createPromiseDeferred();

//            var attachementsSelectorAPI;
//            var attachementsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            // Handler Section
//            var handlerFieldNameSelectorAPI;
//            var handlerFieldNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();

//            var handlerSettingsAPI;
//            var handlerSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            //Filter Section
//            var filterFieldsGridAPI;
//            var filterFieldsGridReadyDeferred = UtilsService.createPromiseDeferred();

//            var filterFieldNameAPI;
//            var filterFieldNameReadyPromiseDeferred = UtilsService.createPromiseDeferred();
//            var filterFielNameSelectionChanged = UtilsService.createPromiseDeferred();

//            var fieldTypeRuntimeDirectiveAPI;
//            var fieldTypeRuntimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.actionTab = { showTab: false };
//                $scope.scopeModel.handlerTab = { showTab: false };
//                $scope.scopeModel.filterTab = { showTab: false };
//                $scope.scopeModel.attachemnts = [];
//                $scope.scopeModel.selectedAttachements = [];
//                $scope.scopeModel.filterFieldNames = [];

//                // Definition Section 
//                $scope.scopeModel.onPeriodSelectorReady = function (api) {
//                    periodSelectorAPI = api;
//                    periodSelectorReadyPromiseDeferred.resolve();
//                };

//                $scope.scopeModel.onReportTypeInfoSelector = function (api) {
//                    reportTypeInfoSelectorAPI = api;
//                    reportTypeInfoReadyPromiseDeferred.resolve();
//                };

//                $scope.scopeModel.onReportTypeSelectorChanged = function (item) {
//                    if (item != undefined) {
//                        VRCommon_VRComponentTypeAPIService.GetVRComponentType(item.VRComponentTypeId).then(function (response) {
//                            console.log(response);
//                            attachementsSelectorAPI.clearDataSource();
//                            $scope.scopeModel.actionTab.showTab = true;
//                            $scope.scopeModel.handlerTab.showTab = true;
//                            $scope.scopeModel.filterTab.showTab = true;

//                            if (response != undefined) {
//                                var dataStorageId = response.Settings.DataStorageId;
//                                var attachements = response.Settings.Attachements;
//                                var filterFields = response.Settings.FilterFields;

//                                for (var i = 0; i < attachements.length; i++) {
//                                    var attachement = attachements[i];
//                                    $scope.scopeModel.attachemnts.push(attachement);
//                                }
//                                if (reportAction != undefined)
//                                    $scope.scopeModel.selectedAttachements = UtilsService.getItemByVal($scope.scopeModel.attachemnts, reportAction.Attachements, "VRReportTypeAttachementId");

                              

//                                VR_GenericData_DataRecordStorageAPIService.GetDataRecordStorage(dataStorageId).then(function (recordStorage) {
//                                    if (recordStorage != undefined) {
//                                        var dataRecordTypeId = recordStorage.DataRecordTypeId;
//                                        getDataRecordFieldsInfo(dataRecordTypeId).then(function () {
//                                            for (var j = 0; j < filterFields.length; j++) {
//                                                var fieldName = filterFields[j].FieldName;
//                                                $scope.scopeModel.filterFieldNames.push({ filedName: fieldName });
//                                            }

//                                        });
//                                        var setLoader = function (value) {
//                                            $scope.scopeModel.isHandlerTabLoading = value;
//                                        };
//                                        var handlerFieldPayload = {
//                                            dataRecordTypeId: dataRecordTypeId
//                                        };
//                                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, handlerFieldNameSelectorAPI, handlerFieldPayload, setLoader);
//                                    }
//                                });
//                            }
//                        });
//                    }
//                };

//                // Action Section 
//                $scope.scopeModel.onActionTypeSelectiveReady = function (api) {
//                    actionTypeSelectiveAPI = api;
//                    actionTypeSelectiveReadyDeferred.resolve();
//                };

//                $scope.scopeModel.onActionTypeSelectorChanged = function (item) {
//                    if (actionTypeSelectionChanged != undefined)
//                        actionTypeSelectionChanged.resolve();
//                    else {
//                        $scope.scopeModel.attachemnts.length = 0;
//                    }
//                };

//                $scope.scopeModel.onAttachementsSelectorReady = function (api) {
//                    attachementsSelectorAPI = api;
//                    attachementsSelectorReadyPromiseDeferred.resolve();
//                };
       
//                // Handler Section 
//                $scope.scopeModel.onHandlerFieldNameSelectorReady = function (api) {
//                    handlerFieldNameSelectorAPI = api;
//                    handlerFieldNameSelectorReadyDeferred.resolve();
//                };

//                $scope.scopeModel.onHandlerSettingsAutomatedReportReady = function (api) {
//                    handlerSettingsAPI = api;
//                    handlerSettingsReadyPromiseDeferred.resolve();
//                };

//                // Filter Section 
//                $scope.scopeModel.onFilterFieldsGridReady = function (api) {
//                    filterFieldsGridAPI = api;
//                    filterFieldsGridReadyDeferred.resolve();
//                };

//                $scope.scopeModel.onFilterFieldNameReady = function (api) {
//                    filterFieldNameAPI = api;
//                    filterFieldNameReadyPromiseDeferred.resolve();
//                };

//                $scope.scopeModel.addFilterField = function () {
//                    addAllColumns();
//                };

//                $scope.scopeModel.onFilterFieldNameSelectorChanged = function (item) {
//                    if (filterFielNameSelectionChanged != undefined) {
//                        filterFielNameSelectionChanged.resolve();
//                    }
//                    else {
//                        if (item != undefined) {
//                            var fieldType = getContext().getFieldType(item.filedName);
//                            var dataRecordFieldType = UtilsService.getItemByVal(dataRecordFieldTypes, fieldType.ConfigId, "ExtensionConfigurationId");

//                            if (dataRecordFieldType != undefined && dataRecordFieldType.FilterEditor != undefined) {
//                                $scope.scopeModel.fieldTypeRuntimeDirective = dataRecordFieldType.FilterEditor;

//                                if (previousFieldTypeRuntimeDirective != $scope.scopeModel.fieldTypeRuntimeDirective) {
//                                    fieldTypeRuntimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
//                                }

//                                fieldTypeRuntimeReadyPromiseDeferred.promise.then(function () {
//                                    var setLoader = function (value) { $scope.scopeModel.isFieldTypeRumtimeDirectiveLoading = value; };
//                                    var directivePayload = {
//                                        fieldTitle: "Field Value",
//                                        fieldType: fieldType,
//                                        fieldName: item.Name,
//                                        fieldValue: undefined
//                                    };
//                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, fieldTypeRuntimeDirectiveAPI, directivePayload, setLoader);
//                                });
//                            }

//                        }
//                        else {
//                            $scope.scopeModel.fieldTypeRuntimeDirective = undefined;
//                            fieldTypeRuntimeDirectiveAPI = undefined;
//                        }
//                    }
//                    previousFieldTypeRuntimeDirective = $scope.scopeModel.fieldTypeRuntimeDirective;
//                };

//                $scope.scopeModel.onFieldTypeRumtimeDirectiveReady = function (api) {
//                    fieldTypeRuntimeDirectiveAPI = api;
//                    fieldTypeRuntimeReadyPromiseDeferred.resolve();
//                };

             

//                UtilsService.waitMultiplePromises([periodSelectorReadyPromiseDeferred.promise, reportTypeInfoReadyPromiseDeferred.promise, actionTypeSelectiveReadyDeferred.promise,
//                attachementsSelectorReadyPromiseDeferred.promise, handlerFieldNameSelectorReadyDeferred.promise, handlerSettingsReadyPromiseDeferred.promise]).then(function () {
//                    defineAPI();
//                });
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    console.log(payload);
//                    var initialPromises = [];

//                    if (payload != undefined) {
//                        genericContext = payload.genericContext;
//                        reportTypeEntity = payload.selectedValues;
//                        if (reportTypeEntity != undefined) {
//                            $scope.scopeModel.name = reportTypeEntity.Name;
//                            var settings = reportTypeEntity.Settings;
//                            timePeriod = settings.TimePeriod;
//                            reportTypeId = settings.VRReportTypeId;
//                            reportAction = settings.ReportAction;
//                            reportHandler = settings.ReportHandler;
//                            filterFields = settings.FilterFields;
//                        }
//                    }

//                    initialPromises.push(getDataRecordFieldTypeConfigs());

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];

//                            // if (reportTypeEntity != undefined) {
//                            directivePromises.push(loadPeriodSelector());
//                            directivePromises.push(loadReportTypeInfoSelector());
//                            directivePromises.push(loadActionTypeSelective());
//                            //  directivePromises.push(loadHandlerFieldNameSelector());
//                            directivePromises.push(loadHandlerSelector());
//                            //  }

//                            return {
//                                promises: directivePromises
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
//                        actionTypeSelectionChanged = undefined;
//                        filterFielNameSelectionChanged = undefined;
//                    });
//                };

//                api.setData = function (obj) {
//                    obj.Name = $scope.scopeModel.name;
//                    obj.Settings = {
//                        TimePeriod: periodSelectorAPI.getData(),
//                        VRReportTypeId: reportTypeInfoSelectorAPI.getSelectedIds(),
//                        ReportAction: {
//                            ActionType: actionTypeSelectiveAPI.getData(),
//                            Attachements: $scope.scopeModel.selectedAttachements
//                        },
//                        ReportHandler: {
//                            Fields: [],
//                            Handler: handlerSettingsAPI.getData()
//                        },
//                        FilterFields: [
//                        ]
//                    };
//                };

//                if (ctrl.onReady != null) {
//                    ctrl.onReady(api);
//                }
//            }

//            // Definition Section 
//            function loadPeriodSelector() {
//                var loadPeriodSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
//                periodSelectorReadyPromiseDeferred.promise.then(function () {
//                    var periodSelectorPayload;
//                    if (timePeriod != undefined)
//                        periodSelectorPayload = {
//                            timePeriod: timePeriod
//                        };
//                    VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, periodSelectorPayload, loadPeriodSelectorPromiseDeferred);
//                });
//                return loadPeriodSelectorPromiseDeferred.promise;
//            }

//            function loadReportTypeInfoSelector() {
//                var loadReportTypeInfoSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
//                reportTypeInfoReadyPromiseDeferred.promise.then(function () {
//                    var payloadSelector = {
//                        selectedIds: reportTypeId,
//                    };
//                    VRUIUtilsService.callDirectiveLoad(reportTypeInfoSelectorAPI, payloadSelector, loadReportTypeInfoSelectorPromiseDeferred);
//                });
//                return loadReportTypeInfoSelectorPromiseDeferred.promise;
//            }

//            // Action Section 
//            function loadActionTypeSelective() {
//                var loadActionTypeSelectiveLoadDeferred = UtilsService.createPromiseDeferred();
//                actionTypeSelectiveReadyDeferred.promise.then(function () {
//                    var actionTypeSelectivePayload = {
//                        context: getContext()
//                    };
//                    if (reportAction != undefined) {
//                        actionTypeSelectivePayload.actionType = reportAction.ActionType;
//                    }
//                    VRUIUtilsService.callDirectiveLoad(actionTypeSelectiveAPI, actionTypeSelectivePayload, loadActionTypeSelectiveLoadDeferred);
//                });
//                return loadActionTypeSelectiveLoadDeferred.promise;
//            }

//            // Handler Section 
//            function loadHandlerFieldNameSelector() {
//                var loadHandlerFieldNameSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

//                handlerFieldNameSelectorReadyDeferred.promise.then(function () {
//                    var handlerFieldPayload = {
//                        dataRecordTypeId: dataRecordTypeId
//                    };
//                    if (reportHandler != undefined) {
//                        for (var i = 0; i < reportHandler.Fields.length; i++) {
//                            var fieldName = reportHandler.Fields[i].FieldName;
//                            handlerFieldPayload.selectedIds.push(fieldName);
//                        }
//                    }
//                    VRUIUtilsService.callDirectiveLoad(handlerFieldNameSelectorAPI, handlerFieldPayload, loadHandlerFieldNameSelectorPromiseDeferred);
//                });
//                return loadHandlerFieldNameSelectorPromiseDeferred.promise;
//            }

//            function loadHandlerSelector() {
//                var handlerSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
//                handlerSettingsReadyPromiseDeferred.promise.then(function () {
//                    var handlerPayload = {
//                        settings: reportHandler != undefined ? handler.Handler : undefined,
//                        context: getContext()
//                    };
//                    VRUIUtilsService.callDirectiveLoad(handlerSettingsAPI, handlerPayload, handlerSettingsLoadPromiseDeferred);

//                });
//                return handlerSettingsLoadPromiseDeferred.promise;
//            }

//            //Filter Section
//            function getDataRecordFieldsInfo(dataRecordTypeId) {
//                if (dataRecordTypeId != undefined) {
//                    return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId, null).then(function (response) {
//                        dataRecordTypeFieldsInfo.length = 0;
//                        if (response != undefined)
//                            for (var i = 0; i < response.length; i++) {
//                                var currentField = response[i];
//                                dataRecordTypeFieldsInfo.push(currentField.Entity);
//                            }
//                        console.log(dataRecordTypeFieldsInfo);
//                    });
//                }
//            }

//            function getDataRecordFieldTypeConfigs() {
//                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
//                    for (var i = 0; i < response.length; i++) {
//                        var dataRecordFieldType = response[i];
//                        dataRecordFieldTypes.push(dataRecordFieldType);
//                    }
//                });
//            }

//            function getContext(fields) {
//                var currentContext = context;

//                if (currentContext == undefined) {
//                    currentContext = {};
//                }

//                currentContext.getQueryInfo = function () {
//                    var queries = [];

//                    if (fields == undefined)
//                        fields = currentContext.getFields();

//                    var columns = [];
//                    for (var i = 0; i < fields.length; i++) {
//                        var column = {
//                            ColumnName: fields[i].FieldName,
//                            ColumnTitle: fields[i].FieldTitle
//                        };
//                        columns.push(column);
//                    }

//                    var sortColumns = [];
//                    if (columns.length > 0) {
//                        sortColumns.push({
//                            FieldName: columns[0].ColumnName,
//                            IsDescending: false
//                        });
//                    }

//                    var query = {
//                        $type: "Vanrise.Analytic.Entities.VRAutomatedReportQuery, Vanrise.Analytic.Entities",
//                        DefinitionId: "6cbb0fc3-a0a9-4d1c-bc3a-8557d7692018",
//                        QueryTitle: "Main",
//                        Settings: {
//                            Columns: columns,
//                            DataRecordStorages: [{ DataRecordStorageId: currentContext.getDataRecordStroage() }],
//                            Direction: 0,
//                            SortColumns: sortColumns,
//                        },
//                        VRAutomatedReportQueryId: "98c00dfe-1bba-794f-86e5-7754fb8c353b"
//                    };
//                    queries.push(query);
//                    return queries;
//                };

//                currentContext.getQueryListNames = function () {
//                    var queryListPromiseDeferred = UtilsService.createPromiseDeferred();
//                    queryListPromiseDeferred.resolve(["Main"]);
//                    return queryListPromiseDeferred.promise;
//                };

//                currentContext.getQueryFields = function () {
//                    var queryFieldsPromiseDeferred = UtilsService.createPromiseDeferred();
//                    var value = [];
//                    if (fields == undefined)
//                        fields = currentContext.getFields();
//                    for (var i = 0; i < fields.length; i++) {
//                        var item = {
//                            FieldName: fields[i].FieldName,
//                            FieldTitle: fields[i].FieldTitle,
//                            Source: VR_Analytic_AutomatedReportQuerySourceEnum.MainTable
//                        };
//                        value.push(item);
//                    }
//                    queryFieldsPromiseDeferred.resolve(value);
//                    return queryFieldsPromiseDeferred.promise;
//                };

//                currentContext.getFieldType = function (fieldName) {
//                    for (var i = 0; i < dataRecordTypeFieldsInfo.length; i++) {
//                        var field = dataRecordTypeFieldsInfo[i];
//                        if (field.Name == fieldName)
//                            return field.Type;
//                    }
//                };


//                return currentContext;
//            }
//        }
//    }

//    app.directive('vrAnalyticReporttypeStaticeditor', AnalyticReportTypeStaticEditor);

//})(app);