//(function (app) {

//    'use strict';

//    recordAnalysisSwitchinterconnectionSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

//    function recordAnalysisSwitchinterconnectionSettingsDirective(UtilsService, VRUIUtilsService) {

//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new RecordAnalysisSwitchinterconnectionSettings(ctrl, $scope, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: "/Client/Modules/RecordAnalysis/Directives/SwitchInterconnection/Templates/SwitchInterconnectionSettingsTemplate.html"
//        };

//        function RecordAnalysisSwitchinterconnectionSettings(ctrl, $scope, $attrs) {
//            this.initializeController = initializeController;

//            var directiveAPI;
//            var directiveReadyDeferred = UtilsService.createPromiseDeferred();


//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.trunksList = [];
//                $scope.scopeModel.trunks = [];
//                $scope.scopeModel.selectedTrunks = [];
//                $scope.scopeModel.trunkGroupsList = [];


//                $scope.scopeModel.addTrunk = function () {
//                    var trunk = {
//                        TrunkName: $scope.scopeModel.trunkName
//                    }

//                    $scope.scopeModel.trunksList.push(trunk);
//                    $scope.scopeModel.trunks.push(trunk);

//                    $scope.scopeModel.trunkName = undefined;
//                };

//                $scope.scopeModel.disableAddTrunk = function () {
//                    if ($scope.scopeModel.trunkName == undefined || $scope.scopeModel.trunkName.length == 0)
//                        return true;
//                    return false;
//                }

//                $scope.scopeModel.disableAddTrunkGroup = function () {
//                    if ($scope.scopeModel.trunkGroupName == undefined || $scope.scopeModel.trunkGroupName.length == 0 || $scope.scopeModel.selectedTrunks.length == 0)
//                        return true;
//                    return false;
//                }
//                $scope.scopeModel.removeTrunk = function (dataItem) {
//                    var index = $scope.scopeModel.trunksList.indexOf(dataItem);
//                    $scope.scopeModel.trunksList.splice(index, 1);

//                    $scope.scopeModel.selectedTrunks = [];
//                    var trunkName = dataItem.TrunkName;

//                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.trunks, trunkName, 'TrunkName');
//                    if (index > - 1)
//                        $scope.scopeModel.trunks.splice(index, 1);
//                    else
//                        for (var z = 0; z < $scope.scopeModel.trunkGroupsList.length; z++) {
//                            var index = $scope.scopeModel.trunkGroupsList[z].Trunks.indexOf(trunkName);

//                            if (index > - 1) {
//                                $scope.scopeModel.trunkGroupsList[z].Trunks.splice(index, 1);

//                                if ($scope.scopeModel.trunkGroupsList[z].Trunks.length == 0)
//                                    $scope.scopeModel.trunkGroupsList.splice(index, 1);
//                                else
//                                    $scope.scopeModel.trunkGroupsList[z].TrunkNamesAsString = $scope.scopeModel.trunkGroupsList[z].Trunks.join(", ");
//                                break;
//                            }
//                        }
//                    setTimeout(function () {
//                        UtilsService.safeApply($scope);
//                    }, 1);
//                };

//                $scope.scopeModel.removeTrunkGroup = function (dataItem) {
//                    var index = $scope.scopeModel.trunkGroupsList.indexOf(dataItem);
//                    $scope.scopeModel.trunkGroupsList.splice(index, 1);

//                    var trunks = dataItem.Trunks;
//                    for (var a = 0; a < trunks.length; a++) {
//                        $scope.scopeModel.trunks.push({
//                            TrunkName: trunks[a]
//                        });
//                    }
//                };

//                $scope.scopeModel.trunkListHasItem = function () {
//                    if ($scope.scopeModel.trunksList != undefined && $scope.scopeModel.trunksList.length > 0)
//                        return;
//                    return 'At least one Trunk must be added';
//                };

//                $scope.scopeModel.addTrunkGroup = function () {
//                    var selectedTrunks = [];
//                    for (var i = 0; i < $scope.scopeModel.selectedTrunks.length; i++) {
//                        var trunkName = $scope.scopeModel.selectedTrunks[i].TrunkName;
//                        selectedTrunks.push(trunkName);
//                        var index = UtilsService.getItemIndexByVal($scope.scopeModel.trunks, trunkName, 'TrunkName');
//                        $scope.scopeModel.trunks.splice(index, 1);
//                    };

//                    $scope.scopeModel.trunkGroupsList.push({
//                        Name: $scope.scopeModel.trunkGroupName,
//                        Trunks: selectedTrunks,
//                        TrunkNamesAsString: selectedTrunks.join(", ")
//                    });

//                    $scope.scopeModel.trunkGroupName = undefined;
//                    $scope.scopeModel.selectedTrunks = [];
//                }

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    $scope.scopeModel.trunksList.length = 0;

//                    var promises = [];
//                    var settings;

//                    if (payload != undefined && payload.selectedValues != undefined) {
//                        settings = payload.selectedValues.Settings;
//                    }

//                    if (settings != undefined) {
//                        for (var i = 0; i < settings.Trunks.length; i++) {
//                            $scope.scopeModel.trunksList.push(settings.Trunks[i]);
//                            $scope.scopeModel.trunks.push(settings.Trunks[i]);
//                        }
//                        var trunkGroups = settings.TrunkGroups;
//                        if (trunkGroups != undefined && trunkGroups.length > 0) {

//                            for (var d = 0; d < trunkGroups.length; d++) {

//                                var trunks=[];

//                                for (var i = 0; i < trunkGroups.Trunks; i++) {
//                                    var trunkName = trunkGroups.Trunks[i].TrunkName;
//                                    trunks.push(trunkName);
//                                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.trunks, trunkName, 'TrunkName');
//                                    $scope.scopeModel.trunks.splice(index, 1);
//                                };

//                                $scope.scopeModel.trunkGroupsList.push({
//                                    Name: trunkGroups[i].Name,
//                                    Trunks: selectedTrunks,
//                                    TrunkNamesAsString: selectedTrunks.join(", ")
//                                });


//                            }
//                        }

//                    }

//                    return UtilsService.waitMultiplePromises(promises);
//                };

//                api.setData = function (switchInterconnectionItem) {
//                    var obj = { $type: "RecordAnalysis.Entities.SwitchInterconnection,RecordAnalysis.Entities" };
//                    var trunks = [];
//                    for (var i = 0; i < $scope.scopeModel.trunksList.length; i++) {
//                        trunks.push({
//                            TrunkName: $scope.scopeModel.trunksList[i].TrunkName
//                        })
//                    }
//                    obj.Trunks = trunks;
//                    switchInterconnectionItem.Settings = obj;
//                };

//                if (ctrl.onReady != null) {
//                    ctrl.onReady(api);
//                }
//            }
//        }
//    }

//    app.directive('recAnalSwitchinterconnectionSettings', recordAnalysisSwitchinterconnectionSettingsDirective);

//})(app);