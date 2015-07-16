'use strict';


app.directive('vrTreeview', [function () {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            onReady: '=',
            datasource:'=',
            datachildrenfield: '@',
            datavaluefield: '@',
            datatextfield: '@',
            selecteditem: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var treeElement = $element.find('#divTree');

            function fillTreeFromDataSource(treeArray, dataSource) {                
                for (var i = 0; i < dataSource.length; i++) {
                    var sourceItem = dataSource[i];
                    var treeItem = {
                        sourceItem: sourceItem,
                        id: sourceItem[ctrl.datavaluefield],
                        text: sourceItem[ctrl.datatextfield],
                        state: { },
                        children: []
                    };
                    if (sourceItem.isOpened)
                        treeItem.state.opened = true;
                    if (sourceItem.isSelected)
                        treeItem.state.selected = true;
                    if (sourceItem.isDisabled)
                        treeItem.state.disabled = true;
                    if (sourceItem[ctrl.datachildrenfield] != undefined)
                        fillTreeFromDataSource(treeItem.children, sourceItem[ctrl.datachildrenfield]);
                    treeArray.push(treeItem);
                }
            }

           

            var api = {};

            api.refreshTree = function () {
                var treeArray = [];
                fillTreeFromDataSource(treeArray, ctrl.datasource);
                treeElement.jstree({
                    core: {
                        data: treeArray
                    }
                });
                treeElement.on('changed.jstree', function (e, data) {
                   ctrl.selecteditem = data.node.original.sourceItem;
                });
            };
            if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                    $scope.$watch('ctrl.selecteditem', function () {
                        if (iAttrs.onvaluechanged != undefined) {
                            var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onvaluechanged);
                            if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                                onvaluechangedMethod();
                            }
                        }
                    });
                }
            }
        },
        template: function (element, attrs) {
            return '<div id="divTree" />';
        }

    };   

    return directiveDefinitionObject;

}]);