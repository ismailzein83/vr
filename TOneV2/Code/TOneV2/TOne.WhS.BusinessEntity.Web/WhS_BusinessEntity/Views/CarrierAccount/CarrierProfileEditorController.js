(function (appControllers) {

    "use strict";

    carrierProfileEditorController.$inject = ['$scope', 'WhS_BE_CarrierProfileAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WhS_Be_ContactTypeEnum'];

    function carrierProfileEditorController($scope, WhS_BE_CarrierProfileAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_Be_ContactTypeEnum) {
        var isEditMode;
        var carrierProfileId;
        var carrierProfileEntity;
        var countryId;
        var countryDirectiveApi;
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
            var index=0;
            $scope.phoneNumbers = [];
            $scope.faxes = ['c', 'd'];
            $scope.contacts = [];


            for (var x in WhS_Be_ContactTypeEnum)
                $scope.contacts.push(WhS_Be_ContactTypeEnum[x]);

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                load();
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
                $scope.phoneNumbers.push({
                    id: index++,
                    phoneNumber: $scope.phoneNumberValue
                });
                $scope.phoneNumberValue = '';
            };


            $scope.removePhoneNumber = function ($event, option) {
                var index = UtilsService.getItemIndexByVal($scope.phoneNumbers, option.id, 'id');
                $scope.phoneNumbers.splice(index, 1);
            };


        }


        function load() {
            $scope.isLoading = true;
            if (countryDirectiveApi == undefined)
                return;
            countryDirectiveApi.load().then(function () {
                if (countryId != undefined) {
                    countryDirectiveApi.setData(countryId);
                    $scope.isLoading = false;
                }
                else if (isEditMode) {
                    getCarrierProfile().then(function () {
                        loadFilterBySection();
                        $scope.isLoading = false;
                    });
                }
                else {
                    $scope.isLoading = false;
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            });
        }

        function getCarrierProfile() {
            return WhS_BE_CarrierProfileAPIService.GetCarrierProfile(carrierProfileId).then(function (carrierProfile) {
                carrierProfileEntity = carrierProfile;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = true;
            }).finally(function () {
                $scope.isLoading = true;
            });
        }
        function loadFilterBySection() {
            if (carrierProfileEntity != undefined) {
                $scope.name = carrierProfileEntity.Name;

                if (carrierProfileEntity.Settings != null) {
                    $scope.company = carrierProfileEntity.Settings.Company;
                    $scope.website = carrierProfileEntity.Settings.Website;
                    $scope.registrationNumber = carrierProfileEntity.Settings.RegistrationNumber;
                    $scope.faxes = carrierProfileEntity.Settings.Faxes;
                    $scope.address = carrierProfileEntity.Settings.Address;
                    $scope.postalCode = carrierProfileEntity.Settings.PostalCode;
                    $scope.town = carrierProfileEntity.Settings.Town;

                    $scope.companyLogo = {
                        fileId: carrierProfileEntity.Settings.CompanyLogo
                    };

                    countryDirectiveApi.setData(carrierProfileEntity.Settings.CountryId)


                    if (carrierProfileEntity.PhoneNumbers != undefined)
                        for (var i = 0; i < carrierProfileEntity.PhoneNumbers.length; i++) {
                            $scope.phoneNumbers.push({
                                id: i,
                                phoneNumber: carrierProfileEntity.PhoneNumbers[i]
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
                    CountryId: countryDirectiveApi.getDataId(),
                    Company: $scope.company, Website: $scope.website,
                    RegistrationNumber: $scope.registrationNumber,
                    Faxes: $scope.faxes,
                    Address: $scope.address,
                    PostalCode: $scope.postalCode,
                    Town: $scope.town,
                    CompanyLogo: $scope.companyLogo.fileId,
                    PhoneNumbers: UtilsService.getPropValuesFromArray($scope.phoneNumbers, "phoneNumber")
                }
            };
            console.log(obj)
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
