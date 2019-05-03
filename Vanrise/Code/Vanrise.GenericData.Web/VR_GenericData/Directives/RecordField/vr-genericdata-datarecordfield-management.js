"use strict";

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

            var dataRecordTypeExtraFieldsApi;
            var dataRecordTypeExtraFieldsReadyDeferred;

            function initializeController() {
                ctrl.datasource = [];
                ctrl.fieldTypeConfigs = [];

                ctrl.isValid = function () {
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

                ctrl.addDataRecordField = function () {

                    var onDataRecordFieldAdded = function (dataRecordField) {
                        addNeededFields(dataRecordField);
                        ctrl.datasource.push(dataRecordField);
                    };

                    var allFields = [];

                    for (var j = 0; j < ctrl.datasource.length; j++) {
                        allFields.push(ctrl.datasource[j]);
                    }

                    getDataRecordExtraFieldsReturnedPromise(allFields).then(function () {
                        VR_GenericData_DataRecordFieldService.addDataRecordField(onDataRecordFieldAdded, allFields, true);
                    });
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var fields;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        fields = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            fields.push({
                                Name: currentItem.Name,
                                Type: currentItem.Type,
                                Title: currentItem.Title,
                                Formula: currentItem.Formula
                            });
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
                    var promises = [];
                    var settings;
                    if (payload != undefined) {
                        settings = payload.Settings;

                        ctrl.hasExtraFields = payload.ExtraFieldsEvaluator != undefined;
                        if (ctrl.hasExtraFields) {
                            dataRecordTypeExtraFieldsReadyDeferred = UtilsService.createPromiseDeferred();
                            var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                            dataRecordTypeExtraFieldsReadyDeferred.promise.then(function () {

                                dataRecordTypeExtraFieldsReadyDeferred = undefined;
                                var dataRecordTypeExtraFieldsPayload = payload.ExtraFieldsEvaluator;
                                VRUIUtilsService.callDirectiveLoad(dataRecordTypeExtraFieldsApi, dataRecordTypeExtraFieldsPayload, directiveLoadDeferred);
                            });

                            promises.push(directiveLoadDeferred.promise);
                        }
                    }

                    if (settings != undefined) {
                        ctrl.dateTimeField = settings.DateTimeField;
                        ctrl.idField = settings.IdField;
                    }

                    var dataRecorddFieldTypePromise = VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                        angular.forEach(response, function (item) {
                            ctrl.fieldTypeConfigs.push(item);
                        });
                        if (payload != undefined) {
                            if (payload.Fields && payload.Fields.length > 0) {
                                for (var i = 0; i < payload.Fields.length; i++) {
                                    var dataItem = payload.Fields[i];
                                    addNeededFields(dataItem);
                                    ctrl.datasource.push(dataItem);
                                }
                            }
                        }
                    });
                    promises.push(dataRecorddFieldTypePromise);
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function addNeededFields(dataItem) {
                var template = UtilsService.getItemByVal(ctrl.fieldTypeConfigs, dataItem.Type.ConfigId, "ExtensionConfigurationId");
                dataItem.TypeDescription = template != undefined ? template.Name : "";
            }

            function defineMenuActions() {
                var defaultMenuActions = [{
                    name: "Edit",
                    clicked: editDataRecordField
                },
                {
                    name: "Delete",
                    clicked: deleteDataRecordField
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editDataRecordField(dataRecordFieldObj) {
                var validationMessage = ctrl.validationcontext.validate();
                if (validationMessage != null) {
                    VRNotificationService.showWarning(validationMessage);
                    return;
                }

                var onDataRecordFieldUpdated = function (dataRecordField) {
                    addNeededFields(dataRecordField);
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataRecordFieldObj.Name, 'Name');
                    ctrl.datasource[index] = dataRecordField;
                };

                var allFields = [];

                for (var j = 0; j < ctrl.datasource.length; j++) {
                    allFields.push(ctrl.datasource[j]);
                }

                getDataRecordExtraFieldsReturnedPromise(allFields).then(function () {
                    VR_GenericData_DataRecordFieldService.editDataRecordField(onDataRecordFieldUpdated, dataRecordFieldObj, allFields, true);
                });
            }

            function deleteDataRecordField(dataRecordFieldObj) {
                var onDataRecordFieldDeleted = function (dataRecordField) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataRecordFieldObj.Name, 'Name');
                    ctrl.datasource.splice(index, 1);
                };

                VR_GenericData_DataRecordFieldService.deleteDataRecordField($scope, dataRecordFieldObj, onDataRecordFieldDeleted);
            }

            function getDataRecordExtraFieldsReturnedPromise(allFields) {
                var addDataRecordFieldReadyDeferred = UtilsService.createPromiseDeferred();

                var extraFieldsEvaluator = dataRecordTypeExtraFieldsApi != undefined ? dataRecordTypeExtraFieldsApi.getData() : undefined;

                if (ctrl.hasExtraFields && extraFieldsEvaluator != undefined) {
                    VR_GenericData_DataRecordTypeAPIService.GetDataRecordExtraFields(extraFieldsEvaluator).then(function (response) {
                        if (response && response.length > 0) {
                            for (var i = 0; i < response.length; i++) {
                                allFields.push(response[i]);
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
        }

        return directiveDefinitionObject;

    }
]);