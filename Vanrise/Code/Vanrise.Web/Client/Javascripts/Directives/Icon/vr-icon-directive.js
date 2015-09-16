'use strict';


app.directive('vrIcon', [function ($compile) {
    var option = {
        true: "Client/Images/true.png",
        false: "Client/Images/onebit_33.png",
        'Y': "Client/Images/true.png",
        'N': "Client/Images/onebit_33.png",
        '1': "Client/Images/true.png",
        '0': "Client/Images/onebit_33.png"
    }
    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            icontype: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.icon = "";
            var value = ctrl.icontype;      
            ctrl.icon =  option[value] != undefined ? option[value]:"Client/Images/true.png";

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
        template += '<div style="text-align: left;"><img style="width:16px;height:16px" title="' + ctrl.icontype + '"  src="' + ctrl.icon + '"  /></div>'
           
        return template;
    }
  

    return directiveDefinitionObject;

}]);