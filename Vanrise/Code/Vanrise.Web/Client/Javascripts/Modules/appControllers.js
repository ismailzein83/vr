
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
    'ngMessages',
    'ivh.treeview'
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
appControllers.directive('resizer', function($document) {

    return function($scope, $element, $attrs) {

        $element.on('mousedown', function(event) {
            event.preventDefault();

            $document.on('mousemove', mousemove);
            $document.on('mouseup', mouseup);
        });
        var x; 
        function mousemove(event) {
            console.log($attrs.ind)
            if ($attrs.resizer == 'vertical') {
               var  x = event.pageX -  $element.parent().width();
            } 
        }

        function mouseup(event) {
            if ($attrs.resizer == 'vertical') {
                x = event.pageX - $element.parent().width();
            }
            //var nextdivwidth = $scope.columns[parseInt($attrs.ind + 1)].colwidth - (x - ($scope.columns[parseInt($attrs.ind + 1)].colwidth));
            $scope.$apply(function () {
               console.log(x)
                $element.parent().css({
                    width: x + 'px'
                });
               // console.log($scope.columns[parseInt($attrs.ind + 1)].colwidth + " befor ")
                $scope.columns[$attrs.ind].colwidth = x;
              //  $scope.columns[parseInt($attrs.ind + 1)].colwidth = nextdivwidth;
               // console.log($scope.columns[parseInt($attrs.ind + 1)].colwidth  + " after ")
            });
            $document.unbind('mousemove', mousemove);
            $document.unbind('mouseup', mouseup);
        }
    };
});

