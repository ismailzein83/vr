'use strict';

var templatesG = {
    dTemplate: "../../Client/Templates/Directives/vr-gridview-standard.html",
    getTemplateByType: function (type) {
        return '../../Client/Templates/Views/vr-gridview-' + type + '.html'
    }
};


app.directive('vrGridview', function () {
    return {
        restrict: 'E',
        scope: {
            gridoptions: '=',
            columndefs: '@columnDefs'
        },
        templateUrl: function (element, attrs) {
            if (attrs.type == undefined) return templatesG.dTemplate;
            return templatesG.getTemplateByType(attrs.type);
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
});