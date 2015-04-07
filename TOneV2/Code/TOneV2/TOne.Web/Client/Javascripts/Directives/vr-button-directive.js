'use strict';

app.directive('vrButton', ['ButtonDirService', function (ButtonDirService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        scope: {
            onclick: '=',
            isasynchronous: '=',
            isdisabled: '='
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
                var isDisabled;
                if (isSubmitting == true)
                    isDisabled = true;
                else
                    isDisabled = false;
                if (this.isdisabled != undefined)
                    this.isdisabled = isDisabled;
                return isDisabled;
            };
        },
        controllerAs: 'ctrl',
        bindToController: true,
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