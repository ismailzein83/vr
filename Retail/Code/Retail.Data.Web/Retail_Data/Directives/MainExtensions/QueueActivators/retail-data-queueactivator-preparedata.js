(function (app) {

    'use strict';

    QueueActivatorPrepareData.$inject = ['UtilsService', 'VRUIUtilsService'];

    function QueueActivatorPrepareData(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var prepareDataActivatorCtor = new PrepareDataActivatorCtor(ctrl, $scope);
                prepareDataActivatorCtor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_Data/Directives/MainExtensions/QueueActivators/Templates/QueueActivatorPrepareDataTemplate.html';
            }
        };

        function PrepareDataActivatorCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.outputStages = [];
                $scope.scopeModel.selectedOutputStages = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var queueActivator;
                    var existingStages;
                    var dataRecordTypeId;

                    if (payload != undefined) {
                        queueActivator = payload.QueueActivator != undefined ? payload.QueueActivator : undefined;
                        existingStages = payload.ExistingStages != undefined ? payload.ExistingStages : undefined;
                        dataRecordTypeId = payload.DataRecordTypeId != undefined ? payload.DataRecordTypeId : undefined;
                    }

                    if (existingStages != undefined && dataRecordTypeId != undefined) {
                        for (var j = 0; j < existingStages.length; j++) {
                            var currentExistingStage = existingStages[j];
                            if (currentExistingStage.DataRecordTypeId == dataRecordTypeId)
                                $scope.scopeModel.outputStages.push({ stageName: existingStages[j].stageName });
                        }
                    }

                    if (queueActivator != undefined && queueActivator.OutputStages != undefined) {
                        for (var i = 0; i < payload.QueueActivator.OutputStages.length; i++) {
                            var currentOutputStage = queueActivator.OutputStages[i];
                            $scope.scopeModel.selectedOutputStages.push({ stageName: currentOutputStage });
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    
                    var selectedOutputStages;
                    if($scope.scopeModel.selectedOutputStages.length > 0)
                        selectedOutputStages  = UtilsService.getPropValuesFromArray($scope.scopeModel.selectedOutputStages, "stageName");

                    return {
                        $type: 'Retail.Data.MainExtensions.QueueActivators.PrepareDataActivator, Retail.Data.MainExtensions',
                        OutputStages: selectedOutputStages
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }
    }
    app.directive('retailDataQueueactivatorPreparedata', QueueActivatorPrepareData);
})(app);