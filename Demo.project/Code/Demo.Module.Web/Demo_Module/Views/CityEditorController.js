(function (appControllers) {

    "use strict";

    cityEditorController.$inject = ['$scope', 'Demo_Module_CityAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function cityEditorController($scope, Demo_Module_CityAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        var Id;
       
        var editMode;
        var cityEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                Id = parameters.Id;

            }
            editMode = (Id != undefined);
        }



        function defineScope() {


            $scope.saveCity = function () {
                if (editMode)
                    return updateCity();
                else
                    return insertCity();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }


        function load() {

            $scope.isLoading = true;

            if (editMode) {
                getCity().then(function () {
                    loadAllControls()
                        .finally(function () {
                            cityEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getCity() {
            return Demo_Module_CityAPIService.GetCity(Id).then(function (city) {
                cityEntity = city;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (editMode && cityEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(cityEntity.Name, "City");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("City");
        }

        function loadStaticData() {

            if (cityEntity == undefined)
                return;

            $scope.name = cityEntity.Name;
        }

        

        function buildCityObjFromScope() {
            var obj = {
                Id: (Id != null) ? Id : 0,
                Name: $scope.name,
               
            };
            return obj;
        }


        function insertCity() {
            $scope.isLoading = true;

            var cityObject = buildCityObjFromScope();
            return Demo_Module_CityAPIService.AddCity(cityObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("City", response, "Name")) {
                    if ($scope.onCityAdded != undefined) {
                       
                        $scope.onCityAdded(response.InsertedObject);
                    } 
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }
        function updateCity() {
            $scope.isLoading = true;

            var cityObject = buildCityObjFromScope();
            Demo_Module_CityAPIService.UpdateCity(cityObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("City", response, "Name")) {
                    if ($scope.onCityUpdated != undefined)
                        $scope.onCityUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('Demo_Module_CityEditorController', cityEditorController);
})(appControllers);
