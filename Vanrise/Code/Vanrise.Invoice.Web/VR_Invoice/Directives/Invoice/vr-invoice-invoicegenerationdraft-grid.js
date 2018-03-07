"use strict";

app.directive("vrInvoiceInvoicegenerationdraftGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceAPIService", "VR_Invoice_InvoiceFieldEnum", "VRUIUtilsService", "VRValidationService", "VR_Invoice_InvoiceActionService", "VRDateTimeService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceAPIService, VR_Invoice_InvoiceFieldEnum, VRUIUtilsService, VRValidationService, VR_Invoice_InvoiceActionService, VRDateTimeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InvoicepartnerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/Invoice/Templates/InvoiceGenerationDraftGridTemplate.html"

        };

        function InvoicepartnerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var changedItems = [];
            var invoiceTypeId;
            var issueDate;
            var totalItemsCount = 0;

            function initializeController() {

                $scope.invoicePartners = [];
                $scope.isValid = function () {
                    if ($scope.invoicePartners == undefined || $scope.invoicePartners.length == 0)
                        return 'At least one item should exist';

                    return null;
                };

                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (payload) {
                            changedItems.length = 0;
                            var query = payload.query;
                            invoiceTypeId = payload.invoiceTypeId;
                            issueDate = payload.issueDate;

                            $scope.generationCustomSectionDirective = payload.customPayloadDirective;
                            return gridAPI.retrieveData(query);
                        };

                        directiveAPI.clearDataSource = function () {
                            gridAPI.clearDataSource();
                        };

                        directiveAPI.getData = function () {
                            var obj = {};
                            var numberOfExcludedItems = 0;
                            var changedItemsAsList;
                            if (changedItems != undefined && changedItems.length > 0) {
                                changedItemsAsList = [];
                                for (var x = 0; x < changedItems.length; x++) {
                                    var currentChangedItem = changedItems[x].value;
                                    var newItem = {
                                        InvoiceGenerationDraftId: currentChangedItem.InvoiceGenerationDraftId,
                                        From: currentChangedItem.From,
                                        To: currentChangedItem.To,
                                        CustomPayload: currentChangedItem.CustomPayload,
                                        IsSelected: currentChangedItem.IsSelected
                                    };
                                    if (!newItem.IsSelected)
                                        numberOfExcludedItems++;

                                    changedItemsAsList.push(newItem);
                                }
                            }
                            obj.changedItems = changedItemsAsList;
                            obj.allItemsExcluded = totalItemsCount == numberOfExcludedItems;
                            return obj;
                        };

                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    $scope.isLodingGrid = true;
                    var promises = [];
                    return VR_Invoice_InvoiceAPIService.GetFilteredInvoiceGenerationDrafts(dataRetrievalInput)
                        .then(function (response) {
                            if (response) {
                                totalItemsCount = response.TotalCount;
                                if (response.Data) {
                                    for (var i = 0; i < response.Data.length; i++) {
                                        var currentItem = response.Data[i];
                                        currentItem.IsSelected = true;
                                        currentItem.menuActions = [];

                                        if (changedItems.length > 0) {
                                            for (var j = 0; j < changedItems.length; j++) {
                                                var currentChangedItem = changedItems[j];
                                                if (currentChangedItem.key == currentItem.InvoiceGenerationDraftId) {
                                                    currentItem.IsSelected = currentChangedItem.value.IsSelected;
                                                    currentItem.From = currentChangedItem.value.From;
                                                    currentItem.To = currentChangedItem.value.To;
                                                    currentItem.CustomPayload = currentChangedItem.value.CustomPayload;
                                                    break;
                                                }
                                            }
                                        }

                                        var promise = extendInvoicePartner(currentItem);

                                        function extendInvoicePartner(currentItem) {
                                            var extendInvoicePartnerPromiseDeferred = UtilsService.createPromiseDeferred();

                                            currentItem.validateFromDate = function () {
                                                return VRValidationService.validateTimeRange(currentItem.From, currentItem.To);
                                            };

                                            currentItem.validateToDate = function () {
                                                var result = VRValidationService.validateTimeRange(currentItem.From, currentItem.To);
                                                if (result != null)
                                                    return result;

                                                if (currentItem.To != undefined && currentItem.To >= VRDateTimeService.getTodayDate()) {
                                                    return 'To should be less than today date';
                                                }
                                                return null;
                                            };

                                            currentItem.onItemChanged = function () {
                                                var alreadyAdded = false;
                                                if (changedItems.length > 0) {
                                                    for (var k = 0; k < changedItems.length; k++) {
                                                        var currentChangedItem = changedItems[k];
                                                        if (currentChangedItem.key == currentItem.InvoiceGenerationDraftId) {
                                                            alreadyAdded = true;
                                                            currentChangedItem.value = currentItem;
                                                            currentChangedItem.value.CustomPayload = currentItem.generationCustomSectionDirectiveAPI != undefined ? currentItem.generationCustomSectionDirectiveAPI.getData() : undefined;
                                                            break;
                                                        }
                                                    }
                                                }

                                                if (!alreadyAdded) {
                                                    var obj = {
                                                        key: currentItem.InvoiceGenerationDraftId,
                                                        value: currentItem
                                                    };
                                                    obj.value.CustomPayload = currentItem.generationCustomSectionDirectiveAPI != undefined ? currentItem.generationCustomSectionDirectiveAPI.getData() : undefined;
                                                    changedItems.push(obj);
                                                }
                                            };

                                            currentItem.generationCustomSectionDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                                            if ($scope.generationCustomSectionDirective == undefined) {
                                                currentItem.generationCustomSectionDirectiveLoadDeferred.resolve();
                                            }
                                            else {
                                                currentItem.onGenerationCustomSectionDirectiveReady = function (api) {
                                                    currentItem.generationCustomSectionDirectiveAPI = api;
                                                    var payload = { partnerId: currentItem.PartnerId, customPayload: currentItem.CustomPayload,fromDate:currentItem.From,toDate:currentItem.To, context: { onvaluechanged: currentItem.onItemChanged }, invoiceTypeId: invoiceTypeId };
                                                    VRUIUtilsService.callDirectiveLoad(currentItem.generationCustomSectionDirectiveAPI, payload, currentItem.generationCustomSectionDirectiveLoadDeferred);
                                                };
                                            }

                                            currentItem.isInvalid = function () {
                                                if (currentItem.From == undefined || currentItem.To == undefined)
                                                    return true;

                                                return false;
                                            };

                                            buildActionsFromDictionary(currentItem.InvoiceGenerationDraftActionDetails);

                                            function buildActionsFromDictionary(actionsDictionary) {
                                                if (actionsDictionary != undefined) {
                                                    for (var prop in actionsDictionary) {
                                                        if (prop == '$type')
                                                            continue;

                                                        if (actionsDictionary[prop].length > 1) {
                                                            var menuActions = [];
                                                            for (var i = 0; i < actionsDictionary[prop].length; i++) {
                                                                if (menuActions == undefined)
                                                                    menuActions = [];
                                                                var object = actionsDictionary[prop][i];
                                                                addMenuAction(object.InvoiceGeneratorAction, object.InvoiceAction);
                                                            }
                                                            function addMenuAction(invoiceGeneratorAction, invoiceAction) {
                                                                menuActions.push({
                                                                    name: invoiceGeneratorAction.Title,
                                                                    clicked: function () {
                                                                        return callActionMethod(invoiceAction);
                                                                    },
                                                                });
                                                            }

                                                            addActionToList(prop, undefined, menuActions);
                                                        } else {
                                                            var object = actionsDictionary[prop][0];
                                                            var clickFunc = function () {
                                                                return callActionMethod(object.InvoiceAction);
                                                            };
                                                            addActionToList(prop, clickFunc, undefined);
                                                        }
                                                    }
                                                }
                                                function addActionToList(buttonType, clickEvent, menuActions) {
                                                    currentItem.menuActions.push({
                                                        type: buttonType,
                                                        onclick: clickEvent,
                                                        menuActions: menuActions
                                                    });
                                                }

                                            }
                                            function callActionMethod(invoiceAction) {
                                                var payload = {
                                                    generatorEntity: {
                                                        invoiceTypeId: invoiceTypeId,
                                                        partnerId: currentItem.PartnerId,
                                                        fromDate: currentItem.From,
                                                        toDate: currentItem.To,
                                                        issueDate: issueDate,
                                                        customSectionPayload: currentItem.CustomPayload
                                                    },
                                                    invoiceAction: invoiceAction,
                                                    isPreGenerateAction: true
                                                };
                                                var actionType = VR_Invoice_InvoiceActionService.getActionTypeIfExist(invoiceAction.Settings.ActionTypeName);

                                                var promise = actionType.actionMethod(payload);

                                                return promise;
                                            }

                                            UtilsService.waitMultiplePromises([currentItem.generationCustomSectionDirectiveLoadDeferred.promise]).then(function () {
                                                extendInvoicePartnerPromiseDeferred.resolve();
                                            });
                                            return extendInvoicePartnerPromiseDeferred.promise;
                                        }

                                        promises.push(promise);
                                    }
                                }
                            }
                            UtilsService.waitMultiplePromises(promises).then(function () { $scope.isLodingGrid = false; });
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };
            }
        }

        return directiveDefinitionObject;

    }
]);