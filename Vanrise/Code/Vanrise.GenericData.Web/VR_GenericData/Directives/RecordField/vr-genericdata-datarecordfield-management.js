﻿"use strict";

app.directive("vrGenericdataDatarecordfieldManagement", ["UtilsService", "VRNotificationService", "VR_GenericData_DataRecordFieldService", "VR_GenericData_DataRecordFieldAPIService", "VRUIUtilsService", "VR_GenericData_DataRecordTypeAPIService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataRecordFieldService, VR_GenericData_DataRecordFieldAPIService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "=",
                validationcontext: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordFieldManagement = new DataRecordFieldManagement($scope, ctrl, $attrs);
                dataRecordFieldManagement.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordField/Templates/DataRecordFieldManagement.html"
        };

        function DataRecordFieldManagement($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridAPIdeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeExtraFieldsApi;
            var dataRecordTypeExtraFieldsReadyDeferred;

            var existingFields = [];
            var extraFields = [];
            var allRegisteredItems = [];
            function initializeController() {

                ctrl.datasource = [];

                ctrl.fieldTypeConfigs = [];

                ctrl.isValid = function () {
                    if (checkDuplicateInArray(ctrl.datasource))
                        return "Same name exist";
                    if (ctrl.hasExtraFields || (ctrl.datasource != undefined && ctrl.datasource.length > 0))
                        return null;
                    return "You Should Add at least one field";
                };

                ctrl.onDataRecordTypeExtraFieldsReady = function (api) {
                    dataRecordTypeExtraFieldsApi = api;
                    var setLoader = function (value) {
                        ctrl.isDirectiveLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordTypeExtraFieldsApi, undefined, setLoader, dataRecordTypeExtraFieldsReadyDeferred);
                };

                ctrl.disableAddButton = function () {
                    if (ctrl.isValid() != null)
                        return false;

                    return ctrl.validationcontext.validate() != null;
                };

                function checkDuplicateInArray(array) {
                    if (array != undefined) {
                        for (var i = 0; i < array.length; i++) {
                            var item = array[i];
                            for (var j = i + 1; j < array.length; j++) {
                                var currentItem = array[j];
                                if (item.Name == currentItem.Name) {
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                }

                ctrl.removeFilter = function (item) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, item.rowIndex, 'rowIndex');
                    if (index > -1) {
                        ctrl.datasource.splice(index, 1);
                        var registerIndex = allRegisteredItems.indexOf(item);
                        if (registerIndex > -1)
                            allRegisteredItems.splice(registerIndex, 1);
                        if (allRegisteredItems != undefined) {
                            for (var i = 0; i < allRegisteredItems.length; i++) {
                                var elemenToTrigger = allRegisteredItems[i];
                                elemenToTrigger();
                            }
                        }
                    }
                };

                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridAPIdeferred.resolve();
                };

                ctrl.addDataRecordField = function () {
                    getDataRecordExtraFieldsReturnedPromise().then(function () {

                        var gridItem = {
                            Name: undefined,
                            Title: undefined,
                            Type: undefined,
                            fieldTypeSelectorAPI: undefined,
                            fieldTypeSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
                            dataRecordTypeFieldsFormulaAPI: undefined,
                            dataRecordTypeFieldsFormulaReadyDeferred: UtilsService.createPromiseDeferred()
                        };

                        gridItem.onFieldTypeSelectorReady = function (api) {
                            gridItem.fieldTypeSelectorAPI = api;
                            gridItem.fieldTypeSelectorReadyDeferred.resolve();
                        };

                        gridItem.onDataRecordTypeFieldsFormulaReady = function (api) {
                            gridItem.dataRecordTypeFieldsFormulaAPI = api;
                            gridItem.dataRecordTypeFieldsFormulaReadyDeferred.resolve();
                        };


                        gridItem.onFieldTileChanged = function (item) {
                            if (allRegisteredItems != undefined) {
                                for (var i = 0; i < allRegisteredItems.length; i++) {
                                    var elemenToTrigger = allRegisteredItems[i];
                                    elemenToTrigger();
                                }
                            }
                        };

                        gridItem.onFieldNameChanged = function (item) {
                            if (allRegisteredItems != undefined) {
                                for (var i = 0; i < allRegisteredItems.length; i++) {
                                    var elemenToTrigger = allRegisteredItems[i];
                                    elemenToTrigger();
                                }
                            }
                        };

                        gridItem.fieldTypeSelectorReadyDeferred.promise.then(function () {
                            var dataRecordFieldTypePayload = {};
                            dataRecordFieldTypePayload.additionalParameters = { showDependantFieldsGrid: true, context: getContext() };
                            var setLoader = function (value) { gridItem.isFieldTypeSelectorLoading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.fieldTypeSelectorAPI, dataRecordFieldTypePayload, setLoader);
                        });
                        gridItem.dataRecordTypeFieldsFormulaReadyDeferred.promise.then(function () {
                            var dataRecordTypeFieldsFormulaPayload = { formula: undefined, context: getContext() };
                            var setLoader = function (value) {
                                gridItem.isDataRecordTypeFormulaLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.dataRecordTypeFieldsFormulaAPI, dataRecordTypeFieldsFormulaPayload, setLoader, undefined);
                        });



                        gridAPI.expandRow(gridItem);

                        ctrl.datasource.push(gridItem);

                    });
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var fields;
                    if (ctrl.datasource != undefined) {
                        fields = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            if (currentItem != undefined) {
                                fields.push({
                                    Name: currentItem.Name,
                                    Type: (currentItem.fieldTypeSelectorAPI != undefined) ? currentItem.fieldTypeSelectorAPI.getData() : currentItem.Type,
                                    Title: currentItem.Title,
                                    Formula: currentItem.dataRecordTypeFieldsFormulaAPI != undefined ? currentItem.dataRecordTypeFieldsFormulaAPI.getData() : currentItem.Formula,
                                });
                            }
                        }
                    }

                    var obj = {
                        Settings: {
                            DateTimeField: ctrl.dateTimeField,
                            IdField: ctrl.idField
                        },
                        Fields: fields,
                        HasExtraFields: ctrl.hasExtraFields,
                        ExtraFieldsEvaluator: ctrl.hasExtraFields ? dataRecordTypeExtraFieldsApi.getData() : undefined
                    };
                    return obj;
                };

                api.load = function (payload) {
                    var firstLayerPromises = [];
                    var settings;
                    if (payload != undefined) {
                        settings = payload.Settings;
                        ctrl.hasExtraFields = payload.ExtraFieldsEvaluator != undefined;

                        if (ctrl.hasExtraFields) {
                            firstLayerPromises.push(loadExtraFields(payload));
                        }

                    }

                    if (settings != undefined) {
                        ctrl.dateTimeField = settings.DateTimeField;
                        ctrl.idField = settings.IdField;
                    }

                    firstLayerPromises.push(loadDataRecordFieldConfigs());
                    var rootPromiseNode = {
                        promises: firstLayerPromises,
                        getChildNode: function () {
                            return {
                                promises: [getDataRecordExtraFieldsReturnedPromise()],
                                getChildNode: function () {
                                    return {
                                        promises: loadDataRecordFieldsGrid(payload)
                                    };
                                }
                            };
                        }
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function addColumnOnEdit(gridItem) {

                gridItem.onFieldTypeSelectorReady = function (api) {
                    gridItem.fieldTypeSelectorAPI = api;
                    gridItem.fieldTypeSelectorReadyDeferred.resolve();
                    var dataRecordFieldTypePayload = gridItem.Type;
                    dataRecordFieldTypePayload.additionalParameters = { showDependantFieldsGrid: true, context: getContext() };
                    var setLoader = function (value) {
                        gridItem.isFieldTypeSelectorLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.fieldTypeSelectorAPI, dataRecordFieldTypePayload, setLoader, undefined);
                };

                gridItem.onFieldTileChanged = function (item) {
                    if (allRegisteredItems != undefined) {
                        for (var i = 0; i < allRegisteredItems.length; i++) {
                            var elemenToTrigger = allRegisteredItems[i];
                            elemenToTrigger();
                        };
                    }
                };

                gridItem.onFieldNameChanged = function (item) {
                    if (allRegisteredItems != undefined) {
                        for (var i = 0; i < allRegisteredItems.length; i++) {
                            var elemenToTrigger = allRegisteredItems[i];
                            elemenToTrigger();
                        }
                    }
                };

                gridItem.onDataRecordTypeFieldsFormulaReady = function (api) {
                    gridItem.dataRecordTypeFieldsFormulaAPI = api;
                    gridItem.dataRecordTypeFieldsFormulaReadyDeferred.resolve();
                };
                gridItem.dataRecordTypeFieldsFormulaReadyDeferred.promise.then(function () {
                    var dataRecordTypeFieldsFormulaPayload = { formula: gridItem.Formula, context: getContext() };
                    var setLoader = function (value) {
                        gridItem.isDataRecordTypeFormulaLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.dataRecordTypeFieldsFormulaAPI, dataRecordTypeFieldsFormulaPayload, setLoader, undefined);
                });
            }

            function addNeededFields(dataItem) {
                var template = UtilsService.getItemByVal(ctrl.fieldTypeConfigs, dataItem.Type.ConfigId, "ExtensionConfigurationId");
                dataItem.TypeDescription = template != undefined ? template.Name : "";
            }


            function getDataRecordExtraFieldsReturnedPromise() {
                var addDataRecordFieldReadyDeferred = UtilsService.createPromiseDeferred();
                var extraFieldsEvaluator = (dataRecordTypeExtraFieldsApi != undefined && ctrl.hasExtraFields) ? dataRecordTypeExtraFieldsApi.getData() : undefined;
                if (ctrl.hasExtraFields && extraFieldsEvaluator != undefined) {
                    VR_GenericData_DataRecordTypeAPIService.GetDataRecordExtraFields(extraFieldsEvaluator).then(function (response) {
                        if (response && response.length > 0) {
                            for (var i = 0; i < response.length; i++) {
                                extraFields.push(response[i]);
                            }
                        }
                        addDataRecordFieldReadyDeferred.resolve();
                    });
                }
                else {
                    addDataRecordFieldReadyDeferred.resolve();
                }

                return addDataRecordFieldReadyDeferred.promise;
            }

            function getContext() {
                var context = {
                    getFields: function () {
                        existingFields = [];
                        for (var j = 0; j < ctrl.datasource.length; j++) {
                            existingFields.push(ctrl.datasource[j]);
                        }
                        for (var j = 0; j < extraFields.length; j++) {
                            existingFields.push(extraFields[j]);
                        }
                        var fields = [];
                        if (existingFields != undefined) {
                            for (var i = 0; i < existingFields.length; i++) {
                                var existingField = existingFields[i];
                                fields.push({ fieldName: existingField.Name, fieldTitle: existingField.Title, fieldType: existingField.Type == undefined && existingField.fieldTypeSelectorAPI != undefined ? existingField.fieldTypeSelectorAPI.getData() : existingField.Type });
                            }
                        }
                        return fields;
                    },
                    subscribeToFieldChangeEvent: subscribeToFieldChangeEvent,
                    unSubscribeToFieldChangeEvent: function (item) {
                        var index = allRegisteredItems.indexOf(item);
                        if (index > -1)
                            allRegisteredItems.splice(index, 1);
                        if (allRegisteredItems != undefined) {
                            for (var i = 0; i < allRegisteredItems.length; i++) {
                                var elemenToTrigger = allRegisteredItems[i];
                                elemenToTrigger();
                            }
                        }
                    }
                };
                return context;
            }

            function subscribeToFieldChangeEvent(subscribeEvent) {
                if (subscribeEvent != undefined && typeof (subscribeEvent) == 'function') {
                    allRegisteredItems.push(subscribeEvent);
                }
            }

            function loadExtraFields(payload) {
                dataRecordTypeExtraFieldsReadyDeferred = UtilsService.createPromiseDeferred();
                var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordTypeExtraFieldsReadyDeferred.promise.then(function () {
                    dataRecordTypeExtraFieldsReadyDeferred = undefined;
                    var dataRecordTypeExtraFieldsPayload = payload.ExtraFieldsEvaluator;
                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeExtraFieldsApi, dataRecordTypeExtraFieldsPayload, directiveLoadDeferred);
                });
                return directiveLoadDeferred.promise;
            }

            function loadDataRecordFieldConfigs() {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.fieldTypeConfigs.push(item);
                    });
                });
            }

            function loadDataRecordFieldsGrid(payload) {
                var promises = [];
                if (payload != undefined && payload.Fields != undefined) {
                    for (var i = 0; i < payload.Fields.length; i++) {
                        var field = payload.Fields[i];
                        if (field != undefined) {
                            var gridItem = {
                                Name: field.Name,
                                Type: field.Type,
                                Title: field.Title,
                                Formula: field.Formula,
                                fieldTypeSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
                                dataRecordTypeFieldsFormulaReadyDeferred: UtilsService.createPromiseDeferred(),
                            };
                            addColumnOnEdit(gridItem);
                            addNeededFields(gridItem);
                            ctrl.datasource.push(gridItem);
                        }
                    }
                    //gridAPIdeferred.promise.then(function () {
                    //    for (var i = 0; i < ctrl.datasource.length; i++) {
                    //        gridAPI.expandRow(ctrl.datasource[i]);
                    //    }
                    //});
                }
                return promises;
            }
        }
        return directiveDefinitionObject;
    }
]);