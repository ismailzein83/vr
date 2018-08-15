"use strict";

app.directive("vrInvoiceInvoicereportfiles", ["UtilsService", "VRNotificationService", "VRUIUtilsService","VR_Invoice_InvoiceTypeAPIService",
function (UtilsService, VRNotificationService,  VRUIUtilsService, VR_Invoice_InvoiceTypeAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var invoiceReportFiles = new InvoiceReportFiles($scope, ctrl, $attrs);
                invoiceReportFiles.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) { 

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/Invoice/Templates/InvoiceReportFilesTemplate.html"

        };

        function InvoiceReportFiles($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var invoiceTypes = [];

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.invoiceReportItems = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.addInvoiceReportItem = function () {
                    var invoiceReportItemDefinition = {
                        id: UtilsService.guid()
                    };
                    invoiceReportItemDefinition.onInvoiceTypeSelectorReady = function (api) {
                        invoiceReportItemDefinition.invoiceTypeSelectorAPI = api;
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingInvoiceReportFiles = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, invoiceReportItemDefinition.invoiceTypeSelectorAPI, undefined, setLoader);
                    };
                    invoiceReportItemDefinition.onInvoiceTypeSelectionChanged = function (selectedInvoiceType) {
                        if (selectedInvoiceType != undefined)
                        {
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingInvoiceReportFiles = value;
                            };
                            var invoiceReportFileSelectorPayload = {
                                businessEntityDefinitionId: "64f8db86-691d-4486-83fb-26a3d3fc095e",
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.Invoice.Business.InvoiceReportFileFilter, Vanrise.Invoice.Business",
                                        InvoiceTypeId: selectedInvoiceType.InvoiceTypeId
                                    }]
                                },
                                selectIfSingleItem: true
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, invoiceReportItemDefinition.invoiceReportFileSelectorAPI, invoiceReportFileSelectorPayload, setLoader);
                        }
                    };

                    invoiceReportItemDefinition.onInvoiceReportFileSelectorReady = function (api)
                    {
                        invoiceReportItemDefinition.invoiceReportFileSelectorAPI = api;
                    };

                    $scope.scopeModel.invoiceReportItems.push(invoiceReportItemDefinition);
                };

                $scope.scopeModel.removeInvoiceReportItem = function (dataItem) {
                    var index = $scope.scopeModel.invoiceReportItems.indexOf(dataItem);
                    $scope.scopeModel.invoiceReportItems.splice(index, 1);
                };
                $scope.scopeModel.disableAddInvoiceReportItemButton = function () {
                    return ($scope.scopeModel.invoiceReportItems.length > 0 && invoiceTypes.length > 0 && $scope.scopeModel.invoiceReportItems.length == invoiceTypes.length);
                };
                $scope.scopeModel.validateInvoiceReportItems = function () {
                    if ($scope.scopeModel.invoiceReportItems.length > 0 && invoiceTypes.length>0) {
                        var invoiceTypeIds = [];
                        for (var i = 0; i < $scope.scopeModel.invoiceReportItems.length; i++) {
                            var invoiceReportItem = $scope.scopeModel.invoiceReportItems[i];
                            if (invoiceReportItem.invoiceTypeSelectorAPI != undefined && invoiceReportItem.invoiceTypeSelectorAPI.getSelectedIds()!=undefined) {
                                invoiceTypeIds.push(invoiceReportItem.invoiceTypeSelectorAPI.getSelectedIds());
                            }
                        }
                        while (invoiceTypeIds.length > 0) {
                            var idToValidate = invoiceTypeIds[0];
                            invoiceTypeIds.splice(0, 1);
                            if (!validateId(idToValidate, invoiceTypeIds)) {
                                return 'Two or more invoice report files have the same invoice type.';
                            }
                        }
                        return null;
                        function validateId(name, array) {
                            for (var j = 0; j < array.length; j++) {
                                if (array[j] == name)
                                    return false;
                            }
                            return true;
                        }
                    }
                    return null;
                };
                getInvoiceTypes().then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    $scope.scopeModel.invoiceReportItems = [];
                    if (payload != undefined && payload.invoiceReportFiles != undefined) {
                        for (var invoiceTypeId in payload.invoiceReportFiles) {
                            if (invoiceTypeId != "$type") {
                                var currentItem = payload.invoiceReportFiles[invoiceTypeId];
                                var gridItem = {
                                    invoiceTypeId: invoiceTypeId,
                                    invoiceReportFileEntity: currentItem,
                                    invoiceSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
                                    invoiceSelectorLoadDeferred: UtilsService.createPromiseDeferred(),
                                    invoiceReportFileSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
                                    invoiceReportFileSelectorLoadDeferred: UtilsService.createPromiseDeferred(),
                                };
                                addItemtoGrid(gridItem);
                            }
                        }
                    }

                    function addItemtoGrid(gridItem) {
                        promises.push(gridItem.invoiceSelectorLoadDeferred.promise);
                        promises.push(gridItem.invoiceReportFileSelectorLoadDeferred.promise);

                        gridItem.onInvoiceTypeSelectorReady = function (api) {
                            gridItem.invoiceTypeSelectorAPI = api;
                            gridItem.invoiceSelectorReadyDeferred.resolve();
                        };

                        gridItem.onInvoiceReportFileSelectorReady = function (api) {
                            gridItem.invoiceReportFileSelectorAPI = api;
                            gridItem.invoiceReportFileSelectorReadyDeferred.resolve();
                        };
                        gridItem.onInvoiceTypeSelectionChanged = function (selectedInvoiceType) {
                            var invoiceTypeIdSelected;
                            if (selectedInvoiceType != undefined) {
                                invoiceTypeIdSelected = selectedInvoiceType.InvoiceTypeId;
                            }
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingInvoiceReportFiles = value;
                            };
                            var invoiceReportFileSelectorPayload = {
                                businessEntityDefinitionId: "64f8db86-691d-4486-83fb-26a3d3fc095e",
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.Invoice.Business.InvoiceReportFileFilter, Vanrise.Invoice.Business",
                                        InvoiceTypeId: invoiceTypeIdSelected
                                    }]
                                },
                                selectIfSingleItem: true
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.invoiceReportFileSelectorAPI, invoiceReportFileSelectorPayload, setLoader, gridItem.invoiceTypeSelectedPromiseDeferred);
                        };
                        UtilsService.waitMultiplePromises([gridItem.invoiceSelectorReadyDeferred.promise, gridItem.invoiceReportFileSelectorReadyDeferred.promise]).then(function () {
                            function loadInvoiceTypeSelector() {
                                if (gridItem.invoiceTypeId != undefined) {
                                    gridItem.invoiceTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                                }
                                gridItem.invoiceSelectorReadyDeferred.promise.then(function () {
                                    var invoiceSelectorPayload = {
                                        selectedIds: gridItem.invoiceTypeId
                                    };
                                    VRUIUtilsService.callDirectiveLoad(gridItem.invoiceTypeSelectorAPI, invoiceSelectorPayload, gridItem.invoiceSelectorLoadDeferred);
                                });

                                return gridItem.invoiceSelectorLoadDeferred.promise;
                            }

                            function loadInvoiceReportFileSelector() {
                                var promises = [gridItem.invoiceReportFileSelectorReadyDeferred.promise];
                                if (gridItem.invoiceTypeSelectedPromiseDeferred != undefined) {
                                    promises.push(gridItem.invoiceTypeSelectedPromiseDeferred.promise);
                                }
                                UtilsService.waitMultiplePromises(promises).then(function () {
                                    var invoiceReportFileSelectorPayload = {
                                        businessEntityDefinitionId: "64f8db86-691d-4486-83fb-26a3d3fc095e",
                                        selectedIds: gridItem.invoiceReportFileEntity != undefined && gridItem.invoiceReportFileEntity.InvoiceReportFileId ? gridItem.invoiceReportFileEntity.InvoiceReportFileId : undefined,
                                        filter: {
                                            Filters: [{
                                                $type: "Vanrise.Invoice.Business.InvoiceReportFileFilter, Vanrise.Invoice.Business",
                                                InvoiceTypeId: gridItem.invoiceTypeId
                                            }]
                                        },
                                        selectIfSingleItem:true
                                        };
                                    VRUIUtilsService.callDirectiveLoad(gridItem.invoiceReportFileSelectorAPI, invoiceReportFileSelectorPayload, gridItem.invoiceReportFileSelectorLoadDeferred);
                                });
                                return gridItem.invoiceReportFileSelectorLoadDeferred.promise;
                            }

                            UtilsService.waitMultiplePromises([loadInvoiceTypeSelector(), loadInvoiceReportFileSelector()]).then(function () {
                                gridItem.invoiceTypeSelectedPromiseDeferred = undefined;
                            });

                        });
                        $scope.scopeModel.invoiceReportItems.push(gridItem);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    var invoiceReportItems;

                    if ($scope.scopeModel.invoiceReportItems != undefined) {
                        invoiceReportItems = {};
                        for (var i = 0; i < $scope.scopeModel.invoiceReportItems.length; i++) {
                            var currentItem = $scope.scopeModel.invoiceReportItems[i];
                            var invoiceTypeId = currentItem.invoiceTypeSelectorAPI.getSelectedIds();
                            if(invoiceTypeId!=undefined){
                                invoiceReportItems[invoiceTypeId] = {
                                $type: "Vanrise.Entities.InvoiceReportFile, Vanrise.Entities",
                                InvoiceReportFileId: currentItem.invoiceReportFileSelectorAPI.getSelectedIds()
                                };
                            }
                        }
                    }
                    return invoiceReportItems;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getInvoiceTypes() {
                return VR_Invoice_InvoiceTypeAPIService.GetInvoiceTypesInfo().then(function (response) {
                    invoiceTypes = response;
                });
            }
        }

        return directiveDefinitionObject;

    }
]);