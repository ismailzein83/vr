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
        templateUrl: "/Client/Modules/QM_CliTester/Directives/MainExtensions/ITest/Templates/TestCallTemplate.html"
    };

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

        var countrySelectedPromiseDeferred;

        function initializeController() {
            $scope.countries = [];
            $scope.zones = [];
            $scope.suppliers = [];

            $scope.selectedSupplier = [];
            $scope.selectedZone = [];
            $scope.selectedCountry = [];

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

            $scope.onCountrySelectionChanged = function () {
                var countries = countryDirectiveAPI.getSelectedIds();
              
                var setLoader = function (value) { $scope.isLoadingZoneSelector = value };
                var payload;
                if (countries != undefined && countries.length > 0)
                    payload = {
                        filter: {
                            CountryId: countries
                        }
                    }

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneDirectiveAPI, payload, setLoader, countrySelectedPromiseDeferred);

            }

            $scope.onSourceTypeDirectiveReady = function (api) {
                sourceTypeDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSourceTypeDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceTypeDirectiveAPI, undefined, setLoader, sourceDirectiveReadyPromiseDeferred);
            }


            defineAPI();
        }

        function defineAPI() {
           
            var api = {};
            api.getData = function () {
                return buildTestCallObjFromScope();               
            };

            function buildTestCallObjFromScope() {
                var obj = {
                    $type: "QM.CLITester.Business.TestCallTaskActionArgument, QM.CLITester.Business",
                    SuppliersIds: UtilsService.getPropValuesFromArray($scope.selectedSupplier, "SupplierId"),
                    SuppliersSourceIds: "",
                    CountryIds: countryDirectiveAPI.getSelectedIds(),
                    ZoneIds: UtilsService.getPropValuesFromArray($scope.selectedZone, "ZoneId"),//$scope.selectedZone.ZoneId,
                    ZoneSourceId: "",
                    ProfileID: $scope.selectedProfile.ProfileId,
                    ListEmails: $scope.toMail.join(";")
                };
                return obj;
            }

            api.load = function (payload) {
                var promises = [];
                
                var profileLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                profileReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                        if (payload != undefined && payload.data != undefined)
                            directivePayload = {
                                selectedIds: payload.data.ProfileID
                            }
                        VRUIUtilsService.callDirectiveLoad(profileDirectiveAPI, directivePayload, profileLoadPromiseDeferred);
                    });

                promises.push(profileLoadPromiseDeferred.promise);

                var supplierLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                supplierReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                        if (payload != undefined && payload.data != undefined)
                            directivePayload = {
                                selectedIds: payload.data.SuppliersIds
                            }
                        VRUIUtilsService.callDirectiveLoad(supplierDirectiveAPI, directivePayload, supplierLoadPromiseDeferred);
                    });

                promises.push(supplierLoadPromiseDeferred.promise);
                
                if (payload != undefined && payload.data != undefined && payload.data) {
                    countrySelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                }

                var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                countryReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                        if (payload != undefined && payload.data != undefined)
                            directivePayload = {
                                selectedIds: payload.data.CountryIds
                            }
                        VRUIUtilsService.callDirectiveLoad(countryDirectiveAPI, directivePayload, countryLoadPromiseDeferred);
                    });

                promises.push(countryLoadPromiseDeferred.promise);

                if(payload != undefined && payload.data != undefined && payload.data)
                {
                     var zoneLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                     UtilsService.waitMultiplePromises([zoneReadyPromiseDeferred.promise, countryLoadPromiseDeferred.promise]).then
                     (function() {
                         
                         var directivePayload;
                         directivePayload = {
                             filter: {
                                 CountryId: payload.data.CountryIds
                             }
                         }
                         if (payload.data != undefined)
                             directivePayload.selectedIds = payload.data.ZoneIds;

                         VRUIUtilsService.callDirectiveLoad(zoneDirectiveAPI, directivePayload, zoneLoadPromiseDeferred);
                         countrySelectedPromiseDeferred = undefined;
                     });
                      promises.push(zoneLoadPromiseDeferred.promise);
                }

                if (payload != undefined && payload.data != undefined && payload.data && payload.data.ListEmails != undefined) {
                    $scope.toMail = payload.data.ListEmails.split(";");
                }

                return UtilsService.waitMultiplePromises(promises);
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
