(function (appControllers) {
    "use strict";
    RuntimeNodeManagementController.$inject = ['$scope', 'VRRuntime_RuntimeNodeService', 'VRRuntime_RuntimeNodeAPIService', 'VRRuntime_RuntimeNodeStateAPIService', 'UtilsService', 'VRUIUtilsService'];

    function RuntimeNodeManagementController($scope, VRRuntime_RuntimeNodeService, VRRuntime_RuntimeNodeAPIService, VRRuntime_RuntimeNodeStateAPIService, UtilsService, VRUIUtilsService) {
        var gridAPI;
        var filter = {};
        var nodeId;
        defineScope();
        var runtimeNodeId;

        function defineScope() {
            $scope.datasource=[];

            $scope.searchClicked = function () {
                getFilterObject();
            };

            function getFilterObject() {
                filter = {
                    Name: $scope.name
                };
            }

            VRRuntime_RuntimeNodeAPIService.GetAllNodes().then(function (response) {
                if (response != null) {
                    for (var key in response) {
                        if (key != '$type') {
                            var item = response[key];
                            $scope.datasource.push({ Entity: item });
                        }
                    }
                }
            });

            VRRuntime_RuntimeNodeStateAPIService.GetAllNodesStates().then(function (response) {
                console.log(response);
            });


            $scope.addNewRuntimeNode = addNewRuntimeNode;
            $scope.editRuntimeNode = editRuntimeNode;
        }


        function addNewRuntimeNode() {
            var onRuntimeNodeAdded = function (Obj) {
            };

            VRRuntime_RuntimeNodeService.addRuntimeNode(onRuntimeNodeAdded);
        }


        function editRuntimeNode(nodeObj) {
            var onRuntimeNodeUpdated = function (node) {
                var index = $scope.datasource.indexOf(nodeObj);
                $scope.datasource[index] = { Entity: node };
            };

            VRRuntime_RuntimeNodeService.editRuntimeNode(nodeObj.Entity.RuntimeNodeId, onRuntimeNodeUpdated);
        }
    }

    appControllers.controller('VRRuntime_RuntimeNodeManagementController', RuntimeNodeManagementController);
})(appControllers);