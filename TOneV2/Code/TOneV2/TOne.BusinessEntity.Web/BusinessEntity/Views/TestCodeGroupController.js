(function (appControllers) {

    "use strict";
    
    testCodeGroupController.$inject = ['$scope', 'CarrierGroupAPIService', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'VRModalService', 'VRNotificationService'];
    function testCodeGroupController($scope, CarrierGroupAPIService, CarrierAccountAPIService, CarrierTypeEnum, VRModalService, VRNotificationService) {

        defineScope();
        load();

        function defineScope() {
            $scope.selectedvaluesneeded=[];
            $scope.selectionchanged = function () {
            }
            $scope.selectedvalues = [];

            $scope.datasource = [];

            $scope.openTreePopup = function () {
                var settings = {
                    useModalTemplate: true,
                };
                settings.onScopeReady = function (modalScope) {
                    modalScope.title = "Carrier Group";
                    modalScope.onTreeSelected = function (selectedtNode) {
                        $scope.currentNode = undefined;
                        $scope.selectedtNode = selectedtNode;
                        //Load Selected
                        CarrierGroupAPIService.GetCarrierGroupMembersDesc($scope.selectedtNode.EntityId).then(function (response) {
                            $scope.selectedvalues = [];
                            angular.forEach(response, function (item) {
                                $scope.selectedvalues.push(item);
                            });
                        }).catch(function (error) {
                            $scope.isGettingData = false;
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        });

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

    appControllers.controller('Carrier_TestCodeGroupController', testCodeGroupController);


})(appControllers);