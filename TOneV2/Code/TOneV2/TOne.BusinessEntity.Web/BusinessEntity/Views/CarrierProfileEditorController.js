CarrierProfileEditorController.$inject = ['$scope', 'CarrierProfileAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];
function CarrierProfileEditorController($scope, CarrierProfileAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    var editMode;
    var dummyId = 0;//this is used to avoid duplicate in ng-repeat
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        $scope.ProfileId = undefined;
        if (parameters != undefined && parameters != null)
            $scope.ProfileId = parameters.profileID;

        if ($scope.ProfileId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {

        $scope.saveCarrierProfile = function () {
            return updateCarrierProfile();
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.addTelephone = function () {
            addTelephone("");
        };

        $scope.addFax = function () {
            addFax("");
        };
    }

    function load() {
        if (editMode) {
            $scope.isGettingData = true;
            getCarrierProfile().finally(function () {
                $scope.isGettingData = false;
            })
        }
    }

    function getCarrierProfile() {
        return CarrierProfileAPIService.GetCarrierProfile($scope.ProfileId)
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
        $scope.telephones = [];
        if (CarrierAccountObject.Telephone != undefined) {
            for (var i = 0; i < CarrierAccountObject.Telephone.length; i++) {
                var telephoneNumber = CarrierAccountObject.Telephone[i];
                if (telephoneNumber != "")
                    addTelephone(telephoneNumber);
            }
        }

        $scope.faxes = [];
        if (CarrierAccountObject.Telephone != undefined) {
            for (var i = 0; i < CarrierAccountObject.Fax.length; i++) {
                var faxNumber = CarrierAccountObject.Fax[i];
                if (faxNumber != "")
                    addFax(faxNumber);
            }
        }

        $scope.Address1 = CarrierAccountObject.Address1;
        $scope.Address2 = CarrierAccountObject.Address2;
        $scope.Address3 = CarrierAccountObject.Address3;
        $scope.Website = CarrierAccountObject.Website;
    }


    function addTelephone(number) {
        var telephone = {
            dummyId: dummyId++,
            number: number,
            remove: function () {
                var index = $scope.telephones.indexOf(telephone);
                if (index >= 0)
                    $scope.telephones.splice(index, 1);
            }
        };
        $scope.telephones.push(telephone);
    }

    function addFax(number) {
        var fax = {
            dummyId: dummyId++,
            number: number,
            remove: function () {
                var index = $scope.faxes.indexOf(fax);
                if (index >= 0)
                    $scope.faxes.splice(index, 1);
            }
        };
        $scope.faxes.push(fax);
    }

    function updateCarrierProfile() {
        var carrierProfileObject = buildCarrierProfileObjFromScope();
        CarrierProfileAPIService.UpdateCarrierProfile(buildCarrierProfileObjFromScope())
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
        var telTab = [];
        var faxTab = [];
        angular.forEach($scope.telephones, function (itm) {
            telTab.push(itm.number);
        });
        angular.forEach($scope.faxes, function (itm) {
            faxTab.push(itm.number);
        });
        return {
            ProfileId: $scope.ProfileId,
            Name: $scope.Name,
            CompanyName: $scope.CompanyName,
            Country: $scope.Country,
            City: $scope.City,
            RegestrationNumber: $scope.RegistrationNumber,
            Telephone: telTab,//$scope.telephones,
            Fax: faxTab,//$scope.faxes,
            Address1: $scope.Address1,
            Address2: $scope.Address2,
            Address3: $scope.Address3,
            Website: $scope.Website
        };

    }
}
appControllers.controller('Carrier_CarrierProfileEditorController', CarrierProfileEditorController);