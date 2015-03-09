'use strict';

app.service('TemplatesService', function () { 

    this.directiveMainURL = "../../Client/Templates/Directives/";

    });

app.service('TextBoxService', ['TemplatesService', function (TemplatesService) {

    this.templates = {
        dTemplate: TemplatesService.directiveMainURL + "VTextboxStandard.html",
        templateIcon: TemplatesService.directiveMainURL + "VTextboxIcon.html",
        templateButton: TemplatesService.directiveMainURL + "VTextboxButton.html",
    };

    this.getTemplatesByType = function (type) {

        return TemplatesService.directiveMainURL + 'VTextbox' + type + '.html';

    };

    this.allDirective =[

        {
            name: "search",
            dPlaceholder: "Search ...",
            dIcon: "search",
            dTemplateURL: this.templates.templateIcon
        },
        {
            name: "mail",
            dPlaceholder: "Mail ...",
            dIcon: "mail",
            dTemplateURL: this.templates.templateIcon
        }
    ];

}]);

app.directive('vTextbox', ['TextBoxService', function (TextBoxService) {
    return {
        restrict: 'E',
        scope: {
            type:'@',
            placeholder: '@',
            icon: '@',
            buttontext: '@',
            value: '=',
            btnClick: '&'
        },
        controller: function () {
        },
        controllerAs: 'ctrl',
        bindToController : true,
        templateUrl: function (element, attrs) {
            for (var index = 0; index < TextBoxService.allDirective.length; ++index) {
                if(attrs.type.toLowerCase() == TextBoxService.allDirective[index].name) return TextBoxService.allDirective[index].dTemplateURL;
            }
            return TextBoxService.getTemplatesByType(attrs.type);
        }
    };
}]);