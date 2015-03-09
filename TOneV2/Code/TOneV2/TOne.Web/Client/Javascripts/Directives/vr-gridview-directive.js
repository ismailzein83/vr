'use strict';

app.service('GridViewService', ['BaseDirService', function (BaseDirService) {

    this.dTemplate = BaseDirService.directiveMainURL + "vr-gridview-standard.html";

    this.getTemplateByType = function (type) {
        return BaseDirService.directiveMainURL + 'vr-gridview-' + type + '.html';
    };

}]);



app.directive('vrGridview', ['GridViewService', function (GridViewService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            gridoptions: '=',
            columndefs: '@columnDefs'
        },
        templateUrl: function (element, attrs) {
            if (attrs.type == undefined) return GridViewService.dTemplate;
            return GridViewService.getTemplateByType(attrs.type);
        },
        compile: function (tElement, attrs) {
            var tr = angular.element(document.getElementById('trbody'));
            var row = '';
            if (attrs.columnDefs == undefined) return;

            angular.forEach(attrs.columnDefs.split(','), function (item) {
                row = row + '<td>{{n.' + item + '}}</td>';
            });

            tr.append(row);
        }
    };

    return directiveDefinitionObject;

}]);