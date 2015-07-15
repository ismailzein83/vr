
(function (angular) {


    angular.module('vrTreeview', []).directive('treeList', ['$compile', function ($compile) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                //tree id
                var treeClass = attrs.treeClass;

                //tree model
                var treeId = attrs.treeId;
                var treeList = attrs.treeList;

                //node id
                var nodeDisabled = attrs.nodeDisabled || 'true';

                //node label
                var nodeLabel = attrs.nodeLabel || 'label';

                //children
                var nodeChildren = attrs.nodeChildren || 'children';

                //tree template
                var template =

                   '<li data-jstree='+'{"opened":false,"selected":true}'+'>'
                      + '<ul ng-repeat="node in ' + treeList + '">'
                            + '<li data-jstree=' + '{"disabled":node.' + nodeDisabled + ',"icon":"glyphicon glyphicon-leaf"}' + '>node.' + nodeChildren + '</li>'
                        + '</ul>'
                    + '</li>'
                //check tree id, tree model
                if (treeId && treeModel) {

                    //root node
                    if (attrs.angularTreeview) {

                        //create tree object if not exists
                        scope[treeId] = scope[treeId] || {};

                        //if node head clicks,
                        scope[treeId].selectNodeHead = scope[treeId].selectNodeHead || function (selectedNode) {

                            //Collapse or Expand
                            selectedNode.collapsed = !selectedNode.collapsed;
                        };

                        //if node label clicks,
                        scope[treeId].selectNodeLabel = scope[treeId].selectNodeLabel || function (selectedNode) {

                            //remove highlight from previous node
                            if (scope[treeId].currentNode && scope[treeId].currentNode.selected) {
                                scope[treeId].currentNode.selected = undefined;
                            }

                            //set highlight to selected node
                            selectedNode.selected = 'selected';

                            //set currentNode
                            scope[treeId].currentNode = selectedNode;
                        };
                    }

                    //Rendering template.
                    element.html('').append($compile(template)(scope));
                }
            }
        };
    }]);
})(angular);
