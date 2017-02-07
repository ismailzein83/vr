'use strict';

app.directive('vrButton', ['ButtonDirService', 'UtilsService', function (ButtonDirService, UtilsService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        scope: {
            onclick: '=',
            isasynchronous: '=',
            formname: '=',
            haspermission: '=',
            validationcontext: '='
        },
        controller: function ($scope, $element, $attrs) {
            var isSubmitting = false;
            var ctrl = this;
            
            ctrl.onInternalClick = function (evnt) {
                if (ctrl.menuActions != undefined) {
                    var self = angular.element(evnt.currentTarget);

                    ctrl.showMenuActions = true;
                    if ($(self).parents('.section-menu').length == 0)
                        return;
                    var selfHeight = $(self).height();
                    var selfOffset = $(self).offset();
                    var dropDown = self.find('.dropdown-menu')[0];
                    var basetop = selfOffset.top - $(window).scrollTop() + $(self).height();
                    var eltop = selfOffset.top - $(window).scrollTop();
                    var elleft = selfOffset.left - $(window).scrollLeft();
                    $(dropDown).css({ position: 'fixed', top: basetop - 10, left: elleft - 100 , bottom: 'unset' });

                    
                }                    
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
            var types = {
                "Close": false,
                "Preview": false
            };

            ctrl.isExculdedOnreadOnly = function () {
                var readOnly = UtilsService.isContextReadOnly($scope);
                return !readOnly || types[$attrs.type] != undefined;

            };
            var menu;
            if ($attrs.menuactions != undefined ) {
                    menu = $scope.$parent.$eval($attrs.menuactions);
                    if (menu != undefined)
                    {
                        var checkMenuActionPermission = function () {
                            for (var i = 0; i < menu.length; i++) {
                                invokeHasPermission(menu[i]);
                            }
                        };

                        var invokeHasPermission = function (menuAction) {
                            if (menuAction.haspermission == undefined || menuAction.haspermission == null) { return; }
                            menuAction.disable = true;
                            UtilsService.convertToPromiseIfUndefined(menuAction.haspermission()).then(function (isAllowed) {
                                if (isAllowed) { menuAction.disable = false; }
                            });
                        };
                        checkMenuActionPermission();
                    }
                    
               
            }
            ctrl.menuActions = menu;

            
            
            ctrl.hideTemplate = false;
            if (ctrl.haspermission != undefined && typeof (ctrl.haspermission) == 'function') {
                ctrl.hideTemplate = true;
                ctrl.haspermission().then(function (isAllowed) {
                    ctrl.hideTemplate = !isAllowed;
                });
            }
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
            };
        },
        template: function (element, attrs) {
            return ButtonDirService.getTemplate(attrs);
        }

    };

    return directiveDefinitionObject;

}]);