(function (app) {

    'use strict';

    GenericTaskTypeSettings.$inject = ['UtilsService', 'VRUIUtilsService','VR_GenericData_DataRecordFieldAPIService'];

    function GenericTaskTypeSettings(UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {
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

            function initializeController() {
                $scope.scopeModel = {};

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

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        dataRecordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(getDataRecordFieldsInfo(payload.RecordTypeId));
                    }

                    promises.push(loadDataRecordTypeSelector());
                    promises.push(loadEditorDefinitionDirective());

                    function loadDataRecordTypeSelector() {
                        var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        dataRecordTypeSelectorReadyPromiseDeferred.promise.then(function () {
                            var dataRecordTypeSelectorPayload = {
                                selectedIds: payload != undefined ? payload.RecordTypeId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypeSelectorPayload, dataRecordTypeSelectorLoadDeferred );
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
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.BusinessProcess.MainExtensions.BPTaskTypes.BPGenericTaskTypeSettings, Vanrise.BusinessProcess.MainExtensions",
                        RecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        EditorSettings: editorDefinitionAPI.getData()
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
        }
    }

    app.directive('bpGenerictasktypeSettings', GenericTaskTypeSettings);

})(app);
