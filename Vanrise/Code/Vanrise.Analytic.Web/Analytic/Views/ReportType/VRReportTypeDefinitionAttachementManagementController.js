(function (appControllers) {

    "use strict";

    VRReportTypeDefinitionAttachementManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VR_Analytic_AutomatedReportQuerySourceEnum'];

    function VRReportTypeDefinitionAttachementManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VR_Analytic_AutomatedReportQuerySourceEnum) {

        var attachementEntity;
        var context;
        var isEditMode;
        var vrAutomatedReportQueryId;

        var attachementFieldNameAPI;
        var attachementFieldNameReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var attachementFieldNameSelectorChanged = UtilsService.createPromiseDeferred();;

        var automatedReportFileGenratorAPI;
        var automatedReportFileGenratorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                attachementEntity = parameters.attachementEntity;
                context = parameters.context;
            }

            isEditMode = (attachementEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAttachementFieldNameSelectorReady = function (api) {
                attachementFieldNameAPI = api;
                attachementFieldNameReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onAutomatedReportFileGeneratorReady = function (api) {
                automatedReportFileGenratorAPI = api;
                automatedReportFileGenratorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onFieldsNameSelectorChanged = function (items) {
                if (items != undefined) {
                    if (attachementFieldNameSelectorChanged != undefined)
                        attachementFieldNameSelectorChanged.resolve();
                    else {
                        automatedReportFileGenratorReadyPromiseDeferred.promise.then(function () {
                            var fields;
                            if (items.length > 0) {
                                fields = [];
                                for (var i = 0; i < items.length; i++) {
                                    var field = {
                                        FieldName: items[i].Name,
                                        FieldTitle: items[i].Title
                                    };
                                    fields.push(field);
                                }
                            }
                            var payload = {
                                context: getContext(fields),
                            };
                            var setLoader = function (value) {
                                $scope.scopeModel.isFileGeneratorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, automatedReportFileGenratorAPI, payload, setLoader);
                        });
                    }
                }
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAttachementFieldNameSelector, loadAutomatedReportGeneratorDirective]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                attachementFieldNameSelectorChanged = undefined;
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor(attachementEntity.Name, 'Attachement') : UtilsService.buildTitleForAddEditor('Attachement');
        }

        function loadStaticData() {
            if (attachementEntity != undefined) {
                $scope.scopeModel.name = attachementEntity.Name;
            }
        }

        function loadAttachementFieldNameSelector() {
            var attachementFieldNameLoadDeferred = UtilsService.createPromiseDeferred();

            attachementFieldNameReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    dataRecordTypeId: getContext().getDataRecordTypeId()
                };

                if (attachementEntity != undefined) {
                    payload.selectedIds = [];
                    for (var i = 0; i < attachementEntity.Fields.length; i++) {
                        var fieldName = attachementEntity.Fields[i].FieldName;
                        payload.selectedIds.push(fieldName)
                    }
                }
                VRUIUtilsService.callDirectiveLoad(attachementFieldNameAPI, payload, attachementFieldNameLoadDeferred);
            });
            return attachementFieldNameLoadDeferred.promise;
        }

        function loadAutomatedReportGeneratorDirective() {
            var automatedReportGeneratorLoadDeferred = UtilsService.createPromiseDeferred();

            automatedReportFileGenratorReadyPromiseDeferred.promise.then(function () {
                var fields;
                if (attachementEntity != undefined && attachementEntity.Fields.length>0) {
                    var fieldsArray = getContext().getFields();
                    fields = [];
                    for (var i = 0; i < attachementEntity.Fields.length; i++) {
                        var field = attachementEntity.Fields[i];
                        var fieldEntity = UtilsService.getItemByVal(fieldsArray, field.FieldName, "FieldName");
                        fields.push({
                            FieldName: fieldEntity.FieldName,
                            FieldTitle: fieldEntity.FieldTitle
                        });
                    }
                }
              
                var payload = {
                    context: getContext(fields),
                    fileGenerator: attachementEntity != undefined ? attachementEntity.Attachement : undefined
                };
                VRUIUtilsService.callDirectiveLoad(automatedReportFileGenratorAPI, payload, automatedReportGeneratorLoadDeferred);
            });
            return automatedReportGeneratorLoadDeferred.promise;
        }

        function insert() {
            if ($scope.onAttachementAdded != undefined && typeof ($scope.onAttachementAdded) == 'function') {
                $scope.onAttachementAdded(buildAttachementObjFromScope());
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            if ($scope.onAttachementUpdated != undefined && typeof ($scope.onAttachementUpdated) == 'function') {
                $scope.onAttachementUpdated(buildAttachementObjFromScope());
            }
            $scope.modalContext.closeModal();
        }

        function buildAttachementObjFromScope() {
            var attachementObj = {
                VRReportTypeAttachementId: attachementEntity != undefined ? attachementEntity.VRReportTypeAttachementId : UtilsService.guid(),
                Name: $scope.scopeModel.name,
                Fields: getFieldsArray(),
                Attachement: automatedReportFileGenratorAPI.getData()
            };
            return attachementObj;
        }

        function getFieldsArray() {
            var fields = [];
            var selectedIds = attachementFieldNameAPI.getSelectedIds();
            if (selectedIds != undefined) {
                for (var i = 0; i < selectedIds.length; i++) {
                    fields.push({ FieldName: selectedIds[i] });
                }
            }
            return fields;
        }

        function getContext(fields) {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};

            currentContext.getQueryInfo = function () {
                var queries = [];

                if (fields == undefined)
                    fields = currentContext.getFields();

                var columns = [];
                for (var i = 0; i < fields.length; i++) {
                    var column = {
                        ColumnName: fields[i].FieldName,
                        ColumnTitle: fields[i].FieldTitle
                    };
                    columns.push(column);
                }

                var sortColumns = [];
                if (columns.length > 0) {
                    sortColumns.push({
                        FieldName: columns[0].ColumnName,
                        IsDescending: false
                    });
                }

                var query = {
                    $type: "Vanrise.Analytic.Entities.VRAutomatedReportQuery, Vanrise.Analytic.Entities",
                    DefinitionId: "6cbb0fc3-a0a9-4d1c-bc3a-8557d7692018",
                    QueryTitle: "Main",
                    Settings: {
                        Columns: columns,
                        DataRecordStorages: [{ DataRecordStorageId: currentContext.getDataRecordStroage() }],
                        Direction: 0,
                        SortColumns: sortColumns,
                    },
                    VRAutomatedReportQueryId: "98c00dfe-1bba-794f-86e5-7754fb8c353b"
                };
                queries.push(query);
                return queries;
            };

            currentContext.getQueryListNames = function () {
                var queryListPromiseDeferred = UtilsService.createPromiseDeferred();
                queryListPromiseDeferred.resolve(["Main"]);
                return queryListPromiseDeferred.promise;
            };

            currentContext.getQueryFields = function () {
                var queryFieldsPromiseDeferred = UtilsService.createPromiseDeferred();
                var value = [];
                if (fields == undefined)
                    fields = currentContext.getFields();
                for (var i = 0; i < fields.length; i++) {
                    var item = {
                        FieldName: fields[i].FieldName,
                        FieldTitle: fields[i].FieldTitle,
                        Source: VR_Analytic_AutomatedReportQuerySourceEnum.MainTable
                    };
                    value.push(item);
                }
                queryFieldsPromiseDeferred.resolve(value);
                return queryFieldsPromiseDeferred.promise;
            };
            return currentContext;
        }
    }

    appControllers.controller('VR_Analytic_VRReportTypeDefinitionAttachementManagementController', VRReportTypeDefinitionAttachementManagementController);

})(appControllers);