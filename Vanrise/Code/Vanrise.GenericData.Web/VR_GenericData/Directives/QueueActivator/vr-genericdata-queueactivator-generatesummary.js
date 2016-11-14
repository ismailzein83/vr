(function (app) {

    'use strict';

    QueueActivatorGenerateSummary.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_SummaryTransformationDefinitionAPIService'];

    function QueueActivatorGenerateSummary(UtilsService, VRUIUtilsService, VR_GenericData_SummaryTransformationDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var queueingGenerateSummaryQueueActivatorCtor = new QueueingGenerateSummaryQueueActivatorCtor(ctrl, $scope);
                queueingGenerateSummaryQueueActivatorCtor.initializeController();
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
                return '/Client/Modules/VR_GenericData/Directives/QueueActivator/Templates/QueueActivatorGenerateSummaryTemplate.html';
            }
        };

        function QueueingGenerateSummaryQueueActivatorCtor(ctrl, $scope) {

            $scope.scopeModal = {};
            $scope.scopeModal.nextStages = [];
            $scope.scopeModal.selectedNextStage;
            this.initializeController = initializeController;

            var existingStages;
            var nextStageName;
            var dataSummaryTransformationDefinitionSelectorAPI;
            var summaryTransformationDefinitionSelectedPromiseDeferred;

            function initializeController() {
                ctrl.onSummaryTransformationDefinitionSelectorReady = function (api) {
                    dataSummaryTransformationDefinitionSelectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.onSelectionChangedSummaryTransformationDefinition = function () {

                    var selectedSummaryTransformationDefinitionId = dataSummaryTransformationDefinitionSelectorAPI.getSelectedIds();
                    if (selectedSummaryTransformationDefinitionId != undefined) {
                        ctrl.isLoadingGenerateSummaryActivatorSection = true;
                        VR_GenericData_SummaryTransformationDefinitionAPIService.GetSummaryTransformationDefinition(selectedSummaryTransformationDefinitionId)
                           .then(function (response) {
                               $scope.scopeModal.nextStages.length = 0;
                               angular.forEach(existingStages, function (item) {
                                   if (item.DataRecordTypeId == response.SummaryItemRecordTypeId)
                                       $scope.scopeModal.nextStages.push(item);
                               });
                               if (nextStageName == undefined)
                                   $scope.scopeModal.selectedNextStage = undefined;
                               else {
                                   $scope.scopeModal.selectedNextStage = UtilsService.getItemByVal($scope.scopeModal.nextStages, nextStageName, "stageName");
                                   nextStageName = undefined;
                               }
                               if (summaryTransformationDefinitionSelectedPromiseDeferred != undefined)
                                   summaryTransformationDefinitionSelectedPromiseDeferred.resolve();
                           })
                        .catch(function (error) {
                            summaryTransformationDefinitionSelectedPromiseDeferred.reject(error);
                        })
                        .finally(function () {
                            ctrl.isLoadingGenerateSummaryActivatorSection = false;
                        });
                    }
                };

            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedId;
                    if (payload != undefined && payload.QueueActivator != undefined) {
                        selectedId = payload.QueueActivator.SummaryTransformationDefinitionId;
                        nextStageName = payload.QueueActivator.NextStageName;
                    }

                    if (payload != undefined && payload.ExistingStages != undefined) {
                        existingStages = payload.ExistingStages;
                    }

                    return loadSelector();

                    function loadSelector() {
                        var promises = [];

                        var dataSummaryTransformationDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        promises.push(dataSummaryTransformationDefinitionSelectorLoadDeferred.promise);

                        if (nextStageName != undefined) {
                            summaryTransformationDefinitionSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(summaryTransformationDefinitionSelectedPromiseDeferred.promise);
                        }

                        var selectorPayload = {};
                        var filter = {};
                        filter.RawItemRecordTypeId = payload.DataRecordTypeId;
                        selectorPayload.filter = filter;

                        if (selectedId != undefined)
                            selectorPayload.selectedIds = selectedId;

                        VRUIUtilsService.callDirectiveLoad(dataSummaryTransformationDefinitionSelectorAPI, selectorPayload, dataSummaryTransformationDefinitionSelectorLoadDeferred);



                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.QueueActivators.GenerateSummaryQueueActivator, Vanrise.GenericData.QueueActivators',
                        SummaryTransformationDefinitionId: dataSummaryTransformationDefinitionSelectorAPI.getSelectedIds(),
                        NextStageName: $scope.scopeModal.selectedNextStage.stageName
                    };
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataQueueactivatorGeneratesummary', QueueActivatorGenerateSummary);

})(app);