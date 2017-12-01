"use strict";

app.directive("vrInvoicePartnerinvoiecsettingSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VR_Invoice_PartnerInvoiceSettingService','VR_Invoice_PartnerInvoiceSettingAPIService','VR_Invoice_InvoiceTypeAPIService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, VR_Invoice_PartnerInvoiceSettingService, VR_Invoice_PartnerInvoiceSettingAPIService, VR_Invoice_InvoiceTypeAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var partnerInvoiceSettingSearch = new PartnerInvoiceSettingSearch($scope, ctrl, $attrs);
            partnerInvoiceSettingSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_Invoice/Directives/PartnerInvoiceSetting/Templates/PartnerInvoiceSettingSearch.html"

    };

    function PartnerInvoiceSettingSearch($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        var gridPromiseDeferred = UtilsService.createPromiseDeferred();

        var partnerSelectorAPI;
        var partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        var accountStatusSelectorAPI;
        var accountStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var accountStatusSelectedPromiseDeferred= UtilsService.createPromiseDeferred();

        var invoiceSettingPartsSelectorAPI;
        var invoiceSettingPartsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceSettingId;
        var invoiceTypeId;
        var invoiceTypeEntity;
        var partnerInvoiceSettingFilterFQTN;
        var partnerIds;
        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.onAccountStatusSelectorReady = function (api) {
                accountStatusSelectorAPI = api;
                accountStatusSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onPartnerSelectorReady = function (api) {
                partnerSelectorAPI = api;
                partnerSelectorReadyDeferred.resolve();

            };
            $scope.scopeModel.searchClicked = function () {
                return loadPartnerInvoiceSettingGrid();
            };
            $scope.scopeModel.onAccountStatusSelectionChanged = function (value) {
                if (value != undefined) {
                    if (accountStatusSelectedPromiseDeferred != undefined)
                        accountStatusSelectedPromiseDeferred.resolve();
                    else {
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingDirective = value;
                        };
                        var partnerSelectorPayload = {
                            extendedSettings: invoiceTypeEntity.Settings.ExtendedSettings,
                            invoiceTypeId: invoiceTypeId,
                            filter: accountStatusSelectorAPI.getData(),
                            partnerInvoiceFilters: [{
                                $type: partnerInvoiceSettingFilterFQTN,
                                OnlyAssignedToInvoiceSetting: true,
                                InvoiceSettingId: invoiceSettingId,
                                InvoiceTypeId: invoiceTypeId
                            }]
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partnerSelectorAPI, partnerSelectorPayload, setLoader);
                    }
                }
            };
            $scope.scopeModel.onInvoiceSettingPartsSelectorReady = function (api) {
                invoiceSettingPartsSelectorAPI = api;
                invoiceSettingPartsSelectorReadyDeferred.resolve();

            };
            $scope.scopeModel.linkPartner = function () {
                var onPartnerInvoiceSettingAdded = function (partnerInvoiceSettingObj) {
                    if (gridAPI != undefined) {
                        gridAPI.onPartnerInvoiceSettingAdded(partnerInvoiceSettingObj);
                    }
                };
                VR_Invoice_PartnerInvoiceSettingService.addPartnerInvoiceSetting(onPartnerInvoiceSettingAdded, invoiceTypeId, invoiceSettingId);
            };

            $scope.scopeModel.HasAssignPartnerAccess = function () {
                return VR_Invoice_PartnerInvoiceSettingAPIService.HasAssignPartnerAccess(invoiceSettingId);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridPromiseDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                $scope.scopeModel.isLoading = true;
                var promises = [];
                if (payload != undefined) {
                    invoiceSettingId = payload.invoiceSettingId;
                    invoiceTypeId = payload.invoiceTypeId;
                    partnerIds = payload.partnerIds;
                    $scope.scopeModel.showAccountSelector = payload.showAccountSelector;
                    promises.push(getInvoicePartnerSelector());
                    promises.push(getInvoiceType());
                    promises.push(getPartnerInvoiceSettingFilterFQTN());
                }
               
                return UtilsService.waitMultiplePromises(promises).then(function () {
                    var promisesArr = [];
                    promisesArr.push(loadAccountStatusSelectorDirective());
                    promisesArr.push(loadPartnerSelectorDirective());
                    promisesArr.push(loadInvoiceSettingPartsSelectorDirective());
                    UtilsService.waitMultiplePromises(promisesArr).then(function () {
                        loadPartnerInvoiceSettingGrid();
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                  
                });
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function getFilterObject() {
            var filteredPartnerIds;
            if ((partnerIds == undefined || partnerIds.length == 0) && partnerSelectorAPI != undefined) {
                var partnerObj = partnerSelectorAPI.getData();
                if (partnerObj != undefined)
                    filteredPartnerIds = partnerObj.selectedIds;
            }
            var filter = {
                invoiceTypeId: invoiceTypeId,
                query: {
                    InvoiceSettingId: invoiceSettingId,
                    PartnerIds: (partnerIds == undefined || partnerIds.length == 0) ? filteredPartnerIds : partnerIds,
                    PartsConfigIds: invoiceSettingPartsSelectorAPI.getSelectedIds(),
                }
            };
            var accountStatusFilter = accountStatusSelectorAPI.getData();
            if(accountStatusFilter != undefined)
            {
                filter.query.IsEffectiveInFuture = accountStatusFilter.IsEffectiveInFuture;
                filter.query.Status = accountStatusFilter.Status;
                filter.query.EffectiveDate = accountStatusFilter.EffectiveDate;
            }
            return filter;
        }

        function loadAccountStatusSelectorDirective() {
            var accountStatusSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
            accountStatusSelectorReadyDeferred.promise.then(function () {
                var accountStatusSelectorPayload = { selectFirstItem: true };
                VRUIUtilsService.callDirectiveLoad(accountStatusSelectorAPI, accountStatusSelectorPayload, accountStatusSelectorPayloadLoadDeferred);
            });
            return accountStatusSelectorPayloadLoadDeferred.promise;
        }
        function loadPartnerSelectorDirective() {
            var partnerSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
            UtilsService.waitMultiplePromises([accountStatusSelectedPromiseDeferred.promise, partnerSelectorReadyDeferred.promise]).then(function () {
                accountStatusSelectedPromiseDeferred = undefined;
                var partnerSelectorPayload = {
                    extendedSettings: invoiceTypeEntity.Settings.ExtendedSettings,
                    invoiceTypeId: invoiceTypeId,
                    filter: accountStatusSelectorAPI.getData(),
                    partnerInvoiceFilters:[{
                        $type: partnerInvoiceSettingFilterFQTN,
                        OnlyAssignedToInvoiceSetting:true,
                        InvoiceSettingId: invoiceSettingId,
                        InvoiceTypeId:invoiceTypeId
                    }]
                };
                //if (partnerSelectorPayload.filter == undefined)
                //    partnerSelectorPayload.filter = {};
                //partnerSelectorPayload.filter.Filters = [{
                //    $type: partnerInvoiceSettingFilterFQTN,
                //    OnlyAssignedToInvoiceSetting:true,
                //    InvoiceSettingId:invoiceSettingId
                //}];
                VRUIUtilsService.callDirectiveLoad(partnerSelectorAPI, partnerSelectorPayload, partnerSelectorPayloadLoadDeferred);
            });
            return partnerSelectorPayloadLoadDeferred.promise;
        }

        function getPartnerInvoiceSettingFilterFQTN() {
            return VR_Invoice_InvoiceTypeAPIService.GetPartnerInvoiceSettingFilterFQTN(invoiceTypeId).then(function (response) {
                partnerInvoiceSettingFilterFQTN = response;
            });
        }
        function loadInvoiceSettingPartsSelectorDirective() {
            var invoiceSettingPartsSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
            invoiceSettingPartsSelectorReadyDeferred.promise.then(function () {
                var invoiceSettingPartsSelectorPayload = {
                    invoiceTypeId:invoiceTypeId,
                    filter:{
                        InvoiceTypeId: invoiceTypeId,
                        OnlyIsOverridable:true
                    }
                };
                VRUIUtilsService.callDirectiveLoad(invoiceSettingPartsSelectorAPI, invoiceSettingPartsSelectorPayload, invoiceSettingPartsSelectorPayloadLoadDeferred);
            });
            return invoiceSettingPartsSelectorPayloadLoadDeferred.promise;
        }
        function getInvoicePartnerSelector() {
            partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            return VR_Invoice_InvoiceTypeAPIService.GetInvoicePartnerSelector(invoiceTypeId).then(function (response) {
                $scope.scopeModel.partnerInvoiceSelector = response;
            });
        }

        function getInvoiceType() {
            return VR_Invoice_InvoiceTypeAPIService.GetInvoiceType(invoiceTypeId).then(function (response) {
                invoiceTypeEntity = response;
            });
        }

        function loadPartnerInvoiceSettingGrid() {
            return gridPromiseDeferred.promise.then(function () {
                gridAPI.loadGrid(getFilterObject());
            });
        }
    }

    return directiveDefinitionObject;

}]);
