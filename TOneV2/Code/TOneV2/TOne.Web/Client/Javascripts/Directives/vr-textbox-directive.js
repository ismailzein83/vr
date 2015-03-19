'use strict';


app.directive('vrTextbox', ['TextBoxService', function (TextBoxService) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            placeholder: '@',
            label:'@',
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
            TextBoxService.allDirective.forEach(function (item) { attrs = TextBoxService.setDefaultAttributes(attrs, item); });
        },
        templateUrl: function (element, attrs) {
            return TextBoxService.getTemplateUrl(attrs.type);
        }

    };

    return directiveDefinitionObject;

}]);