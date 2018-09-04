'use strict';


app.directive('vrTreeview', ['UtilsService', function (UtilsService) {

	var directiveDefinitionObject = {

		restrict: 'E',
		scope: {
			onReady: '=',
			// datasource:'=',
			datachildrenfield: '@',
			datavaluefield: '@',
			datavaluefieldmethod: '=',
			datatextfield: '@',
			selecteditem: '=',
			checkbox: '@',
			wholerow: '@',
			draggabletree: '@',
			state: '@',
			movesettings: '@',
			hasremotechildrenfield: '@',
			loadremotechildren: '=',
			maxlevel: '@',
			onmoveitem: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			ctrl.treeHeight = $element.parents(".modal-dialog").length > 0 ? innerHeight * 0.50 : (innerHeight - 210) + 'px';
			var treeElement = $element.find('#divTree');
			$scope.$on("$destroy", function () {
				$(treeElement).off();
			});
			var incrementalId = 0;
			function fillTreeFromDataSource(treeArray, dataSource) {
				for (var i = 0; i < dataSource.length; i++) {
					var sourceItem = dataSource[i];
					var treeItem = createTreeItemFromSource(sourceItem);
					treeArray.push(treeItem);
				}
			}

			function createTreeItemFromSource(sourceItem) {

				var treeItem = {
					sourceItem: sourceItem,
					text: sourceItem[ctrl.datatextfield],
					state: {},
					children: []
				};
				treeItem.id = getSourceItemId(sourceItem);

				if (treeItem.id == undefined)
					treeItem.id = "generatedId_" + incrementalId++;
				if (sourceItem.isOpened)
					treeItem.state.opened = true;
				if (sourceItem.isSelected)
					treeItem.state.selected = true;
				if (sourceItem.isDisabled)
					treeItem.state.disabled = true;
				if (sourceItem.icon != null)
					treeItem.icon = sourceItem.icon;

				if (sourceItem[ctrl.datachildrenfield] != undefined && sourceItem[ctrl.datachildrenfield].length > 0)
					fillTreeFromDataSource(treeItem.children, sourceItem[ctrl.datachildrenfield]);
				else if (sourceItem[ctrl.hasremotechildrenfield] == true) {
					treeItem.children = true;
					treeItem.state.opened = false;
				}
				if (sourceItem.isLeaf)
					treeItem.icon = "glyphicon glyphicon-file";
				return treeItem;
			}

			function getSourceItemId(sourceItem) {
				if (ctrl.datavaluefieldmethod != undefined && typeof (ctrl.datavaluefieldmethod) == 'function')
					return ctrl.datavaluefieldmethod(sourceItem);
				else
					return sourceItem[ctrl.datavaluefield];
			}


			var api = {};
			api.setSelectedNode = function (menuList, nodeId) {
				return setSelectedNode(menuList, nodeId);
			};
			function setSelectedNode(menuList, nodeId) {
				for (var i = 0; i < menuList.length; i++) {

					if (getSourceItemId(menuList[i]) == nodeId) {
						menuList[i].isSelected = true;
						menuList[i].isOpened = true;
						return menuList[i];
					}
					else if (menuList[i][ctrl.datachildrenfield] != undefined) {
						var node = setSelectedNode(menuList[i][ctrl.datachildrenfield], nodeId);
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
				var treeData = {
					core: {

						'check_callback': true,

						data: function (obj, callback) {

							if (obj.id == '#')//root node
								callback.call(this, treeArray);
							else {
								if (ctrl.loadremotechildren != undefined) {

									ctrl.loadremotechildren(obj.original.sourceItem)
										.then(function (nodeChildrenSource) {
											if (nodeChildrenSource != undefined && nodeChildrenSource != null) {
												var nodeChildren = [];
												fillTreeFromDataSource(nodeChildren, nodeChildrenSource);
												callback.call(this, nodeChildren);
											}

										});
								}
							}

						}
					},

					state: { "key": "state_demo" },

				};
				var plugins = [];
				if (ctrl.checkbox !== undefined) {
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
						if (parent.id == "#") {
							return false;
						}
						if (ctrl.onmoveitem != undefined && typeof (ctrl.onmoveitem) == 'function') {
							if (parent.original != undefined) {
								return ctrl.onmoveitem(node.original.sourceItem, parent.original.sourceItem);
							}


						}

					}



				}
				if (ctrl.state != undefined) {
					plugins.push("state");

				}
				if (ctrl.wholerow !== undefined) {
					plugins.push("wholerow");
				}
				treeData.plugins = plugins;
				treeData.check_callback = true;
				treeElement.jstree(treeData).on('hover_node.jstree', function (e, data) {
					$("#" + data.node.id).attr('title', data.node.text);
				});
				treeElement.bind("move_node.jstree", function (e, data) {

					var jsonTree = treeElement.jstree(true).get_json('#', {});
					var returnedTree = [];
					getFullTreeData(returnedTree, jsonTree, treeElement);
					api.getTree = function () {
						return returnedTree;
					};
				});

				treeElement.on('changed.jstree', function (e, data) {
					if (data.node != undefined) {
						ctrl.selecteditem = data.node.original.sourceItem;
						$scope.$apply();

					}

				});
				treeElement.on('set_state.jstree', function (e, data) {
					api.scrollToSelectedNode();
				});

			};

			api.renameNode = function (newName) {
				var node = treeElement.jstree('get_selected');
				treeElement.jstree(true).rename_node(node, newName);
			};

			api.changeNodeIcon = function (icon) {
				var node = treeElement.jstree('get_selected');
				treeElement.jstree(true).set_icon(node, icon);
			};

			api.createNode = function (node) {
				var treeItem = createTreeItemFromSource(node);
				var parentNode = treeElement.jstree('get_selected');
				if (parentNode != undefined) {
					var nodeId = treeElement.jstree(true).create_node(parentNode, treeItem, "last", null, true);
				}
			};

			api.scrollToSelectedNode = function () {
				var node = treeElement.jstree('get_selected');

				if (node != undefined && node.length > 0) {
					document.getElementById(node[0]).scrollIntoView();
				}
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
			return {
				pre: function ($scope, iElem, iAttrs, ctrl) {
					$scope.$on("$destroy", function () {
						selecteditemWatch();
					});
					var selecteditemWatch = $scope.$watch('ctrl.selecteditem', function () {
						if (iAttrs.onvaluechanged != undefined) {

							var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onvaluechanged);
							if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
								onvaluechangedMethod();

							}
						}
					});


				}
			};
		},
		template: function (element, attrs) {
			return '<div ng-style="::{\'height\':ctrl.treeHeight,\'overflow-y\':\'auto\'}"><div id="divTree" /></div>';
		}

	};

	return directiveDefinitionObject;

}]);