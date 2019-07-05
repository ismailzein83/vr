'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistGridgenericeditorviewDefinition', ['VRUIUtilsService', 'UtilsService', 'VR_GenericData_DataRecordTypeService','VR_GenericData_DataRecordFieldAPIService',
    function (VRUIUtilsService, UtilsService, VR_GenericData_DataRecordTypeService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new gridGenericEditorViewTypeListTypeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeDefinition/Templates/GridGenericEditorViewTypeDefinitionTemplate.html';
            }
        };

        function gridGenericEditorViewTypeListTypeCtor(ctrl, $scope) {

            $scope.scopeModel = {};
            var editorDefinitionAPI;
            var editorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var drillDownSettings;
            var dataRecordTypeId;
            var dataRecordTypeFields = [];
            var context;
            var gridAPI;
            function initializeController() {
                ctrl.datasource = [];
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };
                $scope.scopeModel.onGenericBEEditorDefinitionDirectiveReady = function (api) {
                    editorDefinitionAPI = api;
                    editorDefinitionReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDataRowAdded = function () {
                    var onRowAdded = function (row) {
                        ctrl.datasource.push({ Entity: row });
                    };
                    VR_GenericData_DataRecordTypeService.addListDataRecordTypeGridGenericEditorViewRow(getContext(), onRowAdded);
                };

                $scope.scopeModel.onDeleteRow = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                $scope.scopeModel.onEnableDrillDownValueChanged = function () {
                    $scope.scopeModel.autoExpand = false;
                    if ($scope.scopeModel.enableDrillDown) {

                        editorDefinitionReadyPromiseDeferred.promise.then(function () {
                            var setDrillDownGenericEditorLoader = function (value) {
                                $scope.scopeModel.isLoadingDrillDownGenericEditorDirective = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, editorDefinitionAPI, { context: getContext(), }, setDrillDownGenericEditorLoader);
                        });
                    }
                    else {
                        editorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                        editorDefinitionAPI = undefined;
                    }
                };
                defineMenuActions();
                defineAPI();

            }
            function loadEditorDefinitionDirective() {
                var loadEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                editorDefinitionReadyPromiseDeferred.promise.then(function () {
                    var editorPayload = {
                        settings: drillDownSettings != undefined ? drillDownSettings.DrillDownSettings : undefined,
                        context: getContext()
                    };
                    VRUIUtilsService.callDirectiveLoad(editorDefinitionAPI, editorPayload, loadEditorDefinitionDirectivePromiseDeferred);
                });
                return loadEditorDefinitionDirectivePromiseDeferred.promise;
            }
            function getDataRecordFieldsInfo() {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                    dataRecordTypeFields.length = 0;
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            var currentField = response[i];
                            dataRecordTypeFields.push(currentField.Entity);
                        }
                    }
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        var settings = payload.settings;
                        dataRecordTypeId = payload.dataRecordTypeId;
                        var fields;

                        if (settings != undefined) {
                            $scope.scopeModel.hideAddButton = settings.HideAddButton;
                            $scope.scopeModel.hideSection = settings.HideSection;
                            $scope.scopeModel.enableDraggableRow = settings.EnableDraggableRow;
                            $scope.scopeModel.hideRemoveIcon = settings.HideRemoveIcon;
                            drillDownSettings = settings.DrillDownSettings;
                            fields = settings.Fields;
                        }
                        if (drillDownSettings != undefined) {
                            if (drillDownSettings.EnableDrillDown) {
                                $scope.scopeModel.autoExpand = drillDownSettings.AutoExpand;
                            }
                            $scope.scopeModel.enableDrillDown = drillDownSettings.EnableDrillDown;
                        }

                        if (fields != undefined && fields.length > 0) {
                            for (var i = 0; i < fields.length; i++) {
                                ctrl.datasource.push({ Entity: fields[i] });
                            }
                        }
                        promises.push(getDataRecordFieldsInfo());

                    }
                    return UtilsService.waitPromiseNode({
                        promises: promises,
                        getChildNode: function () {
                            var childPromises = [];
                            if ($scope.scopeModel.enableDrillDown) {
                                childPromises.push(loadEditorDefinitionDirective());
                            }
                            return { promises: childPromises };
                        }
                    });
                };

                api.getData = function () {
                    var fields = [];
                    if (ctrl.datasource.length > 0) {
                        for (var j = 0; j < ctrl.datasource.length; j++) {
                            fields.push(ctrl.datasource[j].Entity);
                        }
                    }
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.GridGenericEditorViewListRecordRuntimeViewType, Vanrise.GenericData.MainExtensions",
                        HideAddButton: $scope.scopeModel.hideAddButton,
                        HideSection: $scope.scopeModel.hideSection,
                        EnableDraggableRow: $scope.scopeModel.enableDraggableRow,
                        HideRemoveIcon: $scope.scopeModel.hideRemoveIcon,
                        Fields: fields,
                        DrillDownSettings: {
                            EnableDrillDown: $scope.scopeModel.enableDrillDown,
                            AutoExpand: $scope.scopeModel.autoExpand,
                            DrillDownSettings: editorDefinitionAPI.getData()
                        }
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined) {
                    currentContext = {};
                    currentContext.getRecordTypeFields = function () {
                        var data = [];
                        for (var i = 0; i < dataRecordTypeFields.length; i++) {
                            data.push(dataRecordTypeFields[i]);
                        }
                        return data;
                    };
                    currentContext.getDataRecordTypeId = function () {
                        return dataRecordTypeId;
                    };
                    currentContext.getFieldType = function (fieldName) {
                        for (var i = 0; i < dataRecordTypeFields.length; i++) {
                            var field = dataRecordTypeFields[i];
                            if (field.Name == fieldName)
                                return field.Type;
                        }
                    };
                    currentContext.getFields = function () {
                        var dataFields = [];
                        for (var i = 0; i < dataRecordTypeFields.length; i++) {
                            dataFields.push({
                                FieldName: dataRecordTypeFields[i].Name,
                                FieldTitle: dataRecordTypeFields[i].Title,
                                Type: dataRecordTypeFields[i].Type
                            });
                        }
                        return dataFields;
                    };
                    currentContext.getFilteredFields = function () {
                        var data = [];
                        var filterData = currentContext.getRecordTypeFields();
                        for (var i = 0; i < filterData.length; i++) {
                            var fieldData = filterData[i];
                            data.push({ FieldPath: fieldData.Name, FieldTitle: fieldData.Title });
                        }
                        return data;
                    }
                } return currentContext;
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                    {
                        name: "Edit",
                        clicked: editRow,
                    }];

                $scope.gridMenuActions = function () {
                    return defaultMenuActions;
                };
            }

            function editRow(row) {

                var onRowUpdated = function (updatedRow) {
                    var index = ctrl.datasource.indexOf(row);
                    ctrl.datasource[index] = { Entity: updatedRow };
                };
                VR_GenericData_DataRecordTypeService.editListDataRecordTypeGridGenericEditorViewRow(row.Entity, getContext(), onRowUpdated);
            }
            this.initializeController = initializeController;
        }
    }]);