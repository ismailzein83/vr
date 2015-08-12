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
                return '<vr-label>' + label + '</vr-label><div><switch ng-model="value" ng-change="notifyUserChange()" class="green"></switch></div>';
            }
            else
                return "";
            
        }

    };

    return directiveDefinitionObject;

}]);