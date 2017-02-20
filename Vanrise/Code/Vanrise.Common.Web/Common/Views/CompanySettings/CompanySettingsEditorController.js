(function (appControllers) {

    "use strict";

    companySettingsEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function companySettingsEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var companySettingEntity;

        var bankDirectiveApi;
        var bankReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var bankSelectedPromiseDeferred;

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

            $scope.onBankDirectiveReady = function (api) {
                bankDirectiveApi = api;
                bankReadyPromiseDeferred.resolve();
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls()
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadBankDetail])
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


        function loadBankDetail() {
            var loadBankPromiseDeferred = UtilsService.createPromiseDeferred();
            
            bankReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;
                    if (companySettingEntity != undefined && companySettingEntity.BankDetails != undefined) {
                        directivePayload = {
                            selectedIds: companySettingEntity.BankDetails
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(bankDirectiveApi, directivePayload, loadBankPromiseDeferred);
                });
            return loadBankPromiseDeferred.promise;
        }

        function loadStaticData() {

            if (companySettingEntity == undefined)
                return;
            $scope.scopeModel.companyName = companySettingEntity.CompanyName;
            $scope.scopeModel.profileName = companySettingEntity.ProfileName;
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

            if (companySettingEntity.BillingEmails != undefined) {
                $scope.scopeModel.toMail = companySettingEntity.BillingEmails.split(";");
            }
        }

        function buildCompanySettingsObjFromScope() {
            var obj = {
                CompanySettingId: companySettingEntity != undefined? companySettingEntity.CompanySettingId : UtilsService.guid(),
                CompanyName: $scope.scopeModel.companyName,
                ProfileName: $scope.scopeModel.profileName,
                RegistrationNumber: $scope.scopeModel.registrationNumber,
                RegistrationAddress: $scope.scopeModel.registrationAddress,
                VatId: $scope.scopeModel.vatId,
                CompanyLogo: ($scope.scopeModel.companyLogo != null) ? $scope.scopeModel.companyLogo.fileId : 0,
                IsDefault: $scope.scopeModel.isDefault,
                BillingEmails: $scope.scopeModel.toMail.join(";"),
                BankDetails: bankDirectiveApi.getSelectedIds()
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
