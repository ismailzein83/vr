﻿(function (appControllers) {

    "use strict";

    carrierProfileEditorController.$inject = ['$scope', 'Demo_CarrierProfileAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'Demo_ContactTypeEnum', 'VRUIUtilsService'];

    function carrierProfileEditorController($scope, Demo_CarrierProfileAPIService, UtilsService, VRNotificationService, VRNavigationService, Demo_ContactTypeEnum, VRUIUtilsService) {
        var isEditMode;
        var carrierProfileId;
        var carrierProfileEntity;
        var countryId;
        var cityId;

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var cityDirectiveApi;
        var cityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            
            $scope.scopeModal = {
                contacts: [],
                faxes: [],
                phoneNumbers : []
               
            };
           
            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            }
            $scope.scopeModal.disabledfax = true;
            $scope.scopeModal.onFaxValueChange = function (value) {
                $scope.scopeModal.disabledfax = (value== undefined);
            }
            $scope.scopeModal.disabledphone = true;
            $scope.scopeModal.onPhoneValueChange = function (value) {
                    $scope.scopeModal.disabledphone = (value == undefined);
           }
            $scope.onCityDirectiveReady = function (api) {
                cityDirectiveApi = api;
                cityReadyPromiseDeferred.resolve();
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
            $scope.onCountrySelctionChanged = function (item, datasource) {
               
                if (item != undefined) {
                    var payload = {};                   
                     payload.filter = { CountryId: item.CountryId }
                     cityDirectiveApi.load(payload)
                }
                else {                  
                    $scope.scopeModal.city = undefined;

                }
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
            return Demo_CarrierProfileAPIService.GetCarrierProfile(carrierProfileId).then(function (carrierProfile) {
                carrierProfileEntity = carrierProfile;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadCountries, loadCities, loadStaticSection, loadContacts])
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

        function loadCountries() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds: (carrierProfileEntity != undefined) ? carrierProfileEntity.Settings.CountryId : undefined
                };

                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });

            return loadCountryPromiseDeferred.promise;
        }

        function loadCities() {
            var loadCityPromiseDeferred = UtilsService.createPromiseDeferred();
            cityReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds: (carrierProfileEntity != undefined) ? carrierProfileEntity.Settings.CityId : undefined
                };
                if (carrierProfileEntity != undefined && carrierProfileEntity.Settings.CountryId != undefined)
                    payload.filter = { CountryId: carrierProfileEntity.Settings.CountryId }
                VRUIUtilsService.callDirectiveLoad(cityDirectiveApi, payload, loadCityPromiseDeferred);
            });

            return loadCityPromiseDeferred.promise;
        }

        function loadContacts() {
            for (var x in Demo_ContactTypeEnum) {
                $scope.scopeModal.contacts.push(addcontactObj(x));
            }
           
            if (carrierProfileEntity!=undefined &&  carrierProfileEntity.Settings.Contacts != null)
                for (var y = 0; y < carrierProfileEntity.Settings.Contacts.length; y++) {
                        var item = carrierProfileEntity.Settings.Contacts[y];
                        var matchedItem = UtilsService.getItemByVal($scope.scopeModal.contacts, item.Type, 'value');
                        if (matchedItem != null)
                            matchedItem.description = item.Description;
             }
        }

        function addcontactObj(x) {
            return {
                label: Demo_ContactTypeEnum[x].label,
                value: Demo_ContactTypeEnum[x].value,
                inputetype: Demo_ContactTypeEnum[x].type
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

        function buildCarrierProfileObjFromScope() {

            var obj = {
                CarrierProfileId: (carrierProfileId != null) ? carrierProfileId : 0,
                Name:  $scope.scopeModal.name,
                Settings: {
                    CountryId: countryDirectiveApi.getSelectedIds(),
                    CityId: cityDirectiveApi.getSelectedIds(),
                    Company: $scope.scopeModal.company,
                    Website: $scope.scopeModal.website,
                    RegistrationNumber: $scope.scopeModal.registrationNumber,
                    Address: $scope.scopeModal.address,
                    PostalCode: $scope.scopeModal.postalCode,
                    Town: $scope.scopeModal.town,
                    CompanyLogo:( $scope.scopeModal.companyLogo!=null)? $scope.scopeModal.companyLogo.fileId : 0,
                    PhoneNumbers: UtilsService.getPropValuesFromArray($scope.scopeModal.phoneNumbers, "phoneNumber"),
                    Faxes: UtilsService.getPropValuesFromArray($scope.scopeModal.faxes, "fax")
                }
            };
           
            obj.Settings.Contacts = [];
            for (var i = 0; i < $scope.scopeModal.contacts.length; i++) {
                var item = $scope.scopeModal.contacts[i];
                if (item.description != undefined )
                    obj.Settings.Contacts.push({ Type: item.value, Description: item.description })
            }; 
           return obj;
        }

        function insertCarrierProfile() {
            $scope.isLoading = true;

            var carrierProfileObject = buildCarrierProfileObjFromScope();
            
            return Demo_CarrierProfileAPIService.AddCarrierProfile(carrierProfileObject)
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

            var carrierProfileObject = buildCarrierProfileObjFromScope();
            
            Demo_CarrierProfileAPIService.UpdateCarrierProfile(carrierProfileObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Carrier Profile", response ,"Name")) {
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
    }

    appControllers.controller('Demo_CarrierProfileEditorController', carrierProfileEditorController);
})(appControllers);
