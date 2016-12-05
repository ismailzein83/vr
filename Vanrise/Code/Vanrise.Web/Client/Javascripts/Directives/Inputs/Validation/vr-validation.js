'use strict';

app.directive('vrValidation', ['ValidationService', 'BaseDirService', function (ValidationService, BaseDirService) {

    var directiveDefinitionObject = {

        restrict: 'A',       
        compile: function (element, attrs) {

            return {
                post: function (scope, iElem, iAttrs, ctrl) {
                    var options = scope.$eval(iAttrs.options);
                    var validationElement = angular.element(iElem[0].querySelector('.validate-element'));


                    validationElement.bind('blur', function () {
                        options = scope.$eval(iAttrs.options);
                        if (options.required && ValidationService.isEmpty(options.selectedvalues))
                            validationElement.toggleClass('required-inpute');
                    });
                }
            };
        }
    };

    return directiveDefinitionObject;

}]);