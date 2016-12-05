/// <reference path="TestNew.html" />
appControllers.controller('TestNewController',
    function TestNewController($scope, CarrierAccountAPIService, CarrierTypeEnum) {
        $scope.CarrierAccountID = { text: "" };
        $scope.Name = { text: "" };
        $scope.ProfileName = { text: "" };
        $scope.ProfileCompanyName = { text: "" };
        $scope.CarrierTypes = CarrierTypeEnum;
        $scope.currentData;
        $scope.CarrierAccounts;
        $scope.update = function () {
            getData($scope.item);
        }
        $scope.searchCarrierAccount = function () {
            getCarrierAccounts($scope.ProfileName, $scope.ProfileCompanyName);
        }
        getData(0);
        getCarrierAccounts("", "");
        function getData(carrierTypeValue) {
            CarrierAccountAPIService.GetCarriers(carrierTypeValue).then(function (responce) {
                $scope.currentData = responce;
            })
            .finally(function () {
            });
        }
        function getCarrierAccounts(name, companyName) {
            CarrierAccountAPIService.GetCarrierAccounts(name, companyName).then(function (responce) {
                $scope.CarrierAccounts = responce;
            })
            .finally(function () {
            });
        }
        $scope.saveCarrier = function () {
            var CarrierAccountID = $scope.CarrierAccountID;
            var Name = $scope.Name;
            CarrierAccountAPIService.insertCarrierTest(CarrierAccountID, Name).then(function (responce) {
                $scope.InsertedData = responce;
            })
            .finally(function () {
            });
        }
    })