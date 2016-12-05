CarrierMaskEditorController.$inject = ['$scope', 'CarrierMaskAPIService', 'LookUpAPIService', 'CurrencyAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];
function CarrierMaskEditorController($scope, CarrierMaskAPIService, LookUpAPIService, CurrencyAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        $scope.MaskId = undefined;
        if (parameters != undefined && parameters != null)
            $scope.MaskId = parameters.ID;

        if ($scope.MaskId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {

        $scope.saveCarrierMask = function () {
            if (editMode) {
                return updateCarrierMask();
            }
            else {
                return insertCarrierMask();
            }
        };

        $scope.labelhint = "<b>Dates:</b> {0} = Invoice Issue Date, {1} = Invoice Begin Date, {2} = Invoice Till Date <br> <b>Overall Counters:</b> {3} = Account Invoices Count, {4} = All Invoices Count (+ Startup Counter Value)<br> <b>Yearly Counters:</b> {5} = Account's Yearly Invoice Count, {6} = Yearly Invoices Count (+ Yearly Startup Counter Value)";
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.optionsCountries = {
            selectedvalues: '',
            datasource: []
        };

        $scope.optionsCurrencies = {
            selectedvalues: '',
            datasource: []
        };
    }

    function loadCountries() {
        return LookUpAPIService.GetCountries().then(function (response) {
            $scope.optionsCountries.datasource = response;
        });
    }

    function loadCurrencies(){
        return CurrencyAPIService.GetCurrencies().then(function (response) {
            $scope.optionsCurrencies.datasource = response;
        });

    }
    function load() {






        UtilsService.waitMultipleAsyncOperations([loadCountries, loadCurrencies])
            .then(function () {
                if (editMode) {
                    $scope.isGettingData = true;
                    getCarrierMask().finally(function () {
                        $scope.isGettingData = false;
                    })
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isGettingData = false;
            });






    }

    function getCarrierMask() {
        return CarrierMaskAPIService.GetCarrierMask($scope.MaskId)
           .then(function (response) {
               fillScopeFromCarrierMaskObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error);
            });
    }

    function fillScopeFromCarrierMaskObj(CarrierMaskObject) {
        $scope.Name = CarrierMaskObject.Name;
        $scope.CompanyName = CarrierMaskObject.CompanyName;
        $scope.Country = CarrierMaskObject.Country;



        $scope.optionsCountries.selectedvalues = $scope.optionsCountries.datasource[UtilsService.getItemIndexByVal($scope.optionsCountries.datasource, CarrierMaskObject.Country, 'ID')];
        $scope.optionsCurrencies.selectedvalues = $scope.optionsCurrencies.datasource[UtilsService.getItemIndexByVal($scope.optionsCurrencies.datasource, CarrierMaskObject.CurrencyId, 'CurrencyID')];

        $scope.RegistrationNumber = CarrierMaskObject.RegistrationNumber;
        $scope.VatID = CarrierMaskObject.VatID;
        $scope.IsBankReferences = CarrierMaskObject.IsBankReferences;

        $scope.Address1 = CarrierMaskObject.Address1;
        $scope.Address2 = CarrierMaskObject.Address2;
        $scope.Address3 = CarrierMaskObject.Address3;

        $scope.Fax1 = CarrierMaskObject.Fax1;
        $scope.Fax2 = CarrierMaskObject.Fax2;
        $scope.Fax3 = CarrierMaskObject.Fax3;

        $scope.Telephone1 = CarrierMaskObject.Telephone1;
        $scope.Telephone2 = CarrierMaskObject.Telephone2;
        $scope.Telephone3 = CarrierMaskObject.Telephone3;

        $scope.BillingContact = CarrierMaskObject.BillingContact;
        $scope.BillingEmail = CarrierMaskObject.BillingEmail;
        $scope.PricingContact = CarrierMaskObject.PricingContact;
        $scope.PricingEmail = CarrierMaskObject.PricingEmail;
        $scope.AccountManagerEmail = CarrierMaskObject.AccountManagerEmail;

        $scope.SupportContact = CarrierMaskObject.SupportContact;
        $scope.SupportEmail = CarrierMaskObject.SupportEmail;

        $scope.PriceList = CarrierMaskObject.PriceList;
        $scope.MaskInvoiceformat = CarrierMaskObject.MaskInvoiceformat;
        $scope.MaskOverAllCounter = CarrierMaskObject.MaskOverAllCounter;
        $scope.YearlyMaskOverAllCounter = CarrierMaskObject.YearlyMaskOverAllCounter;
        $scope.CompanyLogo = {
            fileId: CarrierMaskObject.CompanyLogo
        }; 
    }

    function updateCarrierMask() {
        var CarrierMaskObject = buildCarrierMaskObjFromScope();
        CarrierMaskAPIService.UpdateCarrierMask(buildCarrierMaskObjFromScope())
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Carrier Mask", response)) {
                if ($scope.onCarrierMaskUpdated != undefined)
                    $scope.onCarrierMaskUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        });
    }

    function insertCarrierMask() {
        $scope.issaving = true;
        var CarrierMaskObject = buildCarrierMaskObjFromScope();

        return CarrierMaskAPIService.AddCarrierMask(CarrierMaskObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Carrier Mask", response)) {
                if ($scope.onCarrierMaskAdded != undefined)
                    $scope.onCarrierMaskAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }

    function buildCarrierMaskObjFromScope() {
        return {
            ID: $scope.MaskId,
            Name: $scope.Name,
            CompanyName: $scope.CompanyName,

            CountryId: $scope.optionsCountries.selectedvalues.LookUpID,
            CurrencyId: $scope.optionsCurrencies.selectedvalues.CurrencyID,

            RegistrationNumber: $scope.RegistrationNumber,
            VatID: $scope.VatID,
            IsBankReferences: $scope.IsBankReferences,
            Address1: $scope.Address1,
            Address2: $scope.Address2,
            Address3: $scope.Address3,
            Telephone1: $scope.Telephone1,
            Telephone2: $scope.Telephone2,
            Telephone3: $scope.Telephone3,
            Fax1: $scope.Fax1,
            Fax2: $scope.Fax2,
            Fax3: $scope.Fax3,
            BillingContact: $scope.BillingContact,
            BillingEmail: $scope.BillingEmail,
            PricingContact: $scope.PricingContact,
            PricingEmail: $scope.PricingEmail,
            AccountManagerEmail: $scope.AccountManagerEmail,
            SupportContact: $scope.SupportContact,
            SupportEmail: $scope.SupportEmail,

            PriceList: $scope.PriceList,
            MaskInvoiceformat: $scope.MaskInvoiceformat,
            MaskOverAllCounter: $scope.MaskOverAllCounter,
            YearlyMaskOverAllCounter: $scope.YearlyMaskOverAllCounter,
            CompanyLogo: $scope.CompanyLogo.fileId
        };
    }
}
appControllers.controller('Carrier_CarrierMaskEditorController', CarrierMaskEditorController);