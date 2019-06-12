﻿(function (appControllers) {

    'use strict';

    subSectionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function subSectionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var subSectionEntity;

        var isEditMode;
        var invoiceUISubsectionSettingsAPI;
        var invoiceUISubsectionSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        var recordFilterAPI;
        var recordFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceSubsectionFilterAPI;
        var invoiceSubsectionFilterReadyDeferred = UtilsService.createPromiseDeferred();

        var localizationTextResourceSelectorAPI;
        var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                subSectionEntity = parameters.subSectionEntity;
            }
            isEditMode = (subSectionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onInvoiceUISubsectionSettingsReady = function (api) {
                invoiceUISubsectionSettingsAPI = api;
                invoiceUISubsectionSettingsReadyDeferred.resolve();
            };
            $scope.scopeModel.onRecordFilterReady = function (api) {
                recordFilterAPI = api;
                recordFilterReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onInvoiceSubsectionFilterReady = function (api) {
                invoiceSubsectionFilterAPI = api;
                invoiceSubsectionFilterReadyDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSubSection() : addeSubSection();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onLocalizationTextResourceSelectorReady = function (api) {
                localizationTextResourceSelectorAPI = api;
                localizationTextResourceSelectorReadyPromiseDeferred.resolve();
            };
            function builSubSectionObjFromScope() {
                var filterGroup = recordFilterAPI.getData();

                return {
                    SectionTitle: $scope.scopeModel.sectionTitle,
                    SubSectionFilter: filterGroup != undefined ? filterGroup.filterObj : undefined,
                    InvoiceSubSectionId: subSectionEntity != undefined ? subSectionEntity.InvoiceSubSectionId : UtilsService.guid(),
                    Settings: invoiceUISubsectionSettingsAPI.getData(),
                    Filter: invoiceSubsectionFilterAPI != undefined ? invoiceSubsectionFilterAPI.getData() : undefined,
                    SectionTitleResourceKey: localizationTextResourceSelectorAPI != undefined ? localizationTextResourceSelectorAPI.getSelectedValues() : undefined
                };
            }

            function addeSubSection() {
                var subSectionObj = builSubSectionObjFromScope();
                if ($scope.onSubSectionAdded != undefined) {
                    $scope.onSubSectionAdded(subSectionObj);
                }
                $scope.modalContext.closeModal();
            }

            function updateSubSection() {
                var subSectionObj = builSubSectionObjFromScope();
                if ($scope.onSubSectionUpdated != undefined) {
                    $scope.onSubSectionUpdated(subSectionObj);
                }
                $scope.modalContext.closeModal();
            }

        }

        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {

                function setTitle() {
                    if (isEditMode && subSectionEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(subSectionEntity.SectionTitle, 'Sub Section');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Sub Section');
                }

                function loadStaticData() {
                    if (subSectionEntity != undefined) {
                        $scope.scopeModel.sectionTitle = subSectionEntity.SectionTitle;
                    }
                }

                function loadInvoiceUISubsectionSettingsDirective() {
                    var invoiceUISubsectionSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                    invoiceUISubsectionSettingsReadyDeferred.promise.then(function () {
                        var invoiceUISubsectionSettingsPayload = { context: getContext() };
                        if (subSectionEntity != undefined)
                            invoiceUISubsectionSettingsPayload.invoiceUISubSectionSettingsEntity = subSectionEntity.Settings;
                        VRUIUtilsService.callDirectiveLoad(invoiceUISubsectionSettingsAPI, invoiceUISubsectionSettingsPayload, invoiceUISubsectionSettingsLoadDeferred);
                    });
                    return invoiceUISubsectionSettingsLoadDeferred.promise;
                }

                function loadRecordFilterDirective() {
                    var recordFilterLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    recordFilterReadyPromiseDeferred.promise.then(function () {
                        var recordFilterPayload = { context: getContext() };
                        if (subSectionEntity != undefined) {
                            recordFilterPayload.FilterGroup = subSectionEntity.SubSectionFilter;
                        }
                        VRUIUtilsService.callDirectiveLoad(recordFilterAPI, recordFilterPayload, recordFilterLoadPromiseDeferred);
                    });
                    return recordFilterLoadPromiseDeferred.promise;
                }
                function loadInvoiceSubsectionFilterDirective() {
                    var invoiceSubsectionFilterLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceSubsectionFilterReadyDeferred.promise.then(function () {
                        var invoiceSubsectionFilterPayload = { context: getContext() };
                        if (subSectionEntity != undefined) {
                            invoiceSubsectionFilterPayload.invoiceSubsectionFilterEntity = subSectionEntity.Filter;
                        }
                        VRUIUtilsService.callDirectiveLoad(invoiceSubsectionFilterAPI, invoiceSubsectionFilterPayload, invoiceSubsectionFilterLoadPromiseDeferred);
                    });
                    return invoiceSubsectionFilterLoadPromiseDeferred.promise;
                }

                function loadLocalizationTextResourceSelector() {
                    var localizationTextResourceSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
                        var localizationTextResourcePayload = subSectionEntity != undefined ? { selectedValue: subSectionEntity.SectionTitleResourceKey } : undefined;

                        VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, localizationTextResourcePayload, localizationTextResourceSelectorLoadPromiseDeferred);
                    });
                    return localizationTextResourceSelectorLoadPromiseDeferred.promise;
                }
                var promises = [setTitle, loadStaticData, loadInvoiceUISubsectionSettingsDirective, loadRecordFilterDirective, loadInvoiceSubsectionFilterDirective, loadLocalizationTextResourceSelector];
                return UtilsService.waitMultipleAsyncOperations(promises).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

        }

        function getContext() {
            var currentContext = UtilsService.cloneObject(context, true);

            if (currentContext == undefined) {
                currentContext = {};
            }
            return currentContext;
        }

    }
    appControllers.controller('VR_Invoice_SubSectionEditorController', subSectionEditorController);

})(appControllers);