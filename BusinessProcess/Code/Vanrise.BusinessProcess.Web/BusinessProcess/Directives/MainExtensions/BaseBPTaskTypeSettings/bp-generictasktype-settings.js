(function (app) {

    'use strict';

    GenericTaskTypeSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService', 'BusinessProcess_BPTaskTypeService'];

    function GenericTaskTypeSettings(UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService, BusinessProcess_BPTaskTypeService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/MainExtensions/BaseBPTaskTypeSettings/Templates/BPGenericTaskTypeSettingsTemplate.html"

        };
        function SettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;

            var dataRecordTypeFields = [];

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeSelectedPromiseDeferred;

            var editorDefinitionAPI;
            var editorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;
            var gridPromiseReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.taskTypeActions = [];

                $scope.scopeModel.onDataRecordTypeSelectorDirectiveReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBEEditorDefinitionDirectiveReady = function (api) {
                    editorDefinitionAPI = api;
                    editorDefinitionReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onRecordTypeSelectionChanged = function () {
                    var selectedRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                    if (selectedRecordTypeId != undefined) {
                        if (dataRecordTypeSelectedPromiseDeferred != undefined) {
                            dataRecordTypeSelectedPromiseDeferred.resolve();
                        }
                        else {
                            getDataRecordFieldsInfo(selectedRecordTypeId).then(function () {
                                var editorDefinitionPayload = {
                                    context: getContext()
                                };
                                var setLoader = function (value) {
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, editorDefinitionAPI, editorDefinitionPayload, setLoader);
                            });
                        }
                    }
                };


                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridPromiseReadyDeferred.resolve();
                };

                $scope.scopeModel.onAddTaskTypeAction = function () {
                    var onTaskTypeActionAdded = function (taskTypeAction) {
                        if (taskTypeAction != undefined) {
                            $scope.scopeModel.taskTypeActions.push(taskTypeAction);
                        }
                    };
                    BusinessProcess_BPTaskTypeService.addTaskTypeAction(onTaskTypeActionAdded);
                };

                $scope.scopeModel.removeTaskTypeAction = function (dataItem) {
                    var index = $scope.scopeModel.taskTypeActions.indexOf(dataItem);
                    $scope.scopeModel.taskTypeActions.splice(index, 1);
                };

                $scope.scopeModel.validateColumns = function () {
                    if ($scope.scopeModel.taskTypeActions.length == 0) {
                        return 'Please, one record must be added at least.';
                    }

                    var columnNames = [];
                    for (var i = 0; i < $scope.scopeModel.taskTypeActions.length; i++) {
                        var column = $scope.scopeModel.taskTypeActions[i];
                        if (column.Name != undefined) {
                            columnNames.push(column.Name.toUpperCase());
                        }
                    }

                    while (columnNames.length > 0) {
                        var nameToValidate = columnNames[0];
                        columnNames.splice(0, 1);
                        if (!validateName(nameToValidate, columnNames)) {
                            return 'Two or more columns have the same name';
                        }
                    }

                    return null;

                    function validateName(name, array) {
                        for (var j = 0; j < array.length; j++) {
                            var arrayElement = array[j];
                            if (arrayElement == name)
                                return false;
                        }
                        return true;
                    }
                };

                defineAPI();
                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        dataRecordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                        initialPromises.push(getDataRecordFieldsInfo(payload.RecordTypeId));

                        var taskTypeActions = payload.TaskTypeActions;
                        for (var i = 0; i < taskTypeActions.length; i++) {
                            var taskTypeAction = taskTypeActions[i];
                            $scope.scopeModel.taskTypeActions.push(taskTypeAction);
                        }
                    }

                    function loadDataRecordTypeSelector() {
                        var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        dataRecordTypeSelectorReadyPromiseDeferred.promise.then(function () {
                            var dataRecordTypeSelectorPayload = {
                                selectedIds: payload != undefined ? payload.RecordTypeId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypeSelectorPayload, dataRecordTypeSelectorLoadDeferred);
                        });
                        return dataRecordTypeSelectorLoadDeferred.promise;
                    }

                    function loadEditorDefinitionDirective() {
                        var loadEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        var editorPromises = [editorDefinitionReadyPromiseDeferred.promise];
                        if (payload != undefined) {
                            editorPromises.push(dataRecordTypeSelectedPromiseDeferred.promise);
                        }
                        UtilsService.waitMultiplePromises(editorPromises).then(function () {
                            dataRecordTypeSelectedPromiseDeferred = undefined;
                            var editorPayload = {
                                settings: payload != undefined ? payload.EditorSettings : undefined,
                                context: getContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(editorDefinitionAPI, editorPayload, loadEditorDefinitionDirectivePromiseDeferred);
                        });
                        return loadEditorDefinitionDirectivePromiseDeferred.promise;
                    }


                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];
                            directivePromises.push(loadDataRecordTypeSelector());
                            directivePromises.push(loadEditorDefinitionDirective());

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.BusinessProcess.MainExtensions.BPTaskTypes.BPGenericTaskTypeSettings, Vanrise.BusinessProcess.MainExtensions",
                        RecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        EditorSettings: editorDefinitionAPI.getData(),
                        TaskTypeActions: ($scope.scopeModel.taskTypeActions.length > 0) ? $scope.scopeModel.taskTypeActions : undefined
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function getContext() {
                return {
                    getDataRecordTypeId: function () {
                        return dataRecordTypeSelectorAPI.getSelectedIds();
                    },
                    getRecordTypeFields: function () {
                        var data = [];
                        for (var i = 0; i < dataRecordTypeFields.length; i++) {
                            data.push(dataRecordTypeFields[i]);
                        }
                        return data;
                    },
                    getFields: function () {
                        var dataFields = [];
                        for (var i = 0; i < dataRecordTypeFields.length; i++) {
                            dataFields.push({
                                FieldName: dataRecordTypeFields[i].Name,
                                FieldTitle: dataRecordTypeFields[i].Title,
                                Type: dataRecordTypeFields[i].Type
                            });
                        }
                        return dataFields;
                    },
                    getActionInfos: function () {
                        var data = [];
                        if (actionDefinitionGridAPI == undefined)
                            return data;
                        var actionDefinitions = actionDefinitionGridAPI.getData();
                        for (var i = 0; i < actionDefinitions.length; i++) {
                            data.push({
                                GenericBEActionId: actionDefinitions[i].GenericBEActionId,
                                Name: actionDefinitions[i].Name
                            });
                        }
                        return data;
                    }
                };
            }

            function getDataRecordFieldsInfo(recordTypeId) {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(recordTypeId, null).then(function (response) {
                    dataRecordTypeFields.length = 0;
                    if (response != undefined)
                        for (var i = 0; i < response.length; i++) {
                            var currentField = response[i];
                            dataRecordTypeFields.push(currentField.Entity);
                        }
                });
            }

            function defineMenuActions() {
                $scope.gridMenuActions = function () {
                    var menuActions = [{
                        name: "Edit",
                        clicked: editTasktypeAction
                    }];
                    return menuActions;
                };
            }

            function editTasktypeAction(taskTypeActionObj) {
                var onBPTaskTypeActionUpdated = function (taskTypeAction) {
                    var index = $scope.scopeModel.taskTypeActions.indexOf(taskTypeActionObj);
                    $scope.scopeModel.taskTypeActions[index] = taskTypeAction;
                };

                BusinessProcess_BPTaskTypeService.editTaskTypeAction(taskTypeActionObj, onBPTaskTypeActionUpdated);
            }

        }
    }

    app.directive('bpGenerictasktypeSettings', GenericTaskTypeSettings);

})(app);
