(function (appControllers) {

    "use strict";

    nationalNumberingPlanEditorController.$inject = ['$scope', 'Demo_NationalNumberingPlanAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'Demo_ContactTypeEnum', 'VRUIUtilsService'];

    function nationalNumberingPlanEditorController($scope, Demo_NationalNumberingPlanAPIService, UtilsService, VRNotificationService, VRNavigationService, Demo_ContactTypeEnum, VRUIUtilsService) {
        var isEditMode;
        var nationalNumberingPlanId;
        var nationalNumberingPlanEntity;
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
                nationalNumberingPlanId = parameters.NationalNumberingPlanId;
            }
            isEditMode = (nationalNumberingPlanId != undefined);

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

            $scope.SaveNationalNumberingPlan = function () {
                if (isEditMode) {
                    return updateNationalNumberingPlan();
                }
                else {
                    return insertNationalNumberingPlan();
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
                getNationalNumberingPlan().then(function () {
                    loadAllControls()
                        .finally(function () {
                            nationalNumberingPlanEntity = undefined;
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

        function getNationalNumberingPlan() {
            return Demo_NationalNumberingPlanAPIService.GetNationalNumberingPlan(nationalNumberingPlanId).then(function (nationalNumberingPlan) {
                nationalNumberingPlanEntity = nationalNumberingPlan;
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
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(nationalNumberingPlanEntity ? nationalNumberingPlanEntity.Name : null, 'National Numbering Plan') : UtilsService.buildTitleForAddEditor('National Numbering Plan');
        }

        function loadCountries() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds: (nationalNumberingPlanEntity != undefined) ? nationalNumberingPlanEntity.Settings.CountryId : undefined
                };

                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });

            return loadCountryPromiseDeferred.promise;
        }

        function loadCities() {
            var loadCityPromiseDeferred = UtilsService.createPromiseDeferred();
            cityReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds: (nationalNumberingPlanEntity != undefined) ? nationalNumberingPlanEntity.Settings.CityId : undefined
                };
                if (nationalNumberingPlanEntity != undefined && nationalNumberingPlanEntity.Settings.CountryId != undefined)
                    payload.filter = { CountryId: nationalNumberingPlanEntity.Settings.CountryId }
                VRUIUtilsService.callDirectiveLoad(cityDirectiveApi, payload, loadCityPromiseDeferred);
            });

            return loadCityPromiseDeferred.promise;
        }

        function loadContacts() {
            for (var x in Demo_ContactTypeEnum) {
                $scope.scopeModal.contacts.push(addcontactObj(x));
            }
           
            if (nationalNumberingPlanEntity!=undefined &&  nationalNumberingPlanEntity.Settings.Contacts != null)
                for (var y = 0; y < nationalNumberingPlanEntity.Settings.Contacts.length; y++) {
                        var item = nationalNumberingPlanEntity.Settings.Contacts[y];
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
            if (nationalNumberingPlanEntity != undefined) {
                $scope.scopeModal.name = nationalNumberingPlanEntity.Name;

                if (nationalNumberingPlanEntity.Settings != null) {
                    $scope.scopeModal.company = nationalNumberingPlanEntity.Settings.Company;
                    $scope.scopeModal.website = nationalNumberingPlanEntity.Settings.Website;
                    $scope.scopeModal.registrationNumber = nationalNumberingPlanEntity.Settings.RegistrationNumber;
                    $scope.scopeModal.address = nationalNumberingPlanEntity.Settings.Address;
                    $scope.scopeModal.postalCode = nationalNumberingPlanEntity.Settings.PostalCode;
                    $scope.scopeModal.town = nationalNumberingPlanEntity.Settings.Town;

                    if (nationalNumberingPlanEntity.Settings.CompanyLogo > 0)
                        $scope.scopeModal.companyLogo = {
                            fileId: nationalNumberingPlanEntity.Settings.CompanyLogo
                        };
                    else
                        $scope.scopeModal.companyLogo = null;
                    if (nationalNumberingPlanEntity.Settings.PhoneNumbers == undefined)
                        nationalNumberingPlanEntity.Settings.PhoneNumbers = [];
                    $scope.scopeModal.phoneNumbers = [];
                    for (var i = 0; i < nationalNumberingPlanEntity.Settings.PhoneNumbers.length; i++) {
                        $scope.scopeModal.phoneNumbers.push({
                            phoneNumber: nationalNumberingPlanEntity.Settings.PhoneNumbers[i]
                        });
                    }
                    $scope.scopeModal.faxes = [];
                    if (nationalNumberingPlanEntity.Settings.Faxes == undefined)
                        nationalNumberingPlanEntity.Settings.Faxes = [];
                    for (var j = 0; j < nationalNumberingPlanEntity.Settings.Faxes.length; j++) {
                        $scope.scopeModal.faxes.push({
                            fax: nationalNumberingPlanEntity.Settings.Faxes[j]
                        });
                    }
                    

                   
                }

            }

        }

        function buildNationalNumberingPlanObjFromScope() {

            var obj = {
                NationalNumberingPlanId: (nationalNumberingPlanId != null) ? nationalNumberingPlanId : 0,
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

        function insertNationalNumberingPlan() {
            $scope.isLoading = true;

            var nationalNumberingPlanObject = buildNationalNumberingPlanObjFromScope();
            
            return Demo_NationalNumberingPlanAPIService.AddNationalNumberingPlan(nationalNumberingPlanObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("National Numbering Plan", response, "Name")) {
                    if ($scope.onNationalNumberingPlanAdded != undefined)
                        $scope.onNationalNumberingPlanAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateNationalNumberingPlan() {
            $scope.isLoading = true;

            var nationalNumberingPlanObject = buildNationalNumberingPlanObjFromScope();
            
            Demo_NationalNumberingPlanAPIService.UpdateNationalNumberingPlan(nationalNumberingPlanObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("National Numbering Plan", response, "Name")) {
                    if ($scope.onNationalNumberingPlanUpdated != undefined)
                        $scope.onNationalNumberingPlanUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('Demo_NationalNumberingPlanEditorController', nationalNumberingPlanEditorController);
})(appControllers);
