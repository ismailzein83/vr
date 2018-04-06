(function (app) {

    'use strict';

    EricssonTrunkTrunkGroup.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_CarrierAccountAPIService'];

    function EricssonTrunkTrunkGroup(UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_CarrierAccountAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new EricssonTrunkTrunkGroupCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/Templates/EricssonTrunkTrunkGroupTemplate.html"
        };

        function EricssonTrunkTrunkGroupCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.trunksInfo = [];
                $scope.scopeModel.selectedTrunksInfo = [];
                $scope.scopeModel.trunkTrunkGroups = [];

                $scope.scopeModel.onSelectTrunk = function (addedTrunk) {

                    $scope.scopeModel.trunkTrunkGroups.push({
                        TrunkId: addedTrunk.value,
                        TrunkName: addedTrunk.description
                    });

                    if (context != undefined) {
                        context.updateSupplierDescriptions();
                    }
                };

                $scope.scopeModel.onDeselectTrunk = function (deletedTrunk) {

                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.trunkTrunkGroups, deletedTrunk.TrunkId, 'TrunkId');
                    $scope.scopeModel.trunkTrunkGroups.splice(index, 1);

                    if (context != undefined) {
                        context.updateSupplierDescriptions();
                    }
                };

                $scope.scopeModel.onDeleteTrunkTrunkGroup = function (deletedTrunk) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedTrunksInfo, deletedTrunk.TrunkId, 'TrunkId');
                    $scope.scopeModel.selectedTrunksInfo.splice(index, 1);
                    $scope.scopeModel.onDeselectTrunk(deletedTrunk);
                };

                $scope.scopeModel.updateSupplierDescriptions = function () {
                    if (context != undefined) {
                        context.updateSupplierDescriptions();
                    }
                };

                $scope.scopeModel.isTrunkTrunkGroupsValid = function () {
                    var trunkTrunkGroups = $scope.scopeModel.trunkTrunkGroups;
                    if (trunkTrunkGroups.length == 0)
                        return null;

                    var isPercentageExists = false;
                    var sumOfPercentage = 0;

                    for (var i = 0; i < trunkTrunkGroups.length; i++) {
                        var currentTrunkTrunkGroup = trunkTrunkGroups[i];
                        if (currentTrunkTrunkGroup.Percentage != undefined) {
                            isPercentageExists = true;
                            sumOfPercentage += parseInt(currentTrunkTrunkGroup.Percentage);
                        }
                    }

                    if (isPercentageExists && sumOfPercentage != 100) {
                        return "Percentages summation should be 100";
                    }
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var trunks;
                    var trunkTrunkGroups;

                    if (payload != undefined) {
                        trunks = payload.trunks;
                        trunkTrunkGroups = payload.trunkTrunkGroups;
                        context = payload.context;
                    }

                    loadTrunkSelector();
                    loadTrunkTrunkGroupGrid();


                    function loadTrunkSelector() {
                        if (trunks != undefined) {
                            for (var index = 0; index < trunks.length; index++) {
                                var currentTrunk = trunks[index];
                                $scope.scopeModel.trunksInfo.push({
                                    value: currentTrunk.TrunkId,
                                    description: currentTrunk.TrunkName
                                });
                            }

                            if (trunkTrunkGroups != undefined) {
                                var trunkIds = UtilsService.getPropValuesFromArray(trunkTrunkGroups, "TrunkId");
                                for (var i = 0; i < trunkIds.length; i++) {
                                    var selectedValue = UtilsService.getItemByVal($scope.scopeModel.trunksInfo, trunkIds[i], "value");
                                    if (selectedValue != null) {
                                        $scope.scopeModel.selectedTrunksInfo.push(selectedValue);
                                    }
                                }
                            }
                        }
                    }
                    function loadTrunkTrunkGroupGrid() {
                        if (trunkTrunkGroups == undefined)
                            return;

                        for (var i = 0; i < trunkTrunkGroups.length; i++) {
                            var currentTrunkTrunkGroup = trunkTrunkGroups[i];
                            var currentTrunkInfoItem = UtilsService.getItemByVal($scope.scopeModel.trunksInfo, currentTrunkTrunkGroup.TrunkId, "value");
                            if (currentTrunkInfoItem == undefined)
                                continue;

                            currentTrunkTrunkGroup.TrunkName = currentTrunkInfoItem.description;
                            $scope.scopeModel.trunkTrunkGroups.push(currentTrunkTrunkGroup);
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var trunkTrunkGroups = [];
                    for (var i = 0; i < $scope.scopeModel.trunkTrunkGroups.length; i++) {
                        var currentTrunkTrunkGroup = $scope.scopeModel.trunkTrunkGroups[i];
                        trunkTrunkGroups.push({
                            TrunkId: currentTrunkTrunkGroup.TrunkId,
                            Percentage: currentTrunkTrunkGroup.Percentage,
                            Priority: currentTrunkTrunkGroup.Priority
                        });
                    }
                    return trunkTrunkGroups;
                };

                api.getTrunksInfo = function () {
                    return $scope.scopeModel.trunksInfo;
                };

                api.deleteTrunkTrunkGroup = function (trunk) {

                    var selectedTrunkInfoIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedTrunksInfo, trunk.TrunkId, "value");
                    if (selectedTrunkInfoIndex >= 0) {
                        $scope.scopeModel.selectedTrunksInfo.splice(selectedTrunkInfoIndex, 1);
                    }

                    var trunksInfoIndex = UtilsService.getItemIndexByVal($scope.scopeModel.trunksInfo, trunk.TrunkId, "value");
                    $scope.scopeModel.trunksInfo.splice(trunksInfoIndex, 1);

                    var trunkTrunkGroupsIndex = UtilsService.getItemIndexByVal($scope.scopeModel.trunkTrunkGroups, trunk.TrunkId, "TrunkId");
                    if (trunkTrunkGroupsIndex >= 0) {
                        $scope.scopeModel.trunkTrunkGroups.splice(trunkTrunkGroupsIndex, 1);
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncEricssonTrunktrunkgroup', EricssonTrunkTrunkGroup);
})(app);