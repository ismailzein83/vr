(function (appControllers) {

    "use strict";

    genericInvoiceManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VR_Invoice_InvoiceActionService', 'VR_Invoice_InvoiceTypeAPIService', 'VR_Invoice_InvoiceAPIService', 'VRNotificationService', 'VRDateTimeService','VR_Invoice_InvoiceBulkActionService'];

    function genericInvoiceManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VR_Invoice_InvoiceActionService, VR_Invoice_InvoiceTypeAPIService, VR_Invoice_InvoiceAPIService, VRNotificationService, VRDateTimeService, VR_Invoice_InvoiceBulkActionService) {
        var invoiceTypeId;
        $scope.scopeModel = {};

        $scope.scopeModel.invoiceTypeEntity;
        var accountStatusSelectorAPI;
        var accountStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var partnerSelectorAPI;
        var partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var invoicePaidSelectorAPI;
        var invoicePaidSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceSentSelectorAPI;
        var invoiceSentSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceSettingSelectorAPI;
        var invoiceSettingSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            console.log(parameters);
            if (parameters != undefined && parameters != null) {
                invoiceTypeId = parameters.invoiceTypeId;
            }
        }

        function defineScope() {
           
            $scope.scopeModel.onAccountStatusSelectorReady = function (api) {
                accountStatusSelectorAPI = api;
                accountStatusSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountStatusSelectionChanged = function (value) {
                if (value != undefined) {
                    if (partnerSelectorAPI != undefined) {
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingDirective = value;
                        };
                        var partnerSelectorPayload = {
                            extendedSettings: $scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings.ExtendedSettings,
                            invoiceTypeId: invoiceTypeId,
                            filter: accountStatusSelectorAPI.getData()
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partnerSelectorAPI, partnerSelectorPayload, setLoader, partnerSelectorReadyDeferred);
                    }
                }
            };
            var date = VRDateTimeService.getNowDateTime();
            $scope.scopeModel.fromDate = new Date(date.getFullYear(), date.getMonth(), 1, 0, 0, 0, 0);
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                if (!$scope.scopeModel.isLoadingFilters)
                  gridAPI.loadGrid(getFilterObject());

            };
            $scope.scopeModel.onPartnerSelectorReady = function (api) {
                partnerSelectorAPI = api;
                partnerSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onInvoiceSettingSelectorReady = function (api) {
                invoiceSettingSelectorAPI = api;
                invoiceSettingSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject()).then(function () {
                    reEvaluateButtonsStatus();
                });
            };
            $scope.scopeModel.hasGenerateInvoicePermission = function () {
                return VR_Invoice_InvoiceAPIService.DoesUserHaveGenerateAccess(invoiceTypeId);
            };
            $scope.scopeModel.generateInvoice = generateInvoice;
            $scope.scopeModel.generateInvoices = generateInvoices;

            $scope.scopeModel.disableDeselectAll = true;
            $scope.scopeModel.disableBulkActions = true;
            $scope.scopeModel.disableSelectAll = true;

            $scope.scopeModel.deselectAllClicked = function () {
                gridAPI.deselectAllInvoices();
            };
            $scope.scopeModel.selectAllClicked = function () {
                gridAPI.selectAllInvoices();
            };
            $scope.scopeModel.bulkActionsClicked = function () {
                var  onBulkActionExecuted = function () {

                };
                VR_Invoice_InvoiceBulkActionService.openMenualInvoiceBulkActions(onBulkActionExecuted, invoiceTypeId, getContext());
            };

            $scope.scopeModel.onInvoicePaidSelectorReady = function (api) {
                invoicePaidSelectorAPI = api;
                invoicePaidSelectorReadyDeferred.resolve();
            };


            $scope.scopeModel.onInvoiceSentSelectorReady = function (api) {
                invoiceSentSelectorAPI = api;
                invoiceSentSelectorReadyDeferred.resolve();
            };

        }

        function load() {
            $scope.scopeModel.isLoadingFilters = true;
            getInvoiceTypeRuntime().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoadingFilters = false;
            });
           
        }
        function getFilterObject() {
            var partnerObject;
            if (partnerSelectorAPI !=undefined)
                partnerObject = partnerSelectorAPI.getData();

            var accountStatusObj = accountStatusSelectorAPI.getData();
            var filter = {
                mainGridColumns: $scope.scopeModel.invoiceTypeEntity.MainGridRuntimeColumns,
                subSections: $scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings.SubSections,
                invoiceGridActions: $scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings.InvoiceGridSettings.InvoiceGridActions,
                invoiceActions: $scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings.InvoiceActions,
                InvoiceTypeId: invoiceTypeId,
                invoiceItemGroupings: $scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings.ItemGroupings,
                context: getContext(),
                canSelectInvoices :$scope.scopeModel.showActionButtons,
                query: {
                    FromTime: $scope.scopeModel.fromDate,
                    ToTime: $scope.scopeModel.toDate,
                    PartnerIds: partnerObject != undefined ? partnerObject.selectedIds : undefined,
                    PartnerPrefix:partnerObject != undefined?partnerObject.partnerPrefix:undefined,
                    InvoiceTypeId: invoiceTypeId,
                    IssueDate: $scope.scopeModel.issueDate,
                    EffectiveDate: accountStatusObj != undefined ? accountStatusObj.EffectiveDate : undefined,
                    IsEffectiveInFuture: accountStatusObj != undefined ? accountStatusObj.IsEffectiveInFuture : undefined,
                    Status: accountStatusObj != undefined ? accountStatusObj.Status : undefined,
                    IsSent:invoiceSentSelectorAPI.getSelectedIds(),
                    IsPaid: invoicePaidSelectorAPI.getSelectedIds(),
                    InvoiceSettingIds: invoiceSettingSelectorAPI.getSelectedIds()
                }
            };
            return filter;
        }
        function loadAllControls() {


            function loadPartnerSelectorDirective() {
                var partnerSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                partnerSelectorReadyDeferred.promise.then(function () {
                    partnerSelectorReadyDeferred = undefined;
                    var partnerSelectorPayload = {
                        extendedSettings: $scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings.ExtendedSettings,
                        invoiceTypeId: invoiceTypeId,
                        filter: accountStatusSelectorAPI.getData()
                    };
                    VRUIUtilsService.callDirectiveLoad(partnerSelectorAPI, partnerSelectorPayload, partnerSelectorPayloadLoadDeferred);
                });
                return partnerSelectorPayloadLoadDeferred.promise;
            }

            function loadAccountStatusSelectorDirective() {
                var accountStatusSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                accountStatusSelectorReadyDeferred.promise.then(function () {
                    var accountStatusSelectorPayload = { selectFirstItem: true };
                    VRUIUtilsService.callDirectiveLoad(accountStatusSelectorAPI, accountStatusSelectorPayload, accountStatusSelectorPayloadLoadDeferred);
                });
                return accountStatusSelectorPayloadLoadDeferred.promise;
            }

            function loadInvoiceSentSelectorDirective() {
                var invoiceSentSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                invoiceSentSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(invoiceSentSelectorAPI, undefined, invoiceSentSelectorPayloadLoadDeferred);
                });
                return invoiceSentSelectorPayloadLoadDeferred.promise;
            }
            function loadInvoicePaidSelectorDirective() {
                var invoicePaidSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                invoicePaidSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(invoicePaidSelectorAPI, undefined, invoicePaidSelectorPayloadLoadDeferred);
                });
                return invoicePaidSelectorPayloadLoadDeferred.promise;
            }
            function loadInvoiceSetting() {
                var invoiceSettingDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                invoiceSettingSelectorReadyDeferred.promise.then(function () {
                    var filter = { InvoiceTypeId: invoiceTypeId };
                    var invoiceSettingPayload = { filter: filter };
                    
                    VRUIUtilsService.callDirectiveLoad(invoiceSettingSelectorAPI, invoiceSettingPayload, invoiceSettingDirectiveLoadPromiseDeferred);
                });
                return invoiceSettingDirectiveLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadPartnerSelectorDirective, loadAccountStatusSelectorDirective, loadInvoicePaidSelectorDirective, loadInvoiceSentSelectorDirective, loadInvoiceSetting]).then(function ()
            {
                if(gridAPI != undefined)
                {
                    gridAPI.loadGrid(getFilterObject());
                }
            })
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
               .finally(function () {
                   $scope.scopeModel.isLoadingFilters = false;
              });
        }

        function getInvoiceTypeRuntime()
        {
            return VR_Invoice_InvoiceTypeAPIService.GetInvoiceTypeRuntime(invoiceTypeId).then(function (response) {
                $scope.scopeModel.invoiceTypeEntity = response;
                if ($scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings.InvoiceMenualBulkActions != undefined && $scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings.InvoiceMenualBulkActions.length > 0)
                   $scope.scopeModel.showActionButtons = true;
            });
        }

        function generateInvoice() {
            var onGenerateInvoice = function (invoice) {
                gridAPI.onGenerateInvoice(invoice);
            };

            VR_Invoice_InvoiceActionService.generateInvoice(onGenerateInvoice, invoiceTypeId);
        }

        function generateInvoices() {
            var onGenerateInvoice = function (invoice) {
                //gridAPI.onGenerateInvoice(invoice);
            };

            VR_Invoice_InvoiceActionService.generateInvoices(onGenerateInvoice, invoiceTypeId);
        }

      

        function getContext()
        {
            var context = {
                reEvaluateButtonsStatus: function(){
                    reEvaluateButtonsStatus();
                },
                getTargetInvoicesEntity: function () {
                    return gridAPI.getTargetInvoicesEntity();
                },
                getInvoiceBulkActionIdentifier: function () {
                    return gridAPI.getInvoiceBulkActionIdentifier();
                }
            };
            return context;
        }
        function reEvaluateButtonsStatus()
        {
            var hasInvoices = gridAPI.hasInvoices();
            if (hasInvoices)
            {
                var isInvoiceSelected = gridAPI.anyInvoiceSelected();
                var allInvoicesSelected = gridAPI.allInvoiceSelected();

                if (allInvoicesSelected) {
                    $scope.scopeModel.disableDeselectAll = false;
                    $scope.scopeModel.disableBulkActions = false;
                    $scope.scopeModel.disableSelectAll = true;
                }
                else {
                    if (isInvoiceSelected) {
                        $scope.scopeModel.disableDeselectAll = false;
                        $scope.scopeModel.disableBulkActions = false;
                        $scope.scopeModel.disableSelectAll = false;
                    }
                    else {
                        $scope.scopeModel.disableDeselectAll = true;
                        $scope.scopeModel.disableBulkActions = true;
                        $scope.scopeModel.disableSelectAll = false;
                    }

                }
            } else {
                $scope.scopeModel.disableDeselectAll = true;
                $scope.scopeModel.disableBulkActions = true;
                $scope.scopeModel.disableSelectAll = true;
            }
           
        }

    }

    appControllers.controller('VR_Invoice_GenericInvoiceManagementController', genericInvoiceManagementController);
})(appControllers);