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
                    var options = scope.$eval(iAttrs.options);
                    //console.log(options);
                    var validationElement = angular.element(iElem[0].querySelector('.validate-element'));


                    validationElement.bind('blur', function () {
                        options = scope.$eval(iAttrs.options);
                        // console.log(options);
                        if (options.required && ValidationService.isEmpty(options.selectedvalues))
                            validationElement.toggleClass('required-inpute');
                    });

                    //options.$parsers.unshift(function (value) {
                    //    console.log(value);
                    //    //var valid = value ? value.indexOf('dogs') == -1 : true;
                    //   // options.$setValidity('dogs', valid);
                    //    //return valid ? value : undefined;
                    //});


                    ////For model -> DOM validation
                    //options.$formatters.unshift(function (value) {
                    //    console.log("value");
                    //    console.log(value);
                    //    //options.$setValidity('dogs', value ? value.indexOf('dogs') === -1 : true);
                    //    //return value;
                    //});


                }
            };
        }
    };

    return directiveDefinitionObject;

}]);