"use strict";

app.directive("vrInvoicetypeGridactionsettingsSendemail", ["UtilsService", "VRNotificationService", "VRUIUtilsService","VR_Invoice_InvoiceEmailActionService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_InvoiceEmailActionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SendEmailAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceActions/MainExtensions/SendEmail/Templates/SendEmailActionTemplate.html"

        };

        function SendEmailAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainTypeSelectorAPI;
            var mainTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
                $scope.infoType = "MainTemplate";
                ctrl.addEmailAttachment = function () {
                    var onEmailAttachmentAdded = function (emailAttachment) {
                        ctrl.datasource.push({ Entity: emailAttachment });
                    };

                    VR_Invoice_InvoiceEmailActionService.addEmailAttachment(onEmailAttachmentAdded, getContext());
                };
                $scope.onMainTypeSelectorReady = function (api) {
                    mainTypeSelectorAPI = api;
                    mainTypeSelectorReadyDeferred.resolve();
                };
                ctrl.removeEmailAttachment = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoiceActionEntity;
                    if (payload != undefined) {
                        invoiceActionEntity = payload.invoiceActionEntity;
                        context = payload.context;
                        if (invoiceActionEntity != undefined && invoiceActionEntity.EmailAttachments != undefined) {
                            for (var i = 0; i < invoiceActionEntity.EmailAttachments.length; i++) {
                                var emailAttachment = invoiceActionEntity.EmailAttachments[i];
                                ctrl.datasource.push({ Entity: emailAttachment });
                            }
                        }
                        if (invoiceActionEntity != undefined)
                         $scope.infoType = invoiceActionEntity.InfoType;
                    }
                   
                    var promises = [];
                    function loadMainTypeSelectorDirective() {
                        var mainTypeSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                        mainTypeSelectorReadyDeferred.promise.then(function () {
                            var mainTypeSelectorPayload;
                            if (invoiceActionEntity != undefined)
                            {
                                mainTypeSelectorPayload = {
                                    selectedIds: invoiceActionEntity.InvoiceMailTypeId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(mainTypeSelectorAPI, mainTypeSelectorPayload, mainTypeSelectorPayloadLoadDeferred);
                        });
                        return mainTypeSelectorPayloadLoadDeferred.promise;
                    }
                    promises.push(loadMainTypeSelectorDirective());
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var emailAttachments;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        emailAttachments = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            emailAttachments.push(currentItem.Entity);
                        }
                    }
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.SendEmailAction ,Vanrise.Invoice.MainExtensions",
                        EmailAttachments: emailAttachments,
                        InfoType: $scope.infoType,
                        InvoiceMailTypeId: mainTypeSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editEmailAttachment,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editEmailAttachment(emailAttachmentObj) {
                var onEmailAttachmentUpdated = function (emailAttachment) {
                    var index = ctrl.datasource.indexOf(emailAttachmentObj);
                    ctrl.datasource[index] = { Entity: emailAttachment };
                };
                VR_Invoice_InvoiceEmailActionService.editEmailAttachment(emailAttachmentObj.Entity, onEmailAttachmentUpdated, getContext());
            }
        }

        return directiveDefinitionObject;

    }
]);