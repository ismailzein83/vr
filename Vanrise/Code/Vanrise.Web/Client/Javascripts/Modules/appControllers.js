'use strict';
var appControllers = angular.module('appControllers', [
    'ui.bootstrap',
    'ngAnimate',
    'ng-sortable',
    'ngSanitize',
    'mgcrea.ngStrap',
    'cgNotify',
    'ngMessages',
    'ivh.treeview',
    'angularTreeview',
    'ui.codemirror',
    'ngWYSIWYG'
]);
appControllers.directive('draggable',['$document', function ($document) {
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
            scope.$broadcast("start-drag");
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
}]);


appControllers.directive('draggablemodal',['$document',function ($document) {
    "use strict";
    return function (scope, element) {
        var startX = 0,
          startY = 0,
          x = 0,
          y = 0;
        element.parents().find('.modal-header').last().css({
            cursor: 'move'
        });
        element.parents().find('.modal-header').last().on('mousedown', function (event) {
            // Prevent default dragging of selected content
            event.preventDefault();
            startX = event.screenX - x;
            startY = event.screenY - y;
            $document.on('mousemove', mousemove);
            $document.on('mouseup', mouseup);
            scope.$broadcast("start-drag");
        });

        function mousemove(event) {
            y = (event.screenY - startY > 0 && event.screenY - startY < innerHeight - element.innerHeight()) ? event.screenY - startY : y;
            x = (event.screenX - startX < innerWidth ) ? event.screenX - startX : x;
            element.parents().find('.modal-content').last().css({
                top: y + 'px',
                left: x + 'px'
            });
        }

        function mouseup() {
            $document.unbind('mousemove', mousemove);
            $document.unbind('mouseup', mouseup);
        }
    };
}]);

appControllers.directive('clickOutside',['$document', function ($document) {
    "use strict";
    return {
        restrict: 'A',
        scope: {
            clickOutside: '&'
        },
        link: function ($scope, elem, attr) {
            var classList = (attr.outsideIfNot !== undefined) ? attr.outsideIfNot.replace(', ', ',').split(',') : [];
            if (attr.id !== undefined) classList.push(attr.id);

            $document.on('click', function (e) {
                var i = 0,
                    element;

                if (!e.target) return;

                for (element = e.target; element; element = element.parentNode) {
                    var id = element.id;
                    var classNames = element.className;

                    if (id !== undefined) {
                        for (i = 0; i < classList.length; i++) {
                            if (id.indexOf(classList[i]) > -1 || classNames.indexOf(classList[i]) > -1) {
                                return;
                            }
                        }
                    }
                }

                $scope.$eval($scope.clickOutside);
            });
        }
    };
}])