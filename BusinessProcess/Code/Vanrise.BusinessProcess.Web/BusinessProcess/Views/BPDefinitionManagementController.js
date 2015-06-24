BPDefinitionManagementController.$inject = ['$scope', 'BusinessProcessAPIService', 'VRModalService'];

function BPDefinitionManagementController($scope,BusinessProcessAPIService, VRModalService) {

  
    "use strict";

    var mainGridAPI;
    

    function showStartNewInstance(BPDefinitionObj) {
        console.log("BPDefinitionObj.Configuration.Url")
        console.log(BPDefinitionObj.Configuration.Url)
        VRModalService.showModal(BPDefinitionObj.Configuration.Url, {
            BPDefinitionID: BPDefinitionObj.BPDefinitionID
        },
        {
            onScopeReady: function (modalScope) {
                modalScope.title = "Start";
            }
        });
    }


    function showBPTrackingModal(BPInstanceObj) {

        VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTrackingModal.html', {
            BPInstanceID: BPInstanceObj.ProcessInstanceID
        }, {
            onScopeReady: function (modalScope) {
                modalScope.title = "Tracking";
            }
        });
    }


    function getData() {

        var pageInfo = mainGridAPI.getPageInfo();

        
        var title = $scope.title != undefined ? $scope.title : '';
      
        $scope.openedInstances = [];

        BusinessProcessAPIService.GetOpenedInstances().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.openedInstances.push(itm);
            });
            BusinessProcessAPIService.GetFilteredDefinitions(pageInfo.fromRow, pageInfo.toRow, title).then(function (response) {
                angular.forEach(response, function (def) {
                    def.OpenedInstances = [];
                    var countRunningInstances = 0;
                    angular.forEach($scope.openedInstances, function (inst) {
                        if (inst.DefinitionID == def.BPDefinitionID)
                        {
                            countRunningInstances++;
                            def.OpenedInstances.push(inst);
                        }
                            
                    });
                    def.RunningInstances = countRunningInstances;
                    $scope.filteredDefinitions.push(def);
                });
            });
        });
        //console.log($scope.filteredDefinitions)
    }

    function defineGrid() {
        $scope.filteredDefinitions = [];
        $scope.gridMenuActions = [];
        $scope.loadMoreData = function () {
            return getData();
        };
        $scope.onGridReady = function (api) {
            mainGridAPI = api;
            return getData();
        };
        $scope.gridMenuActions = [{
            name: "Start New Instance",
            clicked: showStartNewInstance
        }];
       
    }

    $scope.searchClicked = function () {
        mainGridAPI.clearDataAndContinuePaging();
        return getData();
    };


    $scope.processInstanceClicked = function (dataItem) {
        showBPTrackingModal(dataItem);

    }


    defineGrid();

};

appControllers.controller('BusinessProcess_BPDefinitionManagementController', BPDefinitionManagementController);


