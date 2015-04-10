'use strict';


app.directive('vrDatagrid', ['DataGridDirService', function (DataGridDirService) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
           
        },
        controller: function ($scope, $element) {
            
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {


                }
            }
        },
        templateUrl: function (element, attrs) {
            return DataGridDirService.dTemplate;
        }

    };

    return directiveDefinitionObject;

}]);