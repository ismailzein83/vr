'use strict';

app.directive('whsBeCasemanagementCustomercaseStaticeditor', ['UtilsService', 'VRUIUtilsService', 'VRDateTimeService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_FaultTicketAPIService',
    function (UtilsService, VRUIUtilsService, VRDateTimeService, WhS_BE_SaleZoneAPIService, WhS_BE_FaultTicketAPIService) {
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

            var faultTicketDescriptionSettingEntity;

            var codeNumber;
            var selectedCustomer;
            var selectedReason;

            var zoneId;
            var oldZoneId;

            var customerSelectorAPI;
            var customerSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var selectedCustomerPromiseDeferred;

            var statusSelectorAPI;
            var statusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var releaseCodeSelectorAPI;
            var releaseCodeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            var reasonSelectorAPI;
            var reasonSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var workGroupSelectorAPI;
            var workGroupSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var attachmentGridAPI;
            var attachmentGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.codeNumberList = [];
                $scope.scopeModel.onReasonSelectorReady = function (api) {
                    reasonSelectorAPI = api;
                    releaseCodeSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.InternationalReleaseCodeSelectorReady = function (api) {
                    releaseCodeSelectorAPI = api;
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
                $scope.scopeModel.hasAddSettingPermission = true;
                $scope.scopeModel.onCodeChanged = function () {
                    codeNumber = $scope.scopeModel.codeNumber;
                    getCustomerSaleZoneByCode();
                };
                $scope.scopeModel.onCustomerSelectionChanged = function (value) {
                    if (value != undefined) {
                        selectedCustomer = customerSelectorAPI.getSelectedIds();
                        getCustomerSaleZoneByCode();
                    }
                };
                $scope.scopeModel.onReasonSelectionChanged = function (value) {
                    selectedReason = reasonSelectorAPI.getSelectedIds();
                };
                $scope.scopeModel.addCodeNumber = function () {
                    var faultTicketDescription = {
                        CodeNumber: $scope.scopeModel.codeNumber,
                        Reason: $scope.scopeModel.reason,
                        InternationalReleaseCode: ($scope.scopeModel.internationalReleaseCode != undefined) ? $scope.scopeModel.internationalReleaseCode : undefined,
                    };
                    if (zoneId == oldZoneId  || $scope.scopeModel.codeNumberList.length == 0)
                        $scope.scopeModel.codeNumberList.push(faultTicketDescription);
                    else
                        $scope.scopeModel.errorMessage = "*Unable to add a new customer case with different zone ID:" + zoneId;
                };
                UtilsService.waitMultiplePromises([customerSelectorReadyPromiseDeferred.promise, statusSelectorReadyPromiseDeferred.promise, reasonSelectorReadyPromiseDeferred.promise, workGroupSelectorReadyPromiseDeferred.promise, attachmentGridReadyPromiseDeferred.promise]).then(function () {
                    defineApi();

                });
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        if (selectedValues != undefined) {
                            zoneId = selectedValues.SaleZoneId;
                            oldZoneId = selectedValues.SaleZoneId;
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
                        if ($scope.scopeModel.isEditMode) {
                            WhS_BE_FaultTicketAPIService.GetCustomerFaultTicketDetails(getCustomerFaultTicketInput()).then(function (response) {
                                if (response != undefined) {
                                    faultTicketDescriptionSettingEntity = response;
                                    $scope.scopeModel.codeNumberList = loadCodeNumberListData();
                                }

                            });
                        }
                    }
                    var promises = [];
                    promises.push(loadCustomerSelector());
                    promises.push(loadStatusSelector());
                    promises.push(loadReasonSelector());
                    promises.push(loadWorkGroupSelector());
                    promises.push(loadAttachmentGrid());
                    promises.push(loadReleaseCodeSelector());
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
                    }
                    caseManagementObject.CarrierReference = $scope.scopeModel.carrierReference;
                    caseManagementObject.Description = $scope.scopeModel.description;
                    caseManagementObject.StatusId = statusSelectorAPI.getSelectedIds();
                    caseManagementObject.Attachments = attachmentGridAPI.getData();
                    caseManagementObject.Settings = getCodeNumberListData();
                    caseManagementObject.SaleZoneId = zoneId;

                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function loadStatusSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "c63202a1-438f-428e-b0fb-3a9bad708e9b"
                };
                if (selectedValues != undefined) {
                    selectorPayload.selectedIds = selectedValues.StatusId;
                }
                return statusSelectorAPI.load(selectorPayload);
            }
            function disableAddCodeNumber() {
              return !(codeNumber != undefined && selectedCustomer != undefined && selectedReason != undefined);
            }
            function loadReasonSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "b8daa0fa-d381-4bb0-b772-2e6b24d199e4"
                };
                if (selectedValues != undefined) {
                    selectorPayload.selectedIds = selectedValues.ReasonId;
                }
                return reasonSelectorAPI.load(selectorPayload);
            }
            function loadReleaseCodeSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "6e7c2b68-3e8e-49a1-8922-796b2ce9cc1c"
                };
                if (selectedValues != undefined) {
                    selectorPayload.selectedIds = selectedValues.InternationalReleaseCodeId;
                }
                return releaseCodeSelectorAPI.load(selectorPayload);
            }
            function loadAttachmentGrid() {
                var payload = {};
                if (selectedValues != undefined) {
                    payload.attachementFieldTypes = selectedValues.Attachments;
                }
                return attachmentGridAPI.loadGrid(payload);
            }
            function loadWorkGroupSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "ef058aa2-2043-4c3b-adfd-36567784aef7"
                };
                if (selectedValues != undefined) {
                    selectorPayload.selectedIds = selectedValues.WorkGroupId;
                }
                return workGroupSelectorAPI.load(selectorPayload);
            }
            function loadCustomerSelector() {
                var selectorPayload;
                if (selectedValues != undefined) {
                    selectorPayload = {
                        selectedIds: selectedValues.CustomerId
                    };
                }
                return customerSelectorAPI.load(selectorPayload);
            }
            function getCustomerFaultTicketInput() {

                var codeList = [];
                if (selectedValues.Settings != null) {
                    for (var i = 0 ; i < selectedValues.Settings.DescriptionSettings.length; i++) {
                        var customerCaseDescription = {
                            CodeNumber: selectedValues.Settings.DescriptionSettings[i].CodeNumber,
                            ReasonId: selectedValues.Settings.DescriptionSettings[i].ReasonId,
                        }
                        if (selectedValues.Settings.DescriptionSettings[i].InternationalReleaseCodeId != undefined)
                            customerCaseDescription.InternationalReleaseCodeId = selectedValues.Settings.DescriptionSettings[i].InternationalReleaseCodeId;
                        codeList.push(customerCaseDescription);
                    }
                }
                var obj = {
                    DescriptionSettings: codeList,
                    ReasonBEDefinitionId: "b8daa0fa-d381-4bb0-b772-2e6b24d199e4",
                    ReleaseCodeBEDefinitionId: "6e7c2b68-3e8e-49a1-8922-796b2ce9cc1c"
                };

                return obj;
            }
            function getCodeNumberListData() {
                var customerFaultTicket = {
                    $type: "TOne.WhS.BusinessEntity.Entities.CustomerFaultTicketSettings,TOne.WhS.BusinessEntity.Entities",
                };
                var codeList = [];
                for (var i = 0; i < $scope.scopeModel.codeNumberList.length; i++) {
                    var faultTicketDescriptionSetting =
                        {
                            $type: "TOne.WhS.BusinessEntity.Entities.CustomerFaultTicketDescriptionSetting,TOne.WhS.BusinessEntity.Entities",
                            CodeNumber: $scope.scopeModel.codeNumberList[i].CodeNumber,
                            ReasonId: $scope.scopeModel.codeNumberList[i].Reason.GenericBusinessEntityId,
                        };
                    if ($scope.scopeModel.codeNumberList[i].InternationalReleaseCode != undefined)
                        faultTicketDescriptionSetting.InternationalReleaseCodeId = $scope.scopeModel.codeNumberList[i].InternationalReleaseCode.GenericBusinessEntityId;
                    codeList.push(faultTicketDescriptionSetting);
                }
                customerFaultTicket.DescriptionSettings = codeList;
                return customerFaultTicket;
            }
            function loadCodeNumberListData() {
                var codeList = [];
                if (faultTicketDescriptionSettingEntity != undefined) {
                    for (var i = 0; i < faultTicketDescriptionSettingEntity.DescriptionSettings.length; i++) {
                        var faultTicketDescriptionSetting =
                            {
                                CodeNumber: faultTicketDescriptionSettingEntity.DescriptionSettings[i].CodeNumber,
                                Reason:{
                                    ReasonId: faultTicketDescriptionSettingEntity.DescriptionSettings[i].ReasonId,
                                    Name: faultTicketDescriptionSettingEntity.DescriptionSettings[i].ReasonDescription
                                },
                                InternationalReleaseCode: {
                                    InternationalReleaseCodeId: faultTicketDescriptionSettingEntity.DescriptionSettings[i] != undefined ? faultTicketDescriptionSettingEntity.DescriptionSettings[i].InternationalReleaseCodeId : undefined,
                                    Name: faultTicketDescriptionSettingEntity.DescriptionSettings[i] != undefined ? faultTicketDescriptionSettingEntity.DescriptionSettings[i].InternationalReleaseCodeDescription : undefined,
                                }
                            };
                        codeList.push(faultTicketDescriptionSetting);
                    }
                }
                return codeList;
            }
            function getCustomerSaleZoneByCode() {
                var codeNumber = $scope.scopeModel.codeNumber;
                var customerId = customerSelectorAPI.getSelectedIds();
                if (codeNumber != undefined && customerId != undefined) {
                    return WhS_BE_SaleZoneAPIService.GetCustomerSaleZoneByCode(customerId, codeNumber).then(function (response) {
                        if (response != undefined) {
                            if (reasonSelectorAPI.selectedValues != undefined)
                            zoneId = response.SaleZoneId;
                            if ($scope.scopeModel.codeNumberList.length == 0 || oldZoneId == undefined)
                                oldZoneId = response.SaleZoneId;
                            $scope.scopeModel.zoneName = response.Name;

                        }

                    });
                }
            }

        }
        return directiveDefinitionObject;
    }]);