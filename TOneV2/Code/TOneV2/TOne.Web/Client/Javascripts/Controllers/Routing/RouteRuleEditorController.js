appControllers.controller('RouteRuleEditorController',
    function RouteRuleEditorController($scope) {
        $('.demo .ui.dropdown').dropdown();
        $scope.model = 'RouteRuleEditorController';
        $scope.selectedCountries = [];
        $scope.itemsSortable = {
            containment : "parent",//Dont let the user drag outside the parent
            cursor : "move",//Change the cursor icon on drag
            tolerance : "pointer"
        }
        $scope.Countries = ['Arabic', 'Chinese', 'Danish', 'Spanish'];       
        $scope.selectLanguage = function (language) {
            var index = null;
            try {
                var index = $scope.selectedCountries.indexOf(language);
            }
            catch (e) {

            }
            if (index >= 0) {
                $scope.selectedCountries.splice(index, 1);
            }
            else
                $scope.selectedCountries.push(language);
        };
    });