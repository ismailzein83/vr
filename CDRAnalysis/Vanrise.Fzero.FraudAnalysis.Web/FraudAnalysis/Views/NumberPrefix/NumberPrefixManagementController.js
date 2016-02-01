﻿(function (appControllers) {

    'use strict';

    numberPrefixManagementController.$inject = ['$scope', 'FraudAnalysis_NumberPrefixAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function numberPrefixManagementController($scope, FraudAnalysis_NumberPrefixAPIService, UtilsService, VRModalService, VRNotificationService) {

        var treeAPI;
        var numberPrefixes = [];


        defineScope();
        load();

        function defineScope() {
            $scope.nodes = [];
            $scope.currentNode;
            $scope.nodesUpdated = false;

            $scope.applyNumberPrefixesClicked = function () {
                applyNumberPrefixes();
            }

            $scope.cancelChanges = function () {
                $scope.nodesUpdated = false;
                load();
            }

            $scope.numberPrefixesTreeReady = function (api) {
                treeAPI = api;
            }

            $scope.newNumberPrefixClicked = function () {
                addNewNumberPrefix();
            }

            $scope.expandNumberPrefixClicked = function () {
                expandNumberPrefix();
            }

            $scope.splitNumberPrefixClicked = function () {
                splitNumberPrefix();
            }

            $scope.mergeNumberPrefixesClicked = function () {
                mergeNumberPrefix();
            }

            $scope.removeNumberPrefixesClicked = function () {
                removeNumberPrefix();
            }

        }

        function load() {
            $scope.isLoading = true;

            UtilsService.waitMultipleAsyncOperations([getNumberPrefixes]).then(function () {
                buildNumberPrefixesTree();
                $scope.currentNode = undefined;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function onNumberPrefixAdded(addedNumberPrefix) {
            if (addedNumberPrefix != undefined) {
                $scope.nodesUpdated = true;
                numberPrefixes.push(mapPrefixtoInfo(addedNumberPrefix.prefix));
                buildNumberPrefixesTree();
            }
        }

        function applyNumberPrefixes() {
            console.log(numberPrefixes)
            return FraudAnalysis_NumberPrefixAPIService.UpdatePrefixes(numberPrefixes).then(function (response) {
                if (response) {
                    $scope.nodesUpdated = false;
                }
                else { }

            });
        }

        function addNewNumberPrefix() {
            var parameters = {
                NumberPrefixes: numberPrefixes
            };
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onNumberPrefixAdded = onNumberPrefixAdded;
            };

            VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/Dialogs/NewNumberPrefixDialog.html", parameters, settings);
        }

        function mapPrefixtoInfo(prefix) {
            return { $type: "Vanrise.Fzero.FraudAnalysis.Entities.NumberPrefix, Vanrise.Fzero.FraudAnalysis.Entities", Prefix: prefix }
        }

        function splitNumberPrefix() {

            var currentPrefix = $scope.currentNode.nodeName;
            for (var i = 0; i <= 9; i++) {
                var mapped = mapPrefixtoInfo(currentPrefix + i);
                if (!contains(numberPrefixes, mapped))
                    numberPrefixes.push(mapPrefixtoInfo(currentPrefix + i));
            }
            $scope.nodesUpdated = true;
            buildNumberPrefixesTree();
        }

        function mergeNumberPrefix() {
            var toBeRemoved = [];
            var currentPrefix = $scope.currentNode.nodeName;

            angular.forEach(numberPrefixes, function (item) {
                if (currentPrefix != item.Prefix)
                    if (item.Prefix.indexOf(currentPrefix) == 0) {
                        toBeRemoved.push(item)
                    }
            });


            for (var i = 0; i <= toBeRemoved.length; i++)
                for (var j = 0; j <= numberPrefixes.length; j++) {
                    if (toBeRemoved[i] == numberPrefixes[j]) {
                        numberPrefixes.splice(j, 1);
                    }
                }

            $scope.nodesUpdated = true;
            buildNumberPrefixesTree();
        }

        function removeNumberPrefix() {
            var toBeRemoved = [];
            var currentPrefix = $scope.currentNode.nodeName;

            angular.forEach(numberPrefixes, function (item) {
                if (item.Prefix.indexOf(currentPrefix) == 0) {
                    toBeRemoved.push(item)
                }
            });


            for (var i = 0; i <= toBeRemoved.length; i++)
                for (var j = 0; j <= numberPrefixes.length; j++) {
                    if (toBeRemoved[i] == numberPrefixes[j]) {
                        numberPrefixes.splice(j, 1);
                    }
                }

            $scope.nodesUpdated = true;
            buildNumberPrefixesTree();
        }

        function getNumberPrefixes() {
            return FraudAnalysis_NumberPrefixAPIService.GetPrefixesInfo().then(function (response) {
                angular.forEach(response, function (itm) {
                    numberPrefixes.push(itm);
                });
            });
        }

        function findRootNodes() {
            for (var i = 0; i < numberPrefixes.length; i++) {
                numberPrefixes[i].isRoot = true;
                numberPrefixes[i].effectiveNumberPrefixes = [];
                if (numberPrefixes[i].Prefix.length > 1) {
                    for (var j = 0; j < numberPrefixes.length; j++) {
                        if (numberPrefixes[i].Prefix.slice(0, -1) == numberPrefixes[j].Prefix)
                            numberPrefixes[i].isRoot = false;
                    }
                }
            }
        }

        function findChildrenNodes(rootNode) {
            var children = [];
            for (var i = 0; i < numberPrefixes.length; i++) {
                if (numberPrefixes[i].isRoot == false && numberPrefixes[i].Prefix.length == rootNode.Prefix.length + 1 && numberPrefixes[i].Prefix.slice(0, -1) == rootNode.Prefix) {
                    rootNode.effectiveNumberPrefixes.push(mapNumberPrefixToNode(numberPrefixes[i]));
                }
            }
        }

        function buildNumberPrefixesTree() {
            numberPrefixes.sort(compare);
            $scope.nodes.length = 0;

            findRootNodes();

            for (var i = 0; i < numberPrefixes.length; i++) {
                findChildrenNodes(numberPrefixes[i])
            }

            for (var i = 0; i < numberPrefixes.length; i++) {
                if (numberPrefixes[i].isRoot == true) {
                    var node = mapNumberPrefixToNode(numberPrefixes[i]);
                    $scope.nodes.push(node);
                }
            }

            treeAPI.refreshTree($scope.nodes);

        }

        function compare(a, b) {
            if (a.Prefix < b.Prefix)
                return -1;
            else if (a.Prefix > b.Prefix)
                return 1;
            else
                return 0;
        }

        function mapNumberPrefixToNode(numberPrefixInfo) {
            return {
                nodeId: numberPrefixInfo.Prefix,
                nodeName: numberPrefixInfo.Prefix,
                hasRemoteChildren: false,
                effectiveNumberPrefixes: numberPrefixInfo.effectiveNumberPrefixes
            };
        }

        function contains(a, obj) {
            for (var i = 0; i < a.length; i++) {
                if (a[i].Prefix === obj.Prefix) {
                    return true;
                }
            }
            return false;
        }

    };

    appControllers.controller('FraudAnalysis_NumberPrefixManagementController', numberPrefixManagementController);

})(appControllers);
