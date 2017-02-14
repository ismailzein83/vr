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

        var invoiceActionsAPI;
        var invoiceActionsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceGeneratorActionAPI;
        var invoiceGeneratorActionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceExtendedSettingsAPI;
        var invoiceExtendedSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var viewPermissionAPI;
        var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var generatePermissionAPI;
        var generatePermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var startCalculationMethodAPI;
        var startCalculationMethodPromiseDeferred = UtilsService.createPromiseDeferred();

        var itemGroupingsDirectiveAPI;
        var itemGroupingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceSettingDefinitionDirectiveAPI;
        var invoiceSettingDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var automaticInvoiceActionsAPI;
        var automaticInvoiceActionsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onItemGroupingsDirectiveReady = function (api) {
                itemGroupingsDirectiveAPI = api;
                itemGroupingsReadyPromiseDeferred.resolve();
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

            $scope.scopeModel.onInvoiceSettingDefinitionReady = function (api)
            {
                invoiceSettingDefinitionDirectiveAPI = api;
                invoiceSettingDefinitionReadyPromiseDeferred.resolve();
            }

            $scope.scopeModel.onConcatenatedPartsReady = function (api) {
                concatenatedPartsAPI = api;
                concatenatedPartsReadyPromiseDeferred.resolve();
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

            $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                viewPermissionAPI = api;
                viewPermissionReadyDeferred.resolve();
            };

            $scope.scopeModel.onGenerateRequiredPermissionReady = function (api) {
                generatePermissionAPI = api;
                generatePermissionReadyDeferred.resolve();
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
                        },
                        SubSections: subSectionsAPI.getData(),
                        Security: {
                            ViewRequiredPermission: viewPermissionAPI.getData(),
                            GenerateRequiredPermission: generatePermissionAPI.getData()
                        },
                        ItemGroupings: itemGroupingsDirectiveAPI.getData(),
                        StartDateCalculationMethod:startCalculationMethodAPI.getData(),
                        InvoiceSettingPartUISections: invoiceSettingDefinitionDirectiveAPI.getData(),
                        AutomaticInvoiceActions: automaticInvoiceActionsAPI.getData(),
                        UseTimeZone:$scope.scopeModel.useTimeZone
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
                        $scope.scopeModel.useTimeZone = invoiceTypeEntity.Settings.UseTimeZone;
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
                        var startCalculationMethodPayload = invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined? { startDateCalculationMethodEntity: invoiceTypeEntity.Settings.StartDateCalculationMethod } : undefined;
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

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeSelector, loadMainGridColumnsSection, loadSubSectionsSection, loadInvoiceGridActionsSection, loadConcatenatedParts, loadInvoiceActionsGrid, loadInvoiceGeneratorActionGrid, loadInvoiceExtendedSettings, loadViewRequiredPermission, loadGenerateRequiredPermission, loadStartCalculationMethod, loadItemGroupingsDirective, loadInvoiceSettingDefinitionDirective, loadAutomaticInvoiceActionsGrid])
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
                },

                getItemGroupingsInfo: function ()
                {
                    var itemGroupingsInfo = [];
                    var itemGroupings = itemGroupingsDirectiveAPI.getData();
                    if(itemGroupings != undefined)
                    {
                        for(var i=0;i<itemGroupings.length;i++)
                        {
                            var itemGrouping = itemGroupings[i];
                            itemGroupingsInfo.push({
                                Name: itemGrouping.ItemSetName,
                                ItemGroupingId: itemGrouping.ItemGroupingId
                            });
                        }
                    }
                    return itemGroupingsInfo;
                },

                getGroupingDimensions: function (itemGroupingId)
                {
                    var groupingDimensions = [];
                    var itemGroupings = itemGroupingsDirectiveAPI.getData();
                    if (itemGroupings != undefined) {
                        var itemGrouping = UtilsService.getItemByVal(itemGroupings, itemGroupingId, "ItemGroupingId");
                        if (itemGrouping.DimensionItemFields != undefined)
                            groupingDimensions = itemGrouping.DimensionItemFields;
                    }
                    return groupingDimensions;
                },

                getGroupingMeasures: function (itemGroupingId)
                {
                    var groupingMeasures = [];
                    var itemGroupings = itemGroupingsDirectiveAPI.getData();
                    if (itemGroupings != undefined) {
                        var itemGrouping = UtilsService.getItemByVal(itemGroupings, itemGroupingId, "ItemGroupingId");
                        if (itemGrouping.AggregateItemFields != undefined)
                        {
                            for(var i=0;i<itemGrouping.AggregateItemFields.length;i++)
                            {
                                var aggregateItem = itemGrouping.AggregateItemFields[i];
                                groupingMeasures.push({
                                    MeasureItemFieldId:aggregateItem.AggregateItemFieldId,
                                    FieldName:aggregateItem.FieldName,
                                    FieldDescription: aggregateItem.FieldDescription,
                                    FieldType: aggregateItem.FieldType
                                });
                            }
                        }
                    }
                    return groupingMeasures;
                },

                getActionsInfoByActionTypeName: function (actionTypeName)
                {
                    var actionsInfo =[];
                    var invoiceActions = invoiceActionsAPI.getData();
                    if(invoiceActions != undefined)
                    {
                        for (var i = 0; i < invoiceActions.length; i++) {
                            var action = invoiceActions[i];
                            if (action.Settings.ActionTypeName == actionTypeName)
                            {
                                actionsInfo.push({
                                    Title: action.Title,
                                    InvoiceActionId: action.InvoiceActionId
                                });
                            }
                        }
                    }
                    return actionsInfo;
                }
            };
            return context;
        }

    }

    appControllers.controller('VR_Invoice_InvoiceTypeEditorController', invoiceTypeEditorController);
})(appControllers);
