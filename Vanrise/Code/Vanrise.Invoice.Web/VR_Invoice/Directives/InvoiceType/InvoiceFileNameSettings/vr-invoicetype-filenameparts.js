"use strict";

app.directive("vrInvoicetypeFilenameparts", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceFileSettingsService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceFileSettingsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new FileNameParts($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceFileNameSettings/Templates/FileNamePartsTemplate.html"

        };

        function FileNameParts($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one part.";
                };

                ctrl.addFileNamePart = function () {
                    var onFileNamePartAdded = function (fileNamePart) {
                        ctrl.datasource.push({ Entity: fileNamePart });
                    };

                    VR_Invoice_InvoiceFileSettingsService.addFileNamePart(onFileNamePartAdded, getContext());
                };

                ctrl.removeFileNamePart = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var fileNameParts;
                    if (ctrl.datasource != undefined) {
                        fileNameParts = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            fileNameParts.push(currentItem.Entity);
                        }
                    }
                    return fileNameParts;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.fileNameParts != undefined) {
                            for (var i = 0; i < payload.fileNameParts.length; i++) {
                                var fileNamePart = payload.fileNameParts[i];
                                ctrl.datasource.push({ Entity: fileNamePart });
                            }
                        }
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editFileNamePart,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editFileNamePart(fileNamePartObj) {
                var onFileNamePartUpdated = function (fileNamePart) {
                    var index = ctrl.datasource.indexOf(fileNamePartObj);
                    ctrl.datasource[index] = { Entity: fileNamePart };
                };

                VR_Invoice_InvoiceFileSettingsService.editFileNamePart(fileNamePartObj.Entity, onFileNamePartUpdated, getContext());
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                currentContext.getExtensionType = function () {
                    return "VR_InvoiceType_FileNameParts";
                };
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);