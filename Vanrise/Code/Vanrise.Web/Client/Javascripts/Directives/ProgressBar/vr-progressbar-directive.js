'use strict';


app.directive('vrProgressbar', [function ($compile) {
    
    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            value: '=',
            gridvalue: '@',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            if (ctrl.gridvalue!=undefined)
                ctrl.values = ctrl.gridvalue.split("|");
            else
                ctrl.values = ctrl.value;
            ctrl.length = ctrl.values.length;
        },
        controllerAs: 'ctrl',
        bindToController: true,
        link: function preLink($scope, iElement, iAttrs) {
            var ctrl = $scope.ctrl;
            var template = getTemplate(ctrl);
           iElement.replaceWith(template);

        },


    };

    function getTemplate(ctrl) {
        var values;
        var gridClass = "";
        var gridCellTextClass = "";
        if (ctrl.gridvalue != undefined) {
            values = ctrl.gridvalue.split("|");
            gridClass = "vr-datagrid-cell";
            gridCellTextClass = "vr-datagrid-celltext";
        }
        else {
            values = ctrl.value;
        }

        if (Object.prototype.toString.call(values) ==='[object Number]') {
            return '<div class="' + gridClass + ' progress" style="margin-bottom:0px;padding:0px;">'
                     + '<div class="' + gridCellTextClass + ' progress-bar progress-bar-gray active" style="width:'+values+'%" title="'+values+'%">'
                     + values + "%"
                     +'</div>'
                  +'</div>';
        }
        else if (values.length>0) {
            var template = '<div class="' + gridClass + ' progress" style="margin-bottom:0px;padding:0px;">';
            for (var i = 0; i < values.length; i++) {
                template += '<div  class="' + gridCellTextClass + ' progress-bar ' + getProgressColor(i) + ' active" style="width:' + values[i] + '%" title="' + values[i] + '%"></div>';
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