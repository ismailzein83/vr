(function (appControllers) {

    "use strict";

    invoiceTypeEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeAPIService', 'VR_GenericData_DataRecordTypeAPIService'];

    function invoiceTypeEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeAPIService, VR_GenericData_DataRecordTypeAPIService) {
        var isEditMode;

        var invoiceTypeId;
        var invoiceTypeEntity;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var selectedDataRecordTypeSelectorReadyPromiseDeferred;

        var mainGridColumnsAPI;
        var mainGridColumnsSectionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var subSectionsAPI;
        var subSectionsSectionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceGeneratorAPI;
        var invoiceGeneratorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceGridActionsAPI;
        var invoiceGridActionsSectionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var concatenatedPartsAPI;
        var concatenatedPartsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceActionsAPI;
        var invoiceActionsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceBulkActionsAPI;
        var invoiceBulkActionsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var commentDefinitionAPI;
        var commentDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceGeneratorActionAPI;
        var invoiceGeneratorActionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceExtendedSettingsAPI;
        var invoiceExtendedSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var viewPermissionAPI;
        var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var generatePermissionAPI;
        var generatePermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var viewSettingsPermissionAPI;
        var viewSettingsPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var addSettingsPermissionAPI;
        var addSettingsPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var editSettingsPermissionAPI;
        var editSettingsPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var assignPartnerPermissionAPI;
        var assignPartnerPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var startCalculationMethodAPI;
        var startCalculationMethodPromiseDeferred = UtilsService.createPromiseDeferred();

        var itemGroupingsDirectiveAPI;
        var itemGroupingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceSettingDefinitionDirectiveAPI;
        var invoiceSettingDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var automaticInvoiceActionsAPI;
        var automaticInvoiceActionsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceAttachmentsAPI;
        var invoiceAttachmentsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var amountFieldAPI;
        var amountFieldReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var currencyFieldAPI;
        var currencyFieldReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var itemSetNameStorageRuleAPI;
        var itemSetNameStorageRuleReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var relationDefinitionSelectorAPI;
        var relationDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var fileNamePartsAPI;
        var fileNamePartsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var filesAttachmentsAPI;
        var filesAttachmentsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var executionFlowDefinitionAPI;
        var executionFlowDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceMenualActionsAPI;
        var invoiceMenualActionsReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        var stagesToHoldAPI;
        var stagesToProcessAPI;

        var dataRecordTypeEntity;

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceTypeId = parameters.invoiceTypeId;
            }
            isEditMode = (invoiceTypeId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onMenualActionsReady = function (api) {
                invoiceMenualActionsAPI = api;
                invoiceMenualActionsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onCommentDefinitionSelectorReady = function (api) {
                commentDefinitionAPI = api;
                commentDefinitionReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onInvoiceAttachmentsReady = function (api) {
                invoiceAttachmentsAPI = api;
                invoiceAttachmentsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.relationDefinitionSelectorReady = function (api) {
                relationDefinitionSelectorAPI = api;
                relationDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onAmountFieldSelectorReady = function (api) {
                amountFieldAPI = api;
                amountFieldReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onCurrencyFieldSelectorReady = function (api) {
                currencyFieldAPI = api;
                currencyFieldReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onItemGroupingsDirectiveReady = function (api) {
                itemGroupingsDirectiveAPI = api;
                itemGroupingsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onExecutionFlowDefinitionSelectorReady = function (api) {
                executionFlowDefinitionAPI = api;
                executionFlowDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onDataRecordTypeSelectionChanged = function () {
                $scope.scopeModel.isLoading = true;
                var dataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                if (dataRecordTypeId != undefined) {
                    VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                        $scope.scopeModel.isLoading = false;
                        dataRecordTypeEntity = response;
                    });

                    if (selectedDataRecordTypeSelectorReadyPromiseDeferred != undefined)
                        selectedDataRecordTypeSelectorReadyPromiseDeferred.resolve();
                    else {
                        var setLoader = function (value) { $scope.scopeModel.isLoadAmountField = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, amountFieldAPI, { dataRecordTypeId: dataRecordTypeId }, setLoader);

                        var setLoader = function (value) { $scope.scopeModel.isLoadCurrencyField = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, currencyFieldAPI, { dataRecordTypeId: dataRecordTypeId }, setLoader);
                    }

                }
            };
            $scope.scopeModel.onItemSetNameStorageRuleReady = function (api) {
                itemSetNameStorageRuleAPI = api;
                itemSetNameStorageRuleReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onInvoiceSettingDefinitionReady = function (api) {
                invoiceSettingDefinitionDirectiveAPI = api;
                invoiceSettingDefinitionReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onConcatenatedPartsReady = function (api) {
                concatenatedPartsAPI = api;
                concatenatedPartsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onFileNamePartsReady = function (api) {
                fileNamePartsAPI = api;
                fileNamePartsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onFilesAttachmentsReady = function (api) {
                filesAttachmentsAPI = api;
                filesAttachmentsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onStartCalculationMethodDirectiveReady = function (api) {
                startCalculationMethodAPI = api;
                startCalculationMethodPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSubSectionsReady = function (api) {
                subSectionsAPI = api;
                subSectionsSectionReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onMainGridColumnsReady = function (api) {
                mainGridColumnsAPI = api;
                mainGridColumnsSectionReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onInvoiceGridActionsReady = function (api) {
                invoiceGridActionsAPI = api;
                invoiceGridActionsSectionReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onInvoiceGeneratorActionsReady = function (api) {
                invoiceGeneratorActionAPI = api;
                invoiceGeneratorActionReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onInvoiceActionsReady = function (api) {
                invoiceActionsAPI = api;
                invoiceActionsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onInvoiceBulkActionsReady = function (api) {
                invoiceBulkActionsAPI = api;
                invoiceBulkActionsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                viewPermissionAPI = api;
                viewPermissionReadyDeferred.resolve();
            };

            $scope.scopeModel.onGenerateRequiredPermissionReady = function (api) {
                generatePermissionAPI = api;
                generatePermissionReadyDeferred.resolve();
            };

            $scope.scopeModel.onViewSettingsRequiredPermissionReady = function (api) {
                viewSettingsPermissionAPI = api;
                viewSettingsPermissionReadyDeferred.resolve();
            };

            $scope.scopeModel.onAddSettingsRequiredPermissionReady = function (api) {
                addSettingsPermissionAPI = api;
                addSettingsPermissionReadyDeferred.resolve();
            };
            $scope.scopeModel.onEditSettingsRequiredPermissionReady = function (api) {
                editSettingsPermissionAPI = api;
                editSettingsPermissionReadyDeferred.resolve();
            };

            $scope.scopeModel.onAssignPartnerRequiredPermissionReady = function (api) {
                assignPartnerPermissionAPI = api;
                assignPartnerPermissionReadyDeferred.resolve();
            };
            $scope.scopeModel.saveInvoiceType = function () {
                $scope.scopeModel.isLoading = true;
                if (isEditMode) {
                    return updateInvoiceType();
                }
                else {
                    return insertInvoiceType();
                }
            };

            $scope.scopeModel.onInvoiceExtendedSettingsReady = function (api) {
                invoiceExtendedSettingsAPI = api;
                invoiceExtendedSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onAutomaticInvoiceActionsReady = function (api) {
                automaticInvoiceActionsAPI = api;
                automaticInvoiceActionsReadyPromiseDeferred.resolve();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onExecutionFlowDefinitionSelectionChanged = function (selectedItem) {
                if (selectedItem != undefined) {
                    loadStagesToHoldSelector(selectedItem.ID);
                    loadStagesToProcessSelector(selectedItem.ID);
                }
            };

            $scope.scopeModel.onStagesToHoldSelectorReady = function (api) {
                stagesToHoldAPI = api;
            };

            $scope.scopeModel.onStagesToProcessSelectorReady = function (api) {
                stagesToProcessAPI = api;
            };

            function loadStagesToHoldSelector(executionFlowDefinitionId) {
                var stagesToHoldPayload = {
                    executionFlowDefinitionId: executionFlowDefinitionId
                };
                if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined) {
                    stagesToHoldPayload.selectedIds = invoiceTypeEntity.Settings.StagesToHoldNames;
                }
                var setLoader = function (value) { $scope.scopeModel.isLoadingSatgesToHold = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, stagesToHoldAPI, stagesToHoldPayload, setLoader);
            }
            function loadStagesToProcessSelector(executionFlowDefinitionId) {
                var stagesToProcessPayload = {
                    executionFlowDefinitionId: executionFlowDefinitionId
                };
                if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined) {
                    stagesToProcessPayload.selectedIds = invoiceTypeEntity.Settings.StagesToProcessNames;
                }
                var setLoader = function (value) { $scope.scopeModel.isLoadingSatgesToProcess = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, stagesToProcessAPI, stagesToProcessPayload, setLoader);
            }

            function buildInvoiceTypeObjFromScope() {
                var obj = {
                    InvoiceTypeId: invoiceTypeId,
                    Name: $scope.scopeModel.name,
                    Settings: {
                        ExecutionFlowDefinitionId: executionFlowDefinitionAPI.getSelectedIds(),
                        StagesToHoldNames: stagesToHoldAPI.getSelectedIds(),
                        StagesToProcessNames: stagesToProcessAPI.getSelectedIds(),
                        InvoiceDetailsRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        InvoiceActions: invoiceActionsAPI.getData(),
                        InvoiceBulkActions: invoiceBulkActionsAPI.getData(),
                        InvoiceGeneratorActions: invoiceGeneratorActionAPI.getData(),
                        ExtendedSettings: invoiceExtendedSettingsAPI.getData(),
                        InvoiceGridSettings: {
                            MainGridColumns: mainGridColumnsAPI.getData(),
                            InvoiceGridActions: invoiceGridActionsAPI.getData(),
                        },
                        InvoiceMenualBulkActions: invoiceMenualActionsAPI.getData(),
                        InvoiceSerialNumberSettings: {
                            SerialNumberParts: concatenatedPartsAPI.getData(),
                        },
                        InvoiceFileSettings: {
                            InvoiceFileNameParts: fileNamePartsAPI.getData(),
                            FilesAttachments: filesAttachmentsAPI.getData(),
                        },
                        SubSections: subSectionsAPI.getData(),
                        Security: {
                            ViewRequiredPermission: viewPermissionAPI.getData(),
                            GenerateRequiredPermission: generatePermissionAPI.getData(),
                            ViewSettingsRequiredPermission: viewSettingsPermissionAPI.getData(),
                            AddSettingsRequiredPermission: addSettingsPermissionAPI.getData(),
                            EditSettingsRequiredPermission: editSettingsPermissionAPI.getData(),
                            AssignPartnerRequiredPermission: assignPartnerPermissionAPI.getData()
                        },
                        ItemGroupings: itemGroupingsDirectiveAPI.getData(),
                        StartDateCalculationMethod: startCalculationMethodAPI.getData(),
                        InvoiceSettingPartUISections: invoiceSettingDefinitionDirectiveAPI.getData(),
                        AutomaticInvoiceActions: automaticInvoiceActionsAPI.getData(),
                        InvoiceAttachments: invoiceAttachmentsAPI.getData(),
                        AmountFieldName: amountFieldAPI.getSelectedIds(),
                        CurrencyFieldName: currencyFieldAPI.getSelectedIds(),
                        ItemSetNamesStorageRules: itemSetNameStorageRuleAPI.getData(),
                        InvToAccBalanceRelationId: relationDefinitionSelectorAPI.getSelectedIds(),
                        InvoiceCommentDefinitionId: commentDefinitionAPI.getSelectedIds()
                    }
                };
                return obj;
            }

            function insertInvoiceType() {

                var invoiceTypeObject = buildInvoiceTypeObjFromScope();
                return VR_Invoice_InvoiceTypeAPIService.AddInvoiceType(invoiceTypeObject)
                .then(function (response) {
                    $scope.scopeModel.isLoading = false;
                    if (VRNotificationService.notifyOnItemAdded("Invoice Type", response)) {
                        if ($scope.onInvoiceTypeAdded != undefined)
                            $scope.onInvoiceTypeAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });

            }

            function updateInvoiceType() {
                var invoiceTypeObject = buildInvoiceTypeObjFromScope();
                VR_Invoice_InvoiceTypeAPIService.UpdateInvoiceType(invoiceTypeObject)
                .then(function (response) {
                    $scope.scopeModel.isLoading = false;
                    if (VRNotificationService.notifyOnItemUpdated("Invoice Type", response)) {
                        if ($scope.onInvoiceTypeUpdated != undefined)
                            $scope.onInvoiceTypeUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            }

        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getInvoiceType().then(function () {
                    loadAllControls()
                        .finally(function () {
                            invoiceTypeEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }

            function getInvoiceType() {
                return VR_Invoice_InvoiceTypeAPIService.GetInvoiceType(invoiceTypeId).then(function (response) {
                    invoiceTypeEntity = response;
                });
            }

            function loadAllControls() {

                function setTitle() {
                    if (isEditMode && invoiceTypeEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(invoiceTypeEntity.Name, 'Invoice Type');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Invoice Type');
                }

                function loadStaticData() {
                    if (invoiceTypeEntity != undefined) {
                        $scope.scopeModel.name = invoiceTypeEntity.Name;
                    }
                }

                function loadDataRecordTypeSelector() {
                    if (invoiceTypeEntity != undefined)
                        selectedDataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    var dataRecordTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    dataRecordTypeSelectorReadyPromiseDeferred.promise.then(function () {
                        var dataRecordTypePayload = invoiceTypeEntity != undefined ? { selectedIds: invoiceTypeEntity.Settings.InvoiceDetailsRecordTypeId } : undefined;
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypePayload, dataRecordTypeSelectorLoadPromiseDeferred);
                    });
                    return dataRecordTypeSelectorLoadPromiseDeferred.promise;
                }

                function loadMainGridColumnsSection() {
                    var mainGridColumnsSectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    mainGridColumnsSectionReadyPromiseDeferred.promise.then(function () {
                        var mainGridColumnsPayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings && invoiceTypeEntity.Settings.InvoiceGridSettings) {
                            mainGridColumnsPayload.mainGridColumns = invoiceTypeEntity.Settings.InvoiceGridSettings.MainGridColumns;
                        }
                        VRUIUtilsService.callDirectiveLoad(mainGridColumnsAPI, mainGridColumnsPayload, mainGridColumnsSectionLoadPromiseDeferred);
                    });
                    return mainGridColumnsSectionLoadPromiseDeferred.promise;
                }

                function loadSubSectionsSection() {
                    var subSectionsSectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    subSectionsSectionReadyPromiseDeferred.promise.then(function () {
                        var mainGridColumnsPayload = { context: getContext() };

                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined)
                            mainGridColumnsPayload.subSections = invoiceTypeEntity.Settings.SubSections;
                        VRUIUtilsService.callDirectiveLoad(subSectionsAPI, mainGridColumnsPayload, subSectionsSectionLoadPromiseDeferred);
                    });
                    return subSectionsSectionLoadPromiseDeferred.promise;
                }

                function loadInvoiceGridActionsSection() {
                    var invoiceGridActionsSectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    invoiceGridActionsSectionReadyPromiseDeferred.promise.then(function () {
                        var invoiceGridActionsPayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings && invoiceTypeEntity.Settings.InvoiceGridSettings) {
                            invoiceGridActionsPayload.invoiceGridActions = invoiceTypeEntity.Settings.InvoiceGridSettings.InvoiceGridActions;
                        }
                        VRUIUtilsService.callDirectiveLoad(invoiceGridActionsAPI, invoiceGridActionsPayload, invoiceGridActionsSectionLoadPromiseDeferred);
                    });
                    return invoiceGridActionsSectionLoadPromiseDeferred.promise;
                }

                function loadConcatenatedParts() {
                    var concatenatedPartsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    concatenatedPartsReadyPromiseDeferred.promise.then(function () {
                        var concatenatedPartsDirectivePayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined && invoiceTypeEntity.Settings.InvoiceSerialNumberSettings != undefined) {
                            concatenatedPartsDirectivePayload.serialNumberParts = invoiceTypeEntity.Settings.InvoiceSerialNumberSettings.SerialNumberParts;
                        }

                        VRUIUtilsService.callDirectiveLoad(concatenatedPartsAPI, concatenatedPartsDirectivePayload, concatenatedPartsDeferredLoadPromiseDeferred);
                    });
                    return concatenatedPartsDeferredLoadPromiseDeferred.promise;
                }
                function loadFileNameParts() {
                    var fileNamePartsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    fileNamePartsReadyPromiseDeferred.promise.then(function () {
                        var fileNamePartsDirectivePayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined && invoiceTypeEntity.Settings.InvoiceFileSettings != undefined) {
                            fileNamePartsDirectivePayload.fileNameParts = invoiceTypeEntity.Settings.InvoiceFileSettings.InvoiceFileNameParts;
                        }

                        VRUIUtilsService.callDirectiveLoad(fileNamePartsAPI, fileNamePartsDirectivePayload, fileNamePartsDeferredLoadPromiseDeferred);
                    });
                    return fileNamePartsDeferredLoadPromiseDeferred.promise;
                }
                function loadInvoiceActionsGrid() {
                    var invoiceActionsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceActionsReadyPromiseDeferred.promise.then(function () {
                        var invoiceActionsPayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined)
                            invoiceActionsPayload.invoiceActions = invoiceTypeEntity.Settings.InvoiceActions;
                        VRUIUtilsService.callDirectiveLoad(invoiceActionsAPI, invoiceActionsPayload, invoiceActionsDeferredLoadPromiseDeferred);
                    });
                    return invoiceActionsDeferredLoadPromiseDeferred.promise;
                }
                function loadInvoiceBulkActionsGrid() {
                    var invoiceBulkActionsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceBulkActionsReadyPromiseDeferred.promise.then(function () {
                        var invoiceBulkActionsPayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined)
                            invoiceBulkActionsPayload.invoiceBulkActions = invoiceTypeEntity.Settings.InvoiceBulkActions;
                        VRUIUtilsService.callDirectiveLoad(invoiceBulkActionsAPI, invoiceBulkActionsPayload, invoiceBulkActionsDeferredLoadPromiseDeferred);
                    });
                    return invoiceBulkActionsDeferredLoadPromiseDeferred.promise;
                }


                function loadInvoiceGeneratorActionGrid() {
                    var invoiceGeneratorActionDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceGeneratorActionReadyPromiseDeferred.promise.then(function () {
                        var invoiceGeneratorActionDirectivePayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined)
                            invoiceGeneratorActionDirectivePayload.invoiceGeneratorActions = invoiceTypeEntity.Settings.InvoiceGeneratorActions;
                        VRUIUtilsService.callDirectiveLoad(invoiceGeneratorActionAPI, invoiceGeneratorActionDirectivePayload, invoiceGeneratorActionDeferredLoadPromiseDeferred);
                    });
                    return invoiceGeneratorActionDeferredLoadPromiseDeferred.promise;
                }

                function loadInvoiceExtendedSettings() {
                    var invoiceExtendedSettingsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceExtendedSettingsReadyPromiseDeferred.promise.then(function () {
                        var invoiceGeneratorActionDirectivePayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined)
                            invoiceGeneratorActionDirectivePayload.extendedSettingsEntity = invoiceTypeEntity.Settings.ExtendedSettings;
                        VRUIUtilsService.callDirectiveLoad(invoiceExtendedSettingsAPI, invoiceGeneratorActionDirectivePayload, invoiceExtendedSettingsDeferredLoadPromiseDeferred);
                    });
                    return invoiceExtendedSettingsDeferredLoadPromiseDeferred.promise;
                }

                function loadViewRequiredPermission() {
                    var viewPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                    viewPermissionReadyDeferred.promise.then(function () {
                        var payload;

                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined && invoiceTypeEntity.Settings.Security != undefined && invoiceTypeEntity.Settings.Security.ViewRequiredPermission != null) {
                            payload = {
                                data: invoiceTypeEntity.Settings.Security.ViewRequiredPermission
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, payload, viewPermissionLoadDeferred);
                    });

                    return viewPermissionLoadDeferred.promise;
                }

                function loadGenerateRequiredPermission() {
                    var generatePermissionLoadDeferred = UtilsService.createPromiseDeferred();
                    generatePermissionReadyDeferred.promise.then(function () {
                        var payload;

                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined && invoiceTypeEntity.Settings.Security != undefined && invoiceTypeEntity.Settings.Security.GenerateRequiredPermission != null) {
                            payload = {
                                data: invoiceTypeEntity.Settings.Security.GenerateRequiredPermission
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(generatePermissionAPI, payload, generatePermissionLoadDeferred);
                    });
                    return generatePermissionLoadDeferred.promise;
                }

                function loadViewSettingsRequiredPermission() {
                    var viewSettingPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                    viewSettingsPermissionReadyDeferred.promise.then(function () {
                        var payload;

                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined && invoiceTypeEntity.Settings.Security != undefined && invoiceTypeEntity.Settings.Security.ViewSettingsRequiredPermission != null) {
                            payload = {
                                data: invoiceTypeEntity.Settings.Security.ViewSettingsRequiredPermission
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(viewSettingsPermissionAPI, payload, viewSettingPermissionLoadDeferred);
                    });
                    return viewSettingPermissionLoadDeferred.promise;
                }
                function loadAddSettingsRequiredPermission() {
                    var addSettingsPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                    addSettingsPermissionReadyDeferred.promise.then(function () {
                        var payload;

                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined && invoiceTypeEntity.Settings.Security != undefined && invoiceTypeEntity.Settings.Security.AddSettingsRequiredPermission != null) {
                            payload = {
                                data: invoiceTypeEntity.Settings.Security.AddSettingsRequiredPermission
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(addSettingsPermissionAPI, payload, addSettingsPermissionLoadDeferred);
                    });
                    return addSettingsPermissionLoadDeferred.promise;
                }
                function loadEditSettingsRequiredPermission() {
                    var editSettingsPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                    editSettingsPermissionReadyDeferred.promise.then(function () {
                        var payload;

                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined && invoiceTypeEntity.Settings.Security != undefined && invoiceTypeEntity.Settings.Security.EditSettingsRequiredPermission != null) {
                            payload = {
                                data: invoiceTypeEntity.Settings.Security.EditSettingsRequiredPermission
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(editSettingsPermissionAPI, payload, editSettingsPermissionLoadDeferred);
                    });
                    return editSettingsPermissionLoadDeferred.promise;
                }
                function loadAssignPartnerRequiredPermission() {
                    var assignPartnerPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                    assignPartnerPermissionReadyDeferred.promise.then(function () {
                        var payload;

                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined && invoiceTypeEntity.Settings.Security != undefined && invoiceTypeEntity.Settings.Security.AssignPartnerRequiredPermission != null) {
                            payload = {
                                data: invoiceTypeEntity.Settings.Security.AssignPartnerRequiredPermission
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(assignPartnerPermissionAPI, payload, assignPartnerPermissionLoadDeferred);
                    });
                    return assignPartnerPermissionLoadDeferred.promise;
                }
                function loadItemGroupingsDirective() {
                    var itemGroupingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    itemGroupingsReadyPromiseDeferred.promise.then(function () {
                        var itemGroupingsPayload = invoiceTypeEntity != undefined ? { itemGroupings: invoiceTypeEntity.Settings.ItemGroupings } : undefined;
                        VRUIUtilsService.callDirectiveLoad(itemGroupingsDirectiveAPI, itemGroupingsPayload, itemGroupingsLoadPromiseDeferred);
                    });
                    return itemGroupingsLoadPromiseDeferred.promise;
                }

                function loadAutomaticInvoiceActionsGrid() {
                    var automaticInvoiceActionsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    automaticInvoiceActionsReadyPromiseDeferred.promise.then(function () {
                        var automaticInvoiceActionsPayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined)
                            automaticInvoiceActionsPayload.automaticInvoiceActions = invoiceTypeEntity.Settings.AutomaticInvoiceActions;
                        VRUIUtilsService.callDirectiveLoad(automaticInvoiceActionsAPI, automaticInvoiceActionsPayload, automaticInvoiceActionsDeferredLoadPromiseDeferred);
                    });
                    return automaticInvoiceActionsDeferredLoadPromiseDeferred.promise;
                }

                function loadStartCalculationMethod() {
                    var startCalculationMethodLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    startCalculationMethodPromiseDeferred.promise.then(function () {
                        var startCalculationMethodPayload = invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined ? { startDateCalculationMethodEntity: invoiceTypeEntity.Settings.StartDateCalculationMethod } : undefined;
                        VRUIUtilsService.callDirectiveLoad(startCalculationMethodAPI, startCalculationMethodPayload, startCalculationMethodLoadPromiseDeferred);
                    });
                    return startCalculationMethodLoadPromiseDeferred.promise;
                }

                function loadInvoiceSettingDefinitionDirective() {
                    var invoiceSettingDefinitionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    invoiceSettingDefinitionReadyPromiseDeferred.promise.then(function () {
                        var invoiceSettingDefinitionPayload = invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined ? { invoiceSettingPartUISections: invoiceTypeEntity.Settings.InvoiceSettingPartUISections } : undefined;
                        VRUIUtilsService.callDirectiveLoad(invoiceSettingDefinitionDirectiveAPI, invoiceSettingDefinitionPayload, invoiceSettingDefinitionLoadPromiseDeferred);
                    });
                    return invoiceSettingDefinitionLoadPromiseDeferred.promise;
                }
                function loadInvoiceAttachmentsGrid() {
                    var invoiceAttachmentsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceAttachmentsReadyPromiseDeferred.promise.then(function () {
                        var invoiceAttachmentsPayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined)
                            invoiceAttachmentsPayload.invoiceAttachments = invoiceTypeEntity.Settings.InvoiceAttachments;
                        VRUIUtilsService.callDirectiveLoad(invoiceAttachmentsAPI, invoiceAttachmentsPayload, invoiceAttachmentsDeferredLoadPromiseDeferred);
                    });
                    return invoiceAttachmentsDeferredLoadPromiseDeferred.promise;
                }

                function loadRelationDefinitionSelector() {
                    var relationDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    relationDefinitionSelectorReadyDeferred.promise.then(function () {
                        var relationDefinitionSelectorPayload;
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined) {
                            relationDefinitionSelectorPayload = { selectedIds: invoiceTypeEntity.Settings.InvToAccBalanceRelationId };
                        }
                        VRUIUtilsService.callDirectiveLoad(relationDefinitionSelectorAPI, relationDefinitionSelectorPayload, relationDefinitionSelectorLoadDeferred);
                    });
                    return relationDefinitionSelectorLoadDeferred.promises;
                }

                function loadCurrencyFieldSelector() {
                    if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined && invoiceTypeEntity.Settings.InvoiceDetailsRecordTypeId != undefined) {
                        var currencyFieldLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        currencyFieldReadyPromiseDeferred.promise.then(function () {
                            var currencyFieldPayload = invoiceTypeEntity != undefined ? { dataRecordTypeId: invoiceTypeEntity.Settings.InvoiceDetailsRecordTypeId, selectedIds: invoiceTypeEntity.Settings.CurrencyFieldName } : undefined;
                            VRUIUtilsService.callDirectiveLoad(currencyFieldAPI, currencyFieldPayload, currencyFieldLoadPromiseDeferred);
                        });
                        return currencyFieldLoadPromiseDeferred.promise;
                    }
                }

                function loadAmountFieldSelector() {
                    if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined && invoiceTypeEntity.Settings.InvoiceDetailsRecordTypeId != undefined) {
                        var amountFieldLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        amountFieldReadyPromiseDeferred.promise.then(function () {
                            var amountFieldPayload = invoiceTypeEntity != undefined ? { dataRecordTypeId: invoiceTypeEntity.Settings.InvoiceDetailsRecordTypeId, selectedIds: invoiceTypeEntity.Settings.AmountFieldName } : undefined;
                            VRUIUtilsService.callDirectiveLoad(amountFieldAPI, amountFieldPayload, amountFieldLoadPromiseDeferred);
                        });
                        return amountFieldLoadPromiseDeferred.promise;
                    }

                }

                function loadItemSetNameStorageRules() {
                    var itemSetNameStorageRuleDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    itemSetNameStorageRuleReadyPromiseDeferred.promise.then(function () {
                        var itemSetNameStorageRulesPayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined)
                            itemSetNameStorageRulesPayload.itemSetNameStorageRules = invoiceTypeEntity.Settings.ItemSetNamesStorageRules;
                        VRUIUtilsService.callDirectiveLoad(itemSetNameStorageRuleAPI, itemSetNameStorageRulesPayload, itemSetNameStorageRuleDeferredLoadPromiseDeferred);
                    });
                    return itemSetNameStorageRuleDeferredLoadPromiseDeferred.promise;
                }

                function loadFilesAttachments() {
                    var filesAttachmentsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    filesAttachmentsReadyPromiseDeferred.promise.then(function () {
                        var filesAttachmentsDirectivePayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined && invoiceTypeEntity.Settings.InvoiceFileSettings != undefined) {
                            filesAttachmentsDirectivePayload.filesAttachments = invoiceTypeEntity.Settings.InvoiceFileSettings.FilesAttachments;
                        }

                        VRUIUtilsService.callDirectiveLoad(filesAttachmentsAPI, filesAttachmentsDirectivePayload, filesAttachmentsDeferredLoadPromiseDeferred);
                    });
                    return filesAttachmentsDeferredLoadPromiseDeferred.promise;
                }

                function loadExecutionFlowDefinitionSelector() {
                    var executionFlowDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    executionFlowDefinitionSelectorReadyDeferred.promise.then(function () {
                        var executionFlowDefinitionSelectorPayload;
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined) {
                            executionFlowDefinitionSelectorPayload = {
                                selectedIds: invoiceTypeEntity.Settings.ExecutionFlowDefinitionId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(executionFlowDefinitionAPI, executionFlowDefinitionSelectorPayload, executionFlowDefinitionSelectorLoadDeferred);
                    });
                    return executionFlowDefinitionSelectorLoadDeferred.promise;
                }

                function loadInvoiceMenualGridActionsSection() {
                    var invoiceMenualActionsSectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    invoiceMenualActionsReadyPromiseDeferred.promise.then(function () {
                        var invoiceMenualActionsPayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings) {
                            invoiceMenualActionsPayload.invoiceMenualBulkActions = invoiceTypeEntity.Settings.InvoiceMenualBulkActions;
                        }
                        VRUIUtilsService.callDirectiveLoad(invoiceMenualActionsAPI, invoiceMenualActionsPayload, invoiceMenualActionsSectionLoadPromiseDeferred);
                    });
                    return invoiceMenualActionsSectionLoadPromiseDeferred.promise;
                }
                function loadCommentDefinitionSelector() {
                    var commentDefinitionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    commentDefinitionReadyPromiseDeferred.promise.then(function () {
                        var commentDefinitionSelectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Vanrise.Common.Business.CommentBEDefinitionFilter, Vanrise.Common.Business"
                                }]
                            }
                        };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined) {
                            commentDefinitionSelectorPayload.selectedIds = invoiceTypeEntity.Settings.InvoiceCommentDefinitionId;
                        }
                        VRUIUtilsService.callDirectiveLoad(commentDefinitionAPI, commentDefinitionSelectorPayload, commentDefinitionSelectorLoadPromiseDeferred);
                    });
                    return commentDefinitionSelectorLoadPromiseDeferred.promise;
                }


                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFilesAttachments, loadInvoiceAttachmentsGrid, loadAmountFieldSelector, loadCurrencyFieldSelector, loadDataRecordTypeSelector, loadMainGridColumnsSection, loadSubSectionsSection, loadInvoiceGridActionsSection, loadFileNameParts, loadConcatenatedParts, loadInvoiceActionsGrid, loadInvoiceGeneratorActionGrid, loadInvoiceExtendedSettings, loadViewRequiredPermission, loadGenerateRequiredPermission, loadViewSettingsRequiredPermission, loadAddSettingsRequiredPermission, loadEditSettingsRequiredPermission, loadAssignPartnerRequiredPermission, loadStartCalculationMethod, loadItemGroupingsDirective, loadInvoiceSettingDefinitionDirective, loadAutomaticInvoiceActionsGrid, loadItemSetNameStorageRules, loadRelationDefinitionSelector, loadExecutionFlowDefinitionSelector, loadInvoiceBulkActionsGrid, loadInvoiceMenualGridActionsSection, loadCommentDefinitionSelector])
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   })
                  .finally(function () {
                      $scope.scopeModel.isLoading = false;
                  });
            }
        }

        function getContext() {
            var context = {
                getFields: function () {
                    var fields = [];
                    if (dataRecordTypeEntity != undefined && dataRecordTypeEntity.Fields != undefined) {
                        for (var i = 0; i < dataRecordTypeEntity.Fields.length; i++) {
                            var field = dataRecordTypeEntity.Fields[i];
                            fields.push({ FieldName: field.Name, FieldTitle: field.Title, Type: field.Type });
                        }
                    }
                    return fields;
                },

                getParts: function () {
                    return concatenatedPartsAPI.getData();
                },

                getExtensionType: function () {
                    return "VR_InvoiceType_SerialNumberParts";
                },

                getInvoiceActionsInfo: function () {
                    var invoiceActionsInfo = [];
                    var actions = invoiceActionsAPI.getData();
                    if (actions != undefined && actions.length > 0) {
                        for (var i = 0; i < actions.length ; i++) {
                            var action = actions[i];
                            invoiceActionsInfo.push({
                                Title: action.Title,
                                InvoiceActionId: action.InvoiceActionId
                            });
                        }
                    }
                    return invoiceActionsInfo;
                },

                getInvoiceBulkActionsInfo: function () {
                    var invoiceBulkActionsInfo = [];
                    var bulkActions = invoiceBulkActionsAPI.getData();
                    if (bulkActions != undefined && bulkActions.length > 0) {
                        for (var i = 0; i < bulkActions.length ; i++) {
                            var bulkAction = bulkActions[i];
                            invoiceBulkActionsInfo.push({
                                Title: bulkAction.Title,
                                InvoiceBulkActionId: bulkAction.InvoiceBulkActionId
                            });
                        }
                    }
                    return invoiceBulkActionsInfo;
                },


                getItemGroupingsInfo: function () {
                    var itemGroupingsInfo = [];
                    var itemGroupings = itemGroupingsDirectiveAPI.getData();
                    if (itemGroupings != undefined) {
                        for (var i = 0; i < itemGroupings.length; i++) {
                            var itemGrouping = itemGroupings[i];
                            itemGroupingsInfo.push({
                                Name: itemGrouping.ItemSetName,
                                ItemGroupingId: itemGrouping.ItemGroupingId
                            });
                        }
                    }
                    return itemGroupingsInfo;
                },

                getGroupingDimensions: function (itemGroupingId) {
                    var groupingDimensions = [];
                    var itemGroupings = itemGroupingsDirectiveAPI.getData();
                    if (itemGroupings != undefined) {
                        var itemGrouping = UtilsService.getItemByVal(itemGroupings, itemGroupingId, "ItemGroupingId");
                        if (itemGrouping.DimensionItemFields != undefined)
                            groupingDimensions = itemGrouping.DimensionItemFields;
                    }
                    return groupingDimensions;
                },

                getGroupingMeasures: function (itemGroupingId) {
                    var groupingMeasures = [];
                    var itemGroupings = itemGroupingsDirectiveAPI.getData();
                    if (itemGroupings != undefined) {
                        var itemGrouping = UtilsService.getItemByVal(itemGroupings, itemGroupingId, "ItemGroupingId");
                        if (itemGrouping.AggregateItemFields != undefined) {
                            for (var i = 0; i < itemGrouping.AggregateItemFields.length; i++) {
                                var aggregateItem = itemGrouping.AggregateItemFields[i];
                                groupingMeasures.push({
                                    MeasureItemFieldId: aggregateItem.AggregateItemFieldId,
                                    FieldName: aggregateItem.FieldName,
                                    FieldDescription: aggregateItem.FieldDescription,
                                    FieldType: aggregateItem.FieldType
                                });
                            }
                        }
                    }
                    return groupingMeasures;
                },

                getActionsInfoByActionTypeName: function (actionTypeName) {
                    var actionsInfo = [];
                    var invoiceActions = invoiceActionsAPI.getData();
                    if (invoiceActions != undefined) {
                        for (var i = 0; i < invoiceActions.length; i++) {
                            var action = invoiceActions[i];
                            if (action.Settings.ActionTypeName == actionTypeName) {
                                actionsInfo.push({
                                    Title: action.Title,
                                    InvoiceActionId: action.InvoiceActionId
                                });
                            }
                        }
                    }
                    return actionsInfo;
                },

                getInvoiceAttachmentsInfo: function () {
                    var invoiceAttchamentsInfo = [];
                    var invoiceAttchaments = invoiceAttachmentsAPI.getData();
                    if (invoiceAttchaments != undefined && invoiceAttchaments.length > 0) {
                        for (var i = 0; i < invoiceAttchaments.length ; i++) {
                            var invoiceAttchament = invoiceAttchaments[i];
                            invoiceAttchamentsInfo.push({
                                Title: invoiceAttchament.Title,
                                InvoiceAttachmentId: invoiceAttchament.InvoiceAttachmentId
                            });
                        }
                    }
                    return invoiceAttchamentsInfo;
                }

            };
            return context;
        }

    }

    appControllers.controller('VR_Invoice_InvoiceTypeEditorController', invoiceTypeEditorController);
})(appControllers);
