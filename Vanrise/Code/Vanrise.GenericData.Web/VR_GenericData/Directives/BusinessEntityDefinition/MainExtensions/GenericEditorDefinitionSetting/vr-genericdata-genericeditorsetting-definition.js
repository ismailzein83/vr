﻿"use strict";

app.directive("vrGenericdataGenericeditorsettingDefinition", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericEditorDefinitionSetting($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/GenericEditorDefinitionSettingTemplate.html"
        };
        function GenericEditorDefinitionSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            ctrl.datasource = [];
            var gridAPI;
            var context;
            var rows;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();

                };
                $scope.scopeModel.addRow = function () {
                    var dataItem = {
                        entity: { fieldsNumber:0}
                    };
                    dataItem.onGenericFieldsDirectiveReady = function (api) {
                        dataItem.genericFieldsDirectiveAPI = api;
                        var setLoader = function (value) { dataItem.isGenericFieldsDirectiveLoading = value; };
                        var payload = {
                            context: getContext(),
                            setFieldsNumber: function (fieldsNumber) {
                                dataItem.entity.fieldsNumber = fieldsNumber;
                            }
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.genericFieldsDirectiveAPI, payload, setLoader);
                    };
                  
                    gridAPI.expandRow(dataItem);

                    ctrl.datasource.push(dataItem);
                };

                $scope.scopeModel.removeRow = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };

                $scope.scopeModel.isValid = function () {
                    if (ctrl.datasource.length == 0)
                        return "You Should add at least one row.";
                    return null;
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.datasource = [];
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.settings != undefined)
                            rows = payload.settings.Rows;
                    }
                    var promises = [];

                    if (rows != undefined && rows.length > 0) {
                        for (var j = 0; j < rows.length; j++) {
                            var row = rows[j];
                            var rowObject = {
                                payload: row,
                                genericFieldsDirectiveReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                genericFieldsDirectiveLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                            };
                            promises.push(rowObject.genericFieldsDirectiveLoadPromiseDeferred.promise);
                            prepareRow(rowObject);

                        }
                    } 
                    return UtilsService.waitPromiseNode({ promises: promises });
                };
                function prepareRow(rowObject) {
                    var dataItem = {
                        entity: { fieldsNumber: rowObject.payload.Fields != undefined ? rowObject.payload.Fields.length : 0 }
                    };

                    dataItem.onGenericFieldsDirectiveReady = function (api) {
                        dataItem.genericFieldsDirectiveAPI = api;
                        rowObject.genericFieldsDirectiveReadyPromiseDeferred.resolve();
                    };

                    rowObject.genericFieldsDirectiveReadyPromiseDeferred.promise.then(function () {

                        var rowPayload = {
                            fields: rowObject.payload.Fields,
                            context: getContext(),
                            setFieldsNumber: function (fieldsNumber) {
                                dataItem.entity.fieldsNumber = fieldsNumber;
                            }
                        };
                        VRUIUtilsService.callDirectiveLoad(dataItem.genericFieldsDirectiveAPI, rowPayload, rowObject.genericFieldsDirectiveLoadPromiseDeferred);
                    });
                    gridAPI.expandRow(dataItem);

                    ctrl.datasource.push(dataItem);

                }
                api.getData = function () {

                    var rows = [];
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var row = ctrl.datasource[i];
                            if (row.genericFieldsDirectiveAPI != undefined) {
                                var data = row.genericFieldsDirectiveAPI.getData();
                                if (data != undefined) {
                                    rows.push({ Fields: data });
                                }
                            }
                        }
                    }
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        Rows: rows
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};

                currentContext.getFilteredFields = function () {
                    var data = [];
                    var filterData = context.getRecordTypeFields();
                    for (var i = 0; i < filterData.length; i++) {
                        data.push({ FieldPath: filterData[i].Name, FieldTitle: filterData[i].Title });
                    }
                    return data;
                };
           
                return currentContext;
            }

        }

        return directiveDefinitionObject;
    }
]);