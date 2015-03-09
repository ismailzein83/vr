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
            gridoptions: '='
        },
        templateUrl: function (element, attrs) {
            return GridViewService.dTemplate;
        },
        compile: function (tElement, attrs) {
            var tr = angular.element(document.getElementById('trbody'));
            var row = '';
            if (attrs.columndefs == undefined) return;

            angular.forEach(attrs.columndefs.split(','), function (item) {
                row = row + '<td>{{item.' + item + '}}</td>';
            });

            tr.append(row);
        }
    };

    return directiveDefinitionObject;

}]);






