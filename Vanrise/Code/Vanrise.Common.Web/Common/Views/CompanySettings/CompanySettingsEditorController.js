(function (appControllers) {

    "use strict";

    companySettingsEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRCommon_CompanySettingsAPIService'];

    function companySettingsEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VRCommon_CompanySettingsAPIService) {

        var isEditMode;
        var setDefault;
        var companySettingEntity;

        var contactsTypes = [];

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
                setDefault = parameters.setDefault;
            }
            isEditMode = (companySettingEntity != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.contactTabObject = { showTab: false };
            $scope.scopeModel.contacts = [];

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
            loadContactsTypes().then(function () {
                loadAllControls();
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadBankDetail, loadCompanyContacts])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function loadContactsTypes() {
            return VRCommon_CompanySettingsAPIService.GetCompanyContactTypes().then(function (response) {
                contactsTypes = response;
                if (response != null && response.length > 0)
                    $scope.scopeModel.contactTabObject.showTab = true;
            });
        }

        function setTitle() {
            if (isEditMode && companySettingEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(companySettingEntity.CompanyName, "Company");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Company");
        }

        function loadCompanyContacts() {
            if (contactsTypes != null ) {
                for (var i = 0; i < contactsTypes.length; i++) {
                    var contactType = contactsTypes[i];
                    var settings;
                    if (companySettingEntity != undefined && companySettingEntity.Contacts != undefined) {
                        settings = companySettingEntity.Contacts[contactType.Name];
                    }
                    addContact(contactType, settings);
                }
            }
            
        }

        function addContact(contactType, settings) {
            var contact = {
                label: contactType.Title,
                contactType :contactType.Name,
                name: settings != undefined ? settings.ContactName : undefined,
                title: settings != undefined ? settings.Title : undefined,
                email: settings != undefined ? settings.Email : undefined
            };
            $scope.scopeModel.contacts.push(contact);
        }
        function getContactsData() {
            var contacts = {};
            if ($scope.scopeModel.contacts.length > 0) {
                for (var i = 0; i < $scope.scopeModel.contacts.length; i++) {
                    var contact = $scope.scopeModel.contacts[i];
                    var obj = {
                        ContactName: contact.name,
                        Title: contact.title,
                        Email: contact.email
                    };
                    if (obj != null)
                        contacts[contact.contactType] = obj;
                }
            }
            return contacts;
        }
        function loadBankDetail() {
            var loadBankPromiseDeferred = UtilsService.createPromiseDeferred();

            bankReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectifsingleitem: (!isEditMode) ? true : false
                    };
                    if (companySettingEntity != undefined && companySettingEntity.BankDetails != undefined) {
                        directivePayload.selectedIds = companySettingEntity.BankDetails;
                    }

                    VRUIUtilsService.callDirectiveLoad(bankDirectiveApi, directivePayload, loadBankPromiseDeferred);
                });
            return loadBankPromiseDeferred.promise;
        }

        function loadStaticData() {

            if (companySettingEntity == undefined) {
                $scope.scopeModel.isDefault = setDefault == true;
                return;
            }

            $scope.scopeModel.companyName = companySettingEntity.CompanyName;
            $scope.scopeModel.profileName = companySettingEntity.ProfileName;
            $scope.scopeModel.registrationNumber = companySettingEntity.RegistrationNumber;
            $scope.scopeModel.registrationAddress = companySettingEntity.RegistrationAddress;
            $scope.scopeModel.vatId = companySettingEntity.VatId;
            $scope.scopeModel.isDefault = companySettingEntity.IsDefault || setDefault;
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
                CompanySettingId: companySettingEntity != undefined ? companySettingEntity.CompanySettingId : UtilsService.guid(),
                CompanyName: $scope.scopeModel.companyName,
                ProfileName: $scope.scopeModel.profileName,
                RegistrationNumber: $scope.scopeModel.registrationNumber,
                RegistrationAddress: $scope.scopeModel.registrationAddress,
                VatId: $scope.scopeModel.vatId,
                CompanyLogo: ($scope.scopeModel.companyLogo != null) ? $scope.scopeModel.companyLogo.fileId : 0,
                IsDefault: $scope.scopeModel.isDefault,
                BillingEmails: $scope.scopeModel.toMail.join(";"),
                BankDetails: bankDirectiveApi.getSelectedIds(),
                Contacts: getContactsData()
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
