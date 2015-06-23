BPDefinitionManagementController.$inject = ['$scope', 'BusinessProcessAPIService', 'VRModalService'];

function BPDefinitionManagementController($scope,BusinessProcessAPIService, VRModalService) {

  
    "use strict";

    var mainGridAPI;
    


    function showStartBPModal(BPDefinition) {

        VRModalService.showModal('/Client/Modules/BusinessProcess/Views/StartBP.html', {
            BPDefinitionID: BPDefinition.ID
        }, {
            onScopeReady: function (modalScope) {
                modalScope.title = "Start Bussiness Process";
            }
        });
    }


    function getData() {

        var pageInfo = mainGridAPI.getPageInfo();

        var name = $scope.name != undefined ? $scope.name : '';
        var title = $scope.title != undefined ? $scope.title : '';
      

        return BusinessProcessAPIService.GetFilteredDefinitions(pageInfo.fromRow, pageInfo.toRow, name, title).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.filteredDefinitions.push(itm);
                console.log($scope.filteredDefinitions)
            });
        });
    }

    function defineGrid() {
        $scope.filteredDefinitions = [];
        $scope.gridMenuActions = [];
        $scope.loadMoreData = function () {
            return getData();
        };
        $scope.onGridReady = function (api) {
            mainGridAPI = api;
        };
        $scope.gridMenuActions = [{
            name: "Start",
            clicked: showStartBPModal
        }];
    }

    $scope.searchClicked = function () {
        mainGridAPI.clearDataAndContinuePaging();
        return getData();
    };


    defineGrid();

};

appControllers.controller('BusinessProcess_BPDefinitionManagementController', BPDefinitionManagementController);


