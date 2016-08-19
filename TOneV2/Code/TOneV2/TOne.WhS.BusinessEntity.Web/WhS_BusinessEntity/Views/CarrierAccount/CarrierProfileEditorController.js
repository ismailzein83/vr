﻿(function (appControllers) {

    "use strict";

    carrierProfileEditorController.$inject = ['$scope', 'WhS_BE_CarrierProfileAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WhS_BE_ContactTypeEnum', 'VRUIUtilsService'];

    function carrierProfileEditorController($scope, WhS_BE_CarrierProfileAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_BE_ContactTypeEnum, VRUIUtilsService) {
        var isEditMode;
        var carrierProfileId;
        var carrierProfileEntity;
        var countryId;
        var cityId;
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var cityDirectiveAPI;
        var cityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countrySelectedPromiseDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                carrierProfileId = parameters.CarrierProfileId;

            }
            isEditMode = (carrierProfileId != undefined);

        }

        function defineScope() {

            $scope.hasSaveCarrierProfilePermission = function () {
                if ($scope.scopeModal.isEditMode)
                    return WhS_BE_CarrierProfileAPIService.HasUpdateCarrierProfilePermission();
                else
                    return WhS_BE_CarrierProfileAPIService.HasAddCarrierProfilePermission();
            }

            $scope.scopeModal = {
                contacts: [],
                faxes: [],
                phoneNumbers: []

            };

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            }

            $scope.onCityyDirectiveReady = function (api) {
                cityDirectiveAPI = api;
                cityReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.disabledfax = true;
            $scope.scopeModal.onFaxValueChange = function (value) {
                $scope.scopeModal.disabledfax = (value == undefined);
            }
            $scope.scopeModal.disabledphone = true;
            $scope.scopeModal.onPhoneValueChange = function (value) {
                $scope.scopeModal.disabledphone = (value == undefined);
            }


            $scope.SaveCarrierProfile = function () {
                if (isEditMode) {
                    return updateCarrierProfile();
                }
                else {
                    return insertCarrierProfile();
                }
            };
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


            $scope.addPhoneNumberOption = function () {

                $scope.scopeModal.phoneNumbers.push({
                    phoneNumber: $scope.scopeModal.phoneNumberValue
                });
                $scope.scopeModal.phoneNumberValue = undefined;
                $scope.scopeModal.disabledphone = true;
            };


            $scope.onCountrySelectionChanged= function () {
                var selectedCountryId = countryDirectiveApi.getSelectedIds();
                if (selectedCountryId != undefined) {
                    var setLoader = function (value) { $scope.isLoadingCities = value };
                    var payload = {
                        countryId: selectedCountryId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, cityDirectiveAPI, payload, setLoader, countrySelectedPromiseDeferred);
                }
                else if (cityDirectiveAPI != undefined)
                    cityDirectiveAPI.clearDataSource();
            }


            $scope.addFaxOption = function () {
                var fax = $scope.scopeModal.faxvalue;
                $scope.scopeModal.faxes.push({
                    fax: fax
                });
                $scope.scopeModal.faxvalue = undefined;
                $scope.scopeModal.disabledfax = true;
            };

        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getCarrierProfile().then(function () {
                    loadAllControls()
                        .finally(function () {
                            carrierProfileEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getCarrierProfile() {
            return WhS_BE_CarrierProfileAPIService.GetCarrierProfile(carrierProfileId).then(function (carrierProfile) {
                carrierProfileEntity = carrierProfile;
                cityId = carrierProfileEntity.Settings.CityId;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadCountryCitySection, loadStaticSection, loadContacts])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(carrierProfileEntity ? carrierProfileEntity.Name : null, 'Carrier Profile') : UtilsService.buildTitleForAddEditor('Carrier Profile');
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



            if (carrierProfileEntity != undefined && carrierProfileEntity.Settings.CountryId != undefined)
            {
                var loadCitiesPromiseDeferred = UtilsService.createPromiseDeferred();

                promises.push(loadCitiesPromiseDeferred.promise);
               
                UtilsService.waitMultiplePromises([cityReadyPromiseDeferred.promise, countrySelectedPromiseDeferred.promise]).then(function () {
                    var citiesPayload = {
                        countryId: carrierProfileEntity.Settings.CountryId,
                        selectedIds: carrierProfileEntity.Settings.CityId
                    }

                    VRUIUtilsService.callDirectiveLoad(cityDirectiveAPI, citiesPayload, loadCitiesPromiseDeferred);
                    countrySelectedPromiseDeferred = undefined;
                });                
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        
        function loadContacts() {
            for (var x in WhS_BE_ContactTypeEnum) {
                $scope.scopeModal.contacts.push(addcontactObj(x));
            }

            if (carrierProfileEntity != undefined && carrierProfileEntity.Settings.Contacts != null)
                for (var y = 0; y < carrierProfileEntity.Settings.Contacts.length; y++) {
                    var item = carrierProfileEntity.Settings.Contacts[y];
                    var matchedItem = UtilsService.getItemByVal($scope.scopeModal.contacts, item.Type, 'value');
                    if (matchedItem != null)
                        matchedItem.description = item.Description;
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
                    $scope.scopeModal.customerByProfile = carrierProfileEntity.Settings.CustomerByProfile;
                    if (carrierProfileEntity.Settings.CompanyLogo > 0)
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
                    CityId: cityDirectiveAPI.getSelectedIds(),
                    Company: $scope.scopeModal.company,
                    Website: $scope.scopeModal.website,
                    RegistrationNumber: $scope.scopeModal.registrationNumber,
                    Address: $scope.scopeModal.address,
                    PostalCode: $scope.scopeModal.postalCode,
                    Town: $scope.scopeModal.town,
                    CompanyLogo: ($scope.scopeModal.companyLogo != null) ? $scope.scopeModal.companyLogo.fileId : 0,
                    PhoneNumbers: UtilsService.getPropValuesFromArray($scope.scopeModal.phoneNumbers, "phoneNumber"),
                    Faxes: UtilsService.getPropValuesFromArray($scope.scopeModal.faxes, "fax"),
                    CustomerByProfile :$scope.scopeModal.customerByProfile

                }
            };

            if ($scope.scopeModal.contacts.length > 0) {
                obj.Settings.Contacts = [];
                for (var i = 0; i < $scope.scopeModal.contacts.length; i++) {
                    var item = $scope.scopeModal.contacts[i];
                    if (item.description != undefined)
                        obj.Settings.Contacts.push({ Type: item.value, Description: item.description })
                };
            }

            return obj;
        }
    }

    appControllers.controller('WhS_BE_CarrierProfileEditorController', carrierProfileEditorController);
})(appControllers);
