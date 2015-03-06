'use strict';

var templatesG = {
    dTemplate: "../../Client/Templates/Directives/VGridviewStandard.html",
    getTemplateByType: function (type) {
        return '../../Client/Templates/Directives/VGridview' + type + '.html'
    }
};


app.directive('vGridview', function () {
    return {
        restrict: 'E',
        scope: {
            source: '=datasource',
            header: '=header',
            uniquecname: '@uniquecname'
        },
        templateUrl: function (element, attrs) {
            if (attrs.type == undefined) return templatesG.dTemplate;
            return templatesG.getTemplateByType(attrs.type);
        },
        compile: function (tElement, attrs) {
            var tr = angular.element(document.getElementById('trbody'));
            var row = '';

            angular.forEach(attrs.uniquecname.split(','), function (item) {
                row = row + '<td>{{n.' + item + '}}</td>';
                console.log(row);
            });
            tr.append(row);
        }
    };
});