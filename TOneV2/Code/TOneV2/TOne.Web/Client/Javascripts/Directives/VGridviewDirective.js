'use strict';

var templatesG = {
    dTemplate: "../../Client/Templates/Directives/VGridviewStandard.html",
    dTemplateGridColumn: "../../Client/Templates/Directives/VGridRowTemplate.html",
    getTemplateByType: function (type) {
        return '../../Client/Templates/Directives/VGridview' + type + '.html'
    }
};


app.directive('vGridview', function () {
    return {
        restrict: 'E',
        transclude: true,
        scope: {
            source: '=datasource',
            header:'=header'
        },
        templateUrl: function (element, attrs) {
            if (attrs.type == undefined) return templatesG.dTemplate;
            return templatesG.getTemplateByType(attrs.type);
        }
    };
});

app.directive('VGridRowTemplate', function () {
    return {
        require: '^vGridview',
        restrict: 'E',
        transclude: true,
        scope: {
            values: '=values'
        },
        templateUrl: function (element, attrs) {
            if (attrs.type == undefined) return templatesG.dTemplate;
            return templatesG.getTemplateByType(attrs.type);
        }
    };
});