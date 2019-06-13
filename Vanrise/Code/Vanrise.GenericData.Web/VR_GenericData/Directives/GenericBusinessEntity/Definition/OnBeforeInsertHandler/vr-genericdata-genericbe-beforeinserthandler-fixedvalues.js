"use strict";

app.directive("vrGenericdataGenericbeBeforeinserthandlerFixedvalues", ["UtilsService", "VRNotificationService","VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new FixedValuesHandler($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/OnBeforeInsertHandler/Templates/FixedValuesHandlerEditor.html"
        };

        function FixedValuesHandler($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var context;
            var selectedField;
            var fields;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.selectedFields = [];
                ctrl.datasource = [];

                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.disableAddButton = function () {
                    selectedField = dataRecordTypeFieldsSelectorAPI.getSelectedValue();
                    if (selectedField == undefined || selectedField.length == 0)
                        return true;
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        if (ctrl.datasource[i].data.FieldName == selectedField.Name)
                            return true;
                    }
                    return false;
                };
                $scope.scopeModel.addField = function () {
                    var entity = {
                        data: { FieldName: selectedField.Name },
                        runtimeEditor: selectedField.Type.RuntimeEditor
                    };

                    entity.onRunTimeEditorDirectiveReady = function (api) {
                        entity.runtimeDirectiveAPI = api;
                        var payload = { fieldTitle: selectedField.Title, fieldType: selectedField.Type };
                        var setLoader = function (value) { $scope.scopeModel.isRuntimeEditorDirectiveloading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, payload, setLoader);
                    };
                    ctrl.datasource.push(entity);
                };

                $scope.scopeModel.onDeleteField = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
             
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var promises = [];
                    context = payload.context;
                    fields = getContext().getFields();
                    var dataRecordTypeId = getContext().getDataRecordTypeId();
                    if (dataRecordTypeId != undefined) {
                        var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                            var fieldsPayload = {
                                dataRecordTypeId: dataRecordTypeId,
                            };
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, fieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                        });

                        promises.push(loadDataRecordTypeFieldsSelectorPromiseDeferred.promise);

                        if (payload != undefined && payload.settings && payload.settings.FixedValues != undefined) {
                            for (var i = 0; i < payload.settings.FixedValues.length; i++) {
                                var genericBEField = payload.settings.FixedValues[i];
                                var genericBEFieldObject = {
                                    payload: genericBEField,
                                    runtimeFieldReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    runtimeFieldLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                promises.push(genericBEFieldObject.runtimeFieldLoadPromiseDeferred.promise);
                                prepareDataItem(genericBEFieldObject);
                            }
                        }

                    }
                    return UtilsService.waitPromiseNode({ promises: promises });
                };
            
                api.getData = function () {
                    var fixedValues;
                    if (ctrl.datasource != undefined) {
                        fixedValues = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            currentItem.data.Value = currentItem.runtimeDirectiveAPI.getData();
                            fixedValues.push(currentItem.data);
                        }
                    }
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers.SetGenericBEFixedValuesBeforeSaveHandler, Vanrise.GenericData.MainExtensions",
                        FixedValues: fixedValues
                    };
                };
                function prepareDataItem(genericBEFieldObject) {

                    var entity = {
                        data: genericBEFieldObject.payload
                    };

                    entity.onRunTimeEditorDirectiveReady = function (api) {
                        entity.runtimeDirectiveAPI = api;
                        genericBEFieldObject.runtimeFieldReadyPromiseDeferred.resolve();
                    };

                    var field = UtilsService.getItemByVal(fields, genericBEFieldObject.payload.FieldName, "FieldName");

                    if (field != undefined) {
                        entity.runtimeEditor = field.Type.RuntimeEditor;
                        genericBEFieldObject.runtimeFieldReadyPromiseDeferred.promise.then(function () {

                            var payloadRuntimeEditor = {
                                fieldTitle: field.FieldTitle,
                                fieldValue: genericBEFieldObject.payload.Value,
                                fieldType: field.Type
                            };
                            VRUIUtilsService.callDirectiveLoad(entity.runtimeDirectiveAPI, payloadRuntimeEditor, genericBEFieldObject.runtimeFieldLoadPromiseDeferred);
                        });
                    }
                    else {
                        genericBEFieldObject.runtimeFieldLoadPromiseDeferred.resolve();
                    }

                    ctrl.datasource.push(entity);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

        }
        return directiveDefinitionObject;
    }
]);