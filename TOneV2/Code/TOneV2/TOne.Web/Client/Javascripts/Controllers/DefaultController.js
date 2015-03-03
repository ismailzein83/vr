appControllers.controller('DefaultController',
    function DefaultController($scope) {
        $scope.selectedCountries = [];
        $scope.testModel = 'initial from default';
        $scope.selectLanguage = function (language) {
            $scope.selectedCountries.push(language);
        };
    });