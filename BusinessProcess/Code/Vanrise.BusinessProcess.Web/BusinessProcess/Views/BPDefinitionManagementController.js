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

        
        var title = $scope.title != undefined ? $scope.title : '';
      

        return BusinessProcessAPIService.GetFilteredDefinitions(pageInfo.fromRow, pageInfo.toRow, title).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.filteredDefinitions.push(itm);
            });
        });
    }

    function defineGrid() {
        $scope.filteredDefinitions = [];
        $scope.loadMoreData = function () {
            return getData();
        };
        $scope.onGridReady = function (api) {
            mainGridAPI = api;
            return getData();
        };
       
    }

    $scope.searchClicked = function () {
        mainGridAPI.clearDataAndContinuePaging();
        return getData();
    };


    defineGrid();

};

appControllers.controller('BusinessProcess_BPDefinitionManagementController', BPDefinitionManagementController);


