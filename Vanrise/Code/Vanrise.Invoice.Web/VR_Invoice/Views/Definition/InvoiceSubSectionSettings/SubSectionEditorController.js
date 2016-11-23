(function (appControllers) {

    'use strict';

    subSectionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService','VRUIUtilsService'];

    function subSectionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var subSectionEntity;

        var isEditMode;
        var invoiceUISubsectionSettingsAPI;
        var invoiceUISubsectionSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        var recordFilterAPI;
        var recordFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSubSection() : addeSubSection();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };


            function builSubSectionObjFromScope() {
                var filterGroup = recordFilterAPI.getData();

                return {
                    SectionTitle: $scope.scopeModel.sectionTitle,
                    SubSectionFilter: filterGroup != undefined ? filterGroup.filterObj : undefined,
                    UniqueSectionID: UtilsService.guid(),
                    Settings: invoiceUISubsectionSettingsAPI.getData()
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
                        var invoiceUISubsectionSettingsPayload;
                        if (subSectionEntity != undefined)
                            invoiceUISubsectionSettingsPayload = { invoiceUISubSectionSettingsEntity: subSectionEntity.Settings };
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

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadInvoiceUISubsectionSettingsDirective, loadRecordFilterDirective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            }

        }

        function getContext() {
            return context;
        }

    }
    appControllers.controller('VR_Invoice_SubSectionEditorController', subSectionEditorController);

})(appControllers);