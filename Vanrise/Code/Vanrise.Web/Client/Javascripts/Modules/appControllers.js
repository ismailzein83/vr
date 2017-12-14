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
appControllers.directive('draggable', ['$document', function ($document) {
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
        scope.$on("$destroy", function () {
            $(window).off("resize.Viewport");
        });
    };
}]);
appControllers.factory('vrInterceptor', ['$q', '$rootScope', 'VRLocalizationService', function ($q, $rootScope, VRLocalizationService) {

    var requestInterceptor = {
        request: function (config) {
            if (/\.html$/.test(config.url) && config.url.indexOf("Client/") > -1) {
                config.url = config.url + "?v=" + $rootScope.version + "&vrlangId=" + VRLocalizationService.getLanguageCookie();
            }

            var deferred = $q.defer();
            deferred.resolve(config);
            return deferred.promise;
        }
    };

    return requestInterceptor;
}]);

appControllers.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('vrInterceptor');
}]);
appControllers.directive('draggablemodal', ['$document', function ($document) {
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
            x = (event.screenX - startX < innerWidth) ? event.screenX - startX : x;
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