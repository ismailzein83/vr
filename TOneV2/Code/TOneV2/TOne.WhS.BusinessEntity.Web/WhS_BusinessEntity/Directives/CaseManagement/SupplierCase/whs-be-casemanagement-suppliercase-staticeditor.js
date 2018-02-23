'use strict';

app.directive('whsBeCasemanagementSuppliercaseStaticeditor', ['UtilsService', 'VRUIUtilsService','WhS_BE_FaultTicketAPIService','WhS_BE_SupplierZoneAPIService',
    function (UtilsService, VRUIUtilsService, WhS_BE_FaultTicketAPIService, WhS_BE_SupplierZoneAPIService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new supplierCaseStaticEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CaseManagement/SupplierCase/Templates/SupplierCaseStaticEditorDefinitionTemplate.html"
        };

        function supplierCaseStaticEditor(ctrl, $scope, $attrs) {

            var selectedValues;
            var codeNumber;
            var selectedSupplier;
            var selectedReason;

            var zoneId;
            var oldZoneId;

            var historyId;

            var faultTicketDescriptionSettingEntity;

            var supplierSelectorAPI;
            var supplierSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var selectedSupplierPromiseDeferred;

            var ticketContactSelectorAPI;
            var ticketContactSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var statusSelectorAPI;
            var statusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var workGroupSelectorAPI;
            var workGroupSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var customerSelectorAPI;
            var customerSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var selectedCustomerPromiseDeferred;

            var reasonSelectorAPI;
            var reasonSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var releaseCodeSelectorAPI;
            var releaseCodeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var attachmentGridAPI;
            var attachmentGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.codeNumberList = [];

                $scope.scopeModel.onSupplierSelectorReady = function (api) {
                    supplierSelectorAPI = api;
                    supplierSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCodeChanged = function () {
                    codeNumber = $scope.scopeModel.codeNumber;
                    getSupplierZoneByCode();
                };

                $scope.onAttachmentGridReady = function (api) {
                    attachmentGridAPI = api;
                    attachmentGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.validateDate = function (date) {
                    return UtilsService.validateDates($scope.scopeModel.fromDate, $scope.scopeModel.toDate);
                };

                $scope.scopeModel.onTicketContactSelectionChanged = function () {
                    var carrierProfileTicketInfo = ticketContactSelectorAPI.getSelectedValues();
                    if (carrierProfileTicketInfo != undefined ) {
                        $scope.scopeModel.contactName = carrierProfileTicketInfo.NameDescription;
                        $scope.scopeModel.email = carrierProfileTicketInfo.Emails.join(';');
                        $scope.scopeModel.phoneNumber = carrierProfileTicketInfo.PhoneNumber.join(';');
                    }
                };

                $scope.scopeModel.codeNumberListHasItem = function () {
                    if ($scope.scopeModel.codeNumberList.length != 0)
                        return null;
                    return "You must add at least one description";
                };

                $scope.removerow = function (dataItem) {
                    if (!$scope.scopeModel.isEditMode) {
                        var index = $scope.scopeModel.codeNumberList.indexOf(dataItem);
                        $scope.scopeModel.codeNumberList.splice(index, 1);
                    }
                };

                $scope.scopeModel.InternationalReleaseCodeSelectorReady = function (api) {
                    releaseCodeSelectorAPI = api;
                    releaseCodeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.addCodeNumber = function () {

                    if ($scope.scopeModel.codeNumber == undefined)
                        $scope.scopeModel.errorMessage = "*The code number field is empty";
                    else {
                        if (doesReasonAlreadyExist())
                            $scope.scopeModel.errorMessage = "*Reason already exists";
                        else {
                            getSupplierZoneByCode().then(function () {
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
                                        $scope.scopeModel.errorMessage = "*Unable to add a new supplier case with different zone";
                                    if ($scope.scopeModel.zoneName == undefined)
                                        $scope.scopeModel.errorMessage = "*The code you entered does not belong to any specific zone";
                                }

                            });
                        }
                    }
                };

                $scope.scopeModel.isReasonSelectorRequired = true;

                $scope.scopeModel.onReasonSelectorReady = function (api) {
                    reasonSelectorAPI = api;
                    reasonSelectorReadyPromiseDeferred.resolve();
                 
                };

                $scope.scopeModel.disableAddCodeNumber = function () {
                    return !(codeNumber != undefined && selectedSupplier != undefined && selectedReason != undefined);
                };

                $scope.scopeModel.onReasonSelectionChanged = function (value) {
                    selectedReason = reasonSelectorAPI.getSelectedIds();
                };

                $scope.scopeModel.onTicketContactSelectorReady = function (api) {
                    ticketContactSelectorAPI = api;
                    ticketContactSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onStatusSelectorReady = function (api) {
                    statusSelectorAPI = api;
                    statusSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onWorkGroupSelectorReady = function (api) {
                    workGroupSelectorAPI = api;
                    workGroupSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.isEditMode = false;

                $scope.scopeModel.onSupplierSelectionChanged = function (value) {
                   
                    if (value != undefined) {
                        if (selectedSupplierPromiseDeferred != undefined)
                            selectedSupplierPromiseDeferred.resolve();
                        else
                        {
                            selectedSupplier = supplierSelectorAPI.getSelectedIds();

                            if (value.CarrierAccountId != selectedSupplier || !$scope.scopeModel.isEditMode) {
                                $scope.scopeModel.codeNumberList = [];
                            }

                            getSupplierZoneByCode();

                            var setLoader = function (value) { $scope.scopeModel.isLoadingTicketDirective = value; };
                            var selectorPayload = {
                                filter: {
                                    CarrierAccountId: supplierSelectorAPI.getSelectedIds()
                                }
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, ticketContactSelectorAPI, selectorPayload, setLoader);

                            $scope.scopeModel.contactName = undefined;
                            $scope.scopeModel.email = undefined;
                            $scope.scopeModel.phoneNumber = undefined;
                        }
                       
                    }
                };

                UtilsService.waitMultiplePromises([supplierSelectorReadyPromiseDeferred.promise, reasonSelectorReadyPromiseDeferred.promise, statusSelectorReadyPromiseDeferred.promise, workGroupSelectorReadyPromiseDeferred.promise, releaseCodeSelectorReadyPromiseDeferred.promise, ticketContactSelectorReadyPromiseDeferred.promise, attachmentGridReadyPromiseDeferred.promise]).then(function () {
                    defineApi();
                });
            }

            function defineApi() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        historyId = payload.historyId;

                        if (selectedValues != undefined) {
                            $scope.scopeModel.isEditMode = true;
                            if (selectedValues.SupplierId != undefined && selectedValues.SupplierZoneId != undefined)
                                selectedSupplierPromiseDeferred = UtilsService.createPromiseDeferred();

                            $scope.scopeModel.fromDate = selectedValues.FromDate;
                            $scope.scopeModel.toDate = selectedValues.ToDate;
                            $scope.scopeModel.attempts = selectedValues.Attempts;
                            $scope.scopeModel.asr = selectedValues.ASR;
                            $scope.scopeModel.acd = selectedValues.ACD;
                            $scope.scopeModel.carrierReference = selectedValues.CarrierReference;
                            $scope.scopeModel.description = selectedValues.Description;
                            $scope.scopeModel.notes = selectedValues.Notes;
                            $scope.scopeModel.zoneName = selectedValues.SupplierZoneName;
                            $scope.scopeModel.notes = selectedValues.Notes;
                            $scope.scopeModel.contactName = selectedValues.ContactName;
                            $scope.scopeModel.email = selectedValues.ContactEmails;
                            $scope.scopeModel.phoneNumber = selectedValues.PhoneNumber;
                        }
                        if ($scope.scopeModel.isEditMode) {
                            getZoneName();
                            $scope.scopeModel.isReasonSelectorRequired = false;
                            WhS_BE_FaultTicketAPIService.GetSupplierFaultTicketDetails(getSupplierFaultTicketInput()).then(function (response) {
                                if (response != undefined) {
                                    faultTicketDescriptionSettingEntity = response;
                                    $scope.scopeModel.codeNumberList = loadCodeNumberListData();
                                }

                            });
                        }
                    }
                    var promises = [];

                    promises.push(loadSupplierSelector());
                    promises.push(loadStatusSelector());
                    promises.push(loadWorkGroupSelector());
                    promises.push(loadAttachmentGrid());

                    if (!$scope.scopeModel.isEditMode)
                    {
                        promises.push(loadReasonSelector());
                        promises.push(loadReleaseCodeSelector());
                    }

                    if ($scope.scopeModel.isEditMode)
                    {
                        promises.push(loadTicketContactSelector());
                    }

                    return UtilsService.waitMultiplePromises(promises).finally(function () {
                        selectedSupplierPromiseDeferred = undefined;
                    });
                };

                api.setData = function (caseManagementObject) {
                    if (!$scope.scopeModel.isEditMode) {
                        caseManagementObject.WorkGroupId = workGroupSelectorAPI.getSelectedIds();
                        caseManagementObject.ASR = $scope.scopeModel.asr;
                        caseManagementObject.ACD = $scope.scopeModel.acd;
                       
                        caseManagementObject.Attempts = $scope.scopeModel.attempts;
                        caseManagementObject.SupplierId = supplierSelectorAPI.getSelectedIds();
                        caseManagementObject.SupplierZoneId = zoneId;
                        caseManagementObject.FromDate = $scope.scopeModel.fromDate;
                        caseManagementObject.ToDate = $scope.scopeModel.toDate;
                    }
                    var attachments = attachmentGridAPI.getData();
                    caseManagementObject.TicketDetails = getCodeNumberListData();
                    caseManagementObject.Attachments = attachments == undefined ? null : attachments;
                    caseManagementObject.CarrierReference = $scope.scopeModel.carrierReference;
                    caseManagementObject.Description = $scope.scopeModel.description;
                    caseManagementObject.Notes = $scope.scopeModel.notes;
                    caseManagementObject.StatusId = statusSelectorAPI.getSelectedIds();
                    caseManagementObject.EscalationLevelId = ticketContactSelectorAPI.getSelectedIds();
                    caseManagementObject.SendEmail = $scope.scopeModel.sendEmail;
                    caseManagementObject.ContactName = $scope.scopeModel.contactName;
                    caseManagementObject.ContactEmails = $scope.scopeModel.email;
                    caseManagementObject.PhoneNumber = $scope.scopeModel.phoneNumber;
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

            function loadSupplierSelector() {
                var selectorPayload;
                if (selectedValues != undefined) {
                    selectorPayload = {
                        selectedIds: selectedValues.SupplierId
                    };
                }
                return supplierSelectorAPI.load(selectorPayload);
            }

            function loadTicketContactSelector() {
                var selectorPayload = {
                    filter: {
                        CarrierAccountId: supplierSelectorAPI.getSelectedIds()
                    }
                };
                if (selectedValues != undefined) {
                    selectorPayload.selectedIds = selectedValues.EscalationLevelId;
                }
                return ticketContactSelectorAPI.load(selectorPayload);
            }

            function loadAttachmentGrid() {
                var payload = {};
                if (selectedValues != undefined) {
                    payload.attachementFieldTypes = selectedValues.Attachments;
                }
                return attachmentGridAPI.loadGrid(payload);
            }

            function getStatusSelectorFiter() {
                return  {
                    $type: "TOne.WhS.BusinessEntity.Business.FaultTicketStatusDefinitionFilter,TOne.WhS.BusinessEntity.Business",
                    BusinessEntityDefinitionId : "551d5b27-a4fb-44e8-82cc-e19fdc1e97ca",
                    StatusId: selectedValues != undefined ? selectedValues.StatusId : undefined
                };
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

            function loadWorkGroupSelector() {
                var selectorPayload = {
                    businessEntityDefinitionId: "ef058aa2-2043-4c3b-adfd-36567784aef7"
                };
                if (selectedValues != undefined) {
                    selectorPayload.selectedIds = selectedValues.WorkGroupId;
                }
                return workGroupSelectorAPI.load(selectorPayload);
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

            function getSupplierFaultTicketInput() {

                var codeList = [];
                if (selectedValues.TicketDetails != null) {
                    for (var i = 0 ; i < selectedValues.TicketDetails.length; i++) {
                        var descriptionSettings = selectedValues.TicketDetails[i];
                        var supplierCaseDescription = {
                            CodeNumber: descriptionSettings.CodeNumber,
                            ReasonId: descriptionSettings.ReasonId,
                        };
                        if (descriptionSettings.InternationalReleaseCodeId != undefined)
                            supplierCaseDescription.InternationalReleaseCodeId = descriptionSettings.InternationalReleaseCodeId;
                        codeList.push(supplierCaseDescription);
                    }
                }
                var obj = {
                    DescriptionSettings: codeList,
                    ReasonBEDefinitionId: "b8daa0fa-d381-4bb0-b772-2e6b24d199e4",
                    ReleaseCodeBEDefinitionId: "6e7c2b68-3e8e-49a1-8922-796b2ce9cc1c"
                };

                return obj;
            }

            function getSupplierZoneByCode() {
                var codeNumber = $scope.scopeModel.codeNumber;
                var supplierId = supplierSelectorAPI.getSelectedIds();
                if (codeNumber != undefined && supplierId != undefined) {
                    $scope.scopeModel.isLoadingSupplierZone = true;

                    return WhS_BE_SupplierZoneAPIService.GetSupplierZoneByCode(supplierId, codeNumber).then(function (response) {
                        $scope.scopeModel.isLoadingSupplierZone = false;

                        if (response != undefined) {

                            zoneId = response.SupplierZoneId;
                            if ($scope.scopeModel.codeNumberList.length == 0 || oldZoneId == undefined)
                                oldZoneId = response.SupplierZoneId;
                            $scope.scopeModel.zoneName = response.Name;
                        }
                        else {
                            zoneId = undefined;
                            $scope.scopeModel.zoneName = undefined;
                        }

                    });
                }
            }

            function getZoneName() {
                if (selectedValues != undefined && selectedValues.SupplierZoneId != undefined)
                {
                    var supplierZoneId = selectedValues.SupplierZoneId;
                    return WhS_BE_SupplierZoneAPIService.GetSupplierZoneName(supplierZoneId).then(function (response) {
                        if (response != undefined) {
                            $scope.scopeModel.zoneName = response;
                        }
                    
                    });
                }
            }

            function getCodeNumberListData() {
                    var codeList = [];
                    for (var i = 0; i < $scope.scopeModel.codeNumberList.length; i++) {
                        var codeNumberObject = $scope.scopeModel.codeNumberList[i];
                        var faultTicketDescriptionSetting =
                            {
                                $type: "TOne.WhS.BusinessEntity.Entities.SupplierFaultTicketDescriptionSetting,TOne.WhS.BusinessEntity.Entities",
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