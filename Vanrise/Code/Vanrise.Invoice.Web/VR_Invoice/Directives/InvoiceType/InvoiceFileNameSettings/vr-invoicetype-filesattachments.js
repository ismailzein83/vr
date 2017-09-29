"use strict";

app.directive("vrInvoicetypeFilesattachments", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceFileSettingsService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceFileSettingsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new FilesAttachments($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceFileNameSettings/Templates/FilesAttachmentsTemplate.html"

        };

        function FilesAttachments($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.addFileAttachment = function () {
                    var onFileAttachmentAdded = function (fileAttachment) {
                        ctrl.datasource.push({ Entity: fileAttachment });
                    };

                    VR_Invoice_InvoiceFileSettingsService.addFileAttachment(onFileAttachmentAdded, getContext());
                };

                ctrl.removeFileAttachment = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var filesAttachments;
                    if (ctrl.datasource != undefined) {
                        filesAttachments = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            filesAttachments.push(currentItem.Entity);
                        }
                    }
                    return filesAttachments;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.filesAttachments != undefined) {
                            for (var i = 0; i < payload.filesAttachments.length; i++) {
                                var fileAttachment = payload.filesAttachments[i];
                                ctrl.datasource.push({ Entity: fileAttachment });
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
                    clicked: editFileAttachment,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editFileAttachment(fileAttachmentObj) {
                var onFileAttachmentUpdated = function (fileAttachment) {
                    var index = ctrl.datasource.indexOf(fileAttachmentObj);
                    ctrl.datasource[index] = { Entity: fileAttachment };
                };

                VR_Invoice_InvoiceFileSettingsService.editFileAttachment(fileAttachmentObj.Entity, onFileAttachmentUpdated, getContext());
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