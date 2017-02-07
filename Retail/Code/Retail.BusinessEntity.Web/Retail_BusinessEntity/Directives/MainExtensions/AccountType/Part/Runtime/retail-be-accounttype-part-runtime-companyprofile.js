'use strict';

app.directive('retailBeAccounttypePartRuntimeCompanyprofile', ["UtilsService", "VRUIUtilsService", function (UtilsService, VRUIUtilsService) {
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

        var countrySelectedPromiseDeferred;

        var mainPayload;

        function initializeController() {

            $scope.scopeModel = {
                contacts: [],
                faxes: [],
                phoneNumbers: []

            };
            $scope.scopeModel.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onCityyDirectiveReady = function (api) {
                cityDirectiveAPI = api;
                cityReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.disabledfax = true;
            $scope.scopeModel.onFaxValueChange = function (value) {
                $scope.scopeModel.disabledfax = (value == undefined);
            };
            $scope.scopeModel.disabledphone = true;
            $scope.scopeModel.onPhoneValueChange = function (value) {
                $scope.scopeModel.disabledphone = (value == undefined);
            };

            $scope.scopeModel.addPhoneNumberOption = function () {

                $scope.scopeModel.phoneNumbers.push({
                    phoneNumber: $scope.scopeModel.phoneNumberValue
                });
                $scope.scopeModel.phoneNumberValue = undefined;
                $scope.scopeModel.disabledphone = true;
            };


            $scope.scopeModel.onCountrySelectionChanged = function () {
                var selectedCountryId = countryDirectiveApi.getSelectedIds();
                if (selectedCountryId != undefined) {
                    var setLoader = function (value) { $scope.scopeModel.isLoadingCities = value };
                    var payload = {
                        countryId: selectedCountryId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, cityDirectiveAPI, payload, setLoader, countrySelectedPromiseDeferred);
                }
                else if (cityDirectiveAPI != undefined)
                    cityDirectiveAPI.clearDataSource();
            };


            $scope.scopeModel.addFaxOption = function () {
                var fax = $scope.scopeModel.faxvalue;
                $scope.scopeModel.faxes.push({
                    fax: fax
                });
                $scope.scopeModel.faxvalue = undefined;
                $scope.scopeModel.disabledfax = true;
            };

      
           
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                mainPayload = payload;
                if (payload != undefined)
                {
                    if (payload.partDefinition != undefined && payload.partDefinition.Settings !=undefined)
                    {
                        $scope.scopeModel.contacts.length = 0;
                        $scope.scopeModel.showArabicName = payload.partDefinition.Settings.IncludeArabicName == true || false;
                        for (var i = 0; i < payload.partDefinition.Settings.ContactTypes.length; i++) {
                            var contactType = payload.partDefinition.Settings.ContactTypes[i];
                            var settings;
                            if(payload.partSettings !=undefined && payload.partSettings.Contacts !=undefined)
                            {
                                settings = payload.partSettings.Contacts[contactType.Name];
                            }
                            addContact(contactType, settings);
                         
                        }
                        function addContact(contactType,settings)
                        {

                           var phoneNumbers = [];
                           if (settings != undefined && settings.PhoneNumbers !=undefined)
                            {
                               for (var i = 0; i < settings.PhoneNumbers.length; i++) {
                                   phoneNumbers.push(settings.PhoneNumbers[i]);
                                }
                            }
                            var contact = {
                                label: contactType.Title,
                                name: settings != undefined ? settings.ContactName : undefined,
                                title: settings != undefined ? settings.Title : undefined,
                                email: settings != undefined ? settings.Email : undefined,
                                contactType: contactType.Name,
                                disabledphone: true,
                                phoneNumbers: settings != undefined ? phoneNumbers : []
                                //,
                                //addPhoneNumberOption: function () {
                                //    contact.phoneNumbers.push({
                                //        phoneNumber: contact.phoneNumberValue
                                //    });
                                //    contact.phoneNumberValue = undefined;
                                //    contact.disabledphone = true;
                                //},
                                //onPhoneValueChange: function (value) {
                                //    contact.disabledphone = (value == undefined);
                                //}
                            };
                            $scope.scopeModel.contacts.push(contact);
                        }
                    }

                    if (payload.partSettings != undefined) {
                        $scope.scopeModel.email = payload.partSettings.Email;
                        $scope.scopeModel.street = payload.partSettings.Street;
                        $scope.scopeModel.town = payload.partSettings.Town;
                        $scope.scopeModel.website = payload.partSettings.Website;
                        $scope.scopeModel.poBox = payload.partSettings.POBox;
                        $scope.scopeModel.arabicName = payload.partSettings.ArabicName;
                        $scope.scopeModel.address = payload.partSettings.Address;

                        $scope.scopeModel.phoneNumbers = [];
                        if (payload.partSettings.PhoneNumbers != undefined)
                        {
                            for (var i = 0; i < payload.partSettings.PhoneNumbers.length; i++) {
                                $scope.scopeModel.phoneNumbers.push({
                                    phoneNumber: payload.partSettings.PhoneNumbers[i]
                                });
                            }
                        }
                        $scope.scopeModel.faxes = [];
                        if (payload.partSettings.Faxes !=undefined)
                        {
                            for (var j = 0; j < payload.partSettings.Faxes.length; j++) {
                                $scope.scopeModel.faxes.push({
                                    fax: payload.partSettings.Faxes[j]
                                });
                            }
                        }
                      
                    }
                }
              
                return loadCountryCitySection();
            };

            api.getData = function () {
                var contacts = {};
                if ($scope.scopeModel.contacts.length > 0)
                {
                    for(var i=0;i<$scope.scopeModel.contacts.length;i++)
                    {
                        var contact = $scope.scopeModel.contacts[i];
                        var obj =  {
                            ContactName: contact.name,
                            Title: contact.title,
                            Email: contact.email,
                            PhoneNumbers: contact.phoneNumbers
                        };
                        if (obj != null)
                            contacts[contact.contactType] = obj;
                    }
                }

                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfile,Retail.BusinessEntity.MainExtensions',
                    CountryId: countryDirectiveApi.getSelectedIds(),
                    CityId: cityDirectiveAPI.getSelectedIds(),
                    Town: $scope.scopeModel.town,
                    Street: $scope.scopeModel.street,
                    Website: $scope.scopeModel.website,
                    POBox:$scope.scopeModel.poBox,
                    PhoneNumbers: UtilsService.getPropValuesFromArray($scope.scopeModel.phoneNumbers, "phoneNumber"),
                    Faxes: UtilsService.getPropValuesFromArray($scope.scopeModel.faxes, "fax"),
                    Contacts: contacts,
                    ArabicName: $scope.scopeModel.arabicName,
                    Address: $scope.scopeModel.address
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
                    };

                    VRUIUtilsService.callDirectiveLoad(cityDirectiveAPI, citiesPayload, loadCitiesPromiseDeferred);
                    countrySelectedPromiseDeferred = undefined;
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

    }
}]);