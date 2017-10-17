(function (appControllers) {

    'use strict';

    invoiceCompareTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_Invoice_InvoiceAPIService','VR_ExcelConversion_FieldTypeEnum','VR_Invoice_InvoiceTypeAPIService','WhS_Invoice_InvoiceAPIService','WhS_Invoice_ComparisonResultEnum','LabelColorsEnum','WhS_Invoice_ComparisonCriteriaEnum'];

    function invoiceCompareTemplateController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_Invoice_InvoiceAPIService, VR_ExcelConversion_FieldTypeEnum, VR_Invoice_InvoiceTypeAPIService, WhS_Invoice_InvoiceAPIService, WhS_Invoice_ComparisonResultEnum, LabelColorsEnum, WhS_Invoice_ComparisonCriteriaEnum) {

        var invoiceAccountEntity;
        var invoiceId;
        var invoiceTypeId;
        var invoiceActionId;

        var invoiceEntity;
        var invoiceActionEntity;
        var mainListAPI;
        var mainListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var gridAPI;
        var inputWorkBookApi;
        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                invoiceId = parameters.invoiceId;
                invoiceTypeId = parameters.invoiceTypeId;
                invoiceActionId = parameters.invoiceActionId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.threshold = 5;
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
               
            };
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };
            $scope.scopeModel.onReadyWoorkBook = function (api) {
                inputWorkBookApi = api;
            };
            $scope.scopeModel.compare = function () {
                $scope.scopeModel.recievedComparisonTab.isSelected = true;
               return startCompare();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Invoice_InvoiceAPIService.CompareInvoices(dataRetrievalInput).then(function (response) {
                    if (response != undefined && response.Data != null) {
                    }
                    onResponseReady(response);
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
                loadAllControls();
            });
        }

        function getInvoice()
        {
            return VR_Invoice_InvoiceAPIService.GetInvoiceDetail(invoiceId).then(function (response) {
                invoiceEntity = response;
                if(invoiceEntity != undefined)
                {
                    $scope.scopeModel.issuedBy = invoiceEntity.UserName;

                    $scope.scopeModel.to = invoiceEntity.PartnerName;
                    $scope.scopeModel.toDate = UtilsService.getShortDate( UtilsService.createDateFromString(invoiceEntity.Entity.ToDate) );
                    $scope.scopeModel.fromDate = UtilsService.getShortDate(UtilsService.createDateFromString(invoiceEntity.Entity.FromDate));
                    $scope.scopeModel.issuedDate = UtilsService.getShortDate(UtilsService.createDateFromString(invoiceEntity.Entity.IssueDate));
                    $scope.scopeModel.dueDate = UtilsService.getShortDate(UtilsService.createDateFromString(invoiceEntity.Entity.DueDate));
                        
                    $scope.scopeModel.serialNumber = invoiceEntity.Entity.SerialNumber;
                    $scope.scopeModel.timeZone = invoiceEntity.TimeZoneName;
                    $scope.scopeModel.calls = invoiceEntity.Entity.Details.TotalNumberOfCalls;
                    $scope.scopeModel.duration = invoiceEntity.Entity.Details.Duration;
                    $scope.scopeModel.totalAmount = invoiceEntity.Entity.Details.TotalAmount;
                    $scope.scopeModel.isLocked = invoiceEntity.Lock;
                    $scope.scopeModel.isPaid = invoiceEntity.Paid;

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
                    listMappingData: {
                        FirstRowIndex: 1,
                        SheetIndex: 0,
                        FieldMappings: [{
                            ConfigId: "ab48ff16-7b04-42ff-8525-c68590a88799",
                            FieldName: "Zone",
                            FieldType: VR_ExcelConversion_FieldTypeEnum.String.value,
                            SheetIndex: 0,
                            RowIndex: 1,
                            CellIndex: 0
                        }, {
                            ConfigId: "ab48ff16-7b04-42ff-8525-c68590a88799",
                            FieldName: "FromDate",
                            FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value,
                            SheetIndex: 0,
                            RowIndex: 1,
                            CellIndex: 1
                        }, {
                            ConfigId: "ab48ff16-7b04-42ff-8525-c68590a88799",
                            FieldName: "ToDate",
                            FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value,
                            SheetIndex: 0,
                            RowIndex: 1,
                            CellIndex: 2
                        }, {
                            ConfigId: "ab48ff16-7b04-42ff-8525-c68590a88799",
                            FieldName: "Duration",
                            FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value,
                            SheetIndex: 0,
                            RowIndex: 1,
                            CellIndex: 3
                        }, {
                            ConfigId: "ab48ff16-7b04-42ff-8525-c68590a88799",
                            FieldName: "NumberOfCalls",
                            FieldType: VR_ExcelConversion_FieldTypeEnum.Int.value,
                            SheetIndex: 0,
                            RowIndex: 1,
                            CellIndex: 4
                        }, {
                            ConfigId: "ab48ff16-7b04-42ff-8525-c68590a88799",
                            FieldName: "Rate",
                            FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value,
                            SheetIndex: 0,
                            RowIndex: 1,
                            CellIndex: 5
                        }, {
                            ConfigId: "ab48ff16-7b04-42ff-8525-c68590a88799",
                            FieldName: "Amount",
                            FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value,
                            SheetIndex: 0,
                            RowIndex: 1,
                            CellIndex: 6
                        }]
                    }
                };
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
            var comparisonInput = buildInvoiceCompareObjFromScope();
           return    gridAPI.retrieveData(comparisonInput);
            
        }
    }

    appControllers.controller('WhS_Invoice_InvoiceCompareTemplateController', invoiceCompareTemplateController);

})(appControllers);