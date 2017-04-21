(function (app) {

    'use strict';

    QueueActivatorStoreStagingRecords.$inject = ['UtilsService', 'VRUIUtilsService', 'Mediation_Generic_StorageStagingStatusEnum', 'VR_GenericData_DataRecordTypeService', 'VR_GenericData_DataRecordFieldAPIService'];

    function QueueActivatorStoreStagingRecords(UtilsService, VRUIUtilsService, Mediation_Generic_StorageStagingStatusEnum, VR_GenericData_DataRecordTypeService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var queueingStoreStagingRecordsQueueActivatorCtor = new QueueingStoreStagingRecordsQueueActivatorCtor(ctrl, $scope);
                queueingStoreStagingRecordsQueueActivatorCtor.initializeController();
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
                return '/Client/Modules/Mediation_Generic/Directives/QueueActivator/Templates/QueueActivatorStoreStagingRecordsTemplate.html';
            }
        };

        function QueueingStoreStagingRecordsQueueActivatorCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var filterObj;
            var dataRecoredId;

            var mediationDefinitionSelectorAPI;
            var mediationDefinitionSelectorAPIReadyDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.statusMappings = [];

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(getDirectiveAPI());
                };

                $scope.scopeModel.onMediationDefintionSelectorReady = function (api) {
                    mediationDefinitionSelectorAPI = api;
                    mediationDefinitionSelectorAPIReadyDeferred.resolve();
                };

            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {

                    var mediationDefinitionId;
                    if (payload != undefined && payload.QueueActivator != undefined) {
                        mediationDefinitionId = payload.QueueActivator.MediationDefinitionId;
                    }

                    dataRecoredId = payload.DataRecordTypeId;
                    var promises = [];


                    var mediationDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    mediationDefinitionSelectorAPIReadyDeferred.promise.then(function () {

                        var selectorPayload = {
                            dataRecordTypeId: payload.DataRecordTypeId,
                            selectedIds: mediationDefinitionId
                        };
                        VRUIUtilsService.callDirectiveLoad(mediationDefinitionSelectorAPI, selectorPayload, mediationDefinitionSelectorLoadDeferred);
                    });
                    promises.push(mediationDefinitionSelectorLoadDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: 'Mediation.Generic.QueueActivators.StoreStagingRecordsQueueActivator, Mediation.Generic.QueueActivators',
                        MediationDefinitionId: mediationDefinitionSelectorAPI.getSelectedIds()
                    };
                };

                return api;
            }


            function loadFields() {
                var obj = { DataRecordTypeId: dataRecoredId };
                var serializedFilter = UtilsService.serializetoJson(obj);
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecoredId, undefined);
            }
        }
    }

    app.directive('mediationGenericQueueactivatorStorestagingrecords', QueueActivatorStoreStagingRecords);

})(app);