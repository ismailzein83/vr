'use strict';

app.directive('retailBeAccounttypePartRuntimeCompanyprofile', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeCompanyProfilePartRuntime = new AccountTypeCompanyProfilePartRuntime($scope, ctrl, $attrs);
            accountTypeCompanyProfilePartRuntime.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Runtime/Templates/AccountTypeCompanyProfilePartRuntimeTemplate.html'
    };

    function AccountTypeCompanyProfilePartRuntime($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var cityDirectiveAPI;
        var cityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var mainPayload;

        function initializeController() {
            $scope.scopeModel = {};

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

      
           
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                mainPayload = payload;
                if (payload != undefined)
                {
                    if (payload.partDefinition != undefined)
                    {
                        $scope.scopeModal.contacts = [];
                        for (var i = 0; i < payload.partDefinition.ContactTypes.length; i++) {
                            var contactType = payload.partDefinition.ContactTypes[i];
                            var contact = {
                                label: contactType.Title,
                                contactType: contactType.Name,
                                disabledphone: true,
                                phoneNumbers: [],
                                addPhoneNumberOption: function () {
                                    contact.phoneNumbers.push({
                                        phoneNumber: contact.phoneNumberValue
                                    });
                                    contact.phoneNumberValue = undefined;
                                    contact.disabledphone = true;
                                },
                                onPhoneValueChange: function (value) {
                                    contact.disabledphone = (value == undefined);
                                }
                            };
                            $scope.scopeModal.contacts.push(contact);
                        }
                    }

                    if (payload.partSettings != undefined) {
                        $scope.scopeModal.email = payload.partSettings.Email;
                        $scope.scopeModal.street = payload.partSettings.Street;
                        $scope.scopeModal.town = payload.partSettings.Town;
                        $scope.scopeModal.phoneNumbers = [];
                        for (var i = 0; i < payload.partSettings.PhoneNumbers.length; i++) {
                            $scope.scopeModal.phoneNumbers.push({
                                phoneNumber: payload.partSettings.PhoneNumbers[i]
                            });
                        }
                        $scope.scopeModal.faxes = [];
                        if (payload.partSettings.Faxes == undefined)
                            payload.partSettings.Faxes = [];
                        for (var j = 0; j < payload.partSettings.Faxes.length; j++) {
                            $scope.scopeModal.faxes.push({
                                fax: payload.partSettings.Faxes[j]
                            });
                        }
                    }
                }
              
                return loadCountryCitySection();
            };

            api.getData = function () {
                var contacts = [];
                if ($scope.scopeModal.contacts.length > 0)
                {
                    for(var i=0;i<$scope.scopeModal.contacts.length;i++)
                    {
                        var contact = $scope.scopeModal.contacts[i];
                        contacts.push({
                            ContactName: contact.name,
                            ContactType: contact.contactType,
                            Email: contact.email,
                            PhoneNumbers: UtilsService.getPropValuesFromArray(contact.phoneNumbers, "phoneNumber"),
                        });
                    }
                }

                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfile,Retail.BusinessEntity.MainExtensions',
                    CountryId: countryDirectiveApi.getSelectedIds(),
                    CityId: cityDirectiveAPI.getSelectedIds(),
                    Town: $scope.scopeModal.town,
                    Street: $scope.scopeModal.street,
                    Website: $scope.scopeModal.website,
                    POBox:$scope.scopeModal.poBox,
                    PhoneNumbers: UtilsService.getPropValuesFromArray($scope.scopeModal.phoneNumbers, "phoneNumber"),
                    Faxes: UtilsService.getPropValuesFromArray($scope.scopeModal.faxes, "fax"),
                    Contacts: contacts,
                };
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }

        function loadCountryCitySection() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();

            var promises = [];
            promises.push(loadCountryPromiseDeferred.promise);

            var payload;

            if (mainPayload != undefined && mainPayload.partSettings != undefined && mainPayload.partSettings.CountryId != undefined) {
                payload = {};
                payload.selectedIds = mainPayload.partSettings.CountryId;
                countrySelectedPromiseDeferred = UtilsService.createPromiseDeferred();
            }

            countryReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });



            if (mainPayload != undefined && mainPayload.partSettings != undefined && mainPayload.CountryId != undefined) {
                var loadCitiesPromiseDeferred = UtilsService.createPromiseDeferred();

                promises.push(loadCitiesPromiseDeferred.promise);

                UtilsService.waitMultiplePromises([cityReadyPromiseDeferred.promise, countrySelectedPromiseDeferred.promise]).then(function () {
                    var citiesPayload = {
                        countryId: mainPayload.partSettings.CountryId,
                        selectedIds: mainPayload.partSettings.CityId
                    }

                    VRUIUtilsService.callDirectiveLoad(cityDirectiveAPI, citiesPayload, loadCitiesPromiseDeferred);
                    countrySelectedPromiseDeferred = undefined;
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

    }
}]);