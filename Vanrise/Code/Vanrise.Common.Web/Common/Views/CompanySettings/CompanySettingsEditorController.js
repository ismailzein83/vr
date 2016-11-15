(function (appControllers) {

    "use strict";

    companySettingsEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function companySettingsEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var companySettingEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                companySettingEntity = parameters.companySettingEntity;
            }
            isEditMode = (companySettingEntity != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.saveCompanySetting = function () {
                if (isEditMode)
                    return updateCompanySettings();
                else
                    return insertCompanySettings();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };


        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls()
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && companySettingEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(companySettingEntity.CompanyName, "Company Setting");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Company Setting");
        }

        function loadStaticData() {

            if (companySettingEntity == undefined)
                return;
            $scope.scopeModel.companyName = companySettingEntity.CompanyName;
            $scope.scopeModel.registrationNumber = companySettingEntity.RegistrationNumber;
            $scope.scopeModel.registrationAddress = companySettingEntity.RegistrationAddress;
            $scope.scopeModel.vatId = companySettingEntity.VatId;
            $scope.scopeModel.isDefault = companySettingEntity.IsDefault;
            if (companySettingEntity.CompanyLogo > 0)
                $scope.scopeModel.companyLogo = {
                    fileId: companySettingEntity.CompanyLogo
                };
            else
                $scope.scopeModel.companyLogo = null;

        }

        function buildCompanySettingsObjFromScope() {
            var obj = {
                CompanyName: $scope.scopeModel.companyName,
                RegistrationNumber: $scope.scopeModel.registrationNumber,
                RegistrationAddress: $scope.scopeModel.registrationAddress,
                VatId: $scope.scopeModel.vatId,
                CompanyLogo: ($scope.scopeModel.companyLogo != null) ? $scope.scopeModel.companyLogo.fileId : 0,
                IsDefault:$scope.scopeModel.isDefault
            };
            return obj;
        }

        function insertCompanySettings() {
            var companySettingsObject = buildCompanySettingsObjFromScope();
            if ($scope.onCompanySettingsAdded != undefined)
                $scope.onCompanySettingsAdded(companySettingsObject);
            $scope.modalContext.closeModal();
        }
        function updateCompanySettings() {
            var companySettingsObject = buildCompanySettingsObjFromScope();
            if ($scope.onCompanySettingsUpdated != undefined)
                $scope.onCompanySettingsUpdated(companySettingsObject);
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('VRCommon_CompanySettingsEditorController', companySettingsEditorController);
})(appControllers);
