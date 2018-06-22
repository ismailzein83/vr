(function (appControllers) {

    "use strict";

    carrierProfileEditorController.$inject = ['$scope', 'WhS_BE_CarrierProfileAPIService', 'WhS_BE_TechnicalSettingsAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WhS_BE_ContactTypeEnum', 'VRUIUtilsService'];

    function carrierProfileEditorController($scope, WhS_BE_CarrierProfileAPIService, WhS_BE_TechnicalSettingsAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_BE_ContactTypeEnum, VRUIUtilsService) {

        var isEditMode;
        var carrierProfileId;
        var carrierProfileEntity;
        var countryId;
        var cityId;

        var context;
        var isViewHistoryMode;

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var cityDirectiveAPI;
        var cityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var currencySelectorAPI;
        var currencySelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var companySettingsSelectorAPI;
        var companySettingsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var documentsGridAPI;
        var documentsGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var ticketContactsGridAPI;
        var ticketContactsGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countrySelectedPromiseDeferred;

        var defaultCustomerTimeZoneDirectiveAPI;
        var defaultCustomerTimeZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var defaultSupplierTimeZoneDirectiveAPI;
        var defaultSupplierTimeZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var bankDetailsSelectorAPI;
        var bankDetailsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                carrierProfileId = parameters.CarrierProfileId;
                context = parameters.context;
            }
            isEditMode = (carrierProfileId != undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
        }

        function defineScope() {

            $scope.hasSaveCarrierProfilePermission = function () {
                if ($scope.scopeModal.isEditMode)
                    return WhS_BE_CarrierProfileAPIService.HasUpdateCarrierProfilePermission();
                else
                    return WhS_BE_CarrierProfileAPIService.HasAddCarrierProfilePermission();
            };

            $scope.scopeModal = {
                contacts: [],
                taxes: [],
                faxes: [],
                phoneNumbers: [],
                documents: [],
                documentCategories: []
            };
            $scope.scopeModal.onDefaultCustomerTimeZoneSelectorReady = function (api) {
                defaultCustomerTimeZoneDirectiveAPI = api;
                defaultCustomerTimeZoneReadyPromiseDeferred.resolve();

            };
            $scope.scopeModal.onDefaultSupplierTimeZoneSelectorReady = function (api) {
                defaultSupplierTimeZoneDirectiveAPI = api;
                defaultSupplierTimeZoneReadyPromiseDeferred.resolve();

            };
            $scope.scopeModal.onCompanySettingSelectorReady = function (api) {
                companySettingsSelectorAPI = api;
                companySettingsSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModal.onBankDetailsSelectorReady = function (api) {
                bankDetailsSelectorAPI = api;
                bankDetailsSelectorReadyPromiseDeferred.resolve();
            };

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };

            $scope.onCityyDirectiveReady = function (api) {
                cityDirectiveAPI = api;
                cityReadyPromiseDeferred.resolve();
            };

            $scope.scopeModal.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModal.disabledfax = true;
            $scope.scopeModal.onFaxValueChange = function (value) {
                $scope.scopeModal.disabledfax = (value == undefined);
            };
            $scope.scopeModal.disabledphone = true;
            $scope.scopeModal.onPhoneValueChange = function (value) {
                $scope.scopeModal.disabledphone = (value == undefined);
            };


            $scope.SaveCarrierProfile = function () {
                if (isEditMode) {
                    return updateCarrierProfile();
                }
                else {
                    return insertCarrierProfile();
                }
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };


            $scope.addPhoneNumberOption = function () {

                $scope.scopeModal.phoneNumbers.push({
                    phoneNumber: $scope.scopeModal.phoneNumberValue
                });
                $scope.scopeModal.phoneNumberValue = undefined;
                $scope.scopeModal.disabledphone = true;
            };


            $scope.onCountrySelectionChanged = function () {
                var selectedCountryId = countryDirectiveApi.getSelectedIds();
                if (selectedCountryId != undefined) {
                    var setLoader = function (value) { $scope.isLoadingCities = value; };
                    var payload = {
                        countryId: selectedCountryId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, cityDirectiveAPI, payload, setLoader, countrySelectedPromiseDeferred);
                }
                else if (cityDirectiveAPI != undefined)
                    cityDirectiveAPI.clearDataSource();
            };


            $scope.addFaxOption = function () {
                var fax = $scope.scopeModal.faxvalue;
                $scope.scopeModal.faxes.push({
                    fax: fax
                });
                $scope.scopeModal.faxvalue = undefined;
                $scope.scopeModal.disabledfax = true;
            };

            $scope.scopeModal.addDocument = function () {
                documentsGridAPI.addDocument();
            };

            $scope.scopeModal.addTicketContact = function () {
                ticketContactsGridAPI.addTicketContact();
            };

            $scope.scopeModal.onDocumentGridReady = function (api) {
                documentsGridAPI = api;
                documentsGridReadyPromiseDeferred.resolve();
            };

            $scope.scopeModal.onTicketContactGridReady = function (api) {
                ticketContactsGridAPI = api;                
                ticketContactsGridReadyPromiseDeferred.resolve();
            };
        }        

        function load() {
            $scope.isLoading = true;
            loadAllControls()
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
        }

        function loadAllControls() {

            var initialPromises = [];
            if (isEditMode) {
                initialPromises.push(getCarrierProfile());
            }
            else if (isViewHistoryMode) {               
                initialPromises.push(getCarrierProfileHistory());
            }

            function getCarrierProfileHistory() {
                return WhS_BE_CarrierProfileAPIService.GetCarrierProfileHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                    carrierProfileEntity = response;

                });
            }

            function getCarrierProfile() {
                return WhS_BE_CarrierProfileAPIService.GetCarrierProfile(carrierProfileId).then(function (carrierProfile) {
                    carrierProfileEntity = carrierProfile;
                    cityId = carrierProfileEntity.Settings.CityId;
                });
            }

            function setTitle() {


                if (isEditMode && carrierProfileEntity != undefined)

                    $scope.title = UtilsService.buildTitleForUpdateEditor(carrierProfileEntity.Name, 'Carrier Profile', $scope);

                else if (isViewHistoryMode && carrierProfileEntity != undefined)
                    $scope.title = "View Carrier Profile: " + carrierProfileEntity.Name;
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Carrier Profile');

            }

            function loadCurrencySelector() {
                var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                currencySelectorReadyPromiseDeferred.promise.then(function () {

                    var payload = {
                        selectedIds: (carrierProfileEntity != undefined && carrierProfileEntity.Settings != undefined ? carrierProfileEntity.Settings.CurrencyId : undefined)
                    };

                    VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, payload, loadCurrencySelectorPromiseDeferred);

                });

                return loadCurrencySelectorPromiseDeferred.promise;
            }

            function loadDefaultCustomerTimeZoneSelector() {
                var loadDefaultCustomerTimeZoneSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                defaultCustomerTimeZoneReadyPromiseDeferred.promise.then(function () {

                    var payload = {
                        selectedIds: (carrierProfileEntity != undefined && carrierProfileEntity.Settings != undefined ? carrierProfileEntity.Settings.DefaultCusotmerTimeZoneId : undefined)
                    };

                    VRUIUtilsService.callDirectiveLoad(defaultCustomerTimeZoneDirectiveAPI, payload, loadDefaultCustomerTimeZoneSelectorPromiseDeferred);

                });

                return loadDefaultCustomerTimeZoneSelectorPromiseDeferred.promise;
            }

            function loadDefaultSupplierTimeZoneSelector() {
                var loadDefaultSupplierTimeZoneSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                defaultSupplierTimeZoneReadyPromiseDeferred.promise.then(function () {

                    var payload = {
                        selectedIds: (carrierProfileEntity != undefined && carrierProfileEntity.Settings != undefined ? carrierProfileEntity.Settings.DefaultSupplierTimeZoneId : undefined)
                    };

                    VRUIUtilsService.callDirectiveLoad(defaultSupplierTimeZoneDirectiveAPI, payload, loadDefaultSupplierTimeZoneSelectorPromiseDeferred);

                });

                return loadDefaultSupplierTimeZoneSelectorPromiseDeferred.promise;
            }

            function loadBankDetailsSelector() {
                var loadBankDetailsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                bankDetailsSelectorReadyPromiseDeferred.promise.then(function () {

                    var payload = {
                        selectedIds: (carrierProfileEntity != undefined && carrierProfileEntity.Settings != undefined ? carrierProfileEntity.Settings.BankDetailsIds : undefined)
                    };

                    VRUIUtilsService.callDirectiveLoad(bankDetailsSelectorAPI, payload, loadBankDetailsSelectorPromiseDeferred);

                });

                return loadBankDetailsSelectorPromiseDeferred.promise;
            }

            function loadCompanySettingSelector() {
                var loadCompanySettingSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                companySettingsSelectorReadyPromiseDeferred.promise.then(function () {

                    var payload = {
                        selectedIds: (carrierProfileEntity != undefined && carrierProfileEntity.Settings != undefined ? carrierProfileEntity.Settings.CompanySettingId : undefined)
                    };

                    VRUIUtilsService.callDirectiveLoad(companySettingsSelectorAPI, payload, loadCompanySettingSelectorPromiseDeferred);

                });

                return loadCompanySettingSelectorPromiseDeferred.promise;
            }

            function loadCountryCitySection() {
                var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();

                var promises = [];
                promises.push(loadCountryPromiseDeferred.promise);

                var payload;

                if (carrierProfileEntity != undefined && carrierProfileEntity.Settings.CountryId != undefined) {
                    payload = {};
                    payload.selectedIds = carrierProfileEntity.Settings.CountryId;
                    countrySelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                }

                countryReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
                });



                if (carrierProfileEntity != undefined && carrierProfileEntity.Settings.CountryId != undefined) {
                    var loadCitiesPromiseDeferred = UtilsService.createPromiseDeferred();

                    promises.push(loadCitiesPromiseDeferred.promise);

                    UtilsService.waitMultiplePromises([cityReadyPromiseDeferred.promise, countrySelectedPromiseDeferred.promise]).then(function () {
                        var citiesPayload = {
                            countryId: carrierProfileEntity.Settings.CountryId,
                            selectedIds: carrierProfileEntity.Settings.CityId
                        };

                        VRUIUtilsService.callDirectiveLoad(cityDirectiveAPI, citiesPayload, loadCitiesPromiseDeferred);
                        countrySelectedPromiseDeferred = undefined;
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            }

            function loadTaxes() {

                $scope.scopeModal.vat = carrierProfileEntity != undefined && carrierProfileEntity.Settings != undefined && carrierProfileEntity.Settings.TaxSetting != undefined ? carrierProfileEntity.Settings.TaxSetting.VAT : undefined;

                $scope.scopeModal.vatId = carrierProfileEntity != undefined && carrierProfileEntity.Settings != undefined && carrierProfileEntity.Settings.TaxSetting != undefined ? carrierProfileEntity.Settings.TaxSetting.VATId : undefined;

                WhS_BE_CarrierProfileAPIService.GetTaxesDefinition().then(function (response) {
                    if (response.length > 0) {
                        for (var i = 0; i < response.length; i++) {
                            var item = response[i];
                            $scope.scopeModal.taxes.push(item);

                            if (carrierProfileEntity != undefined && carrierProfileEntity.Settings != undefined && carrierProfileEntity.Settings.TaxSetting != undefined) {
                                for (var j = 0; j < carrierProfileEntity.Settings.TaxSetting.Items.length; j++) {
                                    if (carrierProfileEntity.Settings.TaxSetting.Items[j].ItemId === item.ItemId) {
                                        item.value = carrierProfileEntity.Settings.TaxSetting.Items[j].Value;
                                    }
                                }

                            }
                        }
                    }
                });

            }

            function loadDocuments() {
                var loadDocumentsPromiseDeferred = UtilsService.createPromiseDeferred();

                documentsGridReadyPromiseDeferred.promise.then(function () {
                    WhS_BE_TechnicalSettingsAPIService.GetDocumentItemDefinitionsInfo().then(function (response) {
                        var documentCategories = [];
                        if (response != undefined) {
                            for (var i = 0; i < response.length; i++)
                                documentCategories.push(response[i]);
                        }
                        var payload = {
                            documentCategories: documentCategories
                        };
                        if (carrierProfileEntity != undefined && carrierProfileEntity.Settings != undefined && carrierProfileEntity.Settings.Documents != undefined) {
                            payload.documents = carrierProfileEntity.Settings.Documents;
                        }
                        VRUIUtilsService.callDirectiveLoad(documentsGridAPI, payload, loadDocumentsPromiseDeferred);
                    });

                });
                return loadDocumentsPromiseDeferred.promise;
            }

            function loadTicketContacts() {
                var loadTicketContactsPromiseDeferred = UtilsService.createPromiseDeferred();
                ticketContactsGridReadyPromiseDeferred.promise.then(function () {
                    var payload = {};
                    if (carrierProfileEntity != undefined && carrierProfileEntity.Settings != undefined && carrierProfileEntity.Settings.TicketContacts != undefined) {
                        payload.ticketContacts = carrierProfileEntity.Settings.TicketContacts;
                    }
                    VRUIUtilsService.callDirectiveLoad(ticketContactsGridAPI, payload, loadTicketContactsPromiseDeferred);

                });
                return loadTicketContactsPromiseDeferred.promise;
            }

            function loadContacts() {
                for (var x in WhS_BE_ContactTypeEnum) {
                    $scope.scopeModal.contacts.push(addcontactObj(x));
                }

                if (carrierProfileEntity != undefined && carrierProfileEntity.Settings.Contacts != null)
                    for (var y = 0; y < carrierProfileEntity.Settings.Contacts.length; y++) {
                        var item = carrierProfileEntity.Settings.Contacts[y];
                        var matchedItem = UtilsService.getItemByVal($scope.scopeModal.contacts, item.Type, 'value');

                        if (matchedItem != null) {
                            if (matchedItem.inputetype == 'text' || matchedItem.inputetype == 'number')
                                matchedItem.description = item.Description;
                            else {
                                if (item.Description != null && item.Description != "") {
                                    matchedItem.description = item.Description;
                                    matchedItem.description = item.Description.split(";");
                                }
                            }
                        }
                    }
            }

            function addcontactObj(x) {
                return {
                    label: WhS_BE_ContactTypeEnum[x].label,
                    value: WhS_BE_ContactTypeEnum[x].value,
                    inputetype: WhS_BE_ContactTypeEnum[x].type
                };
            }

            function loadStaticSection() {
                if (carrierProfileEntity != undefined) {
                    $scope.scopeModal.name = carrierProfileEntity.Name;

                    if (carrierProfileEntity.Settings != null) {
                        $scope.scopeModal.company = carrierProfileEntity.Settings.Company;
                        $scope.scopeModal.website = carrierProfileEntity.Settings.Website;
                        $scope.scopeModal.registrationNumber = carrierProfileEntity.Settings.RegistrationNumber;
                        $scope.scopeModal.address = carrierProfileEntity.Settings.Address;
                        $scope.scopeModal.postalCode = carrierProfileEntity.Settings.PostalCode;
                        $scope.scopeModal.town = carrierProfileEntity.Settings.Town;
                        if (carrierProfileEntity.Settings.CompanyLogo != null)
                            $scope.scopeModal.companyLogo = {
                                fileId: carrierProfileEntity.Settings.CompanyLogo
                            };
                        else
                            $scope.scopeModal.companyLogo = null;
                        if (carrierProfileEntity.Settings.PhoneNumbers == undefined)
                            carrierProfileEntity.Settings.PhoneNumbers = [];
                        $scope.scopeModal.phoneNumbers = [];
                        for (var i = 0; i < carrierProfileEntity.Settings.PhoneNumbers.length; i++) {
                            $scope.scopeModal.phoneNumbers.push({
                                phoneNumber: carrierProfileEntity.Settings.PhoneNumbers[i]
                            });
                        }
                        $scope.scopeModal.faxes = [];
                        if (carrierProfileEntity.Settings.Faxes == undefined)
                            carrierProfileEntity.Settings.Faxes = [];
                        for (var j = 0; j < carrierProfileEntity.Settings.Faxes.length; j++) {
                            $scope.scopeModal.faxes.push({
                                fax: carrierProfileEntity.Settings.Faxes[j]
                            });
                        }



                    }

                }

            }

            var rootPromiseNode = {
                promises: initialPromises,
                getChildNode: function () {                    
                    return {
                        promises: [UtilsService.waitMultipleAsyncOperations([setTitle, loadCountryCitySection, loadStaticSection, loadContacts, loadCurrencySelector, loadTaxes, loadDefaultCustomerTimeZoneSelector, loadDefaultSupplierTimeZoneSelector, loadDocuments, loadCompanySettingSelector,loadBankDetailsSelector, loadTicketContacts])]
                    };
                }
            };

            return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                carrierProfileEntity = undefined;
            });
        }

        function insertCarrierProfile() {
            $scope.isLoading = true;
            return WhS_BE_CarrierProfileAPIService.AddCarrierProfile(buildCarrierProfileObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Carrier Profile", response, "Name")) {
                    if ($scope.onCarrierProfileAdded != undefined)
                        $scope.onCarrierProfileAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function updateCarrierProfile() {
            $scope.isLoading = true;
            return WhS_BE_CarrierProfileAPIService.UpdateCarrierProfile(buildCarrierProfileObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Carrier Profile", response, "Name")) {
                    if ($scope.onCarrierProfileUpdated != undefined)
                        $scope.onCarrierProfileUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function buildCarrierProfileObjFromScope() {
            var obj = {
                CarrierProfileId: (carrierProfileId != null) ? carrierProfileId : 0,
                Name: $scope.scopeModal.name,
                Settings: {
                    CountryId: countryDirectiveApi.getSelectedIds(),
                    CurrencyId: currencySelectorAPI.getSelectedIds(),
                    CityId: cityDirectiveAPI.getSelectedIds(),
                    CompanySettingId: companySettingsSelectorAPI.getSelectedIds(),
                    BankDetailsIds: bankDetailsSelectorAPI.getSelectedIds(),
                    Company: $scope.scopeModal.company,
                    Website: $scope.scopeModal.website,
                    RegistrationNumber: $scope.scopeModal.registrationNumber,
                    Address: $scope.scopeModal.address,
                    PostalCode: $scope.scopeModal.postalCode,
                    Town: $scope.scopeModal.town,
                    CompanyLogo: ($scope.scopeModal.companyLogo != null) ? $scope.scopeModal.companyLogo.fileId : null,
                    PhoneNumbers: UtilsService.getPropValuesFromArray($scope.scopeModal.phoneNumbers, "phoneNumber"),
                    Faxes: UtilsService.getPropValuesFromArray($scope.scopeModal.faxes, "fax"),
                    Documents: documentsGridAPI.getData(),
                    DefaultCusotmerTimeZoneId: defaultCustomerTimeZoneDirectiveAPI.getSelectedIds(),
                    DefaultSupplierTimeZoneId: defaultSupplierTimeZoneDirectiveAPI.getSelectedIds(),
                    TicketContacts:ticketContactsGridAPI.getData()
                }
            };

            if ($scope.scopeModal.contacts.length > 0) {
                obj.Settings.Contacts = [];
                for (var i = 0; i < $scope.scopeModal.contacts.length; i++) {
                    var item = $scope.scopeModal.contacts[i];

                    if (item.description != undefined) {
                        if (item.inputetype == 'email') {
                            var des = item.description.join(";");
                            obj.Settings.Contacts.push({ Type: item.value, Description: des });
                        }
                        else
                            obj.Settings.Contacts.push({ Type: item.value, Description: item.description });
                    }
                }
            }

            if ($scope.scopeModal.taxes.length > 0) {
                obj.Settings.TaxSetting = {};
                obj.Settings.TaxSetting.VAT = $scope.scopeModal.vat;
                obj.Settings.TaxSetting.VATId = $scope.scopeModal.vatId;
                obj.Settings.TaxSetting.Items = [];
                for (var i = 0; i < $scope.scopeModal.taxes.length; i++) {
                    var item = $scope.scopeModal.taxes[i];
                    if (item.value != undefined) {
                        obj.Settings.TaxSetting.Items.push({ ItemId: item.ItemId, Value: item.value });
                    }
                }
            }
            return obj;
        }
    }

    appControllers.controller('WhS_BE_CarrierProfileEditorController', carrierProfileEditorController);
})(appControllers);
