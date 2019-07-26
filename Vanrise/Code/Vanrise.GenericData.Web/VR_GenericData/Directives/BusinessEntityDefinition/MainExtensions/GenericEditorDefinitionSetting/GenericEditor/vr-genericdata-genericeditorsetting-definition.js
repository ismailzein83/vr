"use strict";

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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/GenericEditor/Templates/GenericEditorDefinitionSettingTemplate.html"
        };
       
        function GenericEditorDefinitionSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            ctrl.datasource = [];
            var context;
            var rows;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.addRow = function () {
                    var dataItem = {
                        entity: {
                            fieldsNumber: 0,
                            fieldsWorld: "Field"
                        }
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

                    dataItem.removeRow = function () {
                        var index = ctrl.datasource.indexOf(dataItem);
                        ctrl.datasource.splice(index, 1);
                    };

                    ctrl.datasource.push(dataItem);
                };

                $scope.scopeModel.isValid = function () {
                    if (ctrl.datasource.length == 0)
                        return "You Should add at least one row.";
                    return null;
                };
                defineAPI();
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
                            };
                            prepareRow(rowObject);
                        }
                    }
                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                function prepareRow(rowObject) {
                    var dataItem = {
                        entity: { fieldsNumber: rowObject.payload.Fields != undefined ? rowObject.payload.Fields.length : 0 },
                        oldData: rowObject.payload.Fields
                    };

                    dataItem.entity.fieldsWorld = dataItem.entity.fieldsNumber == 1 || dataItem.entity.fieldsNumber == 0 ? "Field" : "Fields";

                    dataItem.onGenericFieldsDirectiveReady = function (api) {
                        dataItem.genericFieldsDirectiveAPI = api;
                        var setLoader = function (value) { dataItem.isGenericFieldsDirectiveLoading = value; };

                        var rowPayload = {
                            fields: rowObject.payload.Fields,
                            context: getContext(),
                            setFieldsNumber: function (fieldsNumber) {
                                dataItem.entity.fieldsNumber = fieldsNumber;
                            }
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.genericFieldsDirectiveAPI, rowPayload, setLoader);
                    };

                    dataItem.removeRow = function () {
                        var index = ctrl.datasource.indexOf(dataItem);
                        ctrl.datasource.splice(index, 1);
                    };

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
                            else if (row.oldData != undefined)
                                rows.push({ Fields: row.oldData });
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