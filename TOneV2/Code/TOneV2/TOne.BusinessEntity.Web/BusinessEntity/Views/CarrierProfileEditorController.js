CarrierProfileEditorController.$inject = ['$scope', 'CarrierAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];
function CarrierProfileEditorController($scope, CarrierAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        $scope.ProfileId = undefined;
        if (parameters != undefined && parameters != null)
            $scope.ProfileId = parameters.profileID;
        console.log(parameters);
        editMode = true;
    }

    function defineScope() {

        $scope.saveCarrierProfile = function () {
            return updateCarrierProfile();
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }

    function load() {
        $scope.isGettingData = true;
        getCarrierProfile().finally(function () {
            $scope.isGettingData = false;
        })
    }

    function getCarrierProfile() {
        return CarrierAPIService.GetCarrierProfile($scope.ProfileId)
           .then(function (response) {
               fillScopeFromCarrierProfileObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error);
            });
    }

    function fillScopeFromCarrierProfileObj(CarrierAccountObject) {
        $scope.Name = CarrierAccountObject.Name;
        $scope.CompanyName = CarrierAccountObject.CompanyName;
        $scope.Country = CarrierAccountObject.Country;
        $scope.City = CarrierAccountObject.City;
        $scope.RegistrationNumber = CarrierAccountObject.RegistrationNumber;
        $scope.Telephone1 = CarrierAccountObject.Telephone[0];
        $scope.Telephone2 = CarrierAccountObject.Telephone[1];
        $scope.Telephone3 = CarrierAccountObject.Telephone[2];
        $scope.Fax1 = CarrierAccountObject.Fax[0];
        $scope.Fax2 = CarrierAccountObject.Fax[1];
        $scope.Fax3 = CarrierAccountObject.Fax[2];
        $scope.Address1 = CarrierAccountObject.Address1;
        $scope.Address2 = CarrierAccountObject.Address2;
        $scope.Address3 = CarrierAccountObject.Address3;
        $scope.Website = CarrierAccountObject.Website;
    }

    function updateCarrierProfile() {
        var carrierProfileObject = buildCarrierProfileObjFromScope();
        CarrierAPIService.UpdateCarrierProfile(buildCarrierProfileObjFromScope())
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("CarrierProfile", response)) {
                if ($scope.onCarrierProfileUpdated != undefined)
                    $scope.onCarrierProfileUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        });
    }

    function buildCarrierProfileObjFromScope() {
        return {
            Name: $scope.Name,
            CompanyName: $scope.CompanyName,
            Country: $scope.Country,
            City: $scope.City,
            RegestrationNumber: $scope.RegistrationNumber,
            Telephone1: $scope.Telephone[0],
            Telephone2: $scope.Telephone[1],
            Telephone3: $scope.Telephone[2],
            Fax1: $scope.Fax[0],
            Fax2: $scope.Fax[1],
            Fax3: $scope.Fax[2],
            Address1: $scope.Address1,
            Address2: $scope.Address2,
            Address3: $scope.Address3,
            Website: $scope.Website
        };

    }
}
appControllers.controller('Carrier_CarrierProfileEditorController', CarrierProfileEditorController);