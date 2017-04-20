(function (app) {

    'use strict';

    QueueActivatorDistributeBatch.$inject = ['UtilsService', 'VRUIUtilsService'];

    function QueueActivatorDistributeBatch( UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var queueingDistributeBatchQueueActivatorCtor = new QueueingDistributeBatchQueueActivatorCtor(ctrl, $scope);
                queueingDistributeBatchQueueActivatorCtor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/QueueActivator/Templates/QueueActivatorDistributeBatchTemplate.html';
            }
        };

        function QueueingDistributeBatchQueueActivatorCtor(ctrl, $scope) {
            this.initializeController = initializeController;
            $scope.scopeModel = { outputStages: [], selectedOutputStages: [] };

            function initializeController() {
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(getDirectiveAPI());
                }
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var dataRecordTypeId;
                    var existingStages;

                    var promises = [];


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

                    if (payload != undefined && payload.QueueActivator != undefined && payload.QueueActivator.OutputStages != undefined) {
                        for (var i = 0; i < payload.QueueActivator.OutputStages.length; i++) {
                            var currentOutputStage = payload.QueueActivator.OutputStages[i];
                            $scope.scopeModel.selectedOutputStages.push({ stageName: currentOutputStage })
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    var selectedNextStages = [];
                    for (var i = 0; i < $scope.scopeModel.selectedOutputStages.length; i++) {
                        var currentSelectedOutputStage = $scope.scopeModel.selectedOutputStages[i];
                        selectedNextStages.push(currentSelectedOutputStage.stageName);
                    }

                    return {
                        $type: 'Vanrise.GenericData.QueueActivators.DistributeBatchQueueActivator, Vanrise.GenericData.QueueActivators',
                        OutputStages: selectedNextStages,
                    };
                };

                return api;
            }
        }
    }
    app.directive('vrGenericdataQueueactivatorDistributebatch', QueueActivatorDistributeBatch);

})(app);