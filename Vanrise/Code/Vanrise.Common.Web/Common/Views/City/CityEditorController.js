(function (appControllers) {

    "use strict";

    cityEditorController.$inject = ['$scope', 'VRCommon_CityAPIService', 'VRNotificationService', 'VRNavigationService'];

    function cityEditorController($scope, VRCommon_CityAPIService, VRNotificationService, VRNavigationService) {

        
        var cityId;
        var editMode;
        defineScope();
        loadParameters();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                cityId = parameters.CityId;
            }
            editMode = (cityId != undefined);
        }
        function defineScope() {
            $scope.saveCity = function () {
                    if (editMode) {
                        return updateCity();
                    }
                    else {
                        return insertCity();
                    }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }

        function load() {
                $scope.isGettingData = true;
                if (editMode) {
                    getCity();
                }
                else {
                    $scope.isGettingData = false;
                }
          
        }
        function getCity() {
            return VRCommon_CityAPIService.GetCity(cityId).then(function (city) {
                fillScopeFromCityObj(city);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function buildCityObjFromScope() {
            var obj = {
                CityId: (cityId != null) ? cityId : 0,
                Name: $scope.name,
            };
            return obj;
        }

        function fillScopeFromCityObj(city) {
            $scope.name = city.Name;
        }
        function insertCity() {
            var cityObject = buildCityObjFromScope();
            return VRCommon_CityAPIService.AddCity(cityObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("City", response ,"Name")) {
                    if ($scope.onCityAdded != undefined)
                        $scope.onCityAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }
        function updateCity() {
            var cityObject = buildCityObjFromScope();
            VRCommon_CityAPIService.UpdateCity(cityObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("City", response, "Name")) {
                    if ($scope.onCityUpdated != undefined)
                        $scope.onCityUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('VRCommon_CityEditorController', cityEditorController);
})(appControllers);
