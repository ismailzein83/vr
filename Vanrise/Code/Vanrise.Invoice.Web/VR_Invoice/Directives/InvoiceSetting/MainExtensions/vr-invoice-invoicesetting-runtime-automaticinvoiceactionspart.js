'use strict';
app.directive('vrInvoiceInvoicesettingRuntimeAutomaticinvoiceactionspart', ['UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceSettingAPIService',
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
                return '/Client/Modules/VR_Invoice/Directives/InvoiceSetting/MainExtensions/Templates/AutomaticInvoiceActionsPartTemplate.html';
            }

        };

        function RowCtor(ctrl, $scope) {
            var currentContext;
            var automaticInvoiceActions;
            var invoiceAttachments;
            var isActionsShown = false;
            var invoiceTypeId;
            var isSectionsLoaded;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.actions = [];
                $scope.scopeModel.showAutomaticInvoiceActions = function () {
                   
                    if(currentContext != undefined)
                    {
                        if(currentContext.isAutomaticInvoiceActionsShown != undefined)
                        {
                            var isActionsShownResult = currentContext.isAutomaticInvoiceActionsShown();
                            if (isActionsShownResult != undefined)
                            {
                                if (isActionsShownResult != isActionsShown)
                                {
                                    isActionsShown = isActionsShownResult;
                                    if (isSectionsLoaded != undefined)
                                    {
                                        isSectionsLoaded.resolve();
                                    } else
                                    {
                                        $scope.scopeModel.actions.length = 0;
                                        if (isActionsShown)
                                            loadAutomaticInvoiceActions();
                                    }
                                   
                                }
                               
                            }
                           return isActionsShownResult;
                        }
                    }
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        currentContext = payload.context;
                        invoiceTypeId = payload.invoiceTypeId;
                        var promises = [];
                        var finalPromise = UtilsService.createPromiseDeferred();
                        if (currentContext != undefined) {
                            if (currentContext.isAutomaticInvoiceActionsShown != undefined) {
                                if (currentContext.isAutomaticInvoiceActionsShown()) {
                                    isSectionsLoaded = UtilsService.createPromiseDeferred();
                                }
                            }
                        }
                        loadAutomaticInvoiceSettings(invoiceTypeId).then(function () {
                            if (automaticInvoiceActions) {
                                if (currentContext != undefined) {
                                    if (currentContext.isAutomaticInvoiceActionsShown != undefined) {
                                        if (currentContext.isAutomaticInvoiceActionsShown()) {
                                            for (var i = 0, length = automaticInvoiceActions.length; i < length; i++) {
                                                var automaticInvoiceAction = automaticInvoiceActions[i];
                                                var sectionPayload = {
                                                    payload: automaticInvoiceAction,
                                                    invoiceAttachments: invoiceAttachments,
                                                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                                    loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                                };
                                                if (payload.fieldValue != undefined && payload.fieldValue.Actions != undefined) {
                                                    sectionPayload.actionValue = UtilsService.getItemByVal(payload.fieldValue.Actions, automaticInvoiceAction.AutomaticInvoiceActionId, "AutomaticInvoiceActionId");
                                                }
                                                promises.push(sectionPayload.loadPromiseDeferred.promise);
                                                addSectionDirective(sectionPayload);
                                            }
                                        }
                                    }
                                }
                            }

                            UtilsService.waitMultiplePromises(promises).finally(function () {
                                finalPromise.resolve();
                                isSectionsLoaded = undefined;
                            }).catch(function (error) {
                                finalPromise.reject(error);
                            });

                        }).catch(function (error) {
                            finalPromise.reject(error);
                        });

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
                            UtilsService.waitMultiplePromises([isSectionsLoaded.promise, sectionPayload.readyPromiseDeferred.promise]).then(function () {
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
                        return finalPromise.promise;
                    }
                };

                api.getData = function () {
                    if (isActionsShown)
                    {
                        var actions = [];
                        for (var i = 0, length = $scope.scopeModel.actions.length; i < length; i++) {
                            var action = $scope.scopeModel.actions[i];
                            actions.push({
                                AutomaticInvoiceActionId: action.automaticInvoiceActionId,
                                Settings: action.directiveAPI.getData()
                            });
                        }
                        return {
                            $type: "Vanrise.Invoice.Entities.AutomaticInvoiceActionsPart,Vanrise.Invoice.Entities",
                            Actions: actions
                        };
                    }
                 
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadAutomaticInvoiceSettings(invoiceTypeId)
            {
                return VR_Invoice_InvoiceSettingAPIService.GetAutomaticInvoiceSettingPartRuntime(invoiceTypeId).then(function (response) {
                    if (response && response.AutomaticInvoiceActions) {
                        automaticInvoiceActions = response.AutomaticInvoiceActions;
                        invoiceAttachments = response.InvoiceAttachments;
                    }
                });
            }
            function loadAutomaticInvoiceActions() {
                if (automaticInvoiceActions) {
                    for (var i = 0, length = automaticInvoiceActions.length; i < length; i++) {
                        var automaticInvoiceAction = automaticInvoiceActions[i];
                        var sectionPayload = {
                            payload: automaticInvoiceAction,
                            invoiceAttachments: invoiceAttachments,
                        };
                        addGridSectionDirective(sectionPayload);
                    }
                }
            }
            function addGridSectionDirective(sectionPayload) {
                var section = {
                    automaticInvoiceActionId: sectionPayload.payload.AutomaticInvoiceActionId,
                    title: sectionPayload.payload.Title,
                    runtimeEditor: sectionPayload.payload.Settings.RuntimeEditor,
                };
                section.onDirectiveReady = function (api) {
                    section.directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directivePayload = {
                        emailActionSettings: sectionPayload.payload.Settings,
                        invoiceTypeId: invoiceTypeId,
                        invoiceAttachments: sectionPayload.invoiceAttachments,
                        actionValueSettings: sectionPayload.actionValue != undefined ? sectionPayload.actionValue.Settings : undefined
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, section.directiveAPI, directivePayload, setLoader);
                };
                $scope.scopeModel.actions.push(section);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);