(function (app) {

    'use strict';

    recordAnalysisC4ProbeSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function recordAnalysisC4ProbeSettingsDirective(UtilsService, VRUIUtilsService, VRNotificationService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RecordAnalysisC4ProbeSettingsDirective(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/RecordAnalysis/Directives/C4Probe/Templates/C4ProbeSettingsTemplate.html"
        };

        function RecordAnalysisC4ProbeSettingsDirective(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var rowIndex = 0;
            var probeTrunkMappingGridAPI;
            var probeTrunkMappingGridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.probeTrunkMappings = [];

                $scope.scopeModel.onProbeTrunkMappingGridReady = function (api) {
                    probeTrunkMappingGridAPI = api;
                    probeTrunkMappingGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onProbeTrunkMappingAdded = function () {
                    var index = $scope.scopeModel.probeTrunkMappings.length;
                    var dataItem =
                    {
                        MappingId: rowIndex++,
                        switchReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                        switchLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                    };

                    extendTrunkGroupEntity(dataItem);
                };

                $scope.scopeModel.onProbeTrunkMappingDeleted = function (deletedItem) {
                    var trunkIndex = UtilsService.getItemIndexByVal($scope.scopeModel.probeTrunkMappings, deletedItem.MappingId, "MappingId");
                    if (trunkIndex >= 0)
                        $scope.scopeModel.probeTrunkMappings.splice(trunkIndex, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var settings;

                    if (payload != undefined && payload.selectedValues != undefined) {
                        settings = payload.selectedValues.Settings;
                    }

                    if (settings != undefined) {
                        if (settings.ProbeTrunkMappings != undefined) {

                            for (var j = 0; j < settings.ProbeTrunkMappings.length; j++) {

                                var probeTrunkMappingEntity = settings.ProbeTrunkMappings[j];

                                var dataItem = {
                                    MappingId: rowIndex++,
                                    PointCode: probeTrunkMappingEntity.PointCode,
                                    Trunk: probeTrunkMappingEntity.Trunk,
                                    switchReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    switchLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                promises.push(dataItem.switchReadyPromiseDeferred.promise);
                                promises.push(dataItem.switchLoadPromiseDeferred.promise);

                                extendTrunkGroupEntity(dataItem, probeTrunkMappingEntity);
                            }
                        }

                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                    });
                };


                api.setData = function (probeItem) {
                    probeItem.Settings = getProbeTrunkMappings();
                };

                api.getData = function () {
                    return getProbeTrunkMappings();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function extendTrunkGroupEntity(dataItem, payloadEntity) {

                var selectedSwitchId;

                var switchSelectorPayload = {
                    businessEntityDefinitionId: "9e7ecdc0-e19b-43d2-9edb-ddc7bbc0f764"
                };

                if (payloadEntity != undefined) {
                    switchSelectorPayload.selectedIds = payloadEntity.SwitchId;
                }

                dataItem.onSwitchSelectorReady = function (api) {
                    dataItem.switchAPI = api;
                    dataItem.switchReadyPromiseDeferred.resolve();
                };

                dataItem.switchReadyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataItem.switchAPI, switchSelectorPayload, dataItem.switchLoadPromiseDeferred);
                    });

                $scope.scopeModel.probeTrunkMappings.push(dataItem);
            };



            function getProbeTrunkMappings() {

                var probeTrunkMappings = [];
                for (var index = 0; index < $scope.scopeModel.probeTrunkMappings.length; index++) {
                    var currentProbeTrunkMapping = $scope.scopeModel.probeTrunkMappings[index];
                    probeTrunkMappings.push({
                        SwitchId: currentProbeTrunkMapping.switchAPI != undefined ? currentProbeTrunkMapping.switchAPI.getSelectedIds() : undefined,
                        PointCode: currentProbeTrunkMapping.PointCode,
                        Trunk: currentProbeTrunkMapping.Trunk
                    });
                }

                return {
                    $type: "RecordAnalysis.Entities.C4ProbeSettings,RecordAnalysis.Entities",
                    ProbeTrunkMappings: probeTrunkMappings
                };
            };
        }
    }
    app.directive('recAnalC4probeSettings', recordAnalysisC4ProbeSettingsDirective);

})(app);