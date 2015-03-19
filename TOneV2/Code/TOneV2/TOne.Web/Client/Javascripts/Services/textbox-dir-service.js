'use strict';

app.service('TextBoxService', ['BaseDirService', function (BaseDirService) {

    return ({
        dTemplate: BaseDirService.directiveMainURL + "vr-textbox-standard.html",
        getTemplateByType: getTemplateByType,
        getTemplateUrl: getTemplateUrl,
        setDefaultAttributes: setDefaultAttributes,
        allDirective: [
                        {
                            name: "search",
                            dLabel: "Search",
                            dPlaceholder: "Search ...",
                            dIcon: "search",
                            dTemplate: getTemplateByType('icon')
                        },
                        {
                            name: "mail",
                            dLabel: "Email",
                            dPlaceholder: "Mail ...",
                            dIcon: "mail",
                            dTemplate: getTemplateByType('icon')
                        }
                            ]
                        });

    function getTemplateByType(type) {
        return BaseDirService.directiveMainURL + 'vr-textbox-' + type + '.html';
    }

    function getTemplateUrl(type) {
        for (var index = 0; index < this.allDirective.length; ++index) {
            if (type.toLowerCase() == this.allDirective[index].name) return this.allDirective[index].dTemplate;
        }
        return this.getTemplateByType(type);
    }

    function setDefaultAttributes(attrs, obj) {
        if (attrs.type.toLowerCase() == obj.name) {
            if (attrs.icon == undefined) attrs.$set("icon", obj.dIcon);
            if (attrs.placeholder == undefined) attrs.$set("placeholder", obj.dPlaceholder);
            if (attrs.label == undefined) attrs.$set("label", obj.dLabel);
            if (attrs.buttontext == undefined) attrs.$set("buttontext", obj.dButtonText);
        }
        return attrs;
    }

}]);