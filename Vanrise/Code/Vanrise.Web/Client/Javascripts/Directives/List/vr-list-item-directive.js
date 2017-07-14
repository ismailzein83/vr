'use strict';


app.directive('vrListitem', ['UtilsService', function (UtilsService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        scope: {
            onremove: '&'
        },
        require: '^^vrList',
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.readOnly = UtilsService.isContextReadOnly($scope) || $attrs.readonly != undefined;
        },
        controllerAs: 'listItemCtrl',
        bindToController: true,
        link: function ($scope, elem, iAttrs, listCtrl) {

            var ctrl = $scope.listItemCtrl;
            var maxItemsPerRow = listCtrl.maxitemsperrow != undefined ? parseInt(listCtrl.maxitemsperrow) : 1;
            ctrl.hideremoveicon = listCtrl.hideremoveicon;
            var colNumber;
            switch (maxItemsPerRow) {
                case 1: colNumber = 12; break;
                case 2: colNumber = 6; break;
                case 3: colNumber = 4; break;
                case 4: colNumber = 3; break;
                case 5: colNumber = 3; break;
                case 6: colNumber = 2; break;
            }
            if (colNumber == undefined) {
                if (maxItemsPerRow < 12)
                    colNumber = 2;
                else colNumber = 1;
            }
            ctrl.colNumber = colNumber;
        },
        template: function (elem, attrs) {


            var itemContentWidth = '100%';
            var removeItemTemplate = '';
            if (attrs.onremove != undefined) {
                removeItemTemplate = '  <i  ng-if="!ctrl.readOnly && !listItemCtrl.hideremoveicon" class="glyphicon glyphicon-remove hand-cursor" style="width:10px;position:absolute;top:10px;" ng-click="listItemCtrl.onremove()"></i>';
                itemContentWidth = 'calc(100% - 14px)';
            }

            var template = '<vr-columns colnum="{{listItemCtrl.colNumber}}" datalistitem> <div class="list-group-item list-custom" >'
                                 + '<div style="width:100%">'
                                 + '    <div style="display: inline-block;  width: ' + itemContentWidth + ';" ng-transclude></div>'
                                   + removeItemTemplate
                                 + '</div>'
                             + '</div></vr-columns>';
            return template;
        }
    };

    return directiveDefinitionObject;



}]);

