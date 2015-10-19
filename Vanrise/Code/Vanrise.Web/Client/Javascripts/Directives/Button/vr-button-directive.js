'use strict';

app.directive('vrButton', ['ButtonDirService', 'SecurityService', function (ButtonDirService, SecurityService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        scope: {
            onclick: '=',
            isasynchronous: '=',
            formname: '@',
            permissions: '@'
        },
        controller: function ($scope, $element, $attrs) {
            
            var isSubmitting = false;
            var ctrl = this;
           
            this.onInternalClick = function (evnt) {
                if (ctrl.menuActions != undefined)
                    ctrl.showMenuActions = true;
                else {
                    if (this.onclick != undefined && typeof (this.onclick) == 'function') {
                        var promise = this.onclick();//this function should return a promise in case it is performing asynchronous task
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

            $scope.$on('submit' + this.formname, function () {
                if (ctrl.formname != undefined) {
                    var form = $scope.$parent.$eval(ctrl.formname);
                    if (form != undefined && form.$invalid)
                        return true;
                }
                ctrl.onInternalClick();
            }) ;
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