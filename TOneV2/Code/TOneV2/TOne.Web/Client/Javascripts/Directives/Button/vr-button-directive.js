'use strict';

app.directive('vrButton', ['ButtonDirService', function (ButtonDirService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        scope: {
            onclick: '=',
            isasynchronous: '=',
            formname: '@'
        },
        controller: function ($scope, $element) {
            
            var isSubmitting = false;

            this.onInternalClick = function () {
                if (this.onclick != undefined && typeof (this.onclick) == 'function') {
                    if (this.isasynchronous) {
                        var asyncHandle = {
                            operationDone: function(){
                                isSubmitting = false;
                                asyncHandle = null;
                            }
                        };
                        isSubmitting = true;
                        this.onclick(asyncHandle);
                    }
                    else
                        this.onclick();
                }
            };

            this.showIcon = function () {
                return !isSubmitting;
            };

            this.showLoader = function () {
                return isSubmitting;
            };

            this.isDisabled = function () {
                if (this.formname != undefined) {
                    var form = $scope.$parent.$eval(this.formname);
                    if (form != undefined && form.$invalid)
                        return true;
                }
                var isDisabled;
                if (isSubmitting == true)
                    isDisabled = true;
                else
                    isDisabled = false;
                return isDisabled;
            };

            
        },
        controllerAs: 'ctrl',
        bindToController: true,
        //link: function (scope, formElement, attributes, formController) {
        //    if(attributes.isinvalid != undefined)
        //    {
        //        attributes.$observe('isinvalid', function (val) {
        //            scope.isinvalid = val;
        //        });
        //    }
        //    //var fn = $parse(attributes.rcSubmit);
 
        //    //formElement.bind('submit', function (event) {
        //    //    // if form is not valid cancel it.
        //    //    if (!formController.$valid) return false;
 
        //    //    scope.$apply(function() {
        //    //        fn(scope, {$event:event});
        //    //    });
        //    //});
        //},
        compile: function (element, attrs) {

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {                   
                }
            }
        },
        template: function (element, attrs) {
            return ButtonDirService.getTemplate(attrs.type);
        }

    };

    return directiveDefinitionObject;

}]);