'use strict';


app.directive('vrTreeview', [function () {

    var directiveDefinitionObject = {

        restrict: 'E', 
        scope: {
            onReady: '=',
           // datasource:'=',
            datachildrenfield: '@',
            datavaluefield: '@',
            datatextfield: '@',
            selecteditem: '=',
            checkbox: '@',
            wholerow: '@',
            draggabletree: '@',
            state: '@',
            movesettings: '@'
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
            api.setSelectedNode = function (menuList, nodeId) {
                return setSelectedNode(menuList, nodeId);
            };
            function setSelectedNode(menuList, nodeId) {
                for (var i = 0; i < menuList.length; i++) {
                   
                    if (menuList[i][ctrl.datavaluefield] == nodeId) {
                        menuList[i].isSelected = true;
                        menuList[i].isOpened = true;
                        return menuList[i];
                    }
                    else if (menuList[i][ctrl.datachildrenfield] != undefined) {
                        var node= setSelectedNode(menuList[i][ctrl.datachildrenfield], nodeId);
                        if (node != null) {
                            menuList[i].isOpened = true;
                            return node;
                        }
                    }
                      
                        
                }
            }
            api.refreshTree = function (datasource) {
                
                treeElement.jstree("destroy");
                treeElement = $element.find('#divTree');
                var treeArray = [];
                fillTreeFromDataSource(treeArray, datasource);
                var treeData={
                    core: { data: treeArray},
                    "state": { "key": "state_demo" },
                   
                }
                var plugins = [];
                if (ctrl.checkbox !== undefined)
                {
                     plugins.push("checkbox");
                  
                }
                plugins.push("json_data");
                if (ctrl.draggabletree != undefined) {
                    plugins.push("dnd");
                    treeData.core.check_callback = function (operation, node, parent, position, more) {
                        
                        if (ctrl.movesettings != undefined) {
                          
                            if (ctrl.movesettings == 'samelevel') {
                                
                                if (operation === "copy_node" || operation === "move_node") {
                                    if (parent.id != node.parent) {
                                        return false;
                                    } else
                                        return true;
                                }
                            }
                            else if (ctrl.movesettings == 'alllevels')
                                return true;
                        }

                        
                    };
                   
                }
                if (ctrl.state != undefined) {
                    plugins.push("state");
                    
                }
                if (ctrl.wholerow !== undefined) {
                    plugins.push("wholerow");
                }
                treeData.plugins = plugins;
                treeData.check_callback = true;
                treeElement.jstree(treeData);
                console.log(treeElement);
                treeElement.bind("move_node.jstree", function (e, data) {
                    var jsonTree = treeElement.jstree(true).get_json('#', {});
                    var returnedTree=[];
                    getFullTreeData(returnedTree, jsonTree, treeElement);
                    api.getTree = function () {
                        return returnedTree;
                    }
                });

                treeElement.on('changed.jstree', function (e, data) {
                   ctrl.selecteditem = data.node.original.sourceItem;
                });
            };
            function getFullTreeData(treeArray, jsonTree, treeElement) {
               
                for (var i = 0; i < jsonTree.length; i++) {
                    var sourceItem = jsonTree[i]; 
                    var treeItem = treeElement.jstree(true).get_node(sourceItem.id).original.sourceItem;
                    treeItem[ctrl.datachildrenfield] = [];
                    
                    if (treeItem[ctrl.datachildrenfield] != undefined)
                        getFullTreeData(treeItem[ctrl.datachildrenfield], sourceItem.children, treeElement);
                    treeArray.push(treeItem);
                    
                }

            }
           
            if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            console.log(element);
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