CDRLogController.$inject = ['$scope','CDRAPIService'];

function CDRLogController($scope, CDRAPIService) {

    $scope.name = "test";
    $scope.isInitializing = false;

    var mainGridAPI;
    $scope.data = [];
    $scope.Size = ['5', '10', '20','30']
    $scope.fromDate = '1990/01/01';
    $scope.toDate = '2015/05/01';
    $scope.nRecords='5'
    $scope.selectedsize = '5';
    defineScope();

    function defineScope() {
        
        $scope.mainGridPagerSettings = {
            currentPage: 1,
            totalDataCount: 0,
            pageChanged: function () {
                //      return getData();
            }
        };
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }

        $scope.getData = function () {
            $scope.mainGridPagerSettings.currentPage = 1;
            resultKey = null;
            //    mainGridAPI.resetSorting();
            //    resetSorting();
            //  return getData(true);
            return getData(true);
        };

        $scope.GetCDRData = GetCDRData;

    }


    function GetCDRData() {
      
        return CDRAPIService.GetCDRData($scope.fromDate, $scope.toDate, $scope.selectedsize).then(function (response) {
            //  alert(response);
            $scope.data = [];
            console.log(response);
            $scope.isInitializing = false;
            angular.forEach(response, function (itm) {
                $scope.data.push(itm);
            });

        });
    }
   

};

appControllers.controller('Analytics_CDRLogController', CDRLogController);