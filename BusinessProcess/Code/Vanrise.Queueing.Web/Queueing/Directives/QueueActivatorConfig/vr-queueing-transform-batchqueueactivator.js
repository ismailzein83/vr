(function (app) {

    'use strict';

    QueueingStoreBatchQueueActivator.$inject = ['UtilsService', 'VRUIUtilsService'];

    function QueueingStoreBatchQueueActivator(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var queueingTransformBatchQueueActivatorCtor = new QueueingTransformBatchQueueActivatorCtor(ctrl, $scope);
                queueingTransformBatchQueueActivatorCtor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Queueing/Directives/QueueActivatorConfig/Templates/TransformBatchQueueActivatorTemplate.html';
            }
        };

        function QueueingTransformBatchQueueActivatorCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedId;
                    
                    if (payload != undefined) {
                        selectedId = payload.DataTransformationDefinitionId;
                        ctrl.SourceRecordName = payload.SourceRecordName;
                    }

                    return loadSelector();

                    function loadSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                        var selectorPayload = { selectedIds: selectedId };
                        VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);
                        return selectorLoadDeferred.promise;
                    }
                }

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.QueueActivators.TransformBatchQueueActivator, Vanrise.GenericData.QueueActivators',
                        SourceRecordName:ctrl.SourceRecordName,
                        DataTransformationDefiniSourceRecordNametionId: selectorAPI.getSelectedIds()
                    };
                }

                return api;
            }
        }
    }

    app.directive('vrQueueingTransformBatchqueueactivator', QueueingStoreBatchQueueActivator);

})(app);