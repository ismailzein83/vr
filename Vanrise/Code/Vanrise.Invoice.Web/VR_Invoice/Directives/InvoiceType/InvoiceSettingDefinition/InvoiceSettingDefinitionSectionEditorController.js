(function (appControllers) {

    "use strict";

    InvoiceSettingDefinitionSectionEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function InvoiceSettingDefinitionSectionEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var sectionTitleValue;
        var exitingSections;
        var sectionEntity;
        var sectionTitleResourceKey;
        var localizationTextResourceSelectorAPI;
        var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                sectionEntity = parameters.sectionEntity;
                exitingSections = parameters.exitingSections;
            }
            if (sectionEntity != undefined) {
                sectionTitleValue = sectionEntity.sectionTitle;
                sectionTitleResourceKey = sectionEntity.sectionTitleResourceKey;
            }
            isEditMode = (sectionTitleValue != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};

            $scope.scopeModal.SaveSection = function () {
                if (isEditMode) {
                    return updateSection();
                }
                else {
                    return insertSection();
                }
            };

            $scope.scopeModal.validate = function () {
                if (isEditMode && $scope.scopeModal.sectionValue.toLowerCase() == sectionTitleValue.toLowerCase()) {
                    return null;
                }
                else if (UtilsService.contains(exitingSections, $scope.scopeModal.sectionValue.toLowerCase())) {
                    return "Same name exists.";
                }
                return null;
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModal.onLocalizationTextResourceSelectorReady = function (api) {
                localizationTextResourceSelectorAPI = api;
                localizationTextResourceSelectorReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModal.isLoading = true;
            loadAllControls();

            function loadAllControls() {

                function setTitle() {
                    if (isEditMode && sectionTitleValue != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(sectionTitleValue, 'Section');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Section');
                }

                function loadStaticData() {
                    $scope.scopeModal.sectionValue = sectionTitleValue;
                }
                function loadLocalizationTextResourceSelector() {
                    var localizationTextResourceSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
                        var localizationTextResourcePayload = { selectedValue: sectionTitleResourceKey };

                        VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, localizationTextResourcePayload, localizationTextResourceSelectorLoadPromiseDeferred);
                    });
                    return localizationTextResourceSelectorLoadPromiseDeferred.promise;
                }

                var promises = [setTitle, loadStaticData, loadLocalizationTextResourceSelector];
                return UtilsService.waitMultipleAsyncOperations(promises).then(function () {

                }).finally(function () {
                    $scope.scopeModal.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }
        }

        function buildSectionObjFromScope() {
            return {
                sectionTitle: $scope.scopeModal.sectionValue,
                sectionTitleResourceKey: localizationTextResourceSelectorAPI != undefined ? localizationTextResourceSelectorAPI.getSelectedValues() : undefined
            };
        }

        function insertSection() {
            var sectionTitle = buildSectionObjFromScope();
            if ($scope.onSectionAdded != undefined)
                $scope.onSectionAdded(sectionTitle);
            $scope.modalContext.closeModal();
        }

        function updateSection() {
            var sectionTitle = buildSectionObjFromScope();
            if ($scope.onSectionUpdated != undefined)
                $scope.onSectionUpdated(sectionTitle);
            $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('VR_Invoice_InvoiceSettingDefinitionSectionEditorController', InvoiceSettingDefinitionSectionEditorController);
})(appControllers);
