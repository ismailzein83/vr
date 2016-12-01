'use strict';

app.directive('retailBeAccounttypePartRuntimeResidentialprofile', ["UtilsService", "VRUIUtilsService", function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeResidentialProfilePartRuntime = new AccountTypeResidentialProfilePartRuntime($scope, ctrl, $attrs);
            accountTypeResidentialProfilePartRuntime.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Runtime/Templates/AccountTypeResidentialProfilePartRuntimeTemplate.html'
    };

    function AccountTypeResidentialProfilePartRuntime($scope, ctrl, $attrs) {
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
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingCities = value
                    };
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
                if (payload != undefined && payload.partSettings != undefined) {

                    $scope.scopeModel.email = payload.partSettings.Email;
                    $scope.scopeModel.street = payload.partSettings.Street;
                    $scope.scopeModel.town = payload.partSettings.Town;
                    $scope.scopeModel.phoneNumbers = [];
                    if (payload.partSettings.PhoneNumbers !=undefined)
                        {
                            for (var i = 0; i < payload.partSettings.PhoneNumbers.length; i++) {
                        $scope.scopeModel.phoneNumbers.push({
                            phoneNumber: payload.partSettings.PhoneNumbers[i]
                });
                    }
                        }
                    
                    $scope.scopeModel.faxes = [];
                    if (payload.partSettings.Faxes != undefined) {
                        for (var j = 0; j < payload.partSettings.Faxes.length; j++) {
                            $scope.scopeModel.faxes.push({
                                fax: payload.partSettings.Faxes[j]
                            });
                        }
                    }
                }
                return loadCountryCitySection();
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartResidentialProfile,Retail.BusinessEntity.MainExtensions',
                    CountryId: countryDirectiveApi.getSelectedIds(),
                    CityId: cityDirectiveAPI.getSelectedIds(),
                    Town: $scope.scopeModel.town,
                    Street: $scope.scopeModel.street,
                    Email: $scope.scopeModel.email,
                    PhoneNumbers: UtilsService.getPropValuesFromArray($scope.scopeModel.phoneNumbers, "phoneNumber"),
                    Faxes: UtilsService.getPropValuesFromArray($scope.scopeModel.faxes, "fax")
                };
            };

            if (ctrl.onReady != null)
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



            if (mainPayload != undefined && mainPayload.partSettings != undefined && mainPayload.partSettings.CountryId != undefined) {
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