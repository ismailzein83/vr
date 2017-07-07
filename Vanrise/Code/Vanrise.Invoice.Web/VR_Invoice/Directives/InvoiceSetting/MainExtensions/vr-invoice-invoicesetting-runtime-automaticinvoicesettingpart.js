'use strict';
app.directive('vrInvoiceInvoicesettingRuntimeAutomaticinvoicesettingpart', ['UtilsService', 'VRUIUtilsService','VR_Invoice_InvoiceSettingAPIService',
    function (UtilsService, VRUIUtilsService, VR_Invoice_InvoiceSettingAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new RowCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_Invoice/Directives/InvoiceSetting/MainExtensions/Templates/AutomaticInvoiceSettingPartTemplate.html';
            }

        };

        function RowCtor(ctrl, $scope) {
            var currentContext;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onEnableAutomaticInvoiceChanged = function (value) {
                    if (currentContext != undefined && currentContext.setRequiredBillingPeriod != undefined) {
                        currentContext.setRequiredBillingPeriod($scope.scopeModel.enableAutomaticInvoice);
                    }
                };
                $scope.scopeModel.actions = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        currentContext = payload.context;
                        var invoiceTypeId = payload.invoiceTypeId;
                        var promises = [];
                        var finalPromise = UtilsService.createPromiseDeferred();
                        VR_Invoice_InvoiceSettingAPIService.GetAutomaticInvoiceSettingPartRuntime(invoiceTypeId).then(function (response) {
                            if (response && response.AutomaticInvoiceActions) {
                                for (var i = 0, length = response.AutomaticInvoiceActions.length; i < length; i++) {
                                    var automaticInvoiceAction = response.AutomaticInvoiceActions[i];
                                    var sectionPayload = {
                                        payload: automaticInvoiceAction,
                                        invoiceAttachments: response.InvoiceAttachments,
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    };
                                    if (payload.fieldValue != undefined && payload.fieldValue.Actions != undefined)
                                    {
                                        sectionPayload.actionValue = UtilsService.getItemByVal(payload.fieldValue.Actions, automaticInvoiceAction.AutomaticInvoiceActionId, "AutomaticInvoiceActionId");
                                    }
                                    promises.push(sectionPayload.loadPromiseDeferred.promise);
                                    addSectionDirective(sectionPayload);
                                }
                            }
                        }).catch(function (error) {
                            finalPromise.reject(error);
                        });;
                        function addSectionDirective(sectionPayload) {
                            var section = {
                                automaticInvoiceActionId: sectionPayload.payload.AutomaticInvoiceActionId,
                                title: sectionPayload.payload.Title,
                                runtimeEditor: sectionPayload.payload.Settings.RuntimeEditor,
                            };
                            section.onDirectiveReady = function (api) {
                                section.directiveAPI = api;
                                sectionPayload.readyPromiseDeferred.resolve();
                            };
                            sectionPayload.readyPromiseDeferred.promise.then(function () {
                                var directivePayload = {
                                    emailActionSettings: sectionPayload.payload.Settings,
                                    invoiceTypeId: invoiceTypeId,
                                    invoiceAttachments: sectionPayload.invoiceAttachments,
                                    actionValueSettings: sectionPayload.actionValue != undefined ? sectionPayload.actionValue.Settings : undefined
                                };
                                VRUIUtilsService.callDirectiveLoad(section.directiveAPI, directivePayload, sectionPayload.loadPromiseDeferred);
                            });
                            $scope.scopeModel.actions.push(section);
                        }
                        if (payload.fieldValue != undefined) {

                            $scope.scopeModel.enableAutomaticInvoice = payload.fieldValue.IsEnabled;
                            if (currentContext != undefined && currentContext.setRequiredBillingPeriod != undefined)
                                currentContext.setRequiredBillingPeriod($scope.scopeModel.enableAutomaticInvoice);
                        }

                        UtilsService.waitMultiplePromises(promises).finally(function () {
                            finalPromise.resolve();
                        }).catch(function (error) {
                            finalPromise.reject(error);
                        });
                        return finalPromise.promise;
                    }
                };

                api.getData = function () {
                    var actions = [];
                    for (var i = 0, length = $scope.scopeModel.actions.length; i < length; i++) {
                        var action = $scope.scopeModel.actions[i];
                        actions.push({
                            AutomaticInvoiceActionId: action.automaticInvoiceActionId,
                            Settings: action.directiveAPI.getData()
                        });
                    }
                    return {
                        $type: "Vanrise.Invoice.Entities.AutomaticInvoiceSettingPart,Vanrise.Invoice.Entities",
                        IsEnabled: $scope.scopeModel.enableAutomaticInvoice,
                        Actions: actions
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);