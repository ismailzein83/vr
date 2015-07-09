
var appControllers = angular.module('appControllers', [
    'ui.bootstrap',
    'ngAnimate',
    'ng-sortable',
    'ngSanitize',
    'mgcrea.ngStrap',
    'uiSwitch',
    'cgNotify',
    'ui.grid.autoResize',
    'ngMessages',
    'ivh.treeview',
    'angularTreeview'
]);



appControllers.directive('numberFiled', function () {
    return {
        restrict: 'EA',
        template: '<input name="{{inputName}}" ng-model="inputValue" placeholder="{{placeHolder}}" class="form-control" />',
        scope: {
            inputValue: '=',
            inputName: '=',
            placeHolder: '='
        },
        link: function (scope) {
            scope.$watch('inputValue', function (newValue, oldValue) {
                var arr = String(newValue).split("");
                if (arr.length === 0) return;
                if (arr.length === 1 && (arr[0] == '-' || arr[0] === '.')) return;
                if (arr.length === 2 && newValue === '-.') return;
                if (isNaN(newValue)) {
                    scope.inputValue = oldValue;
                }
            });
        }
    };
});
appControllers.directive('draggable', function ($document) {
    "use strict";
    return function (scope, element) {
        var startX = 0,
          startY = 0,
          x = 0,
          y = 0;
        element.css({
            position: 'fixed'
        });
        element.find('.modal-header').css({
            cursor: 'move'
        });
        element.find('.modal-header').on('mousedown', function (event) {
            // Prevent default dragging of selected content
            event.preventDefault();
            startX = event.screenX - x;
            startY = event.screenY - y;
            $document.on('mousemove', mousemove);
            $document.on('mouseup', mouseup);
        });

        function mousemove(event) {
            y = event.screenY - startY;
            x = event.screenX - startX;
            element.css({
                top: y + 'px',
                left: x + 'px'
            });
        }

        function mouseup() {
            $document.unbind('mousemove', mousemove);
            $document.unbind('mouseup', mouseup);
        }
    };
});
