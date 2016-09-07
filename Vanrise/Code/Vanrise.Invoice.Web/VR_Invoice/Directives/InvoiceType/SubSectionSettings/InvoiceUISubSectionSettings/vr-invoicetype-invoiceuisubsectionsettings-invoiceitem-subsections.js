"use strict";

app.directive("vrInvoicetypeInvoiceuisubsectionsettingsInvoiceitemSubsections", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceTypeService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceTypeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SubSections($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/SubSectionSettings/InvoiceUISubSectionSettings/Templates/InvoiceItemSubSectionsManagement.html"

        };

        function SubSections($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one sub section.";
                }

                ctrl.addSubSection = function () {
                    var onSubSectionAdded = function (subSection) {
                        ctrl.datasource.push({ Entity: subSection });
                    }

                    VR_Invoice_InvoiceTypeService.addInvoiceItemSubSection(onSubSectionAdded, getContext());
                };

                ctrl.removeSubSection = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                }
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var subSections;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        subSections = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            subSections.push(currentItem.Entity);
                        }
                    }
                    return subSections;
                }

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.subSections != undefined) {
                            for (var i = 0; i < payload.subSections.length; i++) {
                                var subSection = payload.subSections[i];
                                ctrl.datasource.push({ Entity: subSection });
                            }
                        }
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editSubsection,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editSubsection(subSectionObj) {
                var onSubSectionUpdated = function (subSection) {
                    var index = ctrl.datasource.indexOf(subSectionObj);
                    ctrl.datasource[index] = { Entity: subSection };
                }

                VR_Invoice_InvoiceTypeService.editInvoiceItemSubSection(subSectionObj.Entity, onSubSectionUpdated, getContext());
            }
            function getContext()
            {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);