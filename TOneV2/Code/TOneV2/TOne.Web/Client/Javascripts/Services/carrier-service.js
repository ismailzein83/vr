'use strict'

app.service('CarriersService', function (HttpService, MainService) {

    return ({
        getCustomers: getCustomers,
        getSuppliers: getSuppliers
    });

    function getCustomers() {

        var getCarriersURL = MainService.getBaseURL() + "/api/BusinessEntity/GetCarriers";
        return HttpService.get(getCarriersURL, { carrierType: 1 });
    }
    function getSuppliers() {

        var getCarriersURL = MainService.getBaseURL() + "/api/BusinessEntity/GetCarriers";
        return HttpService.get(getCarriersURL, { carrierType: 2 });
    }
});