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
    var sourceConfigId;

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
        $scope.schedulerTaskAction.getData = function () {
            return {
                $type: "QM.CLITester.Business.TestCallTaskActionArgument, QM.CLITester.Business",
                TestCallQueryInput: buildTestCallObjFromScope()
            };
        };
    }

    function load() {
        $scope.isLoading = true;

        if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined) {
            $scope.SupplierID = UtilsService.getPropValuesFromArray($scope.schedulerTaskAction.data.selectedSupplier, "SupplierId");
            $scope.CountryID = $scope.schedulerTaskAction.data.CountryId;
            $scope.ZoneID = $scope.schedulerTaskAction.data.ZoneId;
            $scope.ProfileID = $scope.schedulerTaskAction.data.ProfileId;
        }

        loadAllControls();
    }
    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadCountries, loadSuppliers, loadProfiles])
          .catch(function (error) {
              VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
         .finally(function () {
             $scope.isLoading = false;
         });
    }


    function loadProfiles() {
        var profileLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        supplierReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload;

                VRUIUtilsService.callDirectiveLoad(profileDirectiveAPI, directivePayload, profileLoadPromiseDeferred);
            });
        return profileLoadPromiseDeferred.promise;
    }

    function loadSuppliers() {
        var supplierLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        supplierReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload;

                VRUIUtilsService.callDirectiveLoad(supplierDirectiveAPI, directivePayload, supplierLoadPromiseDeferred);
            });
        return supplierLoadPromiseDeferred.promise;
    }

    function loadCountries() {
        var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        countryReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload;

                VRUIUtilsService.callDirectiveLoad(countryDirectiveAPI, directivePayload, countryLoadPromiseDeferred);
            });
        return countryLoadPromiseDeferred.promise;
    }

    function buildTestCallObjFromScope() {
        var obj = {
            //TestCallId: 0,
            SupplierID: UtilsService.getPropValuesFromArray($scope.selectedSupplier, "SupplierId"),
            CountryID: $scope.selectedCountry.CountryId,
            ZoneID: $scope.selectedZone.ZoneId,
            ProfileID: $scope.selectedProfile.ProfileId
        };
        return obj;
    }


    function loadSourceType() {
        return Qm_CliTester_TestCallAPIService.GetTestCallTemplates().then(function (response) {
            console.log("1");
            //if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined)
            //    sourceConfigId = $scope.schedulerTaskAction.data.CLITestConnector.ConfigId;
            angular.forEach(response, function (item) {
                $scope.sourceTypeTemplates.push(item);
            });

            if (sourceConfigId != undefined)
                $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");
        });
    }

}
appControllers.controller('QM_CliTester_TestCallTemplateController', TestCallTemplateController);
