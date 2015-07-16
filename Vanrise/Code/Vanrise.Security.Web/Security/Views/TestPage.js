testpage1.$inject = ['$scope'];

function testpage1($scope) {
    var widgetAPI;
    defineScope();
    load();
    var i = 100;
    function defineScope() {
      
        $scope.values = [
        {
            id: "1",
            Name: "child 1",
            isOpened: true,
            children: [{
                id: "2",
                Name: "child 1"
            }
            ,
            {
                id: "3",
                Name: "child 1",
                isSelected: true
            }
            
            ]
        },
        {
            id: "4",
            Name: "child 2",
            isDisabled: true,
            children: []
        }];
        var id = 2;
        $scope.addNode = function () {
            $scope.values.push({
                id: ++id,
                Name: "child " + id,
                children: []
            });
        };
        var tree;
        $scope.refreshTree = function () {

            $('#test2').jstree();
            
        };
        $scope.Search = function () {

            $scope.data[0].state.selected = true;
            $scope.data[0].state.disabled = true;
        };
        $scope.treeValueChanged1 = function () {
            console.log($scope.selectedTreeNode1);
        };
        $scope.treeValueChanged2 = function () {
            console.log($scope.selectedTreeNode2);
        };
        $scope.add = function () {
            var obj = {
                "text": "Root",
                "state": { "opened": true },
                "children": [
                    {
                        "text": "Child node 1",
                        "state": { "selected": false },
                        "icon": "jstree-file"
                    }]
            }
             $scope.data.push(obj);
        }
        $scope.data =  [
				{
				    "id": 1,
                    "Name":"samer",
				    "text": "Root",
				    "state": { "opened": true },
				    "children": [
						{
						    "id": 2,
						    "text": "Child node 1",
						    "state": { "selected": false },
						    "icon": "jstree-file",
						},
						{
						    "id": 3,
						    "text": "Child node 2",
						    "state": { "disabled": true }
						},
                        {
                            "id": 4,
                            "text": "Child node 3",
                            "state": { "disabled": true }
                        },
                        {
                            "id": 5,
                            "text": "Child node 2",
                            "state": { "disabled": true, "opened": true },
                            "children": [
                                {
                                    "id": 6,
                                    "text": "Inner Child node 1",
                                    "state": { "selected": false },
                                    "icon": "jstree-file"
                                },
                                {
                                    "id": 7,
                                    "text": "Inner Child node 2",
                                    "state": { "selected": false, "opened": false},
                                    "children": [
                                                    {
                                                        "id": 10,
                                                        "text": "Inner Child node 1",
                                                        "state": { "selected": false },
                                                        "icon": "jstree-file"
                                                    },
                                                    {
                                                        "id": 11,
                                                        "text": "Inner Child node 2",
                                                        "state": { "selected": false },
                                                    }
                                                ]
                                },
                                {
                                    "id": 9,
                                    "text": "Inner Child node 3",
                                    "state": { "selected": false, "opened": false },
                                    "children": [
                                                    {
                                                        "id": 12,
                                                        "text": "Inner Child node 1",
                                                        "state": { "selected": false },
                                                        "icon": "jstree-file"
                                                    },
                                                    {
                                                        "id": 13,
                                                        "text": "Inner Child node 2",
                                                        "state": { "selected": false },
                                                        "children": [
                                                                     {
                                                                         "id": 14,
                                                                         "text": "Inner Child node 1",
                                                                         "state": { "selected": false },
                                                                         "icon": "jstree-file"
                                                                     },
                                                                    {
                                                                        "id": 15,
                                                                         "text": "Inner Child node 2",
                                                                         "state": { "selected": false },
                                                                    }
                                                                ]
                                                    }
                                    ]
                                }
                            ]
                        }

				    ]
				}
        ];

        $scope.treeReady = function (api) {
            api.refreshTree($scope.data);
        }
        $scope.treeReady2 = function (api) {
            api.refreshTree($scope.values);
        }
        $scope.typesConfig = {
            "default": {
                "icon": "http://jstree.com/tree.png"
            },
            "demo": {
                "icon": "glyphicon glyphicon-leaf"
            }
        }

        $scope.readyCB = function () {
            console.log('ready event call back');
        };
        $scope.changedCB = function (e, data) {
            var obj = {
                "id":i++,
                "text": "Root",
                "state": { "opened": true },
                "children": [
                    {
                        "text": "Child node 1",
                        "state": { "selected": false },
                        "icon": "jstree-file"
                    }]
            }
            console.log(data.node.original.Name);
            addNode($scope.data, 0, obj, data.node.id);
            console.log('changed event call back');

        };
        $scope.openNodeCB = function (e, data) {
            console.log('open-node event call back');
        };
    }
    function addNode(array, i, obj, nodeId) {
        if (i == array.length)
            return;
        else if (array[i].id == nodeId) {
            array[i].children.push(obj);
            return;
        }
        else {
            addNode(array, i + 1, obj, nodeId);
            if (array[i].children.length > 0) {
                addNode(array[i].children, 0, obj, nodeId);
            }
           
        }
    }

    function load() {
        $('#test').jstree();
        $scope.isGettingData = false;

    }
}
appControllers.controller('testpage1', testpage1);
