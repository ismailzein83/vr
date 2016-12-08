(function (appControllers) {

    'use strict';

    itemGroupingSubSectionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function itemGroupingSubSectionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var subSectionEntity;

        var isEditMode;

        var itemGroupingSubSectionSettingsAPI;
        var itemGroupingSubSectionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.onItemGroupingSubSectionSettingsReady = function (api) {
                itemGroupingSubSectionSettingsAPI = api;
                itemGroupingSubSectionSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSubSection() : addeSubSection();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function builSubSectionObjFromScope() {
                return {
                    SectionTitle: $scope.scopeModel.sectionTitle,
                    Settings: itemGroupingSubSectionSettingsAPI.getData(),
                };
            }
            function addeSubSection() {
                var subSectionObj = builSubSectionObjFromScope();
                if ($scope.onItemGroupingSubSectionAdded != undefined) {
                    $scope.onItemGroupingSubSectionAdded(subSectionObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateSubSection() {
                var subSectionObj = builSubSectionObjFromScope();
                if ($scope.onItemGroupingSubSectionUpdated != undefined) {
                    $scope.onItemGroupingSubSectionUpdated(subSectionObj);
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

                function loadItemGroupingSubSectionSettings() {
                    var itemGroupingSubSectionSettingsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    itemGroupingSubSectionSettingsReadyPromiseDeferred.promise.then(function () {
                        var itemGroupingDirectivePayload = { context: getContext() };
                        if(subSectionEntity != undefined)
                            itemGroupingDirectivePayload.subSectionSettingsEntity = subSectionEntity.Settings;
                        VRUIUtilsService.callDirectiveLoad(itemGroupingSubSectionSettingsAPI, itemGroupingDirectivePayload, itemGroupingSubSectionSettingsDeferredLoadPromiseDeferred);
                    });
                    return itemGroupingSubSectionSettingsDeferredLoadPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadItemGroupingSubSectionSettings]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            }
            
        }
        function getContext()
        {
            var currentContext = context;
            if(currentContext == undefined)
            {
                currentContext = {};
            }
            return currentContext;
        }
    }
    appControllers.controller('VR_Invoice_ItemGroupingSubSectionEditorController', itemGroupingSubSectionEditorController);

})(appControllers);