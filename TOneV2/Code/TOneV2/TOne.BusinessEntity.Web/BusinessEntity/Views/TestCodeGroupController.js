TestCodeGroupController.$inject = ['$scope', 'CarrierGroupAPIService', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'VRModalService'];
function TestCodeGroupController($scope, CarrierGroupAPIService,CarrierAccountAPIService,CarrierTypeEnum, VRModalService) {


    defineScope();
    load();

    function defineScope() {

        $scope.selectedvalues = [];

        $scope.datasource = [];

        $scope.openTreePopup = function () {
            var settings = {
                useModalTemplate: true,
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Carrier Group";
                modalScope.onTreeSelected = function (selectedtNode) {
                    load();
                    $scope.currentNode = undefined;
                    $scope.selectedtNode = selectedtNode;
                    console.log($scope.selectedtNode);
                };
            };

            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/TestCarrierGroupTree.html', null, settings);
            
        }
    }

    function load() {

        loadCarriers();
    }

    function loadCarriers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.SaleZone.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.datasource.push(itm);
            });
        });
    }
}

appControllers.controller('Carrier_TestCodeGroupController', TestCodeGroupController);