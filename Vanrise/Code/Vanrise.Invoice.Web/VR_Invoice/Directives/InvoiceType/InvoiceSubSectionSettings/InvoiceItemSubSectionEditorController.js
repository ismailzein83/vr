﻿(function (appControllers) {

    'use strict';

    invoiceItemSubSectionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function invoiceItemSubSectionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var subSectionEntity;

        var isEditMode;

        var invoiceItemSubSectionGridColumnsAPI;
        var invoiceItemSubSectionGridColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onInvoiceItemSubSectionReady = function (api) {
                invoiceItemSubSectionGridColumnsAPI = api;
                invoiceItemSubSectionGridColumnsReadyPromiseDeferred.resolve();
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
                return {
                    SectionTitle: $scope.scopeModel.sectionTitle,
                    UniqueSectionID: UtilsService.guid(),
                    Settings: invoiceItemSubSectionGridColumnsAPI.getData(),
                    SectionTitleResourceKey: localizationTextResourceSelectorAPI != undefined ? localizationTextResourceSelectorAPI.getSelectedValues() : undefined
                };
            }
            function addeSubSection() {
                var subSectionObj = builSubSectionObjFromScope();
                if ($scope.onInvoiceItemSubSectionAdded != undefined) {
                    $scope.onInvoiceItemSubSectionAdded(subSectionObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateSubSection() {
                var subSectionObj = builSubSectionObjFromScope();
                if ($scope.onInvoiceItemSubSectionUpdated != undefined) {
                    $scope.onInvoiceItemSubSectionUpdated(subSectionObj);
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

                function loadInvoiceItemSubSectionSettings() {
                    var invoiceItemSubSectionGridColumnsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceItemSubSectionGridColumnsReadyPromiseDeferred.promise.then(function () {
                        var invoiceItemDirectivePayload = subSectionEntity != undefined ? subSectionEntity.Settings : undefined;
                        VRUIUtilsService.callDirectiveLoad(invoiceItemSubSectionGridColumnsAPI, invoiceItemDirectivePayload, invoiceItemSubSectionGridColumnsDeferredLoadPromiseDeferred);
                    });
                    return invoiceItemSubSectionGridColumnsDeferredLoadPromiseDeferred.promise;
                }
                function loadLocalizationTextResourceSelector() {
                    var localizationTextResourceSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
                        var localizationTextResourcePayload = subSectionEntity != undefined ? { selectedValue: subSectionEntity.SectionTitleResourceKey } : undefined;

                        VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, localizationTextResourcePayload, localizationTextResourceSelectorLoadPromiseDeferred);
                    });
                    return localizationTextResourceSelectorLoadPromiseDeferred.promise;
                }
                var promises = [setTitle, loadStaticData, loadInvoiceItemSubSectionSettings, loadLocalizationTextResourceSelector];

                return UtilsService.waitMultipleAsyncOperations(promises).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
        }
    }
    appControllers.controller('VR_Invoice_InvoiceItemSubSectionEditorController', invoiceItemSubSectionEditorController);

})(appControllers);