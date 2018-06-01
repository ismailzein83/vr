(function (app) {

    'use strict';

    QueueActivatorEvaluateSMSDeal.$inject = ['UtilsService', 'VRUIUtilsService'];

    function QueueActivatorEvaluateSMSDeal(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var evaluateSMSDealActivatorCtor = new EvaluateSMSDealActivatorCtor(ctrl, $scope);
                evaluateSMSDealActivatorCtor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Deal/Directives/QueueActivators/Templates/QueueActivatorEvaluateSMSDealTemplate.html';
            }
        };

        function EvaluateSMSDealActivatorCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = { outputStages: [], selectedMainOutputStages: [], selectedPartialPricedOutputStages: [], selectedBillingOutputStages: [], selectedTrafficOutputStages: [] };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(getDirectiveAPI());
                }
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var dataRecordTypeId;
                    var existingStages;

                    if (payload != undefined && payload.ExistingStages != undefined) {
                        existingStages = payload.ExistingStages;
                    }

                    if (payload != undefined && payload.DataRecordTypeId != undefined) {
                        dataRecordTypeId = payload.DataRecordTypeId;
                    }

                    if (existingStages != undefined && dataRecordTypeId != undefined) {
                        for (var j = 0; j < existingStages.length; j++) {
                            var currentExistingStage = existingStages[j];
                            if (currentExistingStage.DataRecordTypeId == dataRecordTypeId)
                                $scope.scopeModel.outputStages.push({ stageName: existingStages[j].stageName });
                        }
                    }

                    if (payload != undefined && payload.QueueActivator != undefined) {
                        if (payload.QueueActivator.MainOutputStages != undefined) {
                            for (var i = 0; i < payload.QueueActivator.MainOutputStages.length; i++) {
                                var currentMainOutputStage = payload.QueueActivator.MainOutputStages[i];
                                $scope.scopeModel.selectedMainOutputStages.push({ stageName: currentMainOutputStage });
                            }
                        }

                        if (payload.QueueActivator.PartialPricedOutputStages != undefined) {
                            for (var i = 0; i < payload.QueueActivator.PartialPricedOutputStages.length; i++) {
                                var currentPartialPricedOutputStage = payload.QueueActivator.PartialPricedOutputStages[i];
                                $scope.scopeModel.selectedPartialPricedOutputStages.push({ stageName: currentPartialPricedOutputStage });
                            }
                        }

                        if (payload.QueueActivator.BillingOutputStages != undefined) {
                            for (var i = 0; i < payload.QueueActivator.BillingOutputStages.length; i++) {
                                var currentBillingOutputStage = payload.QueueActivator.BillingOutputStages[i];
                                $scope.scopeModel.selectedBillingOutputStages.push({ stageName: currentBillingOutputStage });
                            }
                        }

                        if (payload.QueueActivator.TrafficOutputStages != undefined) {
                            for (var i = 0; i < payload.QueueActivator.TrafficOutputStages.length; i++) {
                                var currentTrafficOutputStage = payload.QueueActivator.TrafficOutputStages[i];
                                $scope.scopeModel.selectedTrafficOutputStages.push({ stageName: currentTrafficOutputStage });
                            }
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var selectedMainNextStages = [];
                    for (var i = 0; i < $scope.scopeModel.selectedMainOutputStages.length; i++) {
                        var currentMainSelectedOutputStage = $scope.scopeModel.selectedMainOutputStages[i];
                        selectedMainNextStages.push(currentMainSelectedOutputStage.stageName);
                    }

                    var selectedPartialPricedNextStages = [];
                    for (var i = 0; i < $scope.scopeModel.selectedPartialPricedOutputStages.length; i++) {
                        var currentPartialPricedSelectedOutputStage = $scope.scopeModel.selectedPartialPricedOutputStages[i];
                        selectedPartialPricedNextStages.push(currentPartialPricedSelectedOutputStage.stageName);
                    }

                    var selectedBillingNextStages = [];
                    for (var i = 0; i < $scope.scopeModel.selectedBillingOutputStages.length; i++) {
                        var currentBillingSelectedOutputStage = $scope.scopeModel.selectedBillingOutputStages[i];
                        selectedBillingNextStages.push(currentBillingSelectedOutputStage.stageName);
                    }

                    var selectedTrafficNextStages = [];
                    for (var i = 0; i < $scope.scopeModel.selectedTrafficOutputStages.length; i++) {
                        var currentTrafficSelectedOutputStage = $scope.scopeModel.selectedTrafficOutputStages[i];
                        selectedTrafficNextStages.push(currentTrafficSelectedOutputStage.stageName);
                    }

                    return {
                        $type: 'TOne.WhS.Deal.MainExtensions.QueueActivators.EvaluateSMSDealActivator, TOne.WhS.Deal.MainExtensions',
                        MainOutputStages: selectedMainNextStages,
                        PartialPricedOutputStages: selectedPartialPricedNextStages,
                        BillingOutputStages: selectedBillingNextStages,
                        TrafficOutputStages: selectedTrafficNextStages
                    };
                };

                return api;
            }
        }
    }

    app.directive('vrWhsDealQueueactivatorEvaluatesmsdeal', QueueActivatorEvaluateSMSDeal);
})(app);