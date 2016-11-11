(function (app) {

    'use strict';

    QueueActivatorStoreBatch.$inject = ['UtilsService', 'VRUIUtilsService'];

    function QueueActivatorStoreBatch(UtilsService, VRUIUtilsService) {
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
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/QueueActivator/Templates/QueueActivatorStoreBatchTemplate.html';
            }
        };

        function QueueingStoreBatchQueueActivatorCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var dataRecordStorageSelectorAPI;

            function initializeController() {
                ctrl.onDataRecordStorageSelectorReady = function (api) {
                    dataRecordStorageSelectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    return loadSelector(payload);
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.QueueActivators.StoreBatchQueueActivator, Vanrise.GenericData.QueueActivators',
                        DataRecordStorageId: dataRecordStorageSelectorAPI.getSelectedIds()
                    };
                };

                return api;
            }

            function loadSelector(payload) {
                var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                var selectedId;
                if (payload != undefined && payload.QueueActivator != undefined) {
                    selectedId = payload.QueueActivator.DataRecordStorageId;
                }

                var selectorPayload = {};
                selectorPayload.DataRecordTypeId = payload.DataRecordTypeId;
                if (selectedId != undefined)
                    selectorPayload.selectedIds = selectedId;
                VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, selectorPayload, dataRecordStorageSelectorLoadDeferred);
                return dataRecordStorageSelectorLoadDeferred.promise;
            }
        }
    }

    app.directive('vrGenericdataQueueactivatorStorebatch', QueueActivatorStoreBatch);

})(app);