(function (appControllers) {
    "use strict";
    RuntimeNodeManagementController.$inject = ['$scope', 'VRRuntime_RuntimeNodeService', 'VRRuntime_RuntimeNodeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function RuntimeNodeManagementController($scope, VRRuntime_RuntimeNodeService, VRRuntime_RuntimeNodeAPIService, UtilsService, VRUIUtilsService) {
       // var gridAPI;
        var filter = {};
        var nodeId;
        defineScope();
        //load();

        function defineScope() {
            $scope.datasource=[];

            $scope.searchClicked = function () {
                getFilterObject();
                //return gridAPI.loadGrid(filter);
            };

            function getFilterObject() {
                filter = {
                    Name: $scope.name
                };
            }

            VRRuntime_RuntimeNodeAPIService.GetAllNodes().then(function (response) {
                    for (var key in response) {
                        if (key != '$type') {
                            var item = response[key];
                            //item.RuntimeServiceConfigurationId = key;
                            $scope.datasource.push({ Entity: item });

                        }
                    }
                    console.log($scope.datasource);
            });



            //$scope.onGridReady = function (api) {
            //    gridAPI = api;
            //    api.loadGrid(filter);
            //};
            $scope.addNewRuntimeNode = addNewRuntimeNode;
        }


        function addNewRuntimeNode() {
            var onRuntimeNodeAdded = function (Obj) {
               // gridAPI.onRuntimeNodeAdded(Obj);
            };

            VRRuntime_RuntimeNodeService.addRuntimeNode(onRuntimeNodeAdded);
        }
    }

    appControllers.controller('VRRuntime_RuntimeNodeManagementController', RuntimeNodeManagementController);
})(appControllers);