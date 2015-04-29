'use strict';


app.directive('vrDatagridrows', [function () {
    var directiveDefinitionObject = {

        restrict: 'E',
        require: '^vrDatagrid',
        scope: {},
        controller: function ($scope, $element) {            

        },
        link: function (scope, elem, attrs, dataGridCtrl) {
            scope.ctrl = dataGridCtrl;
        },
        templateUrl: function (element, attrs) {
            return "/Client/Javascripts/Directives/DataGrid/vr-datagrid.html";
        }
    };

    return directiveDefinitionObject;

}]);

