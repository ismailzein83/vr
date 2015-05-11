/// <reference path="TestNew.html" />
appControllers.controller('TestNewController',
    function TestNewController($scope, BusinessEntityAPIService, CarrierTypeEnum) {
        $scope.CarrierAccountID = { text: "" };
        $scope.Name = { text: "" };
        $scope.CarrierTypes = CarrierTypeEnum;
        $scope.currentData;
        $scope.update = function () {
            getData($scope.item);
        }
        getData(0);
        function getData(carrierTypeValue) {
            BusinessEntityAPIService.GetCarriers(carrierTypeValue).then(function (responce) {
                $scope.currentData = responce;
            })
            .finally(function () {
                console.log("Success");
            });
        }
        $scope.saveCarrier = function () {
            var CarrierAccountID = $scope.CarrierAccountID;
            var Name = $scope.Name;
            BusinessEntityAPIService.insertCarrierTest(CarrierAccountID, Name).then(function (responce) {
                $scope.InsertedData = responce;
            })
            .finally(function () {
                console.log("Success");
            });
        }
    })