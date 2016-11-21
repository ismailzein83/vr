'use strict';

app.service('OperatorsService', function (HttpService, MainService) {

    return ({
        getCustomers: getCustomers,
        getSuppliers: getSuppliers
    });
    function insertOperatorTest() {
        var OperatorAccountID = $scope.OperatorAccountID;
        var Name = $scope.Name;
        var getOperatorsURL = MainService.getBaseURL() + "/api/BusinessEntity/insertOperatorTest";
        return HttpService.post(getOperatorsURL, OperatorAccountID, Name);
    }
    function getCustomers() {

        var getOperatorsURL = MainService.getBaseURL() + "/api/BusinessEntity/GetOperators";
        return HttpService.get(getOperatorsURL, { operatorType: 1 });
    }
    function getSuppliers() {

        var getOperatorsURL = MainService.getBaseURL() + "/api/BusinessEntity/GetOperators";
        return HttpService.get(getOperatorsURL, { operatorType: 2 });
    }
});