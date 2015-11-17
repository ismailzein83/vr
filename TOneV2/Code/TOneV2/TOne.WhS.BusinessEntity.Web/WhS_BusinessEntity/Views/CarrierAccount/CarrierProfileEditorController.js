(function (appControllers) {

    "use strict";

    carrierProfileEditorController.$inject = ['$scope', 'WhS_BE_CarrierProfileAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WhS_Be_ContactTypeEnum', 'VRUIUtilsService'];

    function carrierProfileEditorController($scope, WhS_BE_CarrierProfileAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_Be_ContactTypeEnum, VRUIUtilsService) {
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
            var indexPhone = 0;
            var indexFax = 0;

            $scope.phoneNumbers = [];
            $scope.faxes = [];
            $scope.contacts = [];


            for (var x in WhS_Be_ContactTypeEnum)
                $scope.contacts.push(WhS_Be_ContactTypeEnum[x]);

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
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

                if ($scope.addPhoneNumberOption == undefined)
                    $scope.addPhoneNumberOption = [];

                $scope.phoneNumbers.push({
                    id: indexPhone++,
                    phoneNumber: $scope.phoneNumberValue
                });
                $scope.phoneNumberValue = '';
            };


            $scope.removePhoneNumber = function ($event, option) {
                var indexPhoneInside = UtilsService.getItemIndexByVal($scope.phoneNumbers, option.id, 'id');
                $scope.phoneNumbers.splice(indexPhoneInside, 1);
            };

            $scope.onCountrySelctionChanged = function (item,datasource) {

                if (item != undefined) {
                    var payload = {};                   
                     payload.filter = { CountryId: item.CountryId }
                     cityDirectiveApi.load(payload)
                }
                else {
                    $scope.city = undefined;

                }

                  
            }


            $scope.addFaxOption = function () {

                if ($scope.faxes == undefined)
                    $scope.faxes = [];

                $scope.faxes.push({
                    id: indexFax++,
                    fax: $scope.faxValue
                });
                $scope.faxValue = '';
            };


            $scope.removeFax = function ($event, option) {
                var indexFaxInside = UtilsService.getItemIndexByVal($scope.faxes, option.id, 'id');
                $scope.faxes.splice(indexFaxInside, 1);
            };


        }


        function loadCountries() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds:(carrierProfileEntity!=undefined ) ? carrierProfileEntity.Settings.CountryId : undefined
                };

                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });

            return loadCountryPromiseDeferred.promise;
        }



        function loadCities() {
            var loadCityPromiseDeferred = UtilsService.createPromiseDeferred();
            cityReadyPromiseDeferred.promise.then(function () {             
                var payload = {
                    selectedIds:(carrierProfileEntity!=undefined ) ? carrierProfileEntity.Settings.CityId : undefined
                };
                if (carrierProfileEntity!=undefined && carrierProfileEntity.Settings.CountryId != undefined)
                    payload.filter = { CountryId: carrierProfileEntity.Settings.CountryId }
                VRUIUtilsService.callDirectiveLoad(cityDirectiveApi, payload, loadCityPromiseDeferred);
            });

            return loadCityPromiseDeferred.promise;
        }


        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getCarrierProfile().then(function () {
                    loadAllControls()
                        .finally(function () {
                            carrierProfileEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCountries, loadCities, loadStaticSection])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }




        function getCarrierProfile() {
            return WhS_BE_CarrierProfileAPIService.GetCarrierProfile(carrierProfileId).then(function (carrierProfile) {
                carrierProfileEntity = carrierProfile;
            });
        }

        function loadStaticSection() {
            if (carrierProfileEntity != undefined) {
                $scope.name = carrierProfileEntity.Name;

                if (carrierProfileEntity.Settings != null) {
                    $scope.company = carrierProfileEntity.Settings.Company;
                    $scope.website = carrierProfileEntity.Settings.Website;
                    $scope.registrationNumber = carrierProfileEntity.Settings.RegistrationNumber;
                    $scope.address = carrierProfileEntity.Settings.Address;
                    $scope.postalCode = carrierProfileEntity.Settings.PostalCode;
                    $scope.town = carrierProfileEntity.Settings.Town;

                    $scope.companyLogo = {
                        fileId: carrierProfileEntity.Settings.CompanyLogo
                    };

                    if (carrierProfileEntity.Settings.PhoneNumbers == undefined)
                        carrierProfileEntity.Settings.PhoneNumbers = [];

                    for (var i = 0; i < carrierProfileEntity.Settings.PhoneNumbers.length; i++) {
                        $scope.phoneNumbers.push({
                            id: i,
                            phoneNumber: carrierProfileEntity.Settings.PhoneNumbers[i]
                        });
                    }

                    if (carrierProfileEntity.Settings.Faxes == undefined)
                        carrierProfileEntity.Settings.Faxes = [];
                    for (var j = 0; j < carrierProfileEntity.Settings.Faxes.length; j++) {
                        $scope.faxes.push({
                            id: j,
                            fax: carrierProfileEntity.Settings.Faxes[j]
                        });
                    }

                }

            }

        }

        function insertCarrierProfile() {
            var carrierProfileObject = buildCarrierProfileObjFromScope();
            return WhS_BE_CarrierProfileAPIService.AddCarrierProfile(carrierProfileObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Carrier Profile", response)) {
                    if ($scope.onCarrierProfileAdded != undefined)
                        $scope.onCarrierProfileAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function buildCarrierProfileObjFromScope() {

            var obj = {
                CarrierProfileId: (carrierProfileId != null) ? carrierProfileId : 0,
                Name: $scope.name,
                Settings: {
                    CountryId: countryDirectiveApi.getSelectedIds(),
                    CityId: cityDirectiveApi.getSelectedIds(),
                    Company: $scope.company, Website: $scope.website,
                    RegistrationNumber: $scope.registrationNumber,
                    Address: $scope.address,
                    PostalCode: $scope.postalCode,
                    Town: $scope.town,
                    CompanyLogo: $scope.companyLogo.fileId,
                    PhoneNumbers: UtilsService.getPropValuesFromArray($scope.phoneNumbers, "phoneNumber"),
                    Faxes: UtilsService.getPropValuesFromArray($scope.faxes, "fax")
                }
            };
            return obj;
        }

        function updateCarrierProfile() {
            var carrierProfileObject = buildCarrierProfileObjFromScope();
            WhS_BE_CarrierProfileAPIService.UpdateCarrierProfile(carrierProfileObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Carrier Profile", response)) {
                    if ($scope.onCarrierProfileUpdated != undefined)
                        $scope.onCarrierProfileUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('WhS_BE_CarrierProfileEditorController', carrierProfileEditorController);
})(appControllers);
