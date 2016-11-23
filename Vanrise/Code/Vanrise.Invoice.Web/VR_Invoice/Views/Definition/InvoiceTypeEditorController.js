(function (appControllers) {

    "use strict";

    invoiceTypeEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeAPIService','VR_GenericData_DataRecordTypeAPIService'];

    function invoiceTypeEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeAPIService, VR_GenericData_DataRecordTypeAPIService) {
        var isEditMode;
        var invoiceTypeId;
        var invoiceTypeEntity;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

        var serialNumberPatternAPI;
        var serialNumberPatternReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceActionsAPI;
        var invoiceActionsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceGeneratorActionAPI;
        var invoiceGeneratorActionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceExtendedSettingsAPI;
        var invoiceExtendedSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();


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

            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onDataRecordTypeSelectionChanged = function () {
                $scope.scopeModel.isLoading = true;
                var dataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                if (dataRecordTypeId != undefined) {
                    VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                        $scope.scopeModel.isLoading = false;
                        dataRecordTypeEntity = response;
                    });
                }
            };

            $scope.scopeModel.onConcatenatedPartsReady = function (api) {
                concatenatedPartsAPI = api;
                concatenatedPartsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSerialNumberPatternReady = function (api) {
                serialNumberPatternAPI = api;
                serialNumberPatternReadyPromiseDeferred.resolve();
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


            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
            function buildInvoiceTypeObjFromScope() {
                var obj = {
                    InvoiceTypeId: invoiceTypeId,
                    Name: $scope.scopeModel.name,
                    Settings: {
                        InvoiceDetailsRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        InvoiceActions: invoiceActionsAPI.getData(),
                        InvoiceGeneratorActions: invoiceGeneratorActionAPI.getData(),
                        ExtendedSettings: invoiceExtendedSettingsAPI.getData(),
                        InvoiceGridSettings: {
                            MainGridColumns: mainGridColumnsAPI.getData(),
                            InvoiceGridActions: invoiceGridActionsAPI.getData(),
                        },
                        InvoiceSerialNumberSettings: {
                            SerialNumberParts: concatenatedPartsAPI.getData(),
                            SerialNumberPattern: serialNumberPatternAPI.getData(),
                        },
                        SubSections: subSectionsAPI.getData(),
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

                function loadSerialNumberPattern() {
                    var serialNumberPatternDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    serialNumberPatternReadyPromiseDeferred.promise.then(function () {
                        var serialNumberPatternDirectivePayload = { context: getContext() };
                        if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined && invoiceTypeEntity.Settings.InvoiceSerialNumberSettings != undefined)
                            serialNumberPatternDirectivePayload.serialNumberPattern = invoiceTypeEntity.Settings.InvoiceSerialNumberSettings.SerialNumberPattern;
                        VRUIUtilsService.callDirectiveLoad(serialNumberPatternAPI, serialNumberPatternDirectivePayload, serialNumberPatternDeferredLoadPromiseDeferred);
                    });
                    return serialNumberPatternDeferredLoadPromiseDeferred.promise;
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

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeSelector, loadMainGridColumnsSection, loadSubSectionsSection, loadInvoiceGridActionsSection, loadConcatenatedParts, loadSerialNumberPattern, loadInvoiceActionsGrid, loadInvoiceGeneratorActionGrid, loadInvoiceExtendedSettings])
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   })
                  .finally(function () {
                      $scope.scopeModel.isLoading = false;
                  });
            }
        }

        function getContext()
        {
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
                }
            };
            return context;
        }

    }

    appControllers.controller('VR_Invoice_InvoiceTypeEditorController', invoiceTypeEditorController);
})(appControllers);
