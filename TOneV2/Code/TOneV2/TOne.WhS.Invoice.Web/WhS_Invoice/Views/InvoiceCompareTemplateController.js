(function (appControllers) {

    'use strict';

    invoiceCompareTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_Invoice_InvoiceAPIService', 'VR_ExcelConversion_FieldTypeEnum', 'VR_Invoice_InvoiceTypeAPIService', 'WhS_Invoice_InvoiceAPIService', 'WhS_Invoice_ComparisonResultEnum', 'LabelColorsEnum', 'WhS_Invoice_ComparisonCriteriaEnum', 'UISettingsService', 'WhS_Invoice_InvoiceCompareTemplateAPIService', 'VRCommon_VRTempPayloadAPIService', 'InsertOperationResultEnum', 'WhS_Invoice_SMSComparisonCriteriaEnum', 'WhS_Invoice_InvoiceCompareService', 'WhS_BE_ToneModuleService'];

    function invoiceCompareTemplateController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_Invoice_InvoiceAPIService, VR_ExcelConversion_FieldTypeEnum, VR_Invoice_InvoiceTypeAPIService, WhS_Invoice_InvoiceAPIService, WhS_Invoice_ComparisonResultEnum, LabelColorsEnum, WhS_Invoice_ComparisonCriteriaEnum, UISettingsService, WhS_Invoice_InvoiceCompareTemplateAPIService, VRCommon_VRTempPayloadAPIService, InsertOperationResultEnum, WhS_Invoice_SMSComparisonCriteriaEnum, WhS_Invoice_InvoiceCompareService, WhS_BE_ToneModuleService) {

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


        var voiceMainListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var smsMainListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var voiceGridAPI;
        var voiceGridPromiseDeferred = UtilsService.createPromiseDeferred();
        var smsGridAPI;
        var smsGridPromiseDeferred = UtilsService.createPromiseDeferred();
        var inputWorkBookApi;
        var invoiceCompareTemplateEntity;
        //var financialAccountId;

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
                //financialAccountId = parameters.partnerId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.isVoiceModuleEnabled = WhS_BE_ToneModuleService.isVoiceModuleEnabled();
            $scope.scopeModel.isSMSModuleEnabled = WhS_BE_ToneModuleService.isSMSModuleEnabled();

            //$scope.scopeModel.export = function () {
            //	var objToCompare = {
            //		Settings: buildInvoiceCompareObjFromScope()
            //	};
            //	objToCompare.Settings.IsCustomer = (invoiceCarrierType == 0);
            //	objToCompare.Settings.FinancialAccountId = financialAccountId;
            //	return VRCommon_VRTempPayloadAPIService.AddVRTempPayload(objToCompare).then(function (response) {
            //		if (response.Result == InsertOperationResultEnum.Succeeded.value) {
            //			var tempPayloadId = response.InsertedObject;
            //			var paramsurl = "";
            //			paramsurl += "tempPayloadId=" + tempPayloadId;
            //			var screenWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
            //			var left = ((screenWidth / 2) - (1000 / 2));
            //			window.open("Client/Modules/WhS_Invoice/Reports/CompareInvoiceReport.aspx?" + paramsurl, "_blank", "toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=yes, copyhistory=no,width=1000, height=600,scrollbars=1 , top = 40, left = " + left + "");
            //		}
            //	}).catch(function (error) {
            //	});
            //};

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

                        if ($scope.scopeModel.isVoiceModuleEnabled) {
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

                        if ($scope.scopeModel.isSMSModuleEnabled) {
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
                return WhS_Invoice_InvoiceCompareService.CompareVoiceInvoices(dataRetrievalInput).then(function (response) {

                    if (response != undefined && response.Result != undefined) {
                        VRNotificationService.notifyOnItemAdded("Compare Invoice", response);
                    } else {
                        $scope.scopeModel.recievedVoiceComparisonTab.isSelected = true;
                        onResponseReady(response);
                    }

                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            $scope.scopeModel.smsDataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Invoice_InvoiceCompareService.CompareSMSInvoices(dataRetrievalInput).then(function (response) {

                    if (response != undefined && response.Result != undefined) {
                        VRNotificationService.notifyOnItemAdded("Compare Invoice", response);
                    } else {
                        if (!($scope.scopeModel.isVoiceModuleEnabled && $scope.scopeModel.isVoiceMappingRequired()))
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

            $scope.scopeModel.isVoiceMappingRequired = function () {
                if (voiceMainListAPI != undefined && smsMainListAPI != undefined) {
                    if (isVoiceRequired()) {
                        if (voiceListPayload != undefined && voiceListPayload.fieldMappings != undefined) {
                            setListMappingRequried(voiceListPayload.fieldMappings, voiceMainListAPI, true);
                        }
                        return true;
                    }
                    else {
                        if (voiceListPayload != undefined && voiceListPayload.fieldMappings != undefined) {
                            setListMappingRequried(voiceListPayload.fieldMappings, voiceMainListAPI, false);
                        }
                        return false;
                    }
                }
            };

            $scope.scopeModel.isSMSMappingRequired = function () {
                if (smsMainListAPI != undefined && voiceMainListAPI != undefined) {
                    if (isSMSRequired()) {
                        if (smsListPayload != undefined && smsListPayload.fieldMappings != undefined) {
                            setListMappingRequried(smsListPayload.fieldMappings, smsMainListAPI, true);
                        }
                        return true;
                    }
                    else {
                        if (smsListPayload != undefined && smsListPayload.fieldMappings != undefined) {
                            setListMappingRequried(smsListPayload.fieldMappings, smsMainListAPI, false);
                        }
                        return false;
                    }
                }
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;

            UtilsService.waitMultipleAsyncOperations([getInvoice, getInvoiceAction]).then(function () {
                $scope.scopeModel.showVoiceGrid = true;
                $scope.scopeModel.showSMSGrid = true;

                WhS_Invoice_InvoiceCompareTemplateAPIService.GetInvoiceCompareTemplate(invoiceTypeId, partnerId, invoiceCarrierType).then(function (response) {
                    invoiceCompareTemplateEntity = response;

                    loadAllControls();
                });

            });
        }

        function getInvoice() {
            return WhS_Invoice_InvoiceAPIService.GetInvoiceDetails(invoiceId, invoiceCarrierType).then(function (response) {
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
                for (var prop in WhS_Invoice_ComparisonResultEnum) {
                    var enumObj = UtilsService.cloneObject(WhS_Invoice_ComparisonResultEnum[prop]);
                    if (enumObj.value == WhS_Invoice_ComparisonResultEnum.MissingProvider.value) {
                        enumObj.description = enumObj.description + $scope.scopeModel.partnerLabel;
                    }
                    $scope.scopeModel.voiceComparisonResult.push(enumObj);
                    $scope.scopeModel.smsComparisonResult.push(enumObj);
                }
                if (invoiceCompareTemplateEntity != undefined && invoiceCompareTemplateEntity.Details != undefined) {

                    $scope.scopeModel.voiceDateTimeFormat = invoiceCompareTemplateEntity.Details.DateTimeFormat;
                    if (invoiceCompareTemplateEntity.Details.SMS != undefined) {
                        $scope.scopeModel.smsDateTimeFormat = invoiceCompareTemplateEntity.Details.SMS.DateTimeFormat;
                    };

                }

                $scope.scopeModel.voiceComparisonCriterias = UtilsService.getArrayEnum(WhS_Invoice_ComparisonCriteriaEnum);
                $scope.scopeModel.smsComparisonCriterias = UtilsService.getArrayEnum(WhS_Invoice_SMSComparisonCriteriaEnum);
            }

            promises.push(setTitle);
            promises.push(loadStaticData);

            if ($scope.scopeModel.isVoiceModuleEnabled)
                promises.push(loadVoiceMainListListMapping);

            if ($scope.scopeModel.isSMSModuleEnabled)
                promises.push(loadSMSMainListListMapping);


            return UtilsService.waitMultipleAsyncOperations(promises).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildInvoiceCompareVoiceObjFromScope() {
            var obj = {
                //$type: "TOne.WhS.Invoice.Entities.InvoiceComparisonInput,TOne.WhS.Invoice.Entities",
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
                //$type: "TOne.WhS.Invoice.Entities.InvoiceComparisonInput,TOne.WhS.Invoice.Entities",
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
            var loadMainListMappingPromiseDeferred = UtilsService.createPromiseDeferred();
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
                if (invoiceCompareTemplateEntity != undefined && invoiceCompareTemplateEntity.Details != undefined)
                    voiceListPayload.listMappingData = invoiceCompareTemplateEntity.Details.ListMapping;
                VRUIUtilsService.callDirectiveLoad(voiceMainListAPI, voiceListPayload, loadMainListMappingPromiseDeferred);
            });

            return loadMainListMappingPromiseDeferred.promise;
        }

        function loadSMSMainListListMapping() {
            var loadMainListMappingPromiseDeferred = UtilsService.createPromiseDeferred();
            smsMainListMappingReadyPromiseDeferred.promise.then(function () {
                smsListPayload = {
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
                if (invoiceCompareTemplateEntity != undefined && invoiceCompareTemplateEntity.Details != undefined && invoiceCompareTemplateEntity.Details.SMS != undefined)
                    smsListPayload.listMappingData = invoiceCompareTemplateEntity.Details.SMS.ListMapping;
                VRUIUtilsService.callDirectiveLoad(smsMainListAPI, smsListPayload, loadMainListMappingPromiseDeferred);
            });

            return loadMainListMappingPromiseDeferred.promise;
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


                if ($scope.scopeModel.isVoiceModuleEnabled && $scope.scopeModel.isVoiceMappingRequired()) {
                    var loadVoicePromiseDeferred = UtilsService.createPromiseDeferred();
                    voiceGridPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(voiceGridPromiseDeferred.promise);
                    promises.push(loadVoicePromiseDeferred.promise);

                    voiceGridPromiseDeferred.promise.then(function () {
                        voiceGridPromiseDeferred = undefined;
                        var comparisonInput = buildInvoiceCompareVoiceObjFromScope();
                        voiceGridAPI.retrieveData(comparisonInput).then(function () {
                            //WhS_Invoice_InvoiceAPIService.DoesInvoiceReportExist(invoiceCarrierType==0).then(function (response) {
                            //	$scope.scopeModel.isCompare = response;
                            //});
                            loadVoicePromiseDeferred.resolve();
                        });
                    });
                };

                if ($scope.scopeModel.isSMSModuleEnabled && $scope.scopeModel.isSMSMappingRequired()) {

                    var loadSMSPromiseDeferred = UtilsService.createPromiseDeferred();
                    smsGridPromiseDeferred = UtilsService.createPromiseDeferred();

                    promises.push(smsGridPromiseDeferred.promise);
                    promises.push(loadSMSPromiseDeferred.promise);

                    smsGridPromiseDeferred.promise.then(function () {
                        smsGridPromiseDeferred = undefined;
                        var comparisonInput =
                            buildInvoiceCompareSMSObjFromScope();

                        smsGridAPI.retrieveData(comparisonInput).then(function () {
                            //WhS_Invoice_InvoiceAPIService.DoesInvoiceReportExist(invoiceCarrierType==0).then(function (response) {
                            //	$scope.scopeModel.isCompare = response;
                            //});
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
            if ($scope.scopeModel.isSMSModuleEnabled)
                var details = {
                    SMS: {
                        ListMapping: smsMainListAPI.getData(),
                        DateTimeFormat: $scope.scopeModel.smsDateTimeFormat
                    }
                };

            if ($scope.scopeModel.isVoiceModuleEnabled) {

                details.ListMapping = voiceMainListAPI.getData();
                details.DateTimeFormat = $scope.scopeModel.voiceDateTimeFormat;
            };

            var obj = {
                InvoiceTypeId: invoiceTypeId,
                PartnerId: partnerId,
                Details: details
            };
            return obj;
        }

        function setListMappingRequried(list, api, flag) {
            api.setFirstRowRequired({
                firstRowRequired: flag
            });
            var fieldMappings = list;
            var fieldMappingsLength = fieldMappings.length;
            for (var i = 0; i < fieldMappingsLength; i++) {
                fieldMappings[i].isRequired = flag;
            };
        }

        function isVoiceRequired() {
            return voiceMainListAPI.hasValue() || !smsMainListAPI.hasValue();
        }

        function isSMSRequired() {
            return smsMainListAPI.hasValue() || !voiceMainListAPI.hasValue();
        }

        function saveInvoiceCompareTemplate() {
            var invoiceComparisonTemplate = buildVoiceObjectFromScope();

            return WhS_Invoice_InvoiceCompareTemplateAPIService.SaveInvoiceCompareTemplate(invoiceComparisonTemplate).then(function (response) {
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

    appControllers.controller('WhS_Invoice_InvoiceCompareTemplateController', invoiceCompareTemplateController);

})(appControllers);