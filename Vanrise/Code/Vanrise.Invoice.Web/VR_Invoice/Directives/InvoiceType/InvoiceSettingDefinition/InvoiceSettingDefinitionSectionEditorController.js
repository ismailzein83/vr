(function (appControllers) {

    "use strict";

    InvoiceSettingDefinitionSectionEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function InvoiceSettingDefinitionSectionEditorController($scope, UtilsService, VRNotificationService, VRNavigationService) {

        var isEditMode;
        var sectionTitleValue;
        var exitingSections;
        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                sectionTitleValue = parameters.sectionTitleValue;
                exitingSections = parameters.exitingSections;
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

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).then(function () {

                }).finally(function () {
                    $scope.scopeModal.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }
        }

        function buildSectionObjFromScope() {
            var sectionTitle = $scope.scopeModal.sectionValue;
            return sectionTitle;
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
