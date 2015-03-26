'use strict';

app.directive('vrValidation', ['ValidationService', 'BaseDirService', function (ValidationService, BaseDirService) {

    var directiveDefinitionObject = {

        restrict: 'A',
        //scope: {
        //    options: '='
        //},
        //controller: function () {
            
        //},
        //controllerAs: 'ctrl',
        //bindToController: true,
        compile: function (element, attrs) {

            return {
                post: function (scope, iElem, iAttrs, ctrl) {
                    var options = scope.$eval(iAttrs.vrValidation);
                    console.log(options);
                    var validationElement = angular.element(iElem[0].querySelector('.validate-element'));
                    if (options.required)
                        validationElement.addClass('required-inpute');
                }
            }
        }
    };

    return directiveDefinitionObject;

}]);