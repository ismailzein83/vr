
var appControllers = angular.module('appControllers', ['ui.grid', 'ui.grid.edit', 'ui.grid.pagination', 'ui.grid.selection', 'ui.grid.infiniteScroll', 'ui.grid.resizeColumns', 'ng-sortable', 'ngSanitize', 'mgcrea.ngStrap', 'uiSwitch', 'cgNotify']);
appControllers.directive('resizable', function () {
    return {
        restrict: 'A',
        scope: {
            callback: '&onResize'
        },
        link: function postLink(scope, elem, attrs) {
            elem.resizable();
            elem.on('resizestop', function (evt, ui) {
                if (scope.callback) { scope.callback(); }
            });
        }
    };
});