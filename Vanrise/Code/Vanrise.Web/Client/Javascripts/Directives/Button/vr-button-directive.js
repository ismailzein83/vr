'use strict';

app.directive('vrButton', ['ButtonDirService', 'SecurityService', function (ButtonDirService, SecurityService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        scope: {
            onclick: '=',
            isasynchronous: '=',
            formname: '=',
            permissions: '@',
            validationcontext: '='
        },
        controller: function ($scope, $element, $attrs) {
            
            var isSubmitting = false;
            var ctrl = this;
           
            ctrl.onInternalClick = function (evnt) {
                if (ctrl.menuActions != undefined)
                    ctrl.showMenuActions = true;
                else {
                    if (ctrl.onclick != undefined && typeof (ctrl.onclick) == 'function') {
                        var promise = ctrl.onclick();//this function should return a promise in case it is performing asynchronous task
                        if (promise != undefined && promise != null) {
                            isSubmitting = true;
                            promise.finally(function () {
                                isSubmitting = false;
                            });
                        }
                    }
                }
            };

            ctrl.menuActionClicked = function (action) {
                var promise = action.clicked();//this function should return a promise in case it is performing asynchronous task
                if (promise != undefined && promise != null) {
                    action.isSubmitting = true;
                    promise.finally(function () {
                        action.isSubmitting = false;
                    });
                }
            };
            if ($attrs.submitname != undefined) {
                $scope.$on('submit' + $attrs.submitname, function () {
                    if (!ctrl.isDisabled())
                        ctrl.onInternalClick();
                }) ;
            }
            ctrl.showIcon = function () {
                return !isSubmitting;
            };

            ctrl.showLoader = function () {
                return isSubmitting;
            };

            ctrl.isDisabled = function () {
                var validationContext = ctrl.validationcontext != undefined ? ctrl.validationcontext : ctrl.formname;
                if (validationContext != undefined && validationContext.validate() != null)
                    return true;
                var isDisabled;
                if (isSubmitting == true)
                    isDisabled = true;
                else
                    isDisabled = false;
                return isDisabled;
            };

            ctrl.menuActions = $attrs.menuactions != undefined ? $scope.$parent.$eval($attrs.menuactions) : undefined;
            
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
            if (attrs.permissions === undefined || SecurityService.isAllowed(attrs.permissions))
                return ButtonDirService.getTemplate(attrs);
            else
                return "";
        }

    };

    return directiveDefinitionObject;

}]);