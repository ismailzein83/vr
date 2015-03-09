'use strict';

app.service('TemplatesService', function () { 

    this.directiveMainURL = "../../Client/Templates/Directives/";

    });

app.service('TextBoxService', ['TemplatesService', function (TemplatesService) {

    this.dTemplate =TemplatesService.directiveMainURL + "vr-textbox-standard.html";

    this.getTemplatesByType = function (type) {
        return TemplatesService.directiveMainURL + 'vr-textbox-' + type + '.html';
    };

    this.allDirective =[

        {
            name: "search",
            dPlaceholder: "Search ...",
            dIcon: "search",
            dTemplate: this.getTemplatesByType('icon')
        },
        {
            name: "mail",
            dPlaceholder: "Mail ...",
            dIcon: "mail",
            dTemplate: this.getTemplatesByType('icon')
        }
    ];

}]);

app.directive('vrTextbox', ['TextBoxService', function (TextBoxService) {
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
                if(attrs.type.toLowerCase() == TextBoxService.allDirective[index].name) return TextBoxService.allDirective[index].dTemplate;
            }
            return TextBoxService.getTemplatesByType(attrs.type);
        }
    };
}]);