(function (appControllers) {

    'use strict';

    invoiceCompareTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_Invoice_InvoiceAPIService', 'VR_ExcelConversion_FieldTypeEnum', 'VR_Invoice_InvoiceTypeAPIService', 'WhS_Invoice_InvoiceAPIService', 'WhS_Invoice_ComparisonResultEnum', 'LabelColorsEnum', 'WhS_Invoice_ComparisonCriteriaEnum', 'UISettingsService', 'WhS_Invoice_InvoiceCompareTemplateAPIService'];

    function invoiceCompareTemplateController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_Invoice_InvoiceAPIService, VR_ExcelConversion_FieldTypeEnum, VR_Invoice_InvoiceTypeAPIService, WhS_Invoice_InvoiceAPIService, WhS_Invoice_ComparisonResultEnum, LabelColorsEnum, WhS_Invoice_ComparisonCriteriaEnum, UISettingsService, WhS_Invoice_InvoiceCompareTemplateAPIService) {

        var invoiceAccountEntity;
        var invoiceCarrierType;
        var invoiceId;
        var invoiceTypeId;
        var invoiceActionId;
        var partnerId;
        var invoiceEntity;
        var invoiceActionEntity;
        var mainListAPI;
        var mainListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var gridAPI;
        var gridPromiseDeferred = UtilsService.createPromiseDeferred();
        var inputWorkBookApi;
        var invoiceCompareTemplateEntity;

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
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.threshold = 5;
            $scope.scopeModel.decimalDigits = 2;
            $scope.scopeModel.comparisonResults = [];
            $scope.scopeModel.dateTimeFormat = "yyyy-MM-dd";
            $scope.scopeModel.comparisonResult = [];
            $scope.scopeModel.comparisonCriterias = [];
            $scope.scopeModel.selectedComparisonCriterias = [];
            $scope.scopeModel.reset = function () {
                    var payload = {
                        context: getContext(),
                        fieldMappings: [
                            { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                            { FieldName: "FromDate", FieldTitle: "From", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                            { FieldName: "ToDate", FieldTitle: "Till", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                            { FieldName: "Duration", FieldTitle: "Duration", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value },
                            { FieldName: "NumberOfCalls", FieldTitle: "Calls", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Int.value },
                            { FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value },
                            { FieldName: "Amount", FieldTitle: "Amount", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value },
                        ],
                        listName: "MainList",
                        showDateFormat: false,
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, mainListAPI, payload, setLoader);
            };
            $scope.scopeModel.selectedComparisonResults = [];

            $scope.scopeModel.onMainListMappingReady = function (api) {
                mainListAPI = api;
                mainListMappingReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
               return saveInvoiceCompareTemplate();
            };
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridPromiseDeferred.resolve();
            };
            $scope.scopeModel.onReadyWoorkBook = function (api) {
                inputWorkBookApi = api;
            };
            $scope.scopeModel.compare = function () {
                $scope.scopeModel.comparisonResults.length = 0;
                  return   startCompare();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Invoice_InvoiceAPIService.CompareInvoices(dataRetrievalInput).then(function (response) {

                    if (response != undefined && response.Result != undefined)
                    {
                        VRNotificationService.notifyOnItemAdded("Compare Invoice", response);
                    }else
                    {
                        $scope.scopeModel.recievedComparisonTab.isSelected = true;
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
        }

        function load() {
            $scope.scopeModel.isLoading = true;
           
            UtilsService.waitMultipleAsyncOperations([getInvoice, getInvoiceAction]).then(function () {
                $scope.scopeModel.showGrid = true;
                WhS_Invoice_InvoiceCompareTemplateAPIService.GetInvoiceCompareTemplate(invoiceTypeId, partnerId,invoiceCarrierType).then(function (response) {
                    invoiceCompareTemplateEntity = response;
                   
                    loadAllControls();
                });
              
            });
        } 

        function getInvoice()
        {
            return WhS_Invoice_InvoiceAPIService.GetInvoiceDetails(invoiceId,invoiceCarrierType).then(function (response) {
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
                if (invoiceActionEntity != undefined && invoiceActionEntity.Settings != undefined)
                {
                    $scope.scopeModel.partnerLabel = invoiceActionEntity.Settings.PartnerLabel;
                    $scope.scopeModel.partnerAbbreviationLabel = invoiceActionEntity.Settings.PartnerAbbreviationLabel;
                }
            });
        }


        function loadAllControls() {
         
            function setTitle() {
              
                $scope.title = 'Compare Invoice';
            }

            function loadStaticData() {
                if (invoiceAccountEntity != undefined) {
                }
                for (var prop in WhS_Invoice_ComparisonResultEnum) {
                    var enumObj = WhS_Invoice_ComparisonResultEnum[prop];
                    if (enumObj == WhS_Invoice_ComparisonResultEnum.MissingProvider) {
                        enumObj.description = enumObj.description + $scope.scopeModel.partnerLabel;
                    }
                    $scope.scopeModel.comparisonResult.push(enumObj);
                }
                if (invoiceCompareTemplateEntity != undefined && invoiceCompareTemplateEntity.Details != undefined) {
                    $scope.scopeModel.dateTimeFormat = invoiceCompareTemplateEntity.Details.DateTimeFormat;
                }
                $scope.scopeModel.comparisonCriterias = UtilsService.getArrayEnum(WhS_Invoice_ComparisonCriteriaEnum);
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadMainListListMapping]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildInvoiceCompareObjFromScope() {
            var obj = {
                Threshold: $scope.scopeModel.threshold,
                ListMapping: mainListAPI.getData(),
                DateTimeFormat :$scope.scopeModel.dateTimeFormat,
                InvoiceTypeId :invoiceTypeId,
                InvoiceActionId :invoiceActionId,
                InvoiceId :invoiceId,
                InputFileId: $scope.scopeModel.inPutFile.fileId,
                ComparisonResults: UtilsService.getPropValuesFromArray($scope.scopeModel.selectedComparisonResults, "value"),
                ComparisonCriterias: UtilsService.getPropValuesFromArray($scope.scopeModel.selectedComparisonCriterias, "value"),
                DecimalDigits:$scope.scopeModel.decimalDigits
            };
            return obj;
        }

        function loadMainListListMapping()   {
            var loadMainListMappingPromiseDeferred = UtilsService.createPromiseDeferred();
            mainListMappingReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    context: getContext(),
                    fieldMappings: [
                        { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                        { FieldName: "FromDate", FieldTitle: "From", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                        { FieldName: "ToDate", FieldTitle: "Till", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value },
                        { FieldName: "Duration", FieldTitle: "Duration", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value },
                        { FieldName: "NumberOfCalls", FieldTitle: "Calls", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Int.value },
                        { FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value },
                        { FieldName: "Amount", FieldTitle: "Amount", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value },
                    ],
                    listName: "MainList",
                    showDateFormat: false,
                };
                if (invoiceCompareTemplateEntity != undefined && invoiceCompareTemplateEntity.Details != undefined)
                    payload.listMappingData = invoiceCompareTemplateEntity.Details.ListMapping;
                VRUIUtilsService.callDirectiveLoad(mainListAPI, payload, loadMainListMappingPromiseDeferred);
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


        function startCompare()
        {
            $scope.scopeModel.showGrid = false;
            var loadPromiseDeferred = UtilsService.createPromiseDeferred();
            setTimeout(function () {
                $scope.scopeModel.showGrid = true;
                gridPromiseDeferred = UtilsService.createPromiseDeferred();
                gridPromiseDeferred.promise.then(function () {
                    gridPromiseDeferred = undefined;
                    var comparisonInput = buildInvoiceCompareObjFromScope();
                    gridAPI.retrieveData(comparisonInput).then(function () {
                        loadPromiseDeferred.resolve();
                    });
                });
            });
            return loadPromiseDeferred.promise;
        }
        function buildObjectFromScope() {
            var obj = {
                InvoiceTypeId: invoiceTypeId,
                PartnerId: partnerId,
                Details: {
                    ListMapping: mainListAPI.getData(),
                    DateTimeFormat: $scope.scopeModel.dateTimeFormat
                }
            };
            return obj;
        }
        function saveInvoiceCompareTemplate() {
            var invoiceComparisonTemplate = buildObjectFromScope();
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