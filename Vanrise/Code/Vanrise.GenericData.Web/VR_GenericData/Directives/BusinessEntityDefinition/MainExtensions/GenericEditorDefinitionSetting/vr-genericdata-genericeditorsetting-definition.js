"use strict";

app.directive("vrGenericdataGenericeditorsettingDefinition", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

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

                api.load = function (payload) {
                    if (payload != undefined)
                       context = payload.context;

                    var promises = [];

                    promises.push(loadBusinessEntityDefinitionSelector());

                    function loadBusinessEntityDefinitionSelector() {
                        var sectionDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        sectionDirectivePromiseDeferred.promise.then(function () {
                            var payloadSelector = {
                                rows: payload != undefined ? payload.Rows : undefined,
                                context: getContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(sectionDirectiveApi, payloadSelector, sectionDirectiveLoadDeferred);
                        });
                        return sectionDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                function getContext() {
                    var currentContext = context;
                    if (currentContext == undefined)
                        currentContext = {};
                    currentContext.getFilteredFields = getFilteredFields;
                   
                    return currentContext;
                }

                function getFilteredFields(exceptedFields) {

                    var filteredFields = [];
                    if (context != undefined)
                    {
                        //var filteredFields = recordTypeFields;
                        var recordTypeFields = context.getRecordTypeFields();

                        for (var i = 0; i < recordTypeFields.length; i++) {
                            filteredFields.push({ FieldPath: recordTypeFields[i].Name });
                        }
                        for (var i = 0; i < recordTypeFields.length; i++) {
                            filterSections(ctrl.sections, filteredFields, exceptedFields);

                        }
                    }
                    
                    return filteredFields;
                }

                function filterSections(sections, filteredFields, exceptedFields) {
                    for (var j = 0; j < sections.length; j++) {
                        var section = sections[j];
                        if (section.rowsGridAPI != undefined) {
                            var rows = section.rowsGridAPI.getData();
                            filterRows(rows.Rows, filteredFields, exceptedFields);
                        } else if (section.Rows != undefined) {
                            filterRows(section.Rows, filteredFields, exceptedFields);
                        }
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
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        Rows: sectionDirectiveApi.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);