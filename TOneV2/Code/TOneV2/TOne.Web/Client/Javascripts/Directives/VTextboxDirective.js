'use strict';

var templates = {
    dTemplate: "../../Client/Templates/Directives/VTextboxStandard.html",
    templateIcon: "../../Client/Templates/Directives/VTextboxIcon.html",
    getTemplateByType: function (type) {
        return '../../Client/Templates/Directives/VTextbox' + type + '.html'
    }
};

var allDir = [
        {
            name: "search",
            dPlaceholder: "Search ...",
            dIcon: "search",
            dTemplateURL: templates.templateIcon
        }
        ,
        {
            name: "mail",
            dPlaceholder: "Mail ...",
            dIcon: "mail",
            dTemplateURL: templates.dTemplate
        }
];


var overrideAttributes = function (attrs, obj) {
    if (attrs.type.toLowerCase() == obj.name) {
        if (attrs.icon == undefined) attrs.$set("icon", obj.dIcon);
        if (attrs.placeholder == undefined) attrs.$set("placeholder", obj.dPlaceholder);
    }
    return attrs;
};

app.directive('vTextbox', function () {
    return {
        restrict: 'E',
        scope: {
            holder: '@placeholder',
            icon: '@icon',
            text:'=text'
        },
        compile: function (element, attrs) {
            allDir.forEach(function (item) {
                attrs = overrideAttributes(attrs, item);
            });
        },
        templateUrl: function (element, attrs) {
            for (var index = 0; index < allDir.length; ++index) {
                if (attrs.type.toLowerCase() == allDir[index].name) return allDir[index].dTemplateURL;
            }
            return templates.getTemplateByType(attrs.type);
        }
    };
});