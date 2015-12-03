CarrierProfileEditorController.$inject = ['$scope', 'CarrierProfileAPIService', 'LookUpAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];
function CarrierProfileEditorController($scope, CarrierProfileAPIService, LookUpAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
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

        $scope.telephones = [];

        $scope.faxes = [];

        $scope.saveCarrierProfile = function () {
            if (editMode) {
                return updateCarrierProfile();
            }
            else {
                return insertCarrierProfile();
            }
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

        $scope.optionsCountries = {
            selectedvalues: '',
            datasource: []
        };

        $scope.optionsCities = {
            selectedvalues: '',
            datasource: []
        };
    }

    function load() {
        LookUpAPIService.GetCountries().then(function (response) {
            $scope.optionsCountries.datasource = response;
        });

        LookUpAPIService.GetCities().then(function (response) {
            $scope.optionsCities.datasource = response;
        });

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

    function fillScopeFromCarrierProfileObj(CarrierProfileObject) {
        $scope.Name = CarrierProfileObject.Name;
        $scope.CompanyName = CarrierProfileObject.CompanyName;
        $scope.Country = CarrierProfileObject.Country;
        $scope.optionsCountries.selectedvalues = $scope.optionsCountries.datasource[UtilsService.getItemIndexByVal($scope.optionsCountries.datasource, CarrierProfileObject.Country, 'Description')];
        $scope.optionsCities.selectedvalues = $scope.optionsCities.datasource[UtilsService.getItemIndexByVal($scope.optionsCities.datasource, CarrierProfileObject.City, 'Description')];
        $scope.City = CarrierProfileObject.City;
        $scope.RegistrationNumber = CarrierProfileObject.RegistrationNumber;
        $scope.CompanyLogo = { fileId: CarrierProfileObject.FileID };
        if (CarrierProfileObject.Telephone != undefined) {
            for (var i = 0; i < CarrierProfileObject.Telephone.length; i++) {
                var telephoneNumber = CarrierProfileObject.Telephone[i];
                if (telephoneNumber != "")
                    addTelephone(telephoneNumber);
            }
        }

        if (CarrierProfileObject.Telephone != undefined) {
            for (var i = 0; i < CarrierProfileObject.Fax.length; i++) {
                var faxNumber = CarrierProfileObject.Fax[i];
                if (faxNumber != "")
                    addFax(faxNumber);
            }
        }

        $scope.Address1 = CarrierProfileObject.Address1;
        $scope.Address2 = CarrierProfileObject.Address2;
        $scope.Address3 = CarrierProfileObject.Address3;
        $scope.Website = CarrierProfileObject.Website;
        $scope.BillingContact = CarrierProfileObject.BillingContact;
        $scope.BillingEmail = CarrierProfileObject.BillingEmail;
        $scope.DisputeEmail = CarrierProfileObject.DisputeEmail;
        $scope.PricingContact = CarrierProfileObject.PricingContact;
        $scope.PricingEmail = CarrierProfileObject.PricingEmail;
        $scope.AccountManagerContact = CarrierProfileObject.AccountManagerContact;
        $scope.AccountManagerEmail = CarrierProfileObject.AccountManagerEmail;
        $scope.SupportContact = CarrierProfileObject.SupportContact;
        $scope.SupportEmail = CarrierProfileObject.SupportEmail;
        $scope.TechnicalContact = CarrierProfileObject.TechnicalContact;
        $scope.TechnicalEmail = CarrierProfileObject.TechnicalEmail;
        $scope.CommercialContact = CarrierProfileObject.CommercialContact;
        $scope.CommercialEmail = CarrierProfileObject.CommercialEmail;
        $scope.SMSPhoneNumber = CarrierProfileObject.SMSPhoneNumber;
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
        CarrierProfileAPIService.UpdateCarrierProfile(carrierProfileObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Carrier Profile", response)) {
                if ($scope.onCarrierProfileUpdated != undefined)
                    $scope.onCarrierProfileUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        });
    }

    function insertCarrierProfile() {
        $scope.issaving = true;
        var carrierProfileObject = buildCarrierProfileObjFromScope();

        return CarrierProfileAPIService.AddCarrierProfile(carrierProfileObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Carrier Profile", response)) {
                if ($scope.onCarrierProfileAdded != undefined)
                    $scope.onCarrierProfileAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }

    function buildCarrierProfileObjFromScope() {
        var telTab = [];
        var faxTab = [];
        //var selectedProfileId;
        //angular.forEach($scope.optionsUsers.selectedvalues, function (user) {
        //    selectedUserIds.push(user.UserId);
        //});
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
            Country: $scope.optionsCountries.selectedvalues.Description,
            City: $scope.optionsCities.selectedvalues.Description,
            RegistrationNumber: $scope.RegistrationNumber,
            Telephone: telTab,//$scope.telephones,
            Fax: faxTab,//$scope.faxes,
            Address1: $scope.Address1,
            Address2: $scope.Address2,
            Address3: $scope.Address3,
            Website: $scope.Website,
            BillingContact: $scope.BillingContact,
            BillingEmail: $scope.BillingEmail,
            BillingDisputeEmail: $scope.BillingDisputeEmail,
            PricingContact: $scope.PricingContact,
            PricingEmail: $scope.PricingEmail,
            AccountManagerContact: $scope.AccountManagerContact,
            AccountManagerEmail: $scope.AccountManagerEmail,
            SupportContact: $scope.SupportContact,
            SupportEmail: $scope.SupportEmail,
            TechnicalContact: $scope.TechnicalContact,
            TechnicalEmail: $scope.TechnicalEmail,
            CommercialContact: $scope.CommercialContact,
            CommercialEmail: $scope.CommercialEmail,
            SMSPhoneNumber: $scope.SMSPhoneNumber,
            FileID: $scope.CompanyLogo != null ? $scope.CompanyLogo.fileId : null
        };

    }
}
appControllers.controller('Carrier_CarrierProfileEditorController', CarrierProfileEditorController);