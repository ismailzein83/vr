(function (app) {

    "use strict";

    vrTextarea.$inject = ['BaseDirService' ];

    function vrTextarea(BaseDirService, TextboxTypeEnum) {

        return {
            restrict: 'E',
            require: '^form',
            scope: {
                value: '=',
                hint: '@',               
                customvalidate: '&',
                placeholder:'@'
            },
            controller: function ($scope, $element) {

            },
            compile: function (element, attrs) {

                var inputElement = element.find('#mainInput');
                var validationOptions = {};
                if (attrs.isrequired !== undefined)
                    validationOptions.requiredValue = true;
                if (attrs.customvalidate !== undefined)
                    validationOptions.customValidation = true;
             
                var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, element.find('#rootDiv'));
                return {
                    pre: function ($scope, iElem, iAttrs, formCtrl) {
                        var ctrl = $scope.ctrl;

                        var isUserChange;
                        $scope.$watch('ctrl.value', function (newValue, oldValue) {
                        
                            if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                                return;

                            if (newValue == "") {
                                ctrl.value = undefined;
                            }
                            isUserChange = false;//reset the flag
                           
                            if (iAttrs.onvaluechanged != undefined) {
                                var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onvaluechanged);
                                if (onvaluechangedMethod != undefined && typeof (onvaluechangedMethod) == 'function') {
                                    onvaluechangedMethod();
                                }
                            }
                        });

                        ctrl.notifyUserChange = function () {
                            isUserChange = true;
                        };
                        ctrl.readOnly = attrs.readonly != undefined;
                        ctrl.placelHolder = (attrs.placeholder != undefined) ? ctrl.placeholder : '';

                        if (attrs.hint != undefined) {
                            ctrl.hint = attrs.hint;
                        }
                        ctrl.getInputeStyle = function () {
                            return (attrs.hint != undefined) ? {
                                    "display": "inline-block",
                                    "width": "calc(100% - 15px)",
                                    "margin-right": "1px"
                                } :{} ;
                        }

                        ctrl.adjustTooltipPosition = function (e) {
                            setTimeout(function () {
                                var self = angular.element(e.currentTarget);
                                var selfHeight = $(self).height();
                                var selfOffset = $(self).offset();
                                var tooltip = self.parent().find('.tooltip-info')[0];
                                $(tooltip).css({ display: 'block' });
                                var innerTooltip = self.parent().find('.tooltip-inner')[0];
                                var innerTooltipArrow = self.parent().find('.tooltip-arrow')[0];
                                $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight +5, left: selfOffset.left - 30 });
                                $(innerTooltipArrow).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight , left: selfOffset.left  });

                            }, 1)
                        }
                       
                        BaseDirService.addScopeValidationMethods(ctrl, elementName, formCtrl);

                    }
                }
            },

            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                var startTemplate = '<div id="rootDiv" style="position: relative;">';
                var endTemplate = '</div>';

                    var labelTemplate = '';
                    if (attrs.label != undefined)
                        labelTemplate = '<vr-label>' + attrs.label + '</vr-label>';
                    var textboxTemplate = '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false">'
                            + '<textarea  placeholder="{{ctrl.placelHolder}}" ng-readonly="ctrl.readOnly" id="mainInput" ng-style="ctrl.getInputeStyle()" ng-model="ctrl.value" ng-change="ctrl.notifyUserChange()"rows="3" class="form-control" style="width: 100%; resize: none;" ></textarea>'
                            + '<span ng-if="ctrl.hint!=undefined" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor" html="true" style="color:#337AB7"  placement="bottom"  trigger="hover" ng-mouseenter="ctrl.adjustTooltipPosition($event)"  data-type="info" data-title="{{ctrl.hint}}"></span>'
                        + '</div>';
                

                    var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined ,true);

                return startTemplate + labelTemplate + textboxTemplate + validationTemplate + endTemplate;
            }

        };

    }

    app.directive('vrTextarea', vrTextarea);
    
})(app);



