"use strict";

app.directive("cdranalysisFaStrategyCriteriaDecisiontree", ["StrategyAPIService", "CDRAnalysis_FA_PeriodEnum", "CDRAnalysis_FA_SuspicionLevelEnum", "UtilsService", "Fzero_FraudAnalysis_DecisionTreeService", "CDRAnalysis_FA_DecisionTreeConditionOperatorEnum", "CDRAnalysis_FA_DecisionTreeNodeTypeEnum", "VRNotificationService", function (StrategyAPIService, CDRAnalysis_FA_PeriodEnum, CDRAnalysis_FA_SuspicionLevelEnum, UtilsService, Fzero_FraudAnalysis_DecisionTreeService, CDRAnalysis_FA_DecisionTreeConditionOperatorEnum, CDRAnalysis_FA_DecisionTreeNodeTypeEnum, VRNotificationService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "criteriaCtrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: "/Client/Modules/FraudAnalysis/Directives/Strategy/MainExtensions/Templates/DecisionTreeStrategyTemplate.html"

    };
    function DirectiveConstructor($scope, ctrl) {


        this.initializeController = initializeController;

        var filters;
        var filter;
        var context;
        var strategyEntity;

        var counter = 1;

        var treeAPI;
        var treeReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.suspicionLevels = UtilsService.getArrayEnum(CDRAnalysis_FA_SuspicionLevelEnum);
            $scope.scopeModel.operators = UtilsService.getArrayEnum(CDRAnalysis_FA_DecisionTreeConditionOperatorEnum);

            $scope.scopeModel.filters = [];
            $scope.scopeModel.decisionTree = [
                {
                    Name: "Root",
                    ID: counter++,
                    Childs: [],
                    isRoot: true,
                    isOpened: true
                }
            ];
            $scope.scopeModel.onModulesTreeReady = function (api) {
                treeAPI = api;
                treeReadyDeferred.resolve();
            };
            $scope.scopeModel.addMenu = [{
                disable: false,
                name: "Add Condition",
                clicked: function () {
                  return addCondition();
                }
            },{
                disable: false,
                name: "Add Label",
                clicked: function () {
                    return addLabel();
                }
            }];

            $scope.scopeModel.removeNode = function ()
            {
                VRNotificationService.showConfirmation().then(function (response) {
                    if (response == true) {
                        onItemDeleted($scope.scopeModel.decisionTree, $scope.scopeModel.selectedItem.ID);
                        treeAPI.refreshTree($scope.scopeModel.decisionTree);
                   }
               });
            }


            defineAPI();
        }

        function defineAPI() {


            var api = {};
            api.getData = function () {

                var decisionTree = buildTreeObjectFromScope();
                return {
                    $type: "Vanrise.Fzero.FraudAnalysis.MainExtensions.DecisionTreeStrategySettingsCriteria, Vanrise.Fzero.FraudAnalysis.MainExtensions",
                    DecisionTree: decisionTree,
                }
            };

            api.getFilterHint = function (parameter) {
                if (parameter != undefined) {
                    var filters = [];
                    return filters.join(',');
                }
                return null;
            }

            api.load = function (payload) {
                if (payload) {
                    strategyEntity = payload.strategyCriteria;
                    filter = payload.filter;
                    context = payload.context;
                }
                var promises = [];
                var promiseDeffered = UtilsService.createPromiseDeferred();
                promises.push(promiseDeffered.promise);

                loadFilters().then(function () {
                    loadTree().then(function () {
                        promiseDeffered.resolve();
                    }).catch(function (error) {
                        promiseDeffered.reject(error);
                    });

                })
                return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function addCondition() {
            var onConditionAdded = function (conditionObj) {
                var node = mapConditionNode(conditionObj);
                onAddedItem($scope.scopeModel.decisionTree, node);
                treeAPI.createNode(node);
                var elseNode = getElseNode();
                onAddedItem($scope.scopeModel.decisionTree, elseNode);
                treeAPI.createNode(elseNode);
            };
            Fzero_FraudAnalysis_DecisionTreeService.addCondition(onConditionAdded, $scope.scopeModel.selectedItem);
        }
        function editCondition() {
            var onConditionUpdated = function (moduleObj) {
                //var node = mapModuleNode(moduleObj.Entity);
                //onAddedModule(menuItems, node);
                //treeAPI.createNode(node);
                //treeAPI.refreshTree(menuItems);
            };
            Fzero_FraudAnalysis_DecisionTreeService.editCondition(onConditionUpdated, $scope.scopeModel.selectedItem);
        }
        function addLabel() {
            var onLabelAdded = function (labelObj) {
                var node = mapLabelNode(labelObj);
                onAddedItem($scope.scopeModel.decisionTree, node)
                treeAPI.createNode(node);
            };
            Fzero_FraudAnalysis_DecisionTreeService.addLabel(onLabelAdded, $scope.scopeModel.selectedItem);
        }
        function editLabel() {
            var onLabelUpdated = function (labelObj) {
                //var node = mapModuleNode(labelObj);
                //onAddedModule(menuItems, labelObj);
                //treeAPI.createNode(node);
                //treeAPI.refreshTree(menuItems);
            };
            Fzero_FraudAnalysis_DecisionTreeService.editLabel(onLabelUpdated, $scope.scopeModel.selectedItem);
        }
        function loadTree() {

            var treeLoadDeferred = UtilsService.createPromiseDeferred();
            treeReadyDeferred.promise.then(function () {
                if (strategyEntity && strategyEntity.DecisionTree != undefined)
                {
                    buildTreeScopeFromObject(strategyEntity.DecisionTree.RootNode, $scope.scopeModel.decisionTree[0].Childs);
                }
                treeAPI.refreshTree($scope.scopeModel.decisionTree);
                treeLoadDeferred.resolve();
            }).catch(function (error) {
                treeLoadDeferred.reject(error);
            });

            return treeLoadDeferred.promise;
        }
        function mapLabelNode(labelObj)
        {
            return {
                Name: labelObj.SuspicionLevel != undefined ? UtilsService.getItemByVal($scope.scopeModel.suspicionLevels, labelObj.SuspicionLevel, 'value').description : "Clean",
                Entity:labelObj,
                ID: counter++,
                Childs: [],
                isOpened: true,
                isLeaf :true
            };
        }
        function mapConditionNode(conditionObj) {
            return {
                Name: UtilsService.getItemByVal($scope.scopeModel.filters, conditionObj.FilterId, 'FilterId').Abbreviation + " " + UtilsService.getItemByVal($scope.scopeModel.operators, conditionObj.Operator, 'value').description + " " + conditionObj.Value,
                Entity: conditionObj,
                ID: counter++,
                NodeType: CDRAnalysis_FA_DecisionTreeNodeTypeEnum.TrueBranch.value,

                Childs: [],
                isOpened: true,
            };
        }
        function getElseNode()
        {
            return {
                Name: "Else",
                Entity: {},
                ID: counter++,
                NodeType: CDRAnalysis_FA_DecisionTreeNodeTypeEnum.FalseBranch.value,
                Childs: [],
                isOpened: true,
            }
        }
        function onAddedItem(menuItems, newNode) {
            if (menuItems != undefined) {
                for (var i = 0; i < menuItems.length ; i++) {
                    var menuItem = menuItems[i];
                    if (menuItem.ID != $scope.scopeModel.selectedItem.ID)///module
                    {
                        onAddedItem(menuItem.Childs, newNode);
                    } else if (menuItem.ID == $scope.scopeModel.selectedItem.ID || $scope.scopeModel.selectedItem.isRoot)//View
                    {
                        if (menuItems[i].Childs == undefined)
                            menuItems[i].Childs = [];
                        menuItems[i].Childs.push(newNode);
                    }
                }
            }

        }
        function loadFilters() {
            return StrategyAPIService.GetFilters().then(function (response) {
                $scope.scopeModel.filters = response;
            });
        }

        function buildTreeObjectFromScope()
        {
            var decisionTree = {
                RootNode: {}
            };
            for (var i = 0; i < $scope.scopeModel.decisionTree.length; i++) {
                var item = $scope.scopeModel.decisionTree[i];
                decisionTree.RootNode = buildTreeObjectRecursively(item.Childs);
            }
            return decisionTree;
        }
        function buildTreeObjectRecursively(childs)
        {
            if (childs == undefined || childs.length == 0)
                return;

            if (childs != undefined) {
                var decisionTree = {
                    ConditionNode: {
                        TrueBranch: {},
                        FalseBranch: {}
                    }
                };
                for (var i = 0; i < childs.length ; i++) {
                    var item = childs[i];
                    if (item.isLeaf)
                    {
                        return { SuspicionLevel: item.Entity.SuspicionLevel }
                    }
                    else 
                    {
                         if (item.NodeType == CDRAnalysis_FA_DecisionTreeNodeTypeEnum.TrueBranch.value)
                         {
                             decisionTree.ConditionNode.Condition = {
                                 FilterId: item.Entity.FilterId,
                                 Value: item.Entity.Value,
                                 Operator: item.Entity.Operator
                             };
                             decisionTree.ConditionNode.TrueBranch = buildTreeObjectRecursively(item.Childs);
                         } else if (item.NodeType == CDRAnalysis_FA_DecisionTreeNodeTypeEnum.FalseBranch.value)
                         {
                             decisionTree.ConditionNode.FalseBranch = buildTreeObjectRecursively(item.Childs);
                         }
                    }
                }
                return decisionTree;
            }
        }

        function buildTreeScopeFromObject(decisionTree,childs) {

            if (decisionTree.ConditionNode !=undefined)
            {
                var node = mapConditionNode(decisionTree.ConditionNode.Condition);
                buildTreeScopeFromObject(decisionTree.ConditionNode.TrueBranch, node.Childs);
                childs.push(node);
                var elseNode = getElseNode();
                buildTreeScopeFromObject(decisionTree.ConditionNode.FalseBranch, elseNode.Childs);
                childs.push(elseNode);
            } else {
                var node = mapLabelNode(decisionTree);
                childs.push(node);
            }
        }

        function onItemDeleted(menuItems, nodeId) {
            if (menuItems != undefined) {
                for (var i = 0; i < menuItems.length ; i++) {
                    var item = menuItems[i];
                    if (item.ID != nodeId)
                    {
                        onItemDeleted(item.Childs, nodeId);
                    } else if (item.ID == nodeId && !item.isRoot)
                    {
                        if (item.isLeaf)
                            menuItems.splice(0, 1);
                        else {
                            menuItems.splice(0, 2);
                        }
                        break;
                    }
                }
            }
        }
    }

    return directiveDefinitionObject;
}]);
