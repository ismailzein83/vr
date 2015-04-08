
var appControllers = angular.module('appControllers', ['ui.grid',
    'ui.bootstrap',
    'ngAnimate',
    'ui.grid.saveState',
    'ui.grid.pagination',
    'ui.grid.selection',
    'ui.grid.infiniteScroll',
    'ui.grid.resizeColumns',
    'ui.grid.expandable',
    'ui.grid.autoResize',
    'ng-sortable',
    'ngSanitize',
    'mgcrea.ngStrap',
    'uiSwitch',
    'cgNotify',
    'ui.grid.autoResize',
    'ngMessages'//,
    //'slidePushMenu'
]);
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


appControllers.directive('numberFiled', function () {
    return {
        restrict: 'EA',
        template: '<input name="{{inputName}}" ng-model="inputValue" class="form-control" />',
        scope: {
            inputValue: '=',
            inputName: '='
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