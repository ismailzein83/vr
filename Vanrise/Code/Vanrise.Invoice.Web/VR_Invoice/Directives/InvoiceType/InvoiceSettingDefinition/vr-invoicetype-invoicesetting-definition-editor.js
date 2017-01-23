'use strict';
app.directive('vrInvoicetypeInvoicesettingDefinitionEditor', ['UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeService',
    function (UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeService) {

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
                return '/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSettingDefinition/Templates/DefinitionEditorTemplate.html';
            }

        };

        function EditorCtor(ctrl, $scope) {
            var gridAPI;
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

                    VR_Invoice_InvoiceTypeService.addInvoiceSettingSection(onSectionAdded, getExistingSections());
                };

                ctrl.validateEditor = function () {
                    //if (ctrl.sections.length == 0) {
                    //    return "At least one section should be added.";
                    //}
                    //return null;
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
                        ctrl.sections.length = 0;
                        if (payload.invoiceSettingPartUISections != undefined) {
                            var promises = [];
                            for (var i = 0; i < payload.invoiceSettingPartUISections.length; i++) {
                                var section = payload.invoiceSettingPartUISections[i];
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
                    return sections;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineGridSectionsMenuAction() {
                $scope.gridSectionsMenuActions = [{
                    name: "Add Row",
                    clicked: addRow,
                },
                 {
                     name: "Edit",
                     clicked: editSection,
                 }];
            }

            function addRow(sectionObj) {
                gridAPI.expandRow(sectionObj);
                var onRowAdded = function (rowObj) {
                    var index = UtilsService.getItemIndexByVal(ctrl.sections, sectionObj.sectionTitle, 'sectionTitle');
                    ctrl.sections[index].rowsGridAPI.onAddRow(rowObj);
                };
                VR_Invoice_InvoiceTypeService.addInvoiceSettingPart(onRowAdded);
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
                };
                return context;
            }


            function editSection(dataItem) {
                var onSectionUpdated = function (sectionObj) {
                    var index = UtilsService.getItemIndexByVal(ctrl.sections, dataItem.sectionTitle, 'sectionTitle');
                    ctrl.sections[index].sectionTitle = sectionObj;
                };

                VR_Invoice_InvoiceTypeService.editInvoiceSettingSection(onSectionUpdated, getExistingSections(), dataItem.sectionTitle);
            }

            function getExistingSections() {
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