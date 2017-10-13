"use strict";

app.directive("vrInvoiceInvoicegenerationdraftGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceAPIService", "VR_Invoice_InvoiceFieldEnum", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceAPIService, VR_Invoice_InvoiceFieldEnum, VRUIUtilsService) {

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
                            $scope.generationCustomSectionDirective = payload.customPayloadDirective;
                            return gridAPI.retrieveData(query);
                        };

                        directiveAPI.clearDataSource = function () {
                            gridAPI.clearDataSource();
                        };

                        directiveAPI.getChangedItems = function () {
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
                                    changedItemsAsList.push(newItem);
                                }
                            }

                            return changedItemsAsList;
                        };

                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    $scope.isLodingGrid = true;
                    var promises = [];
                    return VR_Invoice_InvoiceAPIService.GetFilteredInvoiceGenerationDrafts(dataRetrievalInput)
                        .then(function (response) {
                            if (response && response.Data) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    var currentItem = response.Data[i];
                                    currentItem.IsSelected = true;

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
                                        currentItem.onItemChanged = function () {
                                            var alreadyAdded = false;
                                            if (changedItems.length > 0) {
                                                for (var k = 0; k < changedItems.length; k++) {
                                                    var currentChangedItem = changedItems[k];
                                                    if (currentChangedItem.key == currentItem.InvoiceGenerationDraftId) {
                                                        alreadyAdded = true;
                                                        currentChangedItem.value = currentItem;
                                                        currentChangedItem.value.CustomPayload = currentItem.generationCustomSectionDirectiveAPI.getData();
                                                        break;
                                                    }
                                                }
                                            }

                                            if (!alreadyAdded) {
                                                var obj = {
                                                    key: currentItem.InvoiceGenerationDraftId,
                                                    value: currentItem
                                                };
                                                obj.value.CustomPayload = currentItem.generationCustomSectionDirectiveAPI.getData();
                                                changedItems.push(obj);
                                            }
                                        };

                                        currentItem.generationCustomSectionDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                                        currentItem.onGenerationCustomSectionDirectiveReady = function (api) {
                                            currentItem.generationCustomSectionDirectiveAPI = api;
                                            var payload = { partnerId: currentItem.PartnerId, customPayload: currentItem.CustomPayload, context: { onvaluechanged: currentItem.onItemChanged } };
                                            VRUIUtilsService.callDirectiveLoad(currentItem.generationCustomSectionDirectiveAPI, payload, currentItem.generationCustomSectionDirectiveLoadDeferred);
                                        };

                                        return currentItem.generationCustomSectionDirectiveLoadDeferred.promise;
                                    }

                                    promises.push(promise);
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