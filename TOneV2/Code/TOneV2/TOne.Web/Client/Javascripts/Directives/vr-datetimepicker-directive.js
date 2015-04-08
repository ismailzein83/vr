'use strict';

app.directive('vrDatetimepicker', [function () {

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
            var labelTemplate = '';
            if (attrs.label != undefined)
                labelTemplate = '<vr-label>' + attrs.label + '</vr-label>';
            var dateTemplate = '<div style="display:inline-block;position:relative;width:100%;" ng-mouseenter="showtd=true" ng-mouseleave="showtd=false">'
                                + '<input size="10" class="form-control" ng-model="value" data-autoclose="1" placeholder="Date" bs-datepicker type="text" ng-change="notifyUserChange()" >'
                            + '</div>';
            if(attrs.type == 'date')
                return labelTemplate + dateTemplate;
        }

    };

    return directiveDefinitionObject;

}]);