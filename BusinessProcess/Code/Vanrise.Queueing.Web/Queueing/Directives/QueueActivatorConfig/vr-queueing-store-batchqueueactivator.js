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
                var queueingStoreBatchQueueActivatorCtor = new QueueingStoreBatchQueueActivatorCtor(ctrl, $scope);
                queueingStoreBatchQueueActivatorCtor.initializeController();
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
                return '/Client/Modules/Queueing/Directives/QueueActivatorConfig/Templates/StoreBatchQueueActivatorTemplate.html';
            }
        };

        function QueueingStoreBatchQueueActivatorCtor(ctrl, $scope) {
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

                    if (payload != undefined && payload.QueueActivator != undefined) {
                        selectedId = payload.QueueActivator.DataRecordStorageId;
                    }

                    return loadSelector();

                    function loadSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                        var selectorPayload = {};
                        selectorPayload.DataRecordTypeId = payload.DataRecordTypeId;
                        if (selectedId != undefined)
                            selectorPayload.selectedIds = selectedId;
                        VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);
                        return selectorLoadDeferred.promise;
                    }
                }

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.QueueActivators.StoreBatchQueueActivator, Vanrise.GenericData.QueueActivators',
                        DataRecordStorageId: selectorAPI.getSelectedIds()
                    };
                }

                return api;
            }
        }
    }

    app.directive('vrQueueingStoreBatchqueueactivator', QueueingStoreBatchQueueActivator);

})(app);