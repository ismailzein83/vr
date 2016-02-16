(function (appControllers) {

    "use strict";

    operatorProfileEditorController.$inject = ['$scope', 'Demo_OperatorProfileAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'Demo_ContactTypeEnum', 'VRUIUtilsService'];

    function operatorProfileEditorController($scope, Demo_OperatorProfileAPIService, UtilsService, VRNotificationService, VRNavigationService, Demo_ContactTypeEnum, VRUIUtilsService) {
        var isEditMode;
        var operatorProfileId;
        var operatorProfileEntity;
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
                operatorProfileId = parameters.OperatorProfileId;
            }
            isEditMode = (operatorProfileId != undefined);

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

            $scope.SaveOperatorProfile = function () {
                if (isEditMode) {
                    return updateOperatorProfile();
                }
                else {
                    return insertOperatorProfile();
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
                getOperatorProfile().then(function () {
                    loadAllControls()
                        .finally(function () {
                            operatorProfileEntity = undefined;
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

        function getOperatorProfile() {
            return Demo_OperatorProfileAPIService.GetOperatorProfile(operatorProfileId).then(function (operatorProfile) {
                operatorProfileEntity = operatorProfile;
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
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(operatorProfileEntity ? operatorProfileEntity.Name : null, 'Operator Profile') : UtilsService.buildTitleForAddEditor('Operator Profile');
        }

        function loadCountries() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds: (operatorProfileEntity != undefined) ? operatorProfileEntity.Settings.CountryId : undefined
                };

                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });

            return loadCountryPromiseDeferred.promise;
        }

        function loadCities() {
            var loadCityPromiseDeferred = UtilsService.createPromiseDeferred();
            cityReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds: (operatorProfileEntity != undefined) ? operatorProfileEntity.Settings.CityId : undefined
                };
                if (operatorProfileEntity != undefined && operatorProfileEntity.Settings.CountryId != undefined)
                    payload.filter = { CountryId: operatorProfileEntity.Settings.CountryId }
                VRUIUtilsService.callDirectiveLoad(cityDirectiveApi, payload, loadCityPromiseDeferred);
            });

            return loadCityPromiseDeferred.promise;
        }

        function loadContacts() {
            for (var x in Demo_ContactTypeEnum) {
                $scope.scopeModal.contacts.push(addcontactObj(x));
            }
           
            if (operatorProfileEntity!=undefined &&  operatorProfileEntity.Settings.Contacts != null)
                for (var y = 0; y < operatorProfileEntity.Settings.Contacts.length; y++) {
                        var item = operatorProfileEntity.Settings.Contacts[y];
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
            if (operatorProfileEntity != undefined) {
                $scope.scopeModal.name = operatorProfileEntity.Name;

                if (operatorProfileEntity.Settings != null) {
                    $scope.scopeModal.company = operatorProfileEntity.Settings.Company;
                    $scope.scopeModal.website = operatorProfileEntity.Settings.Website;
                    $scope.scopeModal.registrationNumber = operatorProfileEntity.Settings.RegistrationNumber;
                    $scope.scopeModal.address = operatorProfileEntity.Settings.Address;
                    $scope.scopeModal.postalCode = operatorProfileEntity.Settings.PostalCode;
                    $scope.scopeModal.town = operatorProfileEntity.Settings.Town;

                    if (operatorProfileEntity.Settings.CompanyLogo > 0)
                        $scope.scopeModal.companyLogo = {
                            fileId: operatorProfileEntity.Settings.CompanyLogo
                        };
                    else
                        $scope.scopeModal.companyLogo = null;
                    if (operatorProfileEntity.Settings.PhoneNumbers == undefined)
                        operatorProfileEntity.Settings.PhoneNumbers = [];
                    $scope.scopeModal.phoneNumbers = [];
                    for (var i = 0; i < operatorProfileEntity.Settings.PhoneNumbers.length; i++) {
                        $scope.scopeModal.phoneNumbers.push({
                            phoneNumber: operatorProfileEntity.Settings.PhoneNumbers[i]
                        });
                    }
                    $scope.scopeModal.faxes = [];
                    if (operatorProfileEntity.Settings.Faxes == undefined)
                        operatorProfileEntity.Settings.Faxes = [];
                    for (var j = 0; j < operatorProfileEntity.Settings.Faxes.length; j++) {
                        $scope.scopeModal.faxes.push({
                            fax: operatorProfileEntity.Settings.Faxes[j]
                        });
                    }
                    

                   
                }

            }

        }

        function buildOperatorProfileObjFromScope() {

            var obj = {
                OperatorProfileId: (operatorProfileId != null) ? operatorProfileId : 0,
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

        function insertOperatorProfile() {
            $scope.isLoading = true;

            var operatorProfileObject = buildOperatorProfileObjFromScope();
            
            return Demo_OperatorProfileAPIService.AddOperatorProfile(operatorProfileObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Operator Profile", response, "Name")) {
                    if ($scope.onOperatorProfileAdded != undefined)
                        $scope.onOperatorProfileAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateOperatorProfile() {
            $scope.isLoading = true;

            var operatorProfileObject = buildOperatorProfileObjFromScope();
            
            Demo_OperatorProfileAPIService.UpdateOperatorProfile(operatorProfileObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Operator Profile", response ,"Name")) {
                    if ($scope.onOperatorProfileUpdated != undefined)
                        $scope.onOperatorProfileUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('Demo_OperatorProfileEditorController', operatorProfileEditorController);
})(appControllers);
