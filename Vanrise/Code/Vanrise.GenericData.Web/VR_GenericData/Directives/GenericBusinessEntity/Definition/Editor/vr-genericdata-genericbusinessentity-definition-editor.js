'use strict';
app.directive('vrGenericdataGenericbusinessentityDefinitionEditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_ExtensibleBEItemService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_ExtensibleBEItemService) {

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
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/DefinitionEditorTemplate.html';
            }

        };

        function EditorCtor(ctrl, $scope) {
            var gridAPI;
            var recordTypeFields;
            function initializeController() {
                ctrl.sections = [];
                ctrl.addSection = function () {
                    var onSectionAdded = function (sectionObj) {
                        var section = {
                            sectionTitle: sectionObj,
                            onRowsDirectiveReady: function (api) {
                                section.rowsGridAPI = api;
                                var payload = { context: getContext() };
                                var setLoader = function (value) { $scope.isLoading = value; };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, section.rowsGridAPI, payload, setLoader);
                            }
                        };
                        ctrl.sections.push(section);
                    };

                    VR_GenericData_ExtensibleBEItemService.addSection(onSectionAdded, getExistingSections());
                };

                ctrl.validateEditor = function () {
                    if (ctrl.sections.length == 0) {
                        return "At least one section should be added.";
                    }
                    return null;
                };

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                };

                defineGridSectionsMenuAction();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        recordTypeFields = payload.recordTypeFields;
                        ctrl.sections.length = 0;
                        if (payload.sections != undefined) {
                            var promises = [];
                            for (var i = 0; i < payload.sections.length; i++) {
                                var section = payload.sections[i];
                                prepareSectionObject(section);
                            }

                            return UtilsService.waitMultiplePromises(promises);
                        }

                    }
                };

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
                };

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
                     name: "Edit",
                     clicked: editSection,
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
                VR_GenericData_ExtensibleBEItemService.addRow(onRowAdded, getFilteredFields());
            }

            function deleteSection(sectionObj)
            {
                var onSectionDeleted = function (sectionObj) {
                    var index = UtilsService.getItemIndexByVal(ctrl.sections, sectionObj.sectionTitle, 'sectionTitle');
                    ctrl.sections.splice(index, 1);
                };

                VR_GenericData_ExtensibleBEItemService.deleteSection($scope, sectionObj, onSectionDeleted);
            }

            function prepareSectionObject(section) {

                section.sectionTitle = section.SectionTitle;
                section.onRowsDirectiveReady = function (api) {
                    section.rowsGridAPI = api;
                    var payload = { context: getContext(), rows: section.Rows };
                    var setLoader = function (value) { $scope.isLoading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, section.rowsGridAPI, payload, setLoader);
                };
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
                //var filteredFields = recordTypeFields;
                for (var i = 0; i < recordTypeFields.length; i++) {
                    filteredFields.push({ FieldPath: recordTypeFields[i].Name });
                }
                for(var i=0;i<recordTypeFields.length;i++)
                {
                    filterSections(ctrl.sections, filteredFields, exceptedFields);
                   
                }
                return filteredFields;
            }

            function filterSections(sections, filteredFields, exceptedFields)
            {
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

            function filterRows(rows, filteredFields, exceptedFields)
            {
                for (var i = 0; i < rows.length; i++) {
                    var row = rows[i];
                    filterFields(row.Fields, filteredFields, exceptedFields);

                }
            }

            function filterFields(fields, filteredFields, exceptedFields)
            {
                for (var i = 0; i < fields.length; i++) {
                    var field = fields[i];
                    if (exceptedFields == undefined || UtilsService.getItemIndexByVal(exceptedFields, field.FieldPath, 'FieldPath') == -1) {
                        var index = UtilsService.getItemIndexByVal(filteredFields, field.FieldPath, 'FieldPath');
                        if (index != -1)
                            filteredFields.splice(index, 1);
                    }
                }
               
            }

            function editSection(dataItem)
            {
                var onSectionUpdated = function (sectionObj) {
                    var index = UtilsService.getItemIndexByVal(ctrl.sections, dataItem.sectionTitle, 'sectionTitle');
                    ctrl.sections[index].sectionTitle = sectionObj;
                };

                VR_GenericData_ExtensibleBEItemService.editSection(onSectionUpdated, getExistingSections(), dataItem.sectionTitle);
            }

            function getExistingSections()
            {
                var exitingSections = [];
                for (var i = 0; i < ctrl.sections.length; i++) {
                    exitingSections.push(ctrl.sections[i].sectionTitle.toLowerCase());

                }
                return exitingSections;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);