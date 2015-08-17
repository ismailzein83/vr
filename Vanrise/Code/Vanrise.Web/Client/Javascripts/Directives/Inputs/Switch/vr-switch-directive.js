'use strict';

app.directive('vrSwitch', ['SecurityService', function (SecurityService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            value: '='
        },
        controller: function ($scope, $element) {

        },
        link: function (scope, element, attrs, ctrl) {

            var isUserChange;
            scope.$watch('value', function () {
                if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                    return;
                isUserChange = false;//reset the flag
                if (attrs.onvaluechanged != undefined) {
                    var onvaluechangedMethod = scope.$parent.$eval(attrs.onvaluechanged);
                    if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                        onvaluechangedMethod();
                    }
                }
            });
            scope.withLable = false;
            if (attrs.label != undefined)
                scope.withLable = true;

            if (attrs.hint != undefined)
                scope.hint = attrs.hint;
            scope.notifyUserChange = function () {
                isUserChange = true;
            };
            
        },
        //controllerAs: 'ctrl',
        //bindToController: true,
        template: function (element, attrs) {
            if (attrs.permissions === undefined || SecurityService.isAllowed(attrs.permissions)) {
                var label = attrs.label;
                if (label == undefined)
                    label = '';
                return '<vr-label ng-if="withLable">' + label + '</vr-label><div><switch ng-model="value" ng-change="notifyUserChange()" class="green"></switch>'
                    + '<span ng-if="hint!=undefined" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor" style="color:#337AB7;top:-1px" html="true" placement="bottom" trigger="hover" data-type="info" data-title="{{hint}}"></span>'
                    + '</div>';
            }
            else
                return "";
            
        }

    };

    return directiveDefinitionObject;

}]);