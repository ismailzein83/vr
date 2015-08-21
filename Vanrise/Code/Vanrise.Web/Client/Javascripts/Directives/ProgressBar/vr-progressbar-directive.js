'use strict';


app.directive('vrProgressbar', [function () {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            value: '=',
            gridvalue:'@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            if (ctrl.gridvalue!=undefined)
                ctrl.values = ctrl.gridvalue.split("|");
            else
                ctrl.values = ctrl.value;
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                }
            }
        },
        template: function (element, attrs) {
 
            return getTemplate(attrs);
        }

    };

    function getTemplate(attrs) {
        var values;
        var gridClass = "";
        var gridCellTextClass = "";
        if (attrs.gridvalue != undefined){
            values = attrs.gridvalue.split("|");
            gridClass = "vr-datagrid-cell";
            gridCellTextClass = "vr-datagrid-celltext";
        }
        else {
            values = attrs.value;
        }
           
        if (values.length == 0) {
            return '<div class="' + gridClass + ' progress" style="margin-bottom:0px;padding:0px;">'
                     + '<div class="' + gridCellTextClass + ' progress-bar progress-bar-gray active" style="width:{{ctrl.value}}%" title="{{ctrl.value}}%">'
                     +" {{ctrl.value}}%"
                     +'</div>'
                  +'</div>';
        }
        else {
            var template = '<div class="' + gridClass + ' progress" style="margin-bottom:0px;padding:0px;">'
            for (var i = 0; i < values.length; i++) {
                template += '<div class="' + gridCellTextClass + ' progress-bar ' + getProgressColor(i) + ' active" style="width:{{ctrl.values[' + i + ']}}%" title="{{ctrl.values[' + i + ']}}%"></div>'
            }
            template += "</div>";
            return template;
        }
    }
    function getProgressColor( index) {
        switch (index) {
            case 0: return "progress-bar-gray";
            case 1: return "progress-bar-success";
            case 2: return "progress-bar-warning";
        }
    }

    return directiveDefinitionObject;

}]);