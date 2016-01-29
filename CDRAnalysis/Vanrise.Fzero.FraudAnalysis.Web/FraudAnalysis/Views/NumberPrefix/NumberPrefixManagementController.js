(function (appControllers) {

    'use strict';


    NumberPrefixManagementController.$inject = ['$scope', 'FraudAnalysis_NumberPrefixAPIService', 'VRUIUtilsService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function NumberPrefixManagementController($scope, FraudAnalysis_NumberPrefixAPIService, VRUIUtilsService, UtilsService, VRModalService, VRNotificationService) {

        var treeAPI;
        var numberPrefixes = [];

        defineScope();
        load();

        function defineScope() {
            $scope.nodes = [];
            $scope.currentNode;
            $scope.hasState = false;

            $scope.selectedCodes = [];

            $scope.applyNumberPrefixState = function () {
                var onNumberPrefixStateApplied = function () {
                };
                FraudAnalysis_NumberPrefixAPIService.ApplyNumberPrefixState(onNumberPrefixStateApplied);
            }

            $scope.numberPrefixesTreeReady = function (api) {
                treeAPI = api;
            }

            $scope.numberPrefixesTreeValueChanged = function () {

                if ($scope.currentNode != undefined) {
                }
            }

            $scope.loadEffectiveNumberPrefixes = function (numberPrefixNode) {
                var effectiveNumberPrefixesPromiseDeffered = UtilsService.createPromiseDeferred();
                FraudAnalysis_NumberPrefixAPIService.GetNumberPrefixItems(numberPrefixNode.nodeId).then(function (response) {
                    var effectiveNumberPrefixes = [];
                    angular.forEach(response, function (itm) {
                        effectiveNumberPrefixes.push(mapNumberPrefixToNode(itm));
                    });
                    var numberPrefixIndex = UtilsService.getItemIndexByVal($scope.nodes, numberPrefixNode.nodeId, 'nodeId');
                    var parentNode = $scope.nodes[numberPrefixIndex];
                    parentNode.effectiveNumberPrefixes = effectiveNumberPrefixes;
                    $scope.nodes[numberPrefixIndex] = parentNode;
                    effectiveNumberPrefixesPromiseDeffered.resolve(effectiveNumberPrefixes);
                });
                return effectiveNumberPrefixesPromiseDeffered.promise;
            }







            $scope.newNumberPrefixClicked = function () {
                addNewNumberPrefix();
            }


            $scope.cancelState = function () {
                return VRNotificationService.showConfirmation().then(function (result) {
                    if (result) {
                        numberPrefixes.length = 0;
                        return FraudAnalysis_NumberPrefixAPIService.CancelNumberPrefixState().then(function (response) {
                            $scope.hasState = !response;
                            treeAPI.refreshTree($scope.nodes);
                            $scope.currentNode = undefined;
                        });
                    }
                });

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

        function onNumberPrefixAdded(addedNumberPrefixes) {
            if (addedNumberPrefixes != undefined) {
                $scope.hasState = true;
                for (var i = 0; i < addedNumberPrefixes.length; i++) {
                    var node = mapNumberPrefixToNode(addedNumberPrefixes[i]);
                    treeAPI.createNode(node);
                    for (var i = 0; i < $scope.nodes.length; i++) {
                        if ($scope.nodes[i].nodeId == $scope.currentNode.nodeId) {
                            $scope.nodes[i].effectiveNumberPrefixes.push(node);
                        }

                    }
                }


            }
        }

        function addNewNumberPrefix() {
            var parameters = {
            };
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onNumberPrefixAdded = onNumberPrefixAdded;
            };

            VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/Dialogs/NewNumberPrefixDialog.html", parameters, settings);
        }

        function GetCurrentNumberPrefixNodeNumberPrefixes() {
            var numberPrefixIndex = UtilsService.getItemIndexByVal($scope.nodes, $scope.currentNode.numberPrefixId, 'nodeId');
            var numberPrefixNode = $scope.nodes[numberPrefixIndex];
            return getNumberPrefixInfos(numberPrefixNode.effectiveNumberPrefixes);
        }

        function getNumberPrefixInfos(numberPrefixNodes) {
            var numberPrefixInfos = [];
            angular.forEach(numberPrefixNodes, function (itm) {
                numberPrefixInfos.push(mapNumberPrefixInfoFromNode(itm));
            });

            return numberPrefixInfos;
        }

        function mapNumberPrefixInfoFromNode(numberPrefixItem) {
            return {
                Name: numberPrefixItem.nodeName
            };
        }

        function getNumberPrefixes() {
            numberPrefixes.length = 0;
            return FraudAnalysis_NumberPrefixAPIService.GetPrefixesInfo().then(function (response) {
                angular.forEach(response, function (itm) {
                    numberPrefixes.push(itm);
                });
            });
        }

        function buildNumberPrefixesTree() {
            $scope.nodes.length = 0;
            for (var i = 0; i < numberPrefixes.length; i++) {
                var node = mapNumberPrefixToNode(numberPrefixes[i]);
                console.log(node)
                $scope.nodes.push(node);
            }
            treeAPI.refreshTree($scope.nodes);

        }

        function mapNumberPrefixToNode(numberPrefixInfo) {

            return {
                nodeId: numberPrefixInfo.ID,
                nodeName: numberPrefixInfo.Prefix,
                hasRemoteChildren: false,
                effectiveNumberPrefixes: [],
                numberPrefixId: numberPrefixInfo.ID
            };
        }

    };

    appControllers.controller('FraudAnalysis_NumberPrefixManagementController', NumberPrefixManagementController);

})(appControllers);
