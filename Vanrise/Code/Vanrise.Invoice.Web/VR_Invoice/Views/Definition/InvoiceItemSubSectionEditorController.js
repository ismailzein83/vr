(function (appControllers) {

    'use strict';

    invoiceItemSubSectionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function invoiceItemSubSectionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var subSectionEntity;

        var isEditMode;

        var invoiceItemSubSectionGridColumnsAPI;
        var invoiceItemSubSectionGridColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.onInvoiceItemSubSectionReady = function (api)
            {
                invoiceItemSubSectionGridColumnsAPI = api;
                invoiceItemSubSectionGridColumnsReadyPromiseDeferred.resolve();
            }

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSubSection() : addeSubSection();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadInvoiceItemSubSectionSettings]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
        }
      

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

        function builSubSectionObjFromScope() {
            return {
                SectionTitle: $scope.scopeModel.sectionTitle,
                UniqueSectionID:UtilsService.guid(),
                Settings: invoiceItemSubSectionGridColumnsAPI.getData()
            };
        }
        function loadInvoiceItemSubSectionSettings()
        {
            var invoiceItemSubSectionGridColumnsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            invoiceItemSubSectionGridColumnsReadyPromiseDeferred.promise.then(function () {
                var invoiceItemDirectivePayload = subSectionEntity != undefined ?  subSectionEntity.Settings : undefined;
                VRUIUtilsService.callDirectiveLoad(invoiceItemSubSectionGridColumnsAPI, invoiceItemDirectivePayload, invoiceItemSubSectionGridColumnsDeferredLoadPromiseDeferred);
            });
            return invoiceItemSubSectionGridColumnsDeferredLoadPromiseDeferred.promise;
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
    appControllers.controller('VR_Invoice_InvoiceItemSubSectionEditorController', invoiceItemSubSectionEditorController);

})(appControllers);