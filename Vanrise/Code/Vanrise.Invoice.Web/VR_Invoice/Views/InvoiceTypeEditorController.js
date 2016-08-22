(function (appControllers) {

    "use strict";

    invoiceTypeEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeAPIService'];

    function invoiceTypeEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeAPIService) {
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeSelector, loadMainGridColumnsSection, loadSubSectionsSection, loadInvoiceGeneratorDirective, loadInvoiceGridActionsSection])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
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
                $scope.scopeModel.partnerSelector = invoiceTypeEntity.Settings.UISettings.PartnerSelector;
                $scope.scopeModel.partnerManagerFQTN = UtilsService.serializetoJson(invoiceTypeEntity.Settings.UISettings.PartnerManagerFQTN);
            }
        }

        function buildInvoiceTypeObjFromScope() {
            var obj = {
                InvoiceTypeId: invoiceTypeId,
                Name: $scope.scopeModel.name,
                Settings: {
                    InvoiceDetailsRecordTypeId:dataRecordTypeSelectorAPI.getSelectedIds(),
                    UISettings: {
                        PartnerSelector: $scope.scopeModel.partnerSelector,
                        MainGridColumns: mainGridColumnsAPI.getData(),
                        SubSections: subSectionsAPI.getData(),
                        InvoiceGridActions : invoiceGridActionsAPI.getData(),
                        PartnerManagerFQTN: UtilsService.parseStringToJson($scope.scopeModel.partnerManagerFQTN)
                    },
                    InvoiceGenerator: invoiceGeneratorAPI.getData()
                }
            };
            return obj;
        }

        function loadInvoiceGridActionsSection() {
            var invoiceGridActionsSectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            invoiceGridActionsSectionReadyPromiseDeferred.promise.then(function () {
                var invoiceGridActionsPayload = invoiceTypeEntity != undefined ? { invoiceGridActions: invoiceTypeEntity.Settings.UISettings.InvoiceGridActions } : undefined;
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
            console.log(invoiceTypeEntity);
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
                var mainGridColumnsPayload = invoiceTypeEntity != undefined ? { mainGridColumns: invoiceTypeEntity.Settings.UISettings.MainGridColumns } : undefined;
                VRUIUtilsService.callDirectiveLoad(mainGridColumnsAPI, mainGridColumnsPayload, mainGridColumnsSectionLoadPromiseDeferred);
            });
            return mainGridColumnsSectionLoadPromiseDeferred.promise;
        }

        function loadSubSectionsSection() {
            var subSectionsSectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            subSectionsSectionReadyPromiseDeferred.promise.then(function () {
                var mainGridColumnsPayload = invoiceTypeEntity != undefined ? { subSections: invoiceTypeEntity.Settings.UISettings.SubSections } : undefined;
                VRUIUtilsService.callDirectiveLoad(subSectionsAPI, mainGridColumnsPayload, subSectionsSectionLoadPromiseDeferred);
            });
            return subSectionsSectionLoadPromiseDeferred.promise;
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
