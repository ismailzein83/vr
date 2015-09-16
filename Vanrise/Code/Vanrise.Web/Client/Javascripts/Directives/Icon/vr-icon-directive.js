'use strict';


app.directive('vrIcon', [function ($compile) {
    
    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            value: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.icon = "";

            if (ctrl.value == true)
               ctrl.icon = "Client/Images/true.png";
            else if (ctrl.value == false)
                 ctrl.icon = "Client/Images/onebit_33.png";
  

        },
        controllerAs: 'ctrl',
        bindToController: true,
        link: function preLink($scope, iElement, iAttrs) {
            var ctrl = $scope.ctrl;
            var template = getTemplate(ctrl)
           iElement.replaceWith(template);

        },


    };

    function getTemplate(ctrl) {
        
        var template = ''
        template += '<div style="text-align: left;"><img style="width:16px;height:16px" title="' + ctrl.value + '"  src="' + ctrl.icon + '"  /></div>'
           
        return template;
    }
  

    return directiveDefinitionObject;

}]);