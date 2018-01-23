'use strict';

app.directive('whsBeCasemanagementCustomercaseStaticeditor', ['UtilsService', 'VRUIUtilsService','VRDateTimeService',
    function (UtilsService, VRUIUtilsService, VRDateTimeService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new cutomerCaseStaticEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CaseManagement/CustomerCase/Templates/CustomerCaseStaticEditorDefinitionTemplate.html"
        };

        function cutomerCaseStaticEditor(ctrl, $scope, $attrs) {

            var selectedValues;

            var customerSelectorAPI;
            var customerSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var selectedCustomerPromiseDeferred;

            var saleZoneSelectorApi;
            var saleZoneSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var statusSelectorAPI;
            var statusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            var reasonSelectorAPI;
            var reasonSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onReasonSelectorReady = function (api) {
                    reasonSelectorAPI = api;
                    reasonSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCustomerSelectorReady = function (api) {
                    customerSelectorAPI = api;
                    customerSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onStatusSelectorReady = function (api) {
                    statusSelectorAPI = api;
                    statusSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSaleZoneSelectorReady = function (api) {
                    saleZoneSelectorApi = api;
                    saleZoneSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCustomerSelectionChanged = function (value) {

                    if(value != undefined)
                    {
                        if (selectedCustomerPromiseDeferred != undefined)
                            selectedCustomerPromiseDeferred.resolve();
                        else
                        {
                            var setLoader = function (value) { $scope.scopeModel.isLoadingSaleZoneDirective = value; };
                            var payload = {
                                sellingNumberPlanId:value.SellingNumberPlanId,
                                filter: {
                                    Filters: [{
                                        $type: 'TOne.WhS.BusinessEntity.Business.SaleZoneSoldCountryToCustomerFilter, TOne.WhS.BusinessEntity.Business',
                                        CustomerId: customerSelectorAPI.getSelectedIds(),
                                        EffectiveOn: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime()),
                                        IsEffectiveInFuture: true
                                    }]
                                }
                            }
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneSelectorApi, payload, setLoader);
                        }
                    }
                };
                UtilsService.waitMultiplePromises([customerSelectorReadyPromiseDeferred.promise, statusSelectorReadyPromiseDeferred.promise, reasonSelectorReadyPromiseDeferred.promise]).then(function () {
                    defineApi();

                });
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        if(selectedValues != undefined)
                        {
                            if (selectedValues.CustomerId != undefined && selectedValues.SaleZoneId != undefined)
                            {
                                 selectedCustomerPromiseDeferred = UtilsService.createPromiseDeferred();
                            }
                            $scope.scopeModel.fromDate = selectedValues.FromDate;
                            $scope.scopeModel.toDate=  selectedValues.ToDate;
                            $scope.scopeModel.attempts= selectedValues.Attempts;
                            $scope.scopeModel.asr = selectedValues.ASR;
                            $scope.scopeModel.acd = selectedValues.ACD;
                            $scope.scopeModel.carrierReference = selectedValues.CarrierReference;
                            $scope.scopeModel.description = selectedValues.Description;
                            $scope.scopeModel.name = selectedValues.Name;
                        }
                    }
                    var promises = [];
                    promises.push(loadCustomerSelector());
                    promises.push(loadStatusSelector());
                    promises.push(loadReasonSelector());
                    if (selectedValues != undefined && selectedValues.CustomerId != undefined && selectedValues.SaleZoneId != undefined)
                    {
                        promises.push(loadSaleZoneSelector());
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };
                api.setData = function (caseManagementObject) {
                    caseManagementObject.CustomerId = customerSelectorAPI.getSelectedIds();
                    caseManagementObject.SaleZoneId = saleZoneSelectorApi.getSelectedIds();
                    caseManagementObject.FromDate = $scope.scopeModel.fromDate;
                    caseManagementObject.ToDate = $scope.scopeModel.toDate;
                    caseManagementObject.Attempts = $scope.scopeModel.attempts;
                    caseManagementObject.ASR = $scope.scopeModel.asr;
                    caseManagementObject.ACD = $scope.scopeModel.acd;
                    caseManagementObject.CarrierReference = $scope.scopeModel.carrierReference;
                    caseManagementObject.Description = $scope.scopeModel.description;
                    caseManagementObject.StatusId = statusSelectorAPI.getSelectedIds();
                    caseManagementObject.Name = $scope.scopeModel.name;
                    caseManagementObject.ReasonId = reasonSelectorAPI.getSelectedIds();
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function loadStatusSelector() {
                var selectorPayload= {
                    businessEntityDefinitionId: "c63202a1-438f-428e-b0fb-3a9bad708e9b"
                }
                if (selectedValues != undefined) {
                    selectorPayload.selectedIds = selectedValues.StatusId;
                }
                return statusSelectorAPI.load(selectorPayload);
            }
            function loadReasonSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "b8daa0fa-d381-4bb0-b772-2e6b24d199e4"
                }
                if (selectedValues != undefined) {
                    selectorPayload.selectedIds = selectedValues.ReasonId;
                }
                return reasonSelectorAPI.load(selectorPayload);
            }
            function loadCustomerSelector() {
                var selectorPayload;
                if (selectedValues != undefined) {
                    selectorPayload = {
                        selectedIds: selectedValues.CustomerId
                    }
                }
                return customerSelectorAPI.load(selectorPayload);
            }
            function loadSaleZoneSelector() {
                var saleZoneSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([saleZoneSelectorReadyPromiseDeferred.promise, selectedCustomerPromiseDeferred.promise]).then(function () {
                    selectedCustomerPromiseDeferred = undefined;
                    var selectedCustomer = customerSelectorAPI.getSelectedValues();
                    var saleZonePayload = {
                        selectedIds: [selectedValues.SaleZoneId],
                        sellingNumberPlanId: selectedCustomer.SellingNumberPlanId,
                        filter:{
                            Filters:[{
                                $type: 'TOne.WhS.BusinessEntity.Business.SaleZoneSoldCountryToCustomerFilter, TOne.WhS.BusinessEntity.Business',
                                CustomerId: selectedValues.CustomerId,
                                EffectiveOn: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime()),
                                IsEffectiveInFuture: true
                            }]
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(saleZoneSelectorApi, saleZonePayload, saleZoneSelectorLoadPromiseDeferred);
                });
                return saleZoneSelectorLoadPromiseDeferred.promise;
            }
        }
        return directiveDefinitionObject;
    }]);