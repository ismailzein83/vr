(function (app) {

    'use strict';

    QueueActivatorUpdateSummary.$inject = ['UtilsService', 'VRUIUtilsService'];

    function QueueActivatorUpdateSummary(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var queueingUpdateSummaryQueueActivatorCtor = new QueueingUpdateSummaryQueueActivatorCtor(ctrl, $scope);
                queueingUpdateSummaryQueueActivatorCtor.initializeController();
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
                return '/Client/Modules/VR_GenericData/Directives/QueueActivator/Templates/QueueActivatorUpdateSummaryTemplate.html';
            }
        };

        function QueueingUpdateSummaryQueueActivatorCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var dataSummaryTransformationDefinitionSelectorAPI;

            function initializeController() {
                ctrl.onSummaryTransformationDefinitionSelectorReady = function (api) {
                    dataSummaryTransformationDefinitionSelectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedId;

                    if (payload != undefined && payload.QueueActivator != undefined) {
                        selectedId = payload.QueueActivator.SummaryTransformationDefinitionId;
                    }

                    return loadSelector();

                    function loadSelector() {
                        var dataSummaryTransformationDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        var selectorPayload = {};
                        var filter = {}
                        filter.SummaryItemRecordTypeId = payload.DataRecordTypeId;
                        selectorPayload.filter = filter;

                        if (selectedId != undefined)
                            selectorPayload.selectedIds = selectedId;

                        VRUIUtilsService.callDirectiveLoad(dataSummaryTransformationDefinitionSelectorAPI, selectorPayload, dataSummaryTransformationDefinitionSelectorLoadDeferred);
                        return dataSummaryTransformationDefinitionSelectorLoadDeferred.promise;
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.QueueActivators.UpdateSummaryQueueActivator, Vanrise.GenericData.QueueActivators',
                        SummaryTransformationDefinitionId: dataSummaryTransformationDefinitionSelectorAPI.getSelectedIds()
                    };
                };

                api.getQueueItemType = function () {
                    return {
                        $type: "Vanrise.GenericData.QueueActivators.GenericSummaryRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators"
                    };
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataQueueactivatorUpdatesummary', QueueActivatorUpdateSummary);

})(app);