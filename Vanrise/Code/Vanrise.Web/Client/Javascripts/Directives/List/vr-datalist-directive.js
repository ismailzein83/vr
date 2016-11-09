'use strict';

app.directive('vrDatalist', [function () {

    var directiveDefinitionObject = {
        //transclude: true,
        restrict: 'E',
        scope: {
            maxitemsperrow: '@',
            datasource: '=',
            onremoveitem: '&',
            autoremoveitem: '=',
            onitemclicked: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            if (ctrl.datasource == undefined)
                ctrl.datasource = {};

            ctrl.itemsSortable = { handle: '.handeldrag', animation: 150 };

            ctrl.onInternalRemove = function (dataItem) {
                if (ctrl.autoremoveitem == true) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                }
                else {
                    var removeFunction = $scope.$parent.$eval(ctrl.onremoveitem);
                    removeFunction(dataItem);
                }
            }
            $scope.ondataitemclicked = function (dataItem) {
                if (typeof (ctrl.onitemclicked) == 'function')
                    ctrl.onitemclicked(dataItem);
            }
        },
        controllerAs: 'VRDatalistCtrl',
        bindToController: true,
        compile: function (element, attrs) {

            return {
                pre: function (scope, elem, attrs, ctrl) {
                    scope.isDataListScope = true;
                    scope.viewScope = scope.$parent;
                    while (scope.viewScope.isDataListScope) {
                        scope.viewScope = scope.viewScope.$parent;
                    }
                }
            }
        },
        template: function (element, attrs) {

            var draggableIconTemplate = '';
            var contentWidth = 0;
            if (attrs.isitemdraggable != undefined) {
                draggableIconTemplate = '<div style="width: 14px; display:inline-block;height:25px">'
                                            + '<i class="glyphicon glyphicon-th-list handeldrag hand-cursor" style="top: calc(50% - 10px); left: -6px"></i>'
                                        + '</div>';
                contentWidth += 14;
            }

            var onRemoveAttr = '';
            if (attrs.autoremoveitem != undefined || attrs.onremoveitem != undefined) {
                onRemoveAttr = 'onremove="VRDatalistCtrl.onInternalRemove(dataItem)"';
                contentWidth += 14;
            }
               

            var onItemClickedAttr = '';
            if (attrs.onitemclicked != undefined) {
                onItemClickedAttr = 'ng-click="ondataitemclicked(dataItem)"';
            }

            var template = '<vr-list maxitemsperrow="{{VRDatalistCtrl.maxitemsperrow}}">'
                            + '<div ng-sortable="VRDatalistCtrl.itemsSortable">'
                             + '<vr-listitem ng-repeat="dataItem in VRDatalistCtrl.datasource" ' + onRemoveAttr + '>'
                             + draggableIconTemplate
                             + '<div ' + onItemClickedAttr + ' style="width: calc ( 100% - ' + contentWidth + 'px); display:inline-block;text-overflow: ellipsis; overflow: hidden;">' + element.html() + '</div>'
                             + '</vr-listitem>'
                            + '</div>'
                           + '</vr-list>';
            return template;
        }


    };

    return directiveDefinitionObject;



}]);

