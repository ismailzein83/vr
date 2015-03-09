'use strict';

app.service('BaseDirService', function () {

    this.directiveMainURL = "../../Client/Templates/Directives/";

});

app.service('TextBoxService', ['BaseDirService', function (BaseDirService) {

    this.dTemplate = BaseDirService.directiveMainURL + "vr-textbox-standard.html";

    this.getTemplateByType = function (type) {
        return BaseDirService.directiveMainURL + 'vr-textbox-' + type + '.html';
    };

    this.getTemplateUrl = function (type) {
        for (var index = 0; index < this.allDirective.length; ++index) {
            if (type.toLowerCase() == this.allDirective[index].name) return this.allDirective[index].dTemplate;
        }
        return this.getTemplateByType(type);
    };

    this.allDirective = [

    {
        name: "search",
        dPlaceholder: "Search ...",
        dIcon: "search",
        dTemplate: this.getTemplateByType('icon')
    },
    {
        name: "mail",
        dPlaceholder: "Mail ...",
        dIcon: "mail",
        dTemplate: this.getTemplateByType('icon')
    }
    ];


}]);


var defaultAttributes = function (attrs, obj) {
    if (attrs.type.toLowerCase() == obj.name) {
        if (attrs.icon == undefined) attrs.$set("icon", obj.dIcon);
        if (attrs.placeholder == undefined) attrs.$set("placeholder", obj.dPlaceholder);
        if (attrs.buttontext == undefined) attrs.$set("buttontext", obj.dButtonText);
    }
    return attrs;
};


app.directive('vrTextbox', ['TextBoxService', function (TextBoxService) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            type: '@',
            placeholder: '@',
            icon: '@',
            buttontext: '@',
            value: '=',
            btnClick: '&'
        },
        controller: function () {
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            TextBoxService.allDirective.forEach(function (item) { attrs = defaultAttributes(attrs, item); });
        },
        templateUrl: function (element, attrs) {
            return TextBoxService.getTemplateUrl(attrs.type);
        }

    };

    return directiveDefinitionObject;

}]);