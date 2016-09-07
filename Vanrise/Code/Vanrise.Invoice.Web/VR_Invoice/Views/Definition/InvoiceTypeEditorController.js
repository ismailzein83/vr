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

        var invoicePartnerSettingsAPI;
        var invoicePartnerSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();
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
            $scope.scopeModel = {}
            $scope.scopeModel.onDataRecordTypeSelectorReady = function(api)
            {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onDataRecordTypeSelectionChanged = function () {
                $scope.scopeModel.isLoading = true;
                var dataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                if (dataRecordTypeId != undefined) {
                    VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                        $scope.scopeModel.isLoading = false;
                        dataRecordTypeEntity = response;
                    });
                }
            }
            $scope.scopeModel.onInvoicePartnerSettingsReady = function(api)
            {
                invoicePartnerSettingsAPI = api;
                invoicePartnerSettingsReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onInvoiceGeneratorReady = function (api)
            {
                invoiceGeneratorAPI = api;
                invoiceGeneratorReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onMainGridColumnsReady = function (api) {
                mainGridColumnsAPI = api;
                mainGridColumnsSectionReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onInvoiceGridActionsReady = function (api) {
                invoiceGridActionsAPI = api;
                invoiceGridActionsSectionReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onSubSectionsReady = function (api) {
                subSectionsAPI = api;
                subSectionsSectionReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.saveInvoiceType = function () {
                $scope.scopeModel.isLoading = true;
                if (isEditMode) {
                    return updateInvoiceType();
                }
                else {
                    return insertInvoiceType();
                }
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
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
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeSelector, loadMainGridColumnsSection, loadSubSectionsSection, loadInvoiceGeneratorDirective, loadInvoiceGridActionsSection, loadInvoicePartnerSettingsSection])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && invoiceTypeEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(invoiceTypeEntity.Name, 'Invoice Type');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Invoice Type');
        }

        function getInvoiceType() {
            return VR_Invoice_InvoiceTypeAPIService.GetInvoiceType(invoiceTypeId).then(function (response) {
                invoiceTypeEntity = response;
            });
        }

        function loadStaticData() {
            if(invoiceTypeEntity != undefined)
            {
                $scope.scopeModel.name = invoiceTypeEntity.Name;
            }
        }

        function buildInvoiceTypeObjFromScope() {
            var obj = {
                InvoiceTypeId: invoiceTypeId,
                Name: $scope.scopeModel.name,
                Settings: {
                    InvoiceDetailsRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                    UISettings: {
                        MainGridColumns: mainGridColumnsAPI.getData(),
                        SubSections: subSectionsAPI.getData(),
                        InvoiceGridActions: invoiceGridActionsAPI.getData(),
                        PartnerSettings: invoicePartnerSettingsAPI.getData()
                    },
                    InvoiceGenerator: invoiceGeneratorAPI.getData()
                }
            };
            return obj;
        }

        function loadInvoicePartnerSettingsSection() {
            var invoicePartnerSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            invoicePartnerSettingsReadyPromiseDeferred.promise.then(function () {
                var partnerSettingsPayload = invoiceTypeEntity != undefined ? { partnerSettingsEntity: invoiceTypeEntity.Settings.UISettings.PartnerSettings } : undefined;
                VRUIUtilsService.callDirectiveLoad(invoicePartnerSettingsAPI, partnerSettingsPayload, invoicePartnerSettingsLoadPromiseDeferred);
            });
            return invoicePartnerSettingsLoadPromiseDeferred.promise;
        }

        function loadInvoiceGridActionsSection() {
            var invoiceGridActionsSectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            invoiceGridActionsSectionReadyPromiseDeferred.promise.then(function () {
                var invoiceGridActionsPayload = { context: getContext() }
                if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings && invoiceTypeEntity.Settings.UISettings)
                {
                    invoiceGridActionsPayload.invoiceGridActions= invoiceTypeEntity.Settings.UISettings.InvoiceGridActions; 
                }
                VRUIUtilsService.callDirectiveLoad(invoiceGridActionsAPI, invoiceGridActionsPayload, invoiceGridActionsSectionLoadPromiseDeferred);
            });
            return invoiceGridActionsSectionLoadPromiseDeferred.promise;
        }

        function loadDataRecordTypeSelector() {
            var dataRecordTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            dataRecordTypeSelectorReadyPromiseDeferred.promise.then(function () {
                var dataRecordTypePayload = invoiceTypeEntity != undefined ? { selectedIds: invoiceTypeEntity.Settings.InvoiceDetailsRecordTypeId }: undefined;
                VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypePayload, dataRecordTypeSelectorLoadPromiseDeferred);
            });
            return dataRecordTypeSelectorLoadPromiseDeferred.promise;
        }

        function loadInvoiceGeneratorDirective() {
            var invoiceGeneratorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            invoiceGeneratorReadyPromiseDeferred.promise.then(function () {
                var invoiceGeneratorPayload = invoiceTypeEntity != undefined ? { invoiceGeneratorEntity: invoiceTypeEntity.Settings.InvoiceGenerator } : undefined;
                VRUIUtilsService.callDirectiveLoad(invoiceGeneratorAPI, invoiceGeneratorPayload, invoiceGeneratorLoadPromiseDeferred);
            });
            return invoiceGeneratorLoadPromiseDeferred.promise;
        }

        function loadMainGridColumnsSection() {
            var mainGridColumnsSectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            mainGridColumnsSectionReadyPromiseDeferred.promise.then(function () {
                var mainGridColumnsPayload = { context: getContext() }
                if (invoiceTypeEntity != undefined && invoiceTypeEntity.Settings && invoiceTypeEntity.Settings.UISettings)
                {
                    mainGridColumnsPayload.mainGridColumns = invoiceTypeEntity.Settings.UISettings.MainGridColumns;
                }
                VRUIUtilsService.callDirectiveLoad(mainGridColumnsAPI, mainGridColumnsPayload, mainGridColumnsSectionLoadPromiseDeferred);
            });
            return mainGridColumnsSectionLoadPromiseDeferred.promise;
        }

        function loadSubSectionsSection() {
            var subSectionsSectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            subSectionsSectionReadyPromiseDeferred.promise.then(function () {
                var mainGridColumnsPayload = {context:getContext() };

                if(invoiceTypeEntity != undefined)
                    mainGridColumnsPayload.subSections = invoiceTypeEntity.Settings.UISettings.SubSections;
                VRUIUtilsService.callDirectiveLoad(subSectionsAPI, mainGridColumnsPayload, subSectionsSectionLoadPromiseDeferred);
            });
            return subSectionsSectionLoadPromiseDeferred.promise;
        }

        function getContext()
        {
            var context = {
                getFields: function () {
                    var fields = [];
                    if (dataRecordTypeEntity != undefined && dataRecordTypeEntity.Fields !=undefined)
                    {
                        for (var i = 0; i < dataRecordTypeEntity.Fields.length; i++)
                        {
                            var field = dataRecordTypeEntity.Fields[i];
                            fields.push({ FieldName: field.Name, FieldTitle: field.Title, Type: field.Type });
                        }
                    }
                    return fields;
                }
            }
            return context;
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

    appControllers.controller('VR_Invoice_InvoiceTypeEditorController', invoiceTypeEditorController);
})(appControllers);
