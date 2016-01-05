"use strict";

app.directive("qmClitesterTestcall", ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'Qm_CliTester_TestCallAPIService', function (UtilsService, VRUIUtilsService, VRNotificationService , Qm_CliTester_TestCallAPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/QM_CliTester/Directives/MainExtensions/ITest/Templates/TestCallTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;
        
        var countryDirectiveAPI;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierDirectiveAPI;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var zoneDirectiveAPI;
        var zoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var profileDirectiveAPI;
        var profileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sourceTypeDirectiveAPI;
        var sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            $scope.countries = [];
            $scope.zones = [];
            $scope.suppliers = [];

            $scope.selectedProfile;
            $scope.selectedSupplier = [];
            $scope.selectedCountry;
            $scope.selectedZone;

            $scope.onProfileDirectiveReady = function (api) {
                profileDirectiveAPI = api;
                profileReadyPromiseDeferred.resolve();
            }

            $scope.onSupplierDirectiveReady = function (api) {
                supplierDirectiveAPI = api;
                supplierReadyPromiseDeferred.resolve();
            }

            $scope.onZoneDirectiveReady = function (api) {
                zoneDirectiveAPI = api;
                zoneReadyPromiseDeferred.resolve();
            }

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveAPI = api;
                countryReadyPromiseDeferred.resolve();
            }

            $scope.onCountrySelectItem = function (selectedItem) {
                if (selectedItem != undefined) {
                    var setLoader = function (value) { $scope.isLoadingZonesSelector = value };
                    var payload = {
                        filter: {
                            CountryId: selectedItem.CountryId
                        }
                    }

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneDirectiveAPI, payload, setLoader);
                }
            }

            $scope.sourceTypeTemplates = [];
            $scope.onSourceTypeDirectiveReady = function (api) {
                sourceTypeDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSourceTypeDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceTypeDirectiveAPI, undefined, setLoader, sourceDirectiveReadyPromiseDeferred);
            }
           
            var api = {};
            api.getData = function () {
                return {
                    $type: "QM.CLITester.Business.TestCallTaskActionArgument, QM.CLITester.Business",
                    TestCallQueryInput: buildTestCallObjFromScope()
                };
                
            };
            function buildTestCallObjFromScope() {
                var obj = {
                    SupplierID: UtilsService.getPropValuesFromArray($scope.selectedSupplier, "SupplierId"),
                    CountryID: $scope.selectedCountry.CountryId,
                    ZoneID: $scope.selectedZone.ZoneId,
                    ProfileID: $scope.selectedProfile.ProfileId
                };
                return obj;
            }

            api.load = function (payload) {
                var promises = [];
                var profileLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                profileReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                        if (payload != undefined && payload.data != undefined && payload.data.TestCallQueryInput && payload.data.TestCallQueryInput.ProfileID != undefined)
                            directivePayload = {
                                selectedIds: payload.data.TestCallQueryInput.ProfileID
                            }
                        VRUIUtilsService.callDirectiveLoad(profileDirectiveAPI, directivePayload, profileLoadPromiseDeferred);
                    });
                promises.push(profileLoadPromiseDeferred.promise);

                var supplierLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                supplierReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                        if (payload != undefined && payload.data != undefined && payload.data.TestCallQueryInput && payload.data.TestCallQueryInput.SupplierID != undefined)
                            directivePayload = {
                                selectedIds: payload.data.TestCallQueryInput.SupplierID
                            }
                        VRUIUtilsService.callDirectiveLoad(supplierDirectiveAPI, directivePayload, supplierLoadPromiseDeferred);
                    });
                promises.push(supplierLoadPromiseDeferred.promise);

                var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                countryReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                        if (payload != undefined && payload.data != undefined && payload.data.TestCallQueryInput && payload.data.TestCallQueryInput.CountryID != undefined)
                            directivePayload = {
                                selectedIds: payload.data.TestCallQueryInput.CountryID
                            }
                        VRUIUtilsService.callDirectiveLoad(countryDirectiveAPI, directivePayload, countryLoadPromiseDeferred);
                    });
                promises.push(countryLoadPromiseDeferred.promise);


                var zoneLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                zoneReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                        if (payload != undefined && payload.data != undefined && payload.data.TestCallQueryInput && payload.data.TestCallQueryInput.CountryID != undefined) {
                            directivePayload = {
                                filter: {
                                    CountryId: payload.data.CountryID
                                }
                            }
                            if (payload.data.TestCallQueryInput.ZoneID != undefined)
                                directivePayload.selectedIds = payload.data.TestCallQueryInput.ZoneID;
                        }
                            

                        VRUIUtilsService.callDirectiveLoad(zoneDirectiveAPI, directivePayload, zoneLoadPromiseDeferred);
                    });
                promises.push(zoneLoadPromiseDeferred.promise);


                return UtilsService.waitMultiplePromises(promises);
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
