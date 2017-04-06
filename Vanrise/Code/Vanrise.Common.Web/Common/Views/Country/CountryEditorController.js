(function (appControllers) {

    "use strict";

    countryEditorController.$inject = ['$scope', 'VRCommon_CountryAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function countryEditorController($scope, VRCommon_CountryAPIService, VRNotificationService, VRNavigationService ,UtilsService) {
        
        var countrytId;
        var editMode;
        var countryEntity;
        var context;
        var isViewHistoryMode;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                countrytId = parameters.CountryId;
                context = parameters.context;
            }
            editMode = (countrytId != undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
        }
        function defineScope() {
            $scope.saveCountry = function () {
                if (editMode)
                    return updateCountry();
                else
                    return insertCountry();
            };
            $scope.hasSaveCountryPermission = function () {
                if (editMode) {
                    return VRCommon_CountryAPIService.HasEditCountryPermission();
                }
                else {
                    return VRCommon_CountryAPIService.HasAddCountryPermission();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (editMode) {
                getCountry().then(function(){
                    loadAllControls().finally(function () {
                        countryEntity = undefined;
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.isLoading = false;
                    });
                });
            }
            else if (isViewHistoryMode) {
                getCountryHistory().then(function () {
                    loadAllControls().finally(function () {
                        countryEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });

            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor("Country");
                $scope.isLoading = false;
            }

        }
        function getCountryHistory() {
            return VRCommon_CountryAPIService.GetCountryHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                countryEntity = response;

            });
        }
        function getCountry() {
            return VRCommon_CountryAPIService.GetCountry(countrytId).then(function (country) {
                countryEntity = country;
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
            if (editMode && countryEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(countryEntity.Name, "Country");
            else if (isViewHistoryMode && countryEntity != undefined)
                $scope.title = "View Country: " + countryEntity.Name;
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Country");
        }

        function buildCountryObjFromScope() {
            var obj = {
                CountryId: (countrytId != null) ? countrytId : 0,
                Name: $scope.name,
            };
            return obj;
        }

        function loadStaticData() {

            if (countryEntity == undefined)
                return;

            $scope.name = countryEntity.Name;
        }

        function insertCountry() {
            $scope.isLoading = true;

            var countryObject = buildCountryObjFromScope();
            return VRCommon_CountryAPIService.AddCountry(countryObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Country", response ,"Name")) {
                    if ($scope.onCountryAdded != undefined)
                        $scope.onCountryAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateCountry() {
            $scope.isLoading = true;

            var countryObject = buildCountryObjFromScope();

            VRCommon_CountryAPIService.UpdateCountry(countryObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Country", response, "Name")) {
                    if ($scope.onCountryUpdated != undefined)
                        $scope.onCountryUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VRCommon_CountryEditorController', countryEditorController);
})(appControllers);
