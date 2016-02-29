'use strict';
app.directive('vrGenericdataGenericeditorDefinitionEditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericEditorService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericEditorService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new EditorCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/GenericEditor/Definition/Templates/DefinitionEditorTemplate.html';
            }

        };

        function EditorCtor(ctrl, $scope) {
            var selectedValues;
            var gridAPI;
            var recordTypeFields;
            function initializeController() {
                ctrl.sections = [];

                ctrl.addSection = function () {
                    if (ctrl.sectionTitle != undefined && UtilsService.getItemIndexByVal(ctrl.sections, ctrl.sectionTitle, "sectionTitle") == -1)
                    {
                        var section = {
                            sectionTitle: ctrl.sectionTitle,
                            onRowsDirectiveReady: function (api)
                            {
                                section.rowsGridAPI = api;
                                var payload = { context: getContext() };
                                var setLoader = function (value) { $scope.isLoading = value; };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, section.rowsGridAPI, payload, setLoader);
                            }
                        };
                        ctrl.sections.push(section);
                        ctrl.sectionTitle = undefined;
                    }           
                }

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                };

                defineGridSectionsMenuAction();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if(payload !=undefined)
                    {
                        recordTypeFields = payload.recordTypeFields;
                        if (payload.sections != undefined) {
                            selectedValues = payload.selectedValues;
                            ctrl.sections.length = 0;
                            var promises = [];
                            for (var i = 0; i < payload.sections.length; i++) {
                                var section = payload.sections[i];
                                prepareSectionObject(section);
                            }

                            return UtilsService.waitMultiplePromises(promises);
                        }

                    }
                }

                api.getData = function () {
                    var sections = [];
                    for (var i = 0; i < ctrl.sections.length; i++) {

                        var section = ctrl.sections[i];
                        var rows;
                        if (section.rowsGridAPI != undefined)
                            rows = section.rowsGridAPI.getData();
                        else
                            rows = { Rows: section.Rows };
                       
                        rows.SectionTitle = section.sectionTitle;
                        sections.push(rows);
                    }
                    return { Sections: sections };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function defineGridSectionsMenuAction()
            {
                $scope.gridSectionsMenuActions = [{
                        name: "Add Row",
                        clicked: addRow,
                    },
                 {
                    name: "Delete",
                    clicked: deleteSection,
                }];
            }

            function addRow(sectionObj) {
                gridAPI.expandRow(sectionObj);
                var onRowAdded = function (rowObj) {
                    var index = UtilsService.getItemIndexByVal(ctrl.sections, sectionObj.sectionTitle, 'sectionTitle');
                    ctrl.sections[index].rowsGridAPI.onAddRow(rowObj);
                };
                VR_GenericData_GenericEditorService.addRow(onRowAdded, getFilteredFields());
            }

            function deleteSection(sectionObj)
            {
                var onSectionDeleted = function (sectionObj) {
                    var index = UtilsService.getItemIndexByVal(ctrl.sections, sectionObj.sectionTitle, 'sectionTitle');
                    ctrl.sections.splice(index, 1);
                };

                VR_GenericData_GenericEditorService.deleteSection($scope, sectionObj, onSectionDeleted);
            }

            function prepareSectionObject(section) {

                section.sectionTitle = section.SectionTitle
                section.onRowsDirectiveReady = function (api) {
                    section.rowsGridAPI = api;
                    var payload = { context: getContext(), rows: section.Rows };
                    var setLoader = function (value) { $scope.isLoading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, section.rowsGridAPI, payload, setLoader);
                }
                ctrl.sections.push(section);
            }

            function getContext() {
                var context = {
                    getFilteredFields: getFilteredFields,
                };
                return context;
            }

            function getFilteredFields(exceptedFields) {
                var filteredFields = [];

                for (var i = 0; i < recordTypeFields.length; i++) {
                    filteredFields.push({ FieldPath: recordTypeFields[i].Name });
                }
                for(var i=0;i<recordTypeFields.length;i++)
                {
                    var recordTypeField = recordTypeFields[i];
                    for (var j = 0; j < ctrl.sections.length; j++)
                    {
                        var section = ctrl.sections[j];
                        if(section.rowsGridAPI != undefined)
                        {
                            var rows = section.rowsGridAPI.getData();
                            for (var k = 0; k < rows.Rows.length; k++)
                            {
                                var row = rows.Rows[k];
                                for (var l = 0; l < row.Fields.length; l++) {
                                    if (exceptedFields == undefined || UtilsService.getItemIndexByVal(exceptedFields, row.Fields[l].FieldPath, 'FieldPath') == -1)
                                    {
                                        var index = UtilsService.getItemIndexByVal(filteredFields, row.Fields[l].FieldPath, 'FieldPath');
                                        if (index != -1)
                                            filteredFields.splice(index, 1);
                                    }
                                   
                                }
                            }
                           
                        } else if (section.Rows != undefined) {
                            for (var k = 0; k < section.Rows.length; k++) {
                                var row = section.Rows[k];
                                for (var l = 0; l < row.Fields.length; l++) {
                                    if (exceptedFields == undefined || UtilsService.getItemIndexByVal(exceptedFields, row.Fields[l].FieldPath, 'FieldPath') == -1) {
                                        var index = UtilsService.getItemIndexByVal(filteredFields, row.Fields[l].FieldPath, 'FieldPath');
                                        if (index != -1)
                                            filteredFields.splice(index, 1);
                                    }
                                }
                            }

                        }
                    }
                   
                }
                return filteredFields;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);