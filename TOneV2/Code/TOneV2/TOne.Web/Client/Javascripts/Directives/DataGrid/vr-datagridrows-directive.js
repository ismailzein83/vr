'use strict';


app.directive('vrDatagridrows', [function () {
    var directiveDefinitionObject = {

        restrict: 'E',
        require: '^vrDatagrid',
        scope: {},
        controller: function ($scope, $element) {            

        },
        link: function (scope, elem, attrs, dataGridCtrl) {
            scope.isGridScope = true;
            scope.ctrl = dataGridCtrl;
            scope.gridParentScope = scope.$parent;
            scope.viewScope = scope.$parent;
            while (scope.viewScope.isGridScope)
                scope.viewScope = scope.viewScope.$parent;
        },
        templateUrl: function (element, attrs) {
            return "/Client/Javascripts/Directives/DataGrid/vr-datagrid.html";
        }
    };

    return directiveDefinitionObject;

}]);

