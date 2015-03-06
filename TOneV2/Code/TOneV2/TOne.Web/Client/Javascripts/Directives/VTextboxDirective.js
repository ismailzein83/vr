'use strict';

var templatesTxt = {
    dTemplate: "../../Client/Templates/Directives/VTextboxStandard.html",
    templateIcon: "../../Client/Templates/Directives/VTextboxIcon.html",
    templateButton: "../../Client/Templates/Directives/VTextboxButton.html",
    getTemplateByType: function (type) {
        return '../../Client/Templates/Directives/VTextbox' + type + '.html'
    }
};

var allDir = [
{
    name: "search",
    dPlaceholder: "Search ...",
    dIcon: "search",
    dTemplateURL: templatesTxt.templateIcon
}
,
{
    name: "mail",
    dPlaceholder: "Mail ...",
    dIcon: "mail",
        dTemplateURL: templatesTxt.templateIcon
}

];

var defaultAttributes = function (attrs, obj) {
    if (attrs.type.toLowerCase() == obj.name) {
        if (attrs.icon == undefined) attrs.$set("icon", obj.dIcon);
        if (attrs.placeholder == undefined) attrs.$set("placeholder", obj.dPlaceholder);
        if (attrs.buttontext == undefined) attrs.$set("buttontext", obj.dButtonText);
    }
    return attrs;
};

app.directive('vTextbox', function () {
    return {
        restrict: 'E',
        scope: {
            holder: '@placeholder',
            icon: '@icon',
            btnText: '@buttontext',
            text: '=text',
            btnclick: '&btnClick'
        },
        compile: function (element, attrs) {
            allDir.forEach(function (item) {
                attrs = defaultAttributes(attrs, item);
            });
        },
        templateUrl: function (element, attrs) {
            for (var index = 0; index < allDir.length; ++index) {
                if (attrs.type.toLowerCase() == allDir[index].name) return allDir[index].dTemplateURL;
            }
            return templatesTxt.getTemplateByType(attrs.type);
        }
    };
});