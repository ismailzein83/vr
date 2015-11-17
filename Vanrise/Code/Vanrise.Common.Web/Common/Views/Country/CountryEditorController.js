(function (appControllers) {

    "use strict";

    countryEditorController.$inject = ['$scope', 'VRCommon_CountryAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function countryEditorController($scope, VRCommon_CountryAPIService, VRNotificationService, VRNavigationService ,UtilsService) {

        
        var countrytId;
        var editMode;
        defineScope();
        loadParameters();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                countrytId = parameters.CountryId;
            }
            editMode = (countrytId != undefined);
        }
        function defineScope() {
            $scope.saveCountry = function () {
                    if (editMode) {
                        return updateCountry();
                    }
                    else {
                        return insertCountry();
                    }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }

        function load() {
                $scope.isGettingData = true;
                if (editMode) {
                  getCountry();
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor("Country");
                    $scope.isGettingData = false;
                }
          
        }
        function getCountry() {
            return VRCommon_CountryAPIService.GetCountry(countrytId).then(function (country) {
                fillScopeFromCountryObj(country);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function buildCountryObjFromScope() {
            var obj = {
                CountryId: (countrytId != null) ? countrytId : 0,
                Name: $scope.name,
            };
            return obj;
        }

        function fillScopeFromCountryObj(country) {
            $scope.name = country.Name;
            $scope.title = UtilsService.buildTitleForUpdateEditor(country.Name, "Country");
        }
        function insertCountry() {
            var countryObject = buildCountryObjFromScope();
            return VRCommon_CountryAPIService.AddCountry(countryObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Country", response ,"Name")) {
                    if ($scope.onCountryAdded != undefined)
                        $scope.onCountryAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }
        function updateCountry() {
            var countryObject = buildCountryObjFromScope();
            VRCommon_CountryAPIService.UpdateCountry(countryObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Country", response, "Name")) {
                    if ($scope.onCountryUpdated != undefined)
                        $scope.onCountryUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('VRCommon_CountryEditorController', countryEditorController);
})(appControllers);
