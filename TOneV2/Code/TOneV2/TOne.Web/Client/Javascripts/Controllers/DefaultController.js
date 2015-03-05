appControllers.controller('DefaultController',
    function DefaultController($scope) {
       
        $scope.selectedCountries = [];
        $scope.Countries = ['Arabic', 'Chinese', 'Danish','Spanish'];
        $scope.testModel = 'initial from default';
        $scope.selectLanguage = function (language) {
            var index = null;
            try{
                var index = $scope.selectedCountries.indexOf(language);
            }
            catch (e) {
                
            }
            if (index >= 0 ){
                $scope.selectedCountries.splice(index, 1);
            }
            else
                 $scope.selectedCountries.push(language);
        };
    });