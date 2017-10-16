TestCallTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'Qm_CliTester_TestCallAPIService', 'VRNotificationService'];

function TestCallTemplateController($scope, UtilsService, VRUIUtilsService, Qm_CliTester_TestCallAPIService, VRNotificationService) {

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

    defineScope();
    load();

    function defineScope() {
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
        };

        $scope.onSupplierDirectiveReady = function (api) {
            supplierDirectiveAPI = api;
            supplierReadyPromiseDeferred.resolve();
        };

        $scope.onZoneDirectiveReady = function (api) {
            zoneDirectiveAPI = api;
            zoneReadyPromiseDeferred.resolve();
        };

        $scope.onCountryDirectiveReady = function (api) {
            countryDirectiveAPI = api;
            countryReadyPromiseDeferred.resolve();
        };

        $scope.onCountrySelectItem = function (selectedItem) {
            if (selectedItem != undefined) {
                var setLoader = function (value) { $scope.isLoadingZonesSelector = value; };
                var payload = {
                    filter: {
                        CountryId: selectedItem.CountryId
                    }
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneDirectiveAPI, payload, setLoader);
            }
        };

        $scope.sourceTypeTemplates = [];
        $scope.onSourceTypeDirectiveReady = function (api) {
            sourceTypeDirectiveAPI = api;
            var setLoader = function (value) { $scope.isLoadingSourceTypeDirective = value; };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceTypeDirectiveAPI, undefined, setLoader, sourceDirectiveReadyPromiseDeferred);
        };
        $scope.schedulerTaskAction.getData = function () {
            return {
                $type: "QM.CLITester.Business.TestCallTaskActionArgument, QM.CLITester.Business",
                TestCallTaskActionArgument: buildTestCallObjFromScope()
            };
        };
    }

    function load() {
        $scope.isLoading = true;

        loadAllControls();
    }
    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadCountries, loadSuppliers, loadProfiles ,loadZones])
          .catch(function (error) {
              VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
         .finally(function () {
             $scope.isLoading = false;
         });
    }


    function loadProfiles() {
        var profileLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        
        profileReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload;
                if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined && $scope.schedulerTaskAction.data.TestCallTaskActionArgument != undefined) {

                    directivePayload = {
                        selectedIds: $scope.schedulerTaskAction.data.TestCallTaskActionArgument.ProfileID
                    };
                }

                VRUIUtilsService.callDirectiveLoad(profileDirectiveAPI, directivePayload, profileLoadPromiseDeferred);
            });
        return profileLoadPromiseDeferred.promise;
    }

    function loadSuppliers() {
        var supplierLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        supplierReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload;
                if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined && $scope.schedulerTaskAction.data.TestCallTaskActionArgument != undefined
                    && $scope.schedulerTaskAction.data.TestCallTaskActionArgument.SuppliersIds != undefined) {

                    directivePayload = {
                        selectedIds: $scope.schedulerTaskAction.data.TestCallTaskActionArgument.SuppliersIds
                    };
                }


                VRUIUtilsService.callDirectiveLoad(supplierDirectiveAPI, directivePayload, supplierLoadPromiseDeferred);
            });
        return supplierLoadPromiseDeferred.promise;
    }

    function loadCountries() {
        var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        countryReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload;
                if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined && $scope.schedulerTaskAction.data.TestCallTaskActionArgument != undefined) {

                    directivePayload = {
                        selectedIds: $scope.schedulerTaskAction.data.TestCallTaskActionArgument.CountryID
                    };
                }
                VRUIUtilsService.callDirectiveLoad(countryDirectiveAPI, directivePayload, countryLoadPromiseDeferred);
            });
        return countryLoadPromiseDeferred.promise;
    }

    function loadZones() {
        var zoneLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        zoneReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload;
                if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined && $scope.schedulerTaskAction.data.TestCallTaskActionArgument != undefined) {
                    directivePayload = {
                        filter: {
                            CountryId: $scope.schedulerTaskAction.data.TestCallTaskActionArgument.CountryID
                        }
                    };
                    if ($scope.schedulerTaskAction.data.TestCallTaskActionArgument.ZoneID != undefined)
                        directivePayload.selectedIds = $scope.schedulerTaskAction.data.TestCallTaskActionArgument.ZoneID;
                }

                VRUIUtilsService.callDirectiveLoad(zoneDirectiveAPI, directivePayload, zoneLoadPromiseDeferred);
            });
        return zoneLoadPromiseDeferred.promise;
    }
   
    function buildTestCallObjFromScope() {
        var obj = {
            SupplierID: UtilsService.getPropValuesFromArray($scope.selectedSupplier, "SupplierId"),
            CountryID: $scope.selectedCountry.CountryId,
            ZoneID: $scope.selectedZone.ZoneId,
            ProfileID: $scope.selectedProfile.ProfileId
        };
        return obj;
    }
}
appControllers.controller('QM_CliTester_TestCallTemplateController', TestCallTemplateController);
