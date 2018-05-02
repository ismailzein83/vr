(function (appControllers) {
    "use strict";
    RuntimeNodeManagementController.$inject = ['$scope','VRTimerService', 'VRRuntime_RuntimeNodeService', 'VRRuntime_RuntimeNodeAPIService', 'VRRuntime_RuntimeNodeStateAPIService', 'UtilsService', 'VRUIUtilsService'];

    function RuntimeNodeManagementController($scope,VRTimerService, VRRuntime_RuntimeNodeService, VRRuntime_RuntimeNodeAPIService, VRRuntime_RuntimeNodeStateAPIService, UtilsService, VRUIUtilsService) {
        var gridAPI;
        var filter = {};
        var nodeId;
        var runtimeNodeId;
        var job;
        var timeInterval = 0.1;


        defineScope();

        function defineScope() {
            $scope.datasource = [];
            $scope.scopeModel = {};
            filter = {};

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    api.loadGrid(filter);
                };


            if (job != undefined) {
                VRTimerService.unregisterJob(job);
            }
            job = VRTimerService.registerJob(getNodesStates, $scope, timeInterval);



            VRRuntime_RuntimeNodeAPIService.GetAllNodes().then(function (response) {
                if (response != null) {
                    for (var i = 0; i < response.length; i++) {
                        addToDatasource(response[i]);            
                    }
                    function addToDatasource(nodeObj) {

                        nodeObj.editRuntimeNode = function () {
                            var onRuntimeNodeUpdated = function (node) {
                                nodeObj = node;
                            };
                            VRRuntime_RuntimeNodeService.editRuntimeNode(nodeObj.RuntimeNodeId, onRuntimeNodeUpdated);
                        };

                        nodeObj.previewRuntimeNode = function () {

                            filter = {
                                RuntimeNodeInstanceId: nodeObj.State.InstanceId
                            };
                            if (nodeObj.preview == true) {
                                nodeObj.preview = false;
                                $scope.Preview = false;
                            }

                            else {
                                for (var i = 0; i < $scope.datasource.length; i++)
                                {
                                    $scope.datasource[i].preview = false;
                                }
                                nodeObj.preview = true;
                                $scope.Preview = true;
                            }
                        };
                        $scope.datasource.push(nodeObj);
                    }
                }
            });



            function getNodesStates() {
                var promises = [];    
                var getStates = UtilsService.createPromiseDeferred();
                promises.push(getStates.promise);

                VRRuntime_RuntimeNodeStateAPIService.GetAllNodesStates().then(function (response) {
                    for (var i = 0; i < response.length; i++) {
                        for (var j = 0; j < $scope.datasource.length; j++) {
                            if ($scope.datasource[j].RuntimeNodeId == response[i].RuntimeNodeId) {
                                $scope.datasource[j].State = response[i];
                                //if($scope.datasource[i].preview==true){
                                //    call the Grid
                                //}
                            }
                        }
                        getStates.resolve();
                    }
                });
                return UtilsService.waitMultiplePromises(promises);
            }


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
                $scope.datasource[index] = node;
            };

            VRRuntime_RuntimeNodeService.editRuntimeNode(nodeObj.RuntimeNodeId, onRuntimeNodeUpdated);
        }
    }

    appControllers.controller('VRRuntime_RuntimeNodeManagementController', RuntimeNodeManagementController);
})(appControllers);