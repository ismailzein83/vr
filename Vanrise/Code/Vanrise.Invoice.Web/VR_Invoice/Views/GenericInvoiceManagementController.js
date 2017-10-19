(function (appControllers) {

    "use strict";

    genericInvoiceManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VR_Invoice_InvoiceActionService', 'VR_Invoice_InvoiceTypeAPIService', 'VR_Invoice_InvoiceAPIService', 'VRNotificationService', 'VRDateTimeService'];

    function genericInvoiceManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VR_Invoice_InvoiceActionService, VR_Invoice_InvoiceTypeAPIService, VR_Invoice_InvoiceAPIService, VRNotificationService, VRDateTimeService) {
        var invoiceTypeId;
        $scope.invoiceTypeEntity;
        var accountStatusSelectorAPI;
        var accountStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var partnerSelectorAPI;
        var partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        var gridAPI;
        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceTypeId = parameters.invoiceTypeId;
            }
        }

        function defineScope() {

            $scope.onAccountStatusSelectorReady = function (api) {
                accountStatusSelectorAPI = api;
                accountStatusSelectorReadyDeferred.resolve();
            };

            $scope.onAccountStatusSelectionChanged = function (value) {
                if (value != undefined) {
                    if (partnerSelectorAPI != undefined) {
                        var setLoader = function (value) {
                            $scope.isLoadingDirective = value;
                        };
                        var partnerSelectorPayload = {
                            extendedSettings: $scope.invoiceTypeEntity.InvoiceType.Settings.ExtendedSettings,
                            invoiceTypeId: invoiceTypeId,
                            filter: accountStatusSelectorAPI.getData()
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partnerSelectorAPI, partnerSelectorPayload, setLoader, partnerSelectorReadyDeferred);
                    }
                }
            };
            var date = VRDateTimeService.getNowDateTime();
            $scope.fromDate = new Date(date.getFullYear(), date.getMonth(), 1, 0, 0, 0, 0);
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (!$scope.isLoadingFilters)
                  gridAPI.loadGrid(getFilterObject());

            };
            $scope.onPartnerSelectorReady = function (api) {
                partnerSelectorAPI = api;
                partnerSelectorReadyDeferred.resolve();
            };
            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());

            };
            $scope.hasGenerateInvoicePermission = function () {
                return VR_Invoice_InvoiceAPIService.DoesUserHaveGenerateAccess(invoiceTypeId);
            };
            $scope.generateInvoice = generateInvoice;
            $scope.generateInvoices = generateInvoices;
        }

        function load() {
            $scope.isLoadingFilters = true;
            getInvoiceTypeRuntime().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoadingFilters = false;
            });
           
        }
        function getFilterObject() {
            var partnerObject;
            if (partnerSelectorAPI !=undefined)
                partnerObject = partnerSelectorAPI.getData();

            var accountStatusObj = accountStatusSelectorAPI.getData();
            var filter = {
                mainGridColumns: $scope.invoiceTypeEntity.MainGridRuntimeColumns,
                subSections: $scope.invoiceTypeEntity.InvoiceType.Settings.SubSections,
                invoiceGridActions: $scope.invoiceTypeEntity.InvoiceType.Settings.InvoiceGridSettings.InvoiceGridActions,
                invoiceActions:$scope.invoiceTypeEntity.InvoiceType.Settings.InvoiceActions,
                InvoiceTypeId: invoiceTypeId,
                invoiceItemGroupings: $scope.invoiceTypeEntity.InvoiceType.Settings.ItemGroupings,
                query: {
                    FromTime: $scope.fromDate,
                    ToTime: $scope.toDate,
                    PartnerIds: partnerObject != undefined ? partnerObject.selectedIds : undefined,
                    PartnerPrefix:partnerObject != undefined?partnerObject.partnerPrefix:undefined,
                    InvoiceTypeId: invoiceTypeId,
                    IssueDate: $scope.issueDate,
                    EffectiveDate: accountStatusObj != undefined ? accountStatusObj.EffectiveDate : undefined,
                    IsEffectiveInFuture: accountStatusObj != undefined ? accountStatusObj.IsEffectiveInFuture : undefined,
                    Status: accountStatusObj != undefined ? accountStatusObj.Status : undefined,
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
                        extendedSettings: $scope.invoiceTypeEntity.InvoiceType.Settings.ExtendedSettings,
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

            return UtilsService.waitMultipleAsyncOperations([loadPartnerSelectorDirective, loadAccountStatusSelectorDirective]).then(function ()
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
                  $scope.isLoadingFilters = false;
              });
        }

        function getInvoiceTypeRuntime()
        {
            return VR_Invoice_InvoiceTypeAPIService.GetInvoiceTypeRuntime(invoiceTypeId).then(function (response) {
               $scope.invoiceTypeEntity = response;
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

    }

    appControllers.controller('VR_Invoice_GenericInvoiceManagementController', genericInvoiceManagementController);
})(appControllers);