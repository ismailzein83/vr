﻿'use strict';

app.directive('whsBeCasemanagementCustomercaseStaticeditor', ['UtilsService', 'VRUIUtilsService', 'VRDateTimeService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_FaultTicketAPIService', 'WhS_BE_CarrierProfileAPIService',
    function (UtilsService, VRUIUtilsService, VRDateTimeService, WhS_BE_SaleZoneAPIService, WhS_BE_FaultTicketAPIService, WhS_BE_CarrierProfileAPIService) {
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

            var zoneId;
            var oldZoneId;

            var historyId;

            var customerSelectorAPI;
            var customerSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var selectedCustomerPromiseDeferred;

            var statusSelectorAPI;
            var statusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var releaseCodeSelectorAPI;
            var releaseCodeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var ticketContactSelectorAPI;
            var ticketContactSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var selectedTicketPromiseDeferred;

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
                    reasonSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.isReasonSelectorRequired = true;

                $scope.removerow = function (dataItem) {
                    if (!$scope.scopeModel.isEditMode) {
                        var index = $scope.scopeModel.codeNumberList.indexOf(dataItem);
                        $scope.scopeModel.codeNumberList.splice(index, 1);
                    }
                };

                $scope.scopeModel.onTicketContactSelectionChanged = function () {
                    var carrierProfileTicketInfo = ticketContactSelectorAPI.getSelectedValues();
                    if (carrierProfileTicketInfo != undefined) {
                        if (selectedTicketPromiseDeferred != undefined)
                            selectedTicketPromiseDeferred.resolve();
                        else {
                            $scope.scopeModel.contactName = carrierProfileTicketInfo.NameDescription;
                            $scope.scopeModel.email = carrierProfileTicketInfo.Emails.join(';');
                            $scope.scopeModel.phoneNumber = carrierProfileTicketInfo.PhoneNumber.join(';');
                        }
                    }
                };

                $scope.scopeModel.codeNumberListHasItem = function () {
                    if ($scope.scopeModel.codeNumberList.length != 0)
                        return null;
                    return "You must add at least one description";
                };

                $scope.scopeModel.validateDate = function (date) {
                    return UtilsService.validateDates($scope.scopeModel.fromDate, $scope.scopeModel.toDate);
                };

                $scope.scopeModel.InternationalReleaseCodeSelectorReady = function (api) {
                    releaseCodeSelectorAPI = api;
                    releaseCodeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.onAttachmentGridReady = function (api) {
                    attachmentGridAPI = api;
                    attachmentGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onTicketContactSelectorReady = function (api) {
                    ticketContactSelectorAPI = api;
                    ticketContactSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onWorkGroupSelectorReady = function (api) {
                    workGroupSelectorAPI = api;
                    workGroupSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCustomerSelectorReady = function (api) {
                    customerSelectorAPI = api;
                    customerSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.disableAddCodeNumber = function () {
                    return !(codeNumber != undefined && selectedCustomer != undefined && reasonSelectorAPI.getSelectedIds() != undefined);
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
                        if (selectedCustomerPromiseDeferred != undefined)
                            selectedCustomerPromiseDeferred.resolve();
                        else
                        {
                            var setLoader = function (value) { $scope.scopeModel.isLoadingTicketDirective = value; };
                            var selectorPayload = {
                                filter: {
                                    CarrierAccountId: customerSelectorAPI.getSelectedIds()
                                }
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, ticketContactSelectorAPI, selectorPayload, setLoader);
                            getAccountManager(value.CarrierAccountId);
                            $scope.scopeModel.contactName = undefined;
                            $scope.scopeModel.email = undefined;
                            $scope.scopeModel.phoneNumber = undefined;
                            selectedCustomer = customerSelectorAPI.getSelectedIds();
                            if (value.CarrierAccountId != selectedCustomer || !$scope.scopeModel.isEditMode) {
                                $scope.scopeModel.codeNumberList = [];
                            }
                            getCustomerSaleZoneByCode();
                        }

                    }
                };

                $scope.scopeModel.addCodeNumber = function () {

                    if ($scope.scopeModel.codeNumber == undefined)
                        $scope.scopeModel.errorMessage = "*The code number field is empty";
                    else {
                        if (doesReasonAlreadyExist())
                            $scope.scopeModel.errorMessage = "*Reason already exists";
                        else {
                            getCustomerSaleZoneByCode().then(function () {
                                var faultTicketDescription = {
                                    CodeNumber: $scope.scopeModel.codeNumber,
                                    ReasonId: $scope.scopeModel.reason.GenericBusinessEntityId,
                                    ReasonDescription: $scope.scopeModel.reason.Name,
                                    InternationalReleaseCodeId: ($scope.scopeModel.internationalReleaseCode != undefined) ? $scope.scopeModel.internationalReleaseCode.GenericBusinessEntityId : undefined,
                                    InternationalReleaseCodeDescription: ($scope.scopeModel.internationalReleaseCode != undefined) ? $scope.scopeModel.internationalReleaseCode.Name : undefined
                                };
                                if ((zoneId == oldZoneId || $scope.scopeModel.codeNumberList.length == 0) && $scope.scopeModel.zoneName != undefined) {
                                    $scope.scopeModel.codeNumberList.push(faultTicketDescription);
                                    $scope.scopeModel.errorMessage = undefined;
                                }
                                else {
                                    if (zoneId != oldZoneId)
                                        $scope.scopeModel.errorMessage = "*Unable to add a new customer case with different zone";
                                    if ($scope.scopeModel.zoneName == undefined)
                                        $scope.scopeModel.errorMessage = "*The code you entered does not belong to any specific zone";
                                }

                            });
                        }
                    }
                };

                UtilsService.waitMultiplePromises([customerSelectorReadyPromiseDeferred.promise, statusSelectorReadyPromiseDeferred.promise, reasonSelectorReadyPromiseDeferred.promise, workGroupSelectorReadyPromiseDeferred.promise, attachmentGridReadyPromiseDeferred.promise]).then(function () {
                    defineApi();

                });
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    var promises =[];
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        historyId= payload.historyId;
                        if (selectedValues != undefined) {

                            selectedCustomerPromiseDeferred = UtilsService.createPromiseDeferred();
                            if (selectedValues.EscalationLevelId != undefined) {
                                selectedTicketPromiseDeferred = UtilsService.createPromiseDeferred();
                                promises.push(selectedTicketPromiseDeferred.promise);
                            }

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

                            $scope.scopeModel.notes = selectedValues.Notes;
                            $scope.scopeModel.accountManager = selectedValues.AccountManager;
                            if ($scope.scopeModel.carrierReference != undefined) {
                                $scope.scopeModel.carrierReferenceHasValue = true;
                            }

                        }
                    }
                   
                    promises.push(loadCustomerSelector());
                    promises.push(loadStatusSelector());
                    promises.push(loadWorkGroupSelector());
                    promises.push(loadAttachmentGrid());

                    if (!$scope.scopeModel.isEditMode) {
                        promises.push(loadReleaseCodeSelector());
                        promises.push(loadReasonSelector());
                    }

                    if ($scope.scopeModel.isEditMode) {
                        $scope.scopeModel.isReasonSelectorRequired = false;
                        promises.push(getZoneName());
                        promises.push(loadCustomerFaultTicketDetails());
                        promises.push(loadTicketContactSelector());
                    }

                    return UtilsService.waitMultiplePromises(promises).finally(function () {
                        selectedCustomerPromiseDeferred = undefined;
                        selectedTicketPromiseDeferred = undefined;
                    });

                };

                api.setData = function (caseManagementObject) {
                    if (!$scope.scopeModel.isEditMode) {
                        caseManagementObject.CustomerId = customerSelectorAPI.getSelectedIds();
                        caseManagementObject.FromDate = $scope.scopeModel.fromDate;
                        caseManagementObject.ToDate = $scope.scopeModel.toDate;
                        caseManagementObject.Attempts = $scope.scopeModel.attempts;
                        caseManagementObject.ASR = $scope.scopeModel.asr;
                        caseManagementObject.ACD = $scope.scopeModel.acd;
                        caseManagementObject.WorkGroupId = workGroupSelectorAPI.getSelectedIds();
                        caseManagementObject.SaleZoneId = zoneId;
                        caseManagementObject.TicketDetails = {
                            $type: "TOne.WhS.BusinessEntity.Entities.CustomerFaultTicketDescriptionSettingCollection,TOne.WhS.BusinessEntity.Entities",
                            $values: getCodeNumberListData()
                        };
                    }
                    var attachments = attachmentGridAPI.getData();
                    caseManagementObject.CarrierReference = $scope.scopeModel.carrierReference;
                    caseManagementObject.Description = $scope.scopeModel.description;
                    caseManagementObject.StatusId = statusSelectorAPI.getSelectedIds();
                    caseManagementObject.Attachments = attachments == undefined ? null : attachments;
                    caseManagementObject.ContactName = $scope.scopeModel.contactName;
                    caseManagementObject.ContactEmails = $scope.scopeModel.email;
                    caseManagementObject.PhoneNumber = $scope.scopeModel.phoneNumber;
                    caseManagementObject.EscalationLevelId = ticketContactSelectorAPI.getSelectedIds();
                    caseManagementObject.SendEmail = $scope.scopeModel.sendEmail;
                    caseManagementObject.Notes = $scope.scopeModel.notes;
                    caseManagementObject.AccountManager = $scope.scopeModel.accountManager;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadStatusSelector() {
                var selectorPayload;

                selectorPayload = {
                    businessEntityDefinitionId: "c63202a1-438f-428e-b0fb-3a9bad708e9b",
                    filter: {
                        Filters: []
                    }
                };
                selectorPayload.filter.Filters.push(getStatusSelectorFiter());
                if (selectedValues != undefined)
                    selectorPayload.selectedIds = selectedValues.StatusId;
                selectorPayload.selectfirstitem = !$scope.scopeModel.isEditMode;

                return statusSelectorAPI.load(selectorPayload);
            }

            function loadReasonSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "b8daa0fa-d381-4bb0-b772-2e6b24d199e4"
                };
                return reasonSelectorAPI.load(selectorPayload);
            }

            function loadReleaseCodeSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "6e7c2b68-3e8e-49a1-8922-796b2ce9cc1c"
                };
                return releaseCodeSelectorAPI.load(selectorPayload);
            }

            function loadAttachmentGrid() {
                var payload = {};
                if (selectedValues != undefined) {
                    payload.attachementFieldTypes = selectedValues.Attachments;
                }
                return attachmentGridAPI.loadGrid(payload);
            }

            function getStatusSelectorFiter() {
                return {
                    $type: "TOne.WhS.BusinessEntity.Business.FaultTicketStatusDefinitionFilter,TOne.WhS.BusinessEntity.Business",
                    BusinessEntityDefinitionId: "e4053d52-8a52-438e-b353-37acf059a938",
                    StatusId: selectedValues != undefined ? selectedValues.StatusId : undefined
                };
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

            function getZoneName() {
                return WhS_BE_SaleZoneAPIService.GetSaleZoneName(selectedValues.SaleZoneId).then(function (response) {
                    if (response != undefined) {
                        $scope.scopeModel.zoneName = response;
                    }

                });
            }
            function getAccountManager(carrierAccountId) {
                WhS_BE_CarrierProfileAPIService.GetAccountManagerName(carrierAccountId).then(function (response) {
                    $scope.scopeModel.accountManager = response;
                });
            }

            function loadTicketContactSelector() {

                var promises = [];
                var directiveTicketLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(ticketContactSelectorReadyPromiseDeferred.promise);
                promises.push(selectedCustomerPromiseDeferred.promise);
                UtilsService.waitMultiplePromises(promises).then(function () {

                    $scope.scopeModel.contactName = selectedValues.ContactName;
                    $scope.scopeModel.email = selectedValues.ContactEmails;
                    $scope.scopeModel.phoneNumber = selectedValues.PhoneNumber;

                    var selectorPayload = {
                        filter: {
                            CarrierAccountId: customerSelectorAPI.getSelectedIds()
                        }
                    };
                    if (selectedValues != undefined) {
                        selectorPayload.selectedIds = selectedValues.EscalationLevelId;
                    }
                    VRUIUtilsService.callDirectiveLoad(ticketContactSelectorAPI, selectorPayload, directiveTicketLoadDeferred);
                });

                return directiveTicketLoadDeferred.promise;
            }

            function getCustomerFaultTicketInput() {

                var codeList = [];
                if (selectedValues.TicketDetails != null) {
                    for (var i = 0 ; i < selectedValues.TicketDetails.length; i++) {
                        var descriptionSettings = selectedValues.TicketDetails[i];
                        var customerCaseDescription = {
                            CodeNumber: descriptionSettings.CodeNumber,
                            ReasonId: descriptionSettings.ReasonId,
                        };
                        if (descriptionSettings.InternationalReleaseCodeId != undefined)
                            customerCaseDescription.InternationalReleaseCodeId = descriptionSettings.InternationalReleaseCodeId;
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
                var codeList;
                if ($scope.scopeModel.codeNumberList.length > 0)
                    codeList = [];
                for (var i = 0; i < $scope.scopeModel.codeNumberList.length; i++) {
                    var codeNumberObject = $scope.scopeModel.codeNumberList[i];
                    var faultTicketDescriptionSetting =
                        {
                            $type: "TOne.WhS.BusinessEntity.Entities.CustomerFaultTicketDescriptionSetting,TOne.WhS.BusinessEntity.Entities",
                            CodeNumber: codeNumberObject.CodeNumber,
                            ReasonId: codeNumberObject.ReasonId,
                        };
                    if (codeNumberObject.InternationalReleaseCodeId != undefined)
                        faultTicketDescriptionSetting.InternationalReleaseCodeId = codeNumberObject.InternationalReleaseCodeId;
                    codeList.push(faultTicketDescriptionSetting);
                }
                return codeList;
            }

            function loadCodeNumberListData() {
                var codeList = [];
                if (faultTicketDescriptionSettingEntity != undefined) {
                    for (var i = 0; i < faultTicketDescriptionSettingEntity.DescriptionSettings.length; i++) {
                        var descriptionSettings = faultTicketDescriptionSettingEntity.DescriptionSettings[i];
                        var faultTicketDescriptionSetting =
                            {
                                CodeNumber: descriptionSettings.CodeNumber,

                                ReasonId: descriptionSettings.ReasonId,
                                ReasonDescription: descriptionSettings.ReasonDescription,
                                InternationalReleaseCodeId: descriptionSettings != undefined ? descriptionSettings.InternationalReleaseCodeId : undefined,
                                InternationalReleaseCodeDescription: descriptionSettings != undefined ? descriptionSettings.InternationalReleaseCodeDescription : undefined,
                            };
                        codeList.push(faultTicketDescriptionSetting);
                    }
                }
                return codeList;
            }

            function loadCustomerFaultTicketDetails() {
                return WhS_BE_FaultTicketAPIService.GetCustomerFaultTicketDetails(getCustomerFaultTicketInput()).then(function (response) {
                    if (response != undefined) {
                        faultTicketDescriptionSettingEntity = response;
                        $scope.scopeModel.codeNumberList = loadCodeNumberListData();
                    }
                });
            }

            function getCustomerSaleZoneByCode() {
                var codeNumber = $scope.scopeModel.codeNumber;
                var customerId = customerSelectorAPI.getSelectedIds();
                if (codeNumber != undefined && customerId != undefined) {
                    $scope.scopeModel.isLoadingCustomerZone = true;
                    return WhS_BE_SaleZoneAPIService.GetCustomerSaleZoneByCode(customerId, codeNumber).then(function (response) {
                        $scope.scopeModel.isLoadingCustomerZone = false;

                        if (response != undefined) {
                            zoneId = response.SaleZoneId;
                            if ($scope.scopeModel.codeNumberList.length == 0 || oldZoneId == undefined)
                                oldZoneId = response.SaleZoneId;
                            $scope.scopeModel.zoneName = response.Name;
                        }
                        else {
                            zoneId = undefined;
                            $scope.scopeModel.zoneName = undefined;
                        }

                    });
                }
            }

            function doesReasonAlreadyExist() {
                if ($scope.scopeModel.codeNumberList != undefined) {
                    for (var i = 0; i < $scope.scopeModel.codeNumberList.length; i++) {
                        var codeNumber = $scope.scopeModel.codeNumberList[i];
                        if (codeNumber.ReasonId == reasonSelectorAPI.getSelectedIds() && codeNumber.CodeNumber == $scope.scopeModel.codeNumber)
                            return true;
                    }
                }
                return false;
            }

        }
        return directiveDefinitionObject;
    }]);