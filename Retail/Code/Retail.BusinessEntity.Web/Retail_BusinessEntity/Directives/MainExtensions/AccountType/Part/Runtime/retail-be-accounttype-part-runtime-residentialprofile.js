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
                if(payload !=undefined)
                {
                    $scope.scopeModal.email = payload.Email;
                    $scope.scopeModal.street = payload.Street;
                    $scope.scopeModal.town = payload.Town;
                    $scope.scopeModal.phoneNumbers = [];
                    for (var i = 0; i < payload.PhoneNumbers.length; i++) {
                        $scope.scopeModal.phoneNumbers.push({
                            phoneNumber: payload.PhoneNumbers[i]
                        });
                    }
                    $scope.scopeModal.faxes = [];
                    if (payload.Faxes == undefined)
                        payload.Faxes = [];
                    for (var j = 0; j < payload.Faxes.length; j++) {
                        $scope.scopeModal.faxes.push({
                            fax: payload.Faxes[j]
                        });
                    }
                }
                return loadCountryCitySection();
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartResidentialProfile,Retail.BusinessEntity.MainExtensions',
                    CountryId: countryDirectiveApi.getSelectedIds(),
                    CityId: cityDirectiveAPI.getSelectedIds(),
                    Town: $scope.scopeModal.town,
                    Street: $scope.scopeModal.street,
                    Email: $scope.scopeModal.email,
                    PhoneNumbers: UtilsService.getPropValuesFromArray($scope.scopeModal.phoneNumbers, "phoneNumber"),
                    Faxes: UtilsService.getPropValuesFromArray($scope.scopeModal.faxes, "fax")
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

            if (mainPayload != undefined && mainPayload.CountryId != undefined) {
                payload = {};
                payload.selectedIds = mainPayload.CountryId;
                countrySelectedPromiseDeferred = UtilsService.createPromiseDeferred();
            }

            countryReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });



            if (mainPayload != undefined && mainPayload.CountryId != undefined) {
                var loadCitiesPromiseDeferred = UtilsService.createPromiseDeferred();

                promises.push(loadCitiesPromiseDeferred.promise);

                UtilsService.waitMultiplePromises([cityReadyPromiseDeferred.promise, countrySelectedPromiseDeferred.promise]).then(function () {
                    var citiesPayload = {
                        countryId: mainPayload.CountryId,
                        selectedIds: mainPayload.CityId
                    }

                    VRUIUtilsService.callDirectiveLoad(cityDirectiveAPI, citiesPayload, loadCitiesPromiseDeferred);
                    countrySelectedPromiseDeferred = undefined;
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }
    }
}]);