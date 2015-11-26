'use strict';


app.directive('vrIcon', [function ($compile) {
    var option = {
        true:{value: "Client/Images/true.png",isimage:true},
        false:{value: "Client/Images/onebit_33.png",isimage:true} ,
        'Y': {value: "Client/Images/true.png",isimage:true},
        'N': {value: "Client/Images/onebit_33.png",isimage:true},
        'Enabled':{value: "Client/Images/true.png",isimage:true} ,
        'Disabled':{value: "Client/Images/onebit_33.png",isimage:true} ,
        '1': {value: "Client/Images/true.png",isimage:true},
        '0':{value: "Client/Images/onebit_33.png",isimage:true},
        'music': { value: "glyphicon-music", isimage: false }
    }
   
    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            icontype: '=',
            text:'='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.icon = "";
            ctrl.class = "";
            var value = ctrl.icontype;
            ctrl.icon = option[value] != undefined ? option[value] : { value: "Client/Images/true.png", isimage: true };
          


        },
        controllerAs: 'ctrl',
        bindToController: true,
        link: function preLink($scope, iElement, iAttrs) {
            var ctrl = $scope.ctrl;
            var template = getTemplate(ctrl,iAttrs)
           iElement.replaceWith(template);

        },


    };

    function getTemplate(ctrl, attr) {

        var text = "";
        if (ctrl.text != undefined)
            text = ctrl.text;

        var template = ''
        if (ctrl.icon.isimage)
            template += '<div style="text-align: left;"><img style="width:12px;height:12px" title="' + ctrl.icontype + '"  src="' + ctrl.icon.value + '"  /><span>' + text + '</span></div>'
        else
            template += '<div style="text-align: left;"><span class="glyphicon ' + ctrl.icon.value + '"  /></div>'

        return template;
    }
  

    return directiveDefinitionObject;

}]);