'use strict';


app.directive('vrIcon', ['$compile', function ($compile) {
    var option = {
        true: { value: "Client/Images/true.png", isimage: true, tooltip: 'True' },
        false: { value: "Client/Images/onebit_33.png", isimage: true, tooltip: 'False' },
        'Y': { value: "Client/Images/true.png", isimage: true, tooltip: 'Y' },
        'N': { value: "Client/Images/onebit_33.png", isimage: true, tooltip: 'N' },
        'Enabled': { value: "Client/Images/true.png", isimage: true, tooltip: 'Enabled' },
        'Disabled': { value: "Client/Images/onebit_33.png", isimage: true, tooltip: 'Disabled' },
        '1': { value: "Client/Images/true.png", isimage: true, tooltip: '1' },
        '0': { value: "Client/Images/onebit_33.png", isimage: true, tooltip: '0' },
        '2': { value: "Client/Images/banned.png", isimage: true, tooltip: '2' },//#ff1111        
        'music': { value: "glyphicon-music", isimage: false, tooltip: 'Music' },
        'above': { value: "glyphicon-arrow-up arrow-above", isimage: false, tooltip: 'Above' },//#37c737
        'below': { value: "glyphicon-arrow-down arrow-below", isimage: false, tooltip: 'Below' },//#ff1111
        'increase': { value: "glyphicon-arrow-up arrow-below", isimage: false, tooltip: 'Increase' },
        'decrease': { value: "glyphicon-arrow-down arrow-above", isimage: false, tooltip: 'Decrease' }, //#37c737
        'explicit': { value: 'Client/Images/explicit.png', isimage: true, tooltip: 'Explicit', width: 15, height: 15 },
        'inherited': { value: 'Client/Images/inherited.png', isimage: true, tooltip: 'Inherited' }
    };

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            icontype: '=',
            text: '=',
            iconurl: '=',
            tooltip: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.icon = "";
            ctrl.class = "";
            if (ctrl.icontype != undefined) {
                var value = ctrl.icontype;
                ctrl.icon = option[value] != undefined ? option[value] : { value: "Client/Images/true.png", isimage: true, tooltip: 'True' };
            }
            else if (ctrl.iconurl != undefined) {
                ctrl.icon = { value: ctrl.iconurl, isimage: true };
            }


        },
        controllerAs: 'ctrl',
        bindToController: true,
        link: function preLink($scope, iElement, iAttrs) {
            var ctrl = $scope.ctrl;
            var template = getTemplate(ctrl, iAttrs);
            iElement.replaceWith(template);

        },


    };

    function getTemplate(ctrl, attr) {

        var text = "";
        if (ctrl.text != undefined)
            text = ctrl.text;
        var containerstyle = "";
        if (attr.inline != undefined)
            containerstyle = "display:inline-block;";
        var tooltip = ctrl.tooltip != undefined ? ctrl.tooltip : ctrl.icon.tooltip;

        var template = '';

        var widthAsInt = ctrl.icon.width || 20;
        var heightAsInt = ctrl.icon.height || 20;

        var width = widthAsInt + 'px';
        var height = heightAsInt + 'px';

        var style = 'text-align: left;';
        if (attr.centered != undefined)
            style += "position:relative; left:calc(50% - " + widthAsInt / 2 + "px);";

        if (ctrl.icon.isimage) {
            template += '<div style="' + style + containerstyle + '"><img style="width:' + width + ';height:' + height + '" title="' + tooltip + '"  src="' + ctrl.icon.value + '"  /><span>' + text + '</span></div>';
        }
        else
            template += '<div style="' + style + containerstyle + '"><span class="glyphicon ' + ctrl.icon.value + '" title="' + tooltip + '" /></div>';

        return template;
    }


    return directiveDefinitionObject;

}]);
