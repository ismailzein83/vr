'use strict';

app.directive('whsBeCasemanagementCustomercaseStaticeditor', ['UtilsService', 'VRUIUtilsService','VRDateTimeService','WhS_BE_SaleZoneAPIService',
    function (UtilsService, VRUIUtilsService, VRDateTimeService, WhS_BE_SaleZoneAPIService) {
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

            var statusSelectorAPI;
            var statusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            var reasonSelectorAPI;
            var reasonSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var workGroupSelectorAPI;
            var workGroupSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var attachmentGridAPI;
            var attachmentGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onReasonSelectorReady = function (api) {
                    reasonSelectorAPI = api;
                    reasonSelectorReadyPromiseDeferred.resolve();
                };
                $scope.onAttachmentGridReady = function (api) {
                    attachmentGridAPI = api;
                    attachmentGridReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onWorkGroupSelectorReady = function (api) {
                    workGroupSelectorAPI = api;
                    workGroupSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCustomerSelectorReady = function (api) {
                    customerSelectorAPI = api;
                    customerSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onStatusSelectorReady = function (api) {
                    statusSelectorAPI = api;
                    statusSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCodeChanged = function () {
                    getCustomerSaleZoneByCode();
                };
                $scope.scopeModel.onCustomerSelectionChanged = function (value) {

                    if (value != undefined) {
                        getCustomerSaleZoneByCode();
                    }
                };
                UtilsService.waitMultiplePromises([customerSelectorReadyPromiseDeferred.promise, statusSelectorReadyPromiseDeferred.promise, reasonSelectorReadyPromiseDeferred.promise, workGroupSelectorReadyPromiseDeferred.promise,attachmentGridReadyPromiseDeferred.promise]).then(function () {
                    defineApi();

                });
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        if (selectedValues != undefined) {
                            $scope.scopeModel.isEditMode = true;
                            $scope.scopeModel.ownerReference = selectedValues.OwnerReference;
                            $scope.scopeModel.fromDate = selectedValues.FromDate;
                            $scope.scopeModel.toDate = selectedValues.ToDate;
                            $scope.scopeModel.attempts = selectedValues.Attempts;
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
                    promises.push(loadWorkGroupSelector());
                    promises.push(loadAttachmentGrid());
                    return UtilsService.waitMultiplePromises(promises);
                };
                api.setData = function (caseManagementObject) {
                    if (!$scope.scopeModel.isEditMode) {
                        caseManagementObject.CustomerId = customerSelectorAPI.getSelectedIds();
                        caseManagementObject.FromDate = $scope.scopeModel.fromDate;
                        caseManagementObject.ToDate = $scope.scopeModel.toDate;
                        caseManagementObject.Attempts = $scope.scopeModel.attempts;
                        caseManagementObject.ASR = $scope.scopeModel.asr;
                        caseManagementObject.ACD = $scope.scopeModel.acd;
                        caseManagementObject.ReasonId = reasonSelectorAPI.getSelectedIds();
                        caseManagementObject.WorkGroupId = workGroupSelectorAPI.getSelectedIds();
                        caseManagementObject.Attachments = attachmentGridAPI.getData();
                    }
                    caseManagementObject.CarrierReference = $scope.scopeModel.carrierReference;
                    caseManagementObject.Description = $scope.scopeModel.description;
                    caseManagementObject.StatusId = statusSelectorAPI.getSelectedIds();
                    caseManagementObject.Attachments = attachmentGridAPI.getData();
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function loadStatusSelector() {
                var selectorPayload = {
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
            function loadAttachmentGrid() {
                var payload = {};
                if (selectedValues != undefined) {
                    payload.attachementFieldTypes= selectedValues.Attachments
                }
                return attachmentGridAPI.loadGrid(payload);
            }
            function loadWorkGroupSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "ef058aa2-2043-4c3b-adfd-36567784aef7"
                }
                if (selectedValues != undefined) {
                    selectorPayload.selectedIds = selectedValues.ReasonId;
                }
                return workGroupSelectorAPI.load(selectorPayload);
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


            function getCustomerSaleZoneByCode() {
                var codeNumber = $scope.scopeModel.codeNumber;
                var customerId = customerSelectorAPI.getSelectedIds();
                if (codeNumber != undefined && customerId != undefined) {
                    return WhS_BE_SaleZoneAPIService.GetCustomerSaleZoneByCode(customerId, codeNumber).then(function (response) {
                        if (response != undefined) {
                            $scope.scopeModel.zoneName = response.Name;
                        }

                    });
                }
            }

        }
        return directiveDefinitionObject;
    }]);