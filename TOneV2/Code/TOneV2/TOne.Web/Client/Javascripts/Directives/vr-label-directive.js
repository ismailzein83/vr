'use strict';

app.directive('vrLabel', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
        },
        controller: function ($scope, $element) {
            
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (tElement, tAttrs) {
            var newElement = '<label class="control-label" >' + tElement.context.innerHTML + '</label>';
            tElement.html(newElement);
        }//,
        //template: function (element, attrs) {
        //    return '<label class="control-label" >{{ctrl.text}}</label>';
        //}

    };

    return directiveDefinitionObject;

}]);