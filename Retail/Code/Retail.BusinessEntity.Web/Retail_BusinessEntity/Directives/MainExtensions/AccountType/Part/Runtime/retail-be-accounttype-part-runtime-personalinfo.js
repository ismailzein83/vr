'use strict';

app.directive('retailBeAccounttypePartRuntimePersonalinfo', ["UtilsService", "VRUIUtilsService", function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypePersonalInfoPartRuntime = new AccountTypePersonalInfoPartRuntime($scope, ctrl, $attrs);
            accountTypePersonalInfoPartRuntime.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Runtime/Templates/AccountTypePersonalInfoPartRuntimeTemplate.html'
    };

    function AccountTypePersonalInfoPartRuntime($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var genderApi;
        var genderSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var cityDirectiveAPI;
        var cityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countrySelectedPromiseDeferred;
        var mainPayload;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGenderSelectorReady = function (api) {
                genderApi =  api;
                genderSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onCityDirectiveReady = function (api) {
                cityDirectiveAPI = api;
                cityReadyPromiseDeferred.resolve();
            };



            $scope.scopeModel.onCountrySelectionChanged = function () {
                var selectedCountryId = countryDirectiveApi.getSelectedIds();
                if (selectedCountryId != undefined) {
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingCities = value;
                    };
                    var payload = {
                        countryId: selectedCountryId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, cityDirectiveAPI, payload, setLoader, countrySelectedPromiseDeferred);
                }
                else if (cityDirectiveAPI != undefined)
                    cityDirectiveAPI.clearDataSource();
            };


            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                mainPayload = payload;
                var promises = [];

                if (payload != undefined && payload.partSettings != undefined) {

                    $scope.scopeModel.firstName = payload.partSettings.FirstName;
                    $scope.scopeModel.lastName = payload.partSettings.LastName;
                    $scope.scopeModel.dob = payload.partSettings.BirthDate;
                }
                promises.push(loadCountryCitySection());
                promises.push(loadGenderSelector());

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartPersonalInfo,Retail.BusinessEntity.MainExtensions',
                    CountryId: countryDirectiveApi.getSelectedIds(),
                    CityId: cityDirectiveAPI.getSelectedIds(),
                    FirstName: $scope.scopeModel.firstName,
                    LastName: $scope.scopeModel.lastName,
                    BirthDate: $scope.scopeModel.dob,
                    Gender: genderApi.getSelectedIds()
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

        function loadGenderSelector() {
            var genderSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            genderSelectorReadyPromiseDeferred.promise.then(function () {
                var selectorPayload = {
                    selectedIds: mainPayload != undefined && mainPayload.partSettings != undefined ? mainPayload.partSettings.Gender : undefined
                };
                VRUIUtilsService.callDirectiveLoad(genderApi, selectorPayload, genderSelectorLoadDeferred);
            });
            return genderSelectorLoadDeferred.promise;
        }
    }
}]);