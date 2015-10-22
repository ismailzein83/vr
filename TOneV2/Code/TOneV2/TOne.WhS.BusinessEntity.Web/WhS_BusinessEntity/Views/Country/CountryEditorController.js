(function (appControllers) {

    "use strict";

    countryEditorController.$inject = ['$scope', 'WhS_BE_CountryAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function countryEditorController($scope, WhS_BE_CountryAPIService, UtilsService, VRNotificationService, VRNavigationService) {

        
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
            $scope.SaveCountry = function () {
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
                    $scope.isGettingData = false;
                }
           // $scope.isGettingData = true;
            //if (carrierProfileDirectiveAPI == undefined)
            //    return;

            //defineCarrierAccountTypes();
           
            //carrierProfileDirectiveAPI.load().then(function () {
            //    if ($scope.disableCarrierProfile && carrierProfileId != undefined)
            //    {
            //        carrierProfileDirectiveAPI.setData(carrierProfileId);
            //        $scope.isGettingData = false;
            //    }   
            //    else if (editMode) {
            //      getCarrierAccount();
            //    }
            //    else {
            //        $scope.isGettingData = false;
            //    }
            //}).catch(function (error) {
            //    VRNotificationService.notifyExceptionWithClose(error, $scope);
            //    $scope.isGettingData = false;
            //});

        }
        function getCountry() {
            return WhS_BE_CountryAPIService.GetCountry(countrytId).then(function (country) {
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
        }
        //function insertCarrierAccount() {
        //    var carrierAccountObject = buildCarrierAccountObjFromScope();
        //    return WhS_BE_CarrierAccountAPIService.AddCarrierAccount(carrierAccountObject)
        //    .then(function (response) {
        //        if (VRNotificationService.notifyOnItemAdded("Carrier Account", response)) {
        //            if ($scope.onCarrierAccountAdded != undefined)
        //                $scope.onCarrierAccountAdded(response.InsertedObject);
        //            $scope.modalContext.closeModal();
        //        }
        //    }).catch(function (error) {
        //        VRNotificationService.notifyException(error, $scope);
        //    });

        //}
        function updateCountry() {
            var countryObject = buildCountryObjFromScope();
            WhS_BE_CountryAPIService.UpdateCountry(countryObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Country", response)) {
                    if ($scope.onCountryUpdated != undefined)
                        $scope.onCountryUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('WhS_BE_CountryEditorController', countryEditorController);
})(appControllers);
