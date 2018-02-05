"use strict";

app.directive("vrGenericdataGenericeditorsettingDefinition", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_GenericData_ExtensibleBEItemService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_ExtensibleBEItemService) {

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

            var sectionDirectiveApi;
            var sectionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            var context;
            var rows;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onSectionDirectiveReady = function (api) {
                    sectionDirectiveApi = api;
                    sectionDirectivePromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                ctrl.addRowSection = function () {
                    var onRowAdded = function (rowObj) {
                        sectionDirectiveApi.onAddRow(rowObj);
                    };
                    VR_GenericData_ExtensibleBEItemService.addRow(onRowAdded, getFilteredFields());
                };

                ctrl.isValid = function () {
                    if (sectionDirectiveApi == undefined) return null;
                    if (sectionDirectiveApi.getData().Rows.length == 0)
                        return "You Should add at least one row.";
                    return null;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.settings != undefined)
                            rows = payload.settings.Rows;
                    }
                    var promises = [];

                    promises.push(loadBusinessEntityDefinitionSelector());

                    function loadBusinessEntityDefinitionSelector() {
                        var sectionDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        sectionDirectivePromiseDeferred.promise.then(function () {
                            var payloadSelector = {
                                rows: payload != undefined && payload.settings != undefined ? payload.settings.Rows : undefined,
                                context: getContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(sectionDirectiveApi, payloadSelector, sectionDirectiveLoadDeferred);
                        });
                        return sectionDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                function getContext() {
                    return {
                        getFilteredFields: function () {
                            var data = [];
                            var filterData = context.getRecordTypeFields();
                            for (var i = 0; i < filterData.length; i++) {
                                data.push({ FieldPath: filterData[i].Name });
                            }
                            return data;
                        },
                        getDataRecordTypeId: function () {
                            return context.getDataRecordTypeId();
                        }
                    };
                }

                function getFilteredFields(exceptedFields) {
                    var filteredFields = [];
                    if (context != undefined) {
                        var allFields = context.getRecordTypeFields();
                        for (var i = 0; i < allFields.length; i++) {
                            filteredFields.push({ FieldPath: allFields[i].Name });
                        }
                    }
                    return filteredFields;
                }

                function filterSections(filteredFields, exceptedFields) {

                    if (sectionDirectiveApi != undefined) {
                        var section = sectionDirectiveApi.getData();
                        filterRows(section.Rows, filteredFields, exceptedFields);
                    } else if (rows != undefined) {
                        filterRows(rows, filteredFields, exceptedFields);
                    }
                }

                function filterRows(rows, filteredFields, exceptedFields) {
                    for (var i = 0; i < rows.length; i++) {
                        var row = rows[i];
                        filterFields(row.Fields, filteredFields, exceptedFields);
                    }
                }

                function filterFields(fields, filteredFields, exceptedFields) {
                    for (var i = 0; i < fields.length; i++) {
                        var field = fields[i];
                        if (exceptedFields == undefined || UtilsService.getItemIndexByVal(exceptedFields, field.FieldPath, 'FieldPath') == -1) {
                            var index = UtilsService.getItemIndexByVal(filteredFields, field.FieldPath, 'FieldPath');
                            if (index != -1)
                                filteredFields.splice(index, 1);
                        }
                    }

                }

                api.getData = function () {
                    var data = sectionDirectiveApi.getData();
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        Rows: sectionDirectiveApi.getData().Rows
                    }
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);