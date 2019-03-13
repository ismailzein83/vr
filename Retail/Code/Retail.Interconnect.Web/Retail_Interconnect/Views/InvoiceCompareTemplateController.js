(function (appControllers) {

    'use strict';

    invoiceCompareTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_Invoice_InvoiceAPIService', 'VR_ExcelConversion_FieldTypeEnum', 'VR_Invoice_InvoiceTypeAPIService', 'Retail_Interconnect_InvoiceAPIService', 'Retail_Interconnect_Invoice_ComparisonVoiceCriteriaEnum', 'LabelColorsEnum', 'Retail_Interconnect_Invoice_ComparisonResultEnum', 'UISettingsService', 'Retail_Interconnect_InvoiceCompareTemplateAPIService', 'VRCommon_VRTempPayloadAPIService', 'InsertOperationResultEnum', 'Retail_Interconnect_Invoice_ComparisonSMSCriteriaEnum', 'Retail_Interconnect_InvoiceCompareService', 'Retail_Interconnect_RetailModuleService'];

    function invoiceCompareTemplateController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_Invoice_InvoiceAPIService, VR_ExcelConversion_FieldTypeEnum, VR_Invoice_InvoiceTypeAPIService, Retail_Interconnect_InvoiceAPIService, Retail_Interconnect_Invoice_ComparisonVoiceCriteriaEnum, LabelColorsEnum, Retail_Interconnect_Invoice_ComparisonResultEnum, UISettingsService, Retail_Interconnect_InvoiceCompareTemplateAPIService, VRCommon_VRTempPayloadAPIService, InsertOperationResultEnum, Retail_Interconnect_Invoice_ComparisonSMSCriteriaEnum, Retail_Interconnect_InvoiceCompareService, Retail_Interconnect_RetailModuleService) {

        var invoiceAccountEntity;
        var invoiceCarrierType;
        var invoiceId;
        var invoiceTypeId;
        var invoiceActionId;
        var partnerId;
        var invoiceEntity;
        var invoiceActionEntity;

        var voiceListPayload = {};
        var smsListPayload = {};

        var voiceMainListAPI;
        var smsMainListAPI;
        var mappingTabsAPI;

        var mappingTabsAPI;
        var mappingTabsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var voiceMainListMappingReadyPromiseDeferred;
        var smsMainListMappingReadyPromiseDeferred;

        var voiceEnableSelectionChangePromise;
        var smsEnableSelectionChangePromise;

        var voiceGridAPI;
        var voiceGridPromiseDeferred = UtilsService.createPromiseDeferred();
        var smsGridAPI;
        var smsGridPromiseDeferred = UtilsService.createPromiseDeferred();
        var inputWorkBookApi;
        var invoiceCompareTemplateEntity;
        var financialAccountId;

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                invoiceId = parameters.invoiceId;
                invoiceTypeId = parameters.invoiceTypeId;
                invoiceActionId = parameters.invoiceActionId;
                invoiceCarrierType = parameters.invoiceCarrierType;
                financialAccountId = parameters.partnerId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.isVoiceModuleEnabled = Retail_Interconnect_RetailModuleService.isVoiceModuleEnabled();
            $scope.scopeModel.isSMSModuleEnabled = Retail_Interconnect_RetailModuleService.isSMSModuleEnabled();
            $scope.scopeModel.voiceEnabled = false;
            $scope.scopeModel.smsEnabled = false;
            $scope.scopeModel.voiceTab = {};
            $scope.scopeModel.smsTab = {};

            $scope.scopeModel.voiceTab.showTab = false;
            $scope.scopeModel.smsTab.showTab = false;

            $scope.scopeModel.recievedVoiceComparisonTab = {};
            $scope.scopeModel.recievedSMSComparisonTab = {};

            $scope.scopeModel.recievedVoiceComparisonTab.showTab = false;
            $scope.scopeModel.recievedSMSComparisonTab.showTab = false;

            $scope.scopeModel.isCompare = false;

            $scope.scopeModel.export = function () {
                var objToCompare = {
                    Settings: {
                        $type: "Retail.Interconnect.Entities.InvoiceComparisonVRTempPayload,Retail.Interconnect.Entities"
                    }
                };

                if ($scope.scopeModel.voiceEnabled)
                    objToCompare.Settings.VoiceInput = buildInvoiceCompareVoiceObjFromScope();

                if ($scope.scopeModel.smsEnabled)
                    objToCompare.Settings.SMSInput = buildInvoiceCompareSMSObjFromScope();

                objToCompare.Settings.IsCustomer = (invoiceCarrierType == 0);
                objToCompare.Settings.FinancialAccountId = financialAccountId;
                objToCompare.Settings.InvoiceId = invoiceId;

                return VRCommon_VRTempPayloadAPIService.AddVRTempPayload(objToCompare).then(function (response) {
                    if (response.Result == InsertOperationResultEnum.Succeeded.value) {
                        var tempPayloadId = response.InsertedObject;
                        var paramsurl = "";
                        paramsurl += "tempPayloadId=" + tempPayloadId;
                        var screenWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
                        var left = ((screenWidth / 2) - (1000 / 2));
                        window.open("Client/Modules/Retail_Interconnect/Reports/CompareInvoiceReport.aspx?" + paramsurl, "_blank", "toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=yes, copyhistory=no,width=1000, height=600,scrollbars=1 , top = 40, left = " + left + "");
                    }
                }).catch(function (error) {
                });
            };

            $scope.scopeModel.voiceThreshold = 5;
            $scope.scopeModel.smsThreshold = 5;
            $scope.scopeModel.voiceDecimalDigits = 2;
            $scope.scopeModel.smsDecimalDigits = 2;
            $scope.scopeModel.voiceComparisonResults = [];
            $scope.scopeModel.smsComparisonResults = [];
            $scope.scopeModel.voiceDateTimeFormat = "yyyy-MM-dd";
            $scope.scopeModel.smsDateTimeFormat = "yyyy-MM-dd";
            $scope.scopeModel.voiceComparisonResult = [];
            $scope.scopeModel.smsComparisonResult = [];
            $scope.scopeModel.comparisonCriterias = [];
            $scope.scopeModel.selectedVoiceComparisonCriterias = [];
            $scope.scopeModel.selectedSMSComparisonCriterias = [];

            $scope.scopeModel.thresholdHint = 'Represents the max acceptable "%"<br> of difference between the System compared to the Supplier referring to<br> the formula:<div style="text-align:center;">(SystemValue-SupplierValue)<br>%=--------------------------------------*100<br>(SystemValue)</div>';
            $scope.scopeModel.reset = function () {
                VRNotificationService.showConfirmation('Configurations will be lost, are you sure you want to continue?').then(function (response) {
                    if (response) {

                        if ($scope.scopeModel.voiceEnabled) {
                            voiceListPayload = {
                                context: getContext(),
                                fieldMappings: [
                                    { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                                    { FieldName: "FromDate", FieldTitle: "From", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                                    { FieldName: "ToDate", FieldTitle: "Till", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                                    { FieldName: "Duration", FieldTitle: "Duration", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value },
                                    { FieldName: "NumberOfCalls", FieldTitle: "Calls", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Int.value },
                                    { FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value },
                                    { FieldName: "Currency", FieldTitle: "Currency", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                                    { FieldName: "Amount", FieldTitle: "Amount", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value }
                                ],
                                listName: "VoiceMainList",
                                showDateFormat: false
                            };

                            var setVoiceLoader = function (value) {
                                $scope.scopeModel.isVoiceLoadingDirective = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, voiceMainListAPI, voiceListPayload, setVoiceLoader);
                        };

                        if ($scope.scopeModel.smsEnabled) {
                            var smsListPayload = {
                                context: getContext(),
                                fieldMappings: [
                                    { FieldName: "MobileNetwork", FieldTitle: "Mobile Network", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                                    { FieldName: "FromDate", FieldTitle: "From", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                                    { FieldName: "ToDate", FieldTitle: "Till", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                                    { FieldName: "NumberOfSMSs", FieldTitle: "SMS", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Int.value },
                                    { FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value },
                                    { FieldName: "Currency", FieldTitle: "Currency", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                                    { FieldName: "Amount", FieldTitle: "Amount", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value }

                                ],
                                listName: "SMSMainList",
                                showDateFormat: false
                            };

                            var setSMSLoader = function (value) {
                                $scope.scopeModel.isSMSLoadingDirective = value;
                            };

                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, smsMainListAPI, smsListPayload, setSMSLoader);
                        }
                    }
                });
            };
            $scope.scopeModel.selectedVoiceComparisonResults = [];
            $scope.scopeModel.selectedSMSComparisonResults = [];

            $scope.scopeModel.onVoiceMainListMappingReady = function (api) {
                voiceMainListAPI = api;
                voiceMainListMappingReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSMSMainListMappingReady = function (api) {
                smsMainListAPI = api;
                smsMainListMappingReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onMappingTabsReady = function (api) {
                mappingTabsAPI = api;
                mappingTabsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                $scope.scopeModel.isLoading = true;
                return saveInvoiceCompareTemplate();
            };
            $scope.scopeModel.onVoiceGridReady = function (api) {
                voiceGridAPI = api;
                voiceGridPromiseDeferred.resolve();
            };
            $scope.scopeModel.onSMSGridReady = function (api) {
                smsGridAPI = api;
                smsGridPromiseDeferred.resolve();
            };
            $scope.scopeModel.onReadyWoorkBook = function (api) {
                inputWorkBookApi = api;
            };
            $scope.scopeModel.compare = function () {
                $scope.scopeModel.isLoading = true;
                $scope.scopeModel.voiceComparisonResults.length = 0;
                $scope.scopeModel.smsComparisonResults.length = 0;
                return startCompare();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.voiceDataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_Interconnect_InvoiceCompareService.CompareVoiceInvoices(dataRetrievalInput).then(function (response) {

                    if (response != undefined && response.Result != undefined) {
                        VRNotificationService.notifyOnItemAdded("Compare Invoice", response);
                    } else {
                        if (!($scope.scopeModel.smsEnabled && $scope.scopeModel.recievedSMSComparisonTab != undefined && $scope.scopeModel.recievedSMSComparisonTab.isSelected))
                            $scope.scopeModel.recievedVoiceComparisonTab.isSelected = true;
                        onResponseReady(response);
                    }

                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            $scope.scopeModel.smsDataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_Interconnect_InvoiceCompareService.CompareSMSInvoices(dataRetrievalInput).then(function (response) {

                    if (response != undefined && response.Result != undefined) {
                        VRNotificationService.notifyOnItemAdded("Compare Invoice", response);
                    } else {
                        if (!$scope.scopeModel.voiceEnabled && $scope.scopeModel.recievedSMSComparisonTab != undefined && !$scope.scopeModel.recievedSMSComparisonTab.isSelected)
                            $scope.scopeModel.recievedSMSComparisonTab.isSelected = true;

                        onResponseReady(response);
                    }

                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
            $scope.scopeModel.getResultColor = function (dataItem, coldef) {

                if (dataItem.Entity.ResultColor != undefined) {
                    var color = UtilsService.getEnum(LabelColorsEnum, 'value', dataItem.Entity.ResultColor);
                    if (color != undefined)
                        return color.color;
                }
            };
            $scope.scopeModel.getDiffCallsColor = function (dataItem, coldef) {
                if (dataItem.Entity.DiffCallsColor != undefined) {
                    var color = UtilsService.getEnum(LabelColorsEnum, 'value', dataItem.Entity.DiffCallsColor);
                    if (color != undefined)
                        return color.color;
                }
            };
            $scope.scopeModel.getDiffDurationColor = function (dataItem, coldef) {
                if (dataItem.Entity.DiffDurationColor != undefined) {
                    var color = UtilsService.getEnum(LabelColorsEnum, 'value', dataItem.Entity.DiffDurationColor);
                    if (color != undefined)
                        return color.color;
                }
            };
            $scope.scopeModel.getDiffAmountColor = function (dataItem, coldef) {
                if (dataItem.Entity.DiffAmountColor != undefined) {
                    var color = UtilsService.getEnum(LabelColorsEnum, 'value', dataItem.Entity.DiffAmountColor);
                    if (color != undefined)
                        return color.color;
                }
            };
            $scope.scopeModel.getDiffSMSsColor = function (dataItem, coldef) {
                if (dataItem.Entity.DiffSMSsColor != undefined) {
                    var color = UtilsService.getEnum(LabelColorsEnum, 'value', dataItem.Entity.DiffSMSsColor);
                    if (color != undefined)
                        return color.color;
                }
            };

            $scope.scopeModel.enableVoiceSelectionChange = function () {
                if ($scope.scopeModel.voiceEnabled != undefined) {

                    if (voiceEnableSelectionChangePromise != undefined) {
                        voiceEnableSelectionChangePromise.resolve();
                    }
                    else {
                        $scope.scopeModel.recievedVoiceComparisonTab.showTab = $scope.scopeModel.voiceEnabled;
                        if (mappingTabsAPI != undefined) {

                            $scope.scopeModel.voiceTab.showTab = $scope.scopeModel.voiceEnabled;
                            if ($scope.scopeModel.voiceEnabled) {

                                mappingTabsAPI.setTabSelected(0);
                                voiceMainListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                loadVoiceMainListListMapping();
                            }
                        };
                    }
                }
            }
            $scope.scopeModel.enableSMSSelectionChange = function () {

                if ($scope.scopeModel.smsEnabled != undefined) {

                    if (smsEnableSelectionChangePromise != undefined) {
                        smsEnableSelectionChangePromise.resolve();
                    }
                    else {
                        $scope.scopeModel.recievedSMSComparisonTab.showTab = $scope.scopeModel.smsEnabled;
                        if (mappingTabsAPI != undefined) {
                            $scope.scopeModel.smsTab.showTab = $scope.scopeModel.smsEnabled;
                            if ($scope.scopeModel.smsEnabled) {
                                mappingTabsAPI.setTabSelected(1);
                                smsMainListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                loadSMSMainListListMapping();
                            }
                        }
                    }
                };
            }

        }

        function load() {
            $scope.scopeModel.isLoading = true;

            UtilsService.waitMultipleAsyncOperations([getInvoice, getInvoiceAction]).then(function () {
                $scope.scopeModel.showVoiceGrid = true;
                $scope.scopeModel.showSMSGrid = true;

                Retail_Interconnect_InvoiceCompareTemplateAPIService.GetInvoiceCompareTemplate(invoiceTypeId, partnerId, invoiceCarrierType).then(function (response) {
                    invoiceCompareTemplateEntity = response;

                    loadAllControls();
                });

            });
        }

        function getInvoice() {
            return Retail_Interconnect_InvoiceAPIService.GetInvoiceDetails(invoiceId, invoiceCarrierType).then(function (response) {
                invoiceEntity = response;
                if (invoiceEntity != undefined) {
                    partnerId = invoiceEntity.PartnerId;
                    var normalPrecision = UISettingsService.getNormalPrecision();
                    $scope.scopeModel.issuedBy = invoiceEntity.IssuedBy;
                    $scope.scopeModel.to = invoiceEntity.To;
                    $scope.scopeModel.toDate = UtilsService.getShortDate(UtilsService.createDateFromString(invoiceEntity.ToDate));
                    $scope.scopeModel.fromDate = UtilsService.getShortDate(UtilsService.createDateFromString(invoiceEntity.FromDate));
                    $scope.scopeModel.issuedDate = UtilsService.getShortDate(UtilsService.createDateFromString(invoiceEntity.IssuedDate));
                    $scope.scopeModel.dueDate = UtilsService.getShortDate(UtilsService.createDateFromString(invoiceEntity.DueDate));

                    $scope.scopeModel.serialNumber = invoiceEntity.SerialNumber;
                    $scope.scopeModel.timeZone = invoiceEntity.TimeZone;
                    $scope.scopeModel.calls = invoiceEntity.Calls.toLocaleString();
                    $scope.scopeModel.duration = Number(invoiceEntity.Duration.toFixed(normalPrecision)).toLocaleString();
                    $scope.scopeModel.smss = invoiceEntity.TotalNumberOfSMS.toLocaleString();
                    $scope.scopeModel.totalAmount = Number(invoiceEntity.TotalAmount.toFixed(normalPrecision)).toLocaleString();
                    $scope.scopeModel.isLocked = invoiceEntity.IsLocked;
                    $scope.scopeModel.isPaid = invoiceEntity.IsPaid;
                    $scope.scopeModel.currency = invoiceEntity.Currency;

                }
            });
        }
        function getInvoiceAction() {

            return VR_Invoice_InvoiceTypeAPIService.GetInvoiceAction(invoiceTypeId, invoiceActionId).then(function (response) {
                invoiceActionEntity = response;
                if (invoiceActionEntity != undefined && invoiceActionEntity.Settings != undefined) {
                    $scope.scopeModel.partnerLabel = invoiceActionEntity.Settings.PartnerLabel;
                    $scope.scopeModel.partnerAbbreviationLabel = invoiceActionEntity.Settings.PartnerAbbreviationLabel;
                    $scope.scopeModel.diffCallPercentage = '(SystemCalls-' + $scope.scopeModel.partnerAbbreviationLabel + 'Calls)\n---------------------------*100\n\t(SystemCalls)';
                    $scope.scopeModel.diffCall = '(SystemCalls-' + $scope.scopeModel.partnerAbbreviationLabel + 'Calls)';
                    $scope.scopeModel.diffDurationPercentage = '(SystemDuration-' + $scope.scopeModel.partnerAbbreviationLabel + 'Duration)\n-----------------------------------*100\n\t(SystemDuration)';
                    $scope.scopeModel.diffDuration = '(SystemDuration-' + $scope.scopeModel.partnerAbbreviationLabel + 'Duration)';
                    $scope.scopeModel.diffAmountPercentage = '(SystemAmount-' + $scope.scopeModel.partnerAbbreviationLabel + 'Amount)\n----------------------------------*100\n\t(SystemAmount)';
                    $scope.scopeModel.diffAmount = '(SystemAmount-' + $scope.scopeModel.partnerAbbreviationLabel + 'Amount)';
                }
            });
        }

        function loadAllControls() {
            var promises = [];
            function setTitle() {

                $scope.title = 'Compare Invoice';
            }

            function loadStaticData() {
                if (invoiceAccountEntity != undefined) {
                }
                for (var prop in Retail_Interconnect_Invoice_ComparisonResultEnum) {
                    var enumObj = UtilsService.cloneObject(Retail_Interconnect_Invoice_ComparisonResultEnum[prop]);
                    if (enumObj.value == Retail_Interconnect_Invoice_ComparisonResultEnum.MissingProvider.value) {
                        enumObj.description = enumObj.description + $scope.scopeModel.partnerLabel;
                    }
                    $scope.scopeModel.voiceComparisonResult.push(enumObj);
                    $scope.scopeModel.smsComparisonResult.push(enumObj);
                }
                if (invoiceCompareTemplateEntity != undefined && invoiceCompareTemplateEntity.Details != undefined) {
                    if (invoiceCompareTemplateEntity.Details.VoiceTemplate != undefined) {

                        voiceEnableSelectionChangePromise = UtilsService.createPromiseDeferred();
                        voiceMainListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                        $scope.scopeModel.voiceEnabled = true;
                        $scope.scopeModel.recievedVoiceComparisonTab.showTab = true;

                        mappingTabsReadyPromiseDeferred.promise.then(function () {
                            $scope.scopeModel.voiceEnabled = true;
                            $scope.scopeModel.voiceTab.showTab = true;
                            mappingTabsAPI.setTabSelected(0);
                            loadVoiceMainListListMapping();
                        });
                        $scope.scopeModel.voiceDateTimeFormat = invoiceCompareTemplateEntity.Details.VoiceTemplate.DateTimeFormat;
                    };

                    if (invoiceCompareTemplateEntity.Details.SMSTemplate != undefined) {

                        smsEnableSelectionChangePromise = UtilsService.createPromiseDeferred();
                        smsMainListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                        $scope.scopeModel.smsEnabled = true;

                        mappingTabsReadyPromiseDeferred.promise.then(function () {
                            $scope.scopeModel.smsTab.showTab = true;
                            $scope.scopeModel.recievedSMSComparisonTab.showTab = true;

                            if (!$scope.scopeModel.voiceEnabled)
                                mappingTabsAPI.setTabSelected(1);
                            loadSMSMainListListMapping();
                        });

                        $scope.scopeModel.smsDateTimeFormat = invoiceCompareTemplateEntity.Details.SMSTemplate.DateTimeFormat;
                    };

                }

                $scope.scopeModel.voiceComparisonCriterias = UtilsService.getArrayEnum(Retail_Interconnect_Invoice_ComparisonVoiceCriteriaEnum);
                $scope.scopeModel.smsComparisonCriterias = UtilsService.getArrayEnum(Retail_Interconnect_Invoice_ComparisonSMSCriteriaEnum);
            }

            function loadExportButton() {
                var promise = UtilsService.createPromiseDeferred();
                Retail_Interconnect_InvoiceAPIService.DoesInvoiceReportExist(invoiceCarrierType == 0).then(function (response) {
                    $scope.scopeModel.isCompare = response;
                    promise.resolve();
                });
                return promise.promise;
            }
            promises.push(loadExportButton);
            promises.push(setTitle);
            promises.push(loadStaticData);


            return UtilsService.waitMultipleAsyncOperations(promises).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                voiceEnableSelectionChangePromise = undefined;
                smsEnableSelectionChangePromise = undefined;
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildInvoiceCompareVoiceObjFromScope() {
            var obj = {
                $type: "Retail.Interconnect.Entities.InvoiceComparisonVoiceInput,Retail.Interconnect.Entities",
                Threshold: $scope.scopeModel.voiceThreshold,
                ListMapping: voiceMainListAPI.getData(),
                DateTimeFormat: $scope.scopeModel.voiceDateTimeFormat,
                InvoiceTypeId: invoiceTypeId,
                InvoiceActionId: invoiceActionId,
                InvoiceId: invoiceId,
                InputFileId: $scope.scopeModel.inPutFile.fileId,
                ComparisonResults: UtilsService.getPropValuesFromArray($scope.scopeModel.selectedVoiceComparisonResults, "value"),
                ComparisonCriterias: UtilsService.getPropValuesFromArray($scope.scopeModel.selectedVoiceComparisonCriterias, "value"),
                DecimalDigits: $scope.scopeModel.voiceDecimalDigits
            };
            return obj;
        }

        function buildInvoiceCompareSMSObjFromScope() {
            var obj = {
                $type: "Retail.Interconnect.Entities.InvoiceComparisonSMSInput,Retail.Interconnect.Entities",
                Threshold: $scope.scopeModel.smsThreshold,
                ListMapping: smsMainListAPI.getData(),
                DateTimeFormat: $scope.scopeModel.smsDateTimeFormat,
                InvoiceTypeId: invoiceTypeId,
                InvoiceActionId: invoiceActionId,
                InvoiceId: invoiceId,
                InputFileId: $scope.scopeModel.inPutFile.fileId,
                ComparisonResults: UtilsService.getPropValuesFromArray($scope.scopeModel.selectedSMSComparisonResults, "value"),
                ComparisonCriterias: UtilsService.getPropValuesFromArray($scope.scopeModel.selectedSMSComparisonCriterias, "value"),
                DecimalDigits: $scope.scopeModel.smsDecimalDigits
            };
            return obj;
        }

        function loadVoiceMainListListMapping() {
            voiceMainListMappingReadyPromiseDeferred.promise.then(function () {
                voiceListPayload = {
                    context: getContext(),
                    fieldMappings: [
                        { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                        { FieldName: "FromDate", FieldTitle: "From", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                        { FieldName: "ToDate", FieldTitle: "Till", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                        { FieldName: "Duration", FieldTitle: "Duration", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value },
                        { FieldName: "NumberOfCalls", FieldTitle: "Calls", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Int.value },
                        { FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value },
                        { FieldName: "Currency", FieldTitle: "Currency", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                        { FieldName: "Amount", FieldTitle: "Amount", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value }

                    ],
                    listName: "VoiceMainList",
                    showDateFormat: false
                };
                if (invoiceCompareTemplateEntity != undefined && invoiceCompareTemplateEntity.Details != undefined && invoiceCompareTemplateEntity.Details.VoiceTemplate != undefined)
                    voiceListPayload.listMappingData = invoiceCompareTemplateEntity.Details.VoiceTemplate.ListMapping;

                var setVoiceLoader = function (value) {
                    $scope.scopeModel.isVoiceLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, voiceMainListAPI, voiceListPayload, setVoiceLoader);
            });
        }

        function loadSMSMainListListMapping() {
            smsMainListMappingReadyPromiseDeferred.promise.then(function () {
                smsListPayload = {
                    context: getContext(),
                    fieldMappings: [
                        { FieldName: "MobileNetwork", FieldTitle: "Destination Mobile Network", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                        { FieldName: "FromDate", FieldTitle: "From", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                        { FieldName: "ToDate", FieldTitle: "Till", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                        { FieldName: "NumberOfSMSs", FieldTitle: "SMS", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Int.value },
                        { FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value },
                        { FieldName: "Currency", FieldTitle: "Currency", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                        { FieldName: "Amount", FieldTitle: "Amount", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value }

                    ],
                    listName: "SMSMainList",
                    showDateFormat: false
                };
                if (invoiceCompareTemplateEntity != undefined && invoiceCompareTemplateEntity.Details != undefined && invoiceCompareTemplateEntity.Details.SMSTemplate != undefined)
                    smsListPayload.listMappingData = invoiceCompareTemplateEntity.Details.SMSTemplate.ListMapping;

                var setSMSLoader = function (value) {
                    $scope.scopeModel.isSMSLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, smsMainListAPI, smsListPayload, setSMSLoader);
            });
        }

        function getContext() {
            var context = {
                getSelectedCell: getSelectedCell,
                setSelectedCell: selectCellAtSheet,
                getSelectedSheet: getSelectedSheet
            };
            function selectCellAtSheet(row, col, s) {
                var a = parseInt(row);
                var b = parseInt(col);
                if (inputWorkBookApi != undefined && inputWorkBookApi.getSelectedSheetApi() != undefined)
                    inputWorkBookApi.selectCellAtSheet(a, b, s);
            }
            function getSelectedCell() {
                if (inputWorkBookApi != undefined && inputWorkBookApi.getSelectedSheetApi() != undefined)
                    return inputWorkBookApi.getSelectedSheetApi().getSelected();
            }
            function getSelectedSheet() {
                if (inputWorkBookApi != undefined)
                    return inputWorkBookApi.getSelectedSheet();
            }
            return context;
        }


        function startCompare() {
            $scope.scopeModel.showVoiceGrid = false;
            $scope.scopeModel.showSMSGrid = false;
            var loadPromiseDeferred = UtilsService.createPromiseDeferred();
            //$scope.scopeModel.isCompare = false;
            setTimeout(function () {
                var promises = [];

                $scope.scopeModel.showVoiceGrid = true;
                $scope.scopeModel.showSMSGrid = true;


                if ($scope.scopeModel.voiceEnabled) {
                    var loadVoicePromiseDeferred = UtilsService.createPromiseDeferred();
                    voiceGridPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(voiceGridPromiseDeferred.promise);
                    promises.push(loadVoicePromiseDeferred.promise);

                    voiceGridPromiseDeferred.promise.then(function () {
                        voiceGridPromiseDeferred = undefined;
                        var comparisonInput = buildInvoiceCompareVoiceObjFromScope();
                        voiceGridAPI.retrieveData(comparisonInput).then(function () {

                            loadVoicePromiseDeferred.resolve();
                        });
                    });
                };

                if ($scope.scopeModel.smsEnabled) {

                    var loadSMSPromiseDeferred = UtilsService.createPromiseDeferred();
                    smsGridPromiseDeferred = UtilsService.createPromiseDeferred();

                    promises.push(smsGridPromiseDeferred.promise);
                    promises.push(loadSMSPromiseDeferred.promise);

                    smsGridPromiseDeferred.promise.then(function () {
                        smsGridPromiseDeferred = undefined;
                        var comparisonInput =
                            buildInvoiceCompareSMSObjFromScope();

                        smsGridAPI.retrieveData(comparisonInput).then(function () {

                            loadSMSPromiseDeferred.resolve();
                        });
                    });
                };
                UtilsService.waitMultiplePromises(promises).finally(function () {
                    loadPromiseDeferred.resolve();
                    $scope.scopeModel.isLoading = false;
                });
            });

            return loadPromiseDeferred.promise;
        }
        function buildVoiceObjectFromScope() {

            var details = {};

            if ($scope.scopeModel.smsEnabled)

                details.SMSTemplate = {
                    ListMapping: smsMainListAPI.getData(),
                    DateTimeFormat: $scope.scopeModel.smsDateTimeFormat
                };

            if ($scope.scopeModel.voiceEnabled) {

                details.VoiceTemplate = {
                    ListMapping: voiceMainListAPI.getData(),
                    DateTimeFormat: $scope.scopeModel.voiceDateTimeFormat
                }

            };

            var obj = {
                InvoiceTypeId: invoiceTypeId,
                PartnerId: partnerId,
                Details: details
            };
            return obj;
        }


        function saveInvoiceCompareTemplate() {
            var invoiceComparisonTemplate = buildVoiceObjectFromScope();

            return Retail_Interconnect_InvoiceCompareTemplateAPIService.SaveInvoiceCompareTemplate(invoiceComparisonTemplate).then(function (response) {
                if (response == true) {
                    VRNotificationService.showSuccess("Added successfully");
                }
                else
                    VRNotificationService.showError("An error has occured");
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }

    appControllers.controller('Retail_Interconnect_InvoiceCompareTemplateController', invoiceCompareTemplateController);

})(appControllers);