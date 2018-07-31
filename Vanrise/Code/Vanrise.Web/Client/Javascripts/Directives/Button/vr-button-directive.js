'use strict';

app.directive('vrButton', ['ButtonDirService', 'UtilsService', 'VRLocalizationService', 'VRModalService', 'MobileService', function (ButtonDirService, UtilsService, VRLocalizationService, VRModalService, MobileService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        scope: {
            onclick: '=',
            isasynchronous: '=',
            formname: '=',
            haspermission: '=',
            validationcontext: '=',
            showbutton: "=",
            disabledbtn: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.isMobile = MobileService.isMobile();

            ctrl.isSubmitting = false;

            ctrl.onInternalClick = function (evnt, btn) {
                if (ctrl.menuActions != undefined) {
                    if (ctrl.isMobile) {

                        var modalSettings = {
                            autoclose: true
                        };
                        modalSettings.onScopeReady = function (modalScope) {
                            modalScope.btn = btn;
                        };
                        VRModalService.showModal("/Client/Javascripts/Directives/Button/MenuActionsModalPopup.html", null, modalSettings);
                        return;
                    }


                    var self = angular.element(evnt.currentTarget);
                    ctrl.showMenuActions = true;

                    var selfHeight = $(self).height();
                    var selfOffset = $(self).offset();
                    var dropDown = self.parent().find('.dropdown-menu')[0];
                    var basetop = selfOffset.top - $(window).scrollTop() + $(self).height();
                    var eltop = selfOffset.top - $(window).scrollTop();
                    var elleft = selfOffset.left - $(window).scrollLeft();
                    if ($(self).parents('.section-menu').length > 0 && !VRLocalizationService.isLocalizationRTL())
                        elleft -= $(self).width();
                    if ($(self).parents('.vr-datagrid-celltext').length > 0)
                        basetop -= 10;

                    $(dropDown).css({ position: 'fixed', top: basetop, left: elleft, bottom: 'unset' });

                }
                else {
                    if (ctrl.onclick != undefined && typeof (ctrl.onclick) == 'function' && ctrl.isSubmitting == false) {
                        ctrl.isSubmitting = true;
                        var promise = ctrl.onclick();//this function should return a promise in case it is performing asynchronous task
                        if (promise != undefined && promise != null) {
                            promise.finally(function () {
                                ctrl.isSubmitting = false;
                            });
                        }
                        else {
                            var dummypromise = UtilsService.createPromiseDeferred();
                            setTimeout(function () {
                                dummypromise.resolve();
                            }, 10);
                            dummypromise.promise.finally(function () {
                                ctrl.isSubmitting = false;
                            });
                        }
                    }
                }
            };

            ctrl.showButton = function () {
                if ($attrs.showbutton == undefined)
                    return true;
                else return ctrl.showbutton;
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
                });
            }
            ctrl.showIcon = function () {
                return !ctrl.isSubmitting;
            };

            ctrl.showLoader = function () {
                return ctrl.isSubmitting;
            };

            ctrl.isDisabled = function () {
                var validationContext = ctrl.validationcontext != undefined ? ctrl.validationcontext : ctrl.formname;
                if (validationContext != undefined && validationContext.validate() != null)
                    return true;
                if (ctrl.formname != undefined && ctrl.formname.validate() != null)
                    return true;
                if (ctrl.disabledbtn != undefined)
                    return ctrl.disabledbtn;
                var isDisabled;
                if (ctrl.isSubmitting == true)
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
                return !readOnly || types[$attrs.type] != undefined || $attrs.stopreadonly != undefined;

            };
            var menu;
            if ($attrs.menuactions != undefined) {
                menu = $scope.$parent.$eval($attrs.menuactions);
                if (menu != undefined) {
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
        require: "?^vrActionbar",
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
                pre: function ($scope, iElem, iAttrs, parentCtrl) {
                    if (parentCtrl != null && typeof (parentCtrl.addButton) == 'function') {
                        var btnObj = $scope.ctrl;
                        var btnProp = ButtonDirService.getButtonAttributes(iAttrs.type);
                        btnObj.name = btnProp.text;
                        btnObj.iconClass = btnProp.class;
                        btnObj.showButtonVal = btnObj.showButton();
                        parentCtrl.addButton(btnObj);
                    }
                }
            };
        },
        template: function (element, attrs) {
            return ButtonDirService.getTemplate(attrs);
        }

    };

    return directiveDefinitionObject;

}]);