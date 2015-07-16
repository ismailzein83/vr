testpage1.$inject = ['$scope'];

function testpage1($scope) {
    var widgetAPI;
    defineScope();
    load();

    function defineScope() {
      
        $scope.Search = function () {

        };

      
        $scope.treeReady2 = function (api) {
            api.refreshTree($scope.values);
        }
     
        
       
    }

    function load() {
        $scope.isGettingData = false;

    }
}
appControllers.controller('testpage1', testpage1);
