(function (appControllers) {

    'use strict';

    AccountEditorController.$inject = ['$scope', 'Retail_BE_AccountAPIService', 'Retail_BE_PaymentMethodEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function AccountEditorController($scope, Retail_BE_AccountAPIService, Retail_BE_PaymentMethodEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService)
    {
        var isEditMode;

        var accountId;
        var accountEntity;
        var parentAccountId;

        var accountTypeSelectorAPI;
        var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var countrySelectorAPI;
        var countrySelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var countrySelectedDeferred;

        var citySelectorAPI;
        var citySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters()
        {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined)
            {
                accountId = parameters.accountId;
                parentAccountId = parameters.parentAccountId;
            }

            isEditMode = (accountId != undefined);
        }
        function defineScope()
        {
            $scope.scopeModel = {};
            $scope.scopeModel.contactSettings = {};
            $scope.scopeModel.billingSettings = {};

            $scope.scopeModel.paymentMethods = UtilsService.getArrayEnum(Retail_BE_PaymentMethodEnum);

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                accountTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onCountrySelectorReady = function (api) {
                countrySelectorAPI = api;
                countrySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onCountrySelectionChanged = function ()
            {
                var selectedCountryId = countrySelectorAPI.getSelectedIds();
                if (selectedCountryId != undefined) {
                    var setLoader = function (value) { $scope.scopeModel.isLoadingCities = value };
                    var citySelectorPayload = {
                        countryId: selectedCountryId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, citySelectorAPI, citySelectorPayload, setLoader, countrySelectedDeferred);
                }
                else if (citySelectorAPI != undefined) {
                    citySelectorAPI.clearDataSource();
                }
            };

            $scope.scopeModel.onCitySelectorReady = function (api) {
                citySelectorAPI = api;
                citySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.validateCardNumber = function () {
                return ($scope.scopeModel.billingSettings.cardNumber == undefined || $scope.scopeModel.billingSettings.cardNumber.length < 16) ?
                    'Card number is a 16 digit number' : null;
            };

            $scope.scopeModel.validateCardSecurityCode = function () {
                return ($scope.scopeModel.billingSettings.cardSecurityCode == undefined || $scope.scopeModel.billingSettings.cardSecurityCode.length < 3) ?
                    'Card security code is a 3 or 4 digit number' : null;
            };

            $scope.scopeModel.save = function ()
            {
                return (isEditMode) ? updateAccount() : insertAccount();
            };

            $scope.scopeModel.hasSaveAccountPermission = function ()
            {
                return (isEditMode) ? Retail_BE_AccountAPIService.HasUpdateAccountPermission() : Retail_BE_AccountAPIService.HasAddAccountPermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load()
        {
            $scope.scopeModel.isLoading = true;

            if (isEditMode)
            {
                getAccount().then(function () {
                    loadAllControls().finally(function () {
                        accountEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }
        function getAccount() {
            return Retail_BE_AccountAPIService.GetAccount(accountId).then(function (response) {
                accountEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadAccountTypeSelector, loadCountryCitySection, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                   $scope.scopeModel.isLoading = false;
              });
        }
        function setTitle()
        {
            var title;
            if (isEditMode) {
                var accountName = (accountEntity != undefined) ? accountEntity.Name : undefined;
                title = UtilsService.buildTitleForUpdateEditor(accountName, 'Account');
            }
            else {
                title = UtilsService.buildTitleForAddEditor('Account');
            }

            if (parentAccountId != undefined) {
                return Retail_BE_AccountAPIService.GetAccountName(parentAccountId).then(function (response) {
                    var titleExtension = ' for ' + response;
                    $scope.title = title += titleExtension;
                });
            }
            else {
                $scope.title = title;
            }
        }
        function loadStaticData()
        {
            if (accountEntity == undefined)
                return;

            $scope.scopeModel.name = accountEntity.Name;

            $scope.scopeModel.contactSettings.town = accountEntity.Settings.ContactSettings.Town;
            $scope.scopeModel.contactSettings.street = accountEntity.Settings.ContactSettings.Street;
            $scope.scopeModel.contactSettings.poBox = accountEntity.Settings.ContactSettings.POBox;
            $scope.scopeModel.contactSettings.email = accountEntity.Settings.ContactSettings.Email;
            $scope.scopeModel.contactSettings.phone = accountEntity.Settings.ContactSettings.Phone;
            $scope.scopeModel.contactSettings.fax = accountEntity.Settings.ContactSettings.Fax;

            $scope.scopeModel.billingSettings.contactName = accountEntity.Settings.BillingSettings.ContactName;
            $scope.scopeModel.billingSettings.contactEmail = accountEntity.Settings.BillingSettings.ContactEmail;
            $scope.scopeModel.billingSettings.contactPhone = accountEntity.Settings.BillingSettings.ContactPhone;

            if (accountEntity.Settings.BillingSettings.PaymentMethod != null)
            {
                $scope.scopeModel.billingSettings.selectedPaymentMethod =
                UtilsService.getItemByVal($scope.scopeModel.paymentMethods, accountEntity.Settings.BillingSettings.PaymentMethod, 'value');
            }
        }
        function loadAccountTypeSelector() {
            var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            accountTypeSelectorReadyDeferred.promise.then(function () {
                var accountTypeSelectorPayload = (accountEntity != undefined) ? {
                    selectedIds: accountEntity.TypeId
                } : undefined;
                VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
            });

            return accountTypeSelectorLoadDeferred.promise;
        }
        function loadCountryCitySection()
        {
            var promises = [];

            var countrySelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(countrySelectorLoadDeferred.promise);

            var countrySelectorPayload;

            if (accountEntity != undefined && accountEntity.Settings.ContactSettings != null && accountEntity.Settings.ContactSettings.CountryId != null)
            {
                countrySelectorPayload = {
                    selectedIds: accountEntity.Settings.ContactSettings.CountryId
                };
                countrySelectedDeferred = UtilsService.createPromiseDeferred();
            }

            countrySelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, countrySelectorPayload, countrySelectorLoadDeferred);
            });

            if (accountEntity != undefined && accountEntity.Settings.ContactSettings != null && accountEntity.Settings.ContactSettings.CityId != null)
            {
                var citySelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(citySelectorLoadDeferred.promise);

                UtilsService.waitMultiplePromises([citySelectorReadyDeferred.promise, countrySelectedDeferred.promise]).then(function () {
                    var citySelectorPayload = {
                        countryId: accountEntity.Settings.ContactSettings.CountryId,
                        selectedIds: accountEntity.Settings.ContactSettings.CityId
                    }

                    VRUIUtilsService.callDirectiveLoad(citySelectorAPI, citySelectorPayload, citySelectorLoadDeferred);
                    countrySelectedDeferred = undefined;
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function insertAccount()
        {
            $scope.scopeModel.isLoading = true;

            var accountObj = buildAccountObjFromScope();

            return Retail_BE_AccountAPIService.AddAccount(accountObj).then(function (response)
            {
                if (VRNotificationService.notifyOnItemAdded('Account', response, 'Name'))
                {
                    if ($scope.onAccountAdded != undefined)
                        $scope.onAccountAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateAccount()
        {
            $scope.scopeModel.isLoading = true;

            var accountObj = buildAccountObjFromScope();

            return Retail_BE_AccountAPIService.UpdateAccount(accountObj).then(function (response)
            {
                if (VRNotificationService.notifyOnItemUpdated('Account', response, 'Name'))
                {
                    if ($scope.onAccountUpdated != undefined) {
                        $scope.onAccountUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function buildAccountObjFromScope()
        {
            var obj = {
                AccountId: accountId,
                Name: $scope.scopeModel.name,
                TypeId: accountTypeSelectorAPI.getSelectedIds()
            };

            obj.Settings = {};

            obj.Settings.ContactSettings = {
                CountryId: countrySelectorAPI.getSelectedIds(),
                CityId: citySelectorAPI.getSelectedIds(),
                Town: $scope.scopeModel.contactSettings.town,
                Street: $scope.scopeModel.contactSettings.street,
                POBox: $scope.scopeModel.contactSettings.poBox,
                Email: $scope.scopeModel.contactSettings.email,
                Phone: $scope.scopeModel.contactSettings.phone,
                Fax: $scope.scopeModel.contactSettings.fax
            };

            obj.Settings.BillingSettings = {
                ContactName: $scope.scopeModel.billingSettings.contactName,
                ContactEmail: $scope.scopeModel.billingSettings.contactEmail,
                ContactPhone: $scope.scopeModel.billingSettings.contactPhone
            };

            if ($scope.scopeModel.billingSettings.selectedPaymentMethod != undefined) {
                obj.Settings.BillingSettings.PaymentMethod = $scope.scopeModel.billingSettings.selectedPaymentMethod.value;
            }

            if (!isEditMode) {
                obj.ParentAccountId = parentAccountId;
            }

            return obj;
        }
    }

    appControllers.controller('Retail_BE_AccountEditorController', AccountEditorController);

})(appControllers);