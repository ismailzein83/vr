'use strict';

restrict: 'E',
app.directive('mediationOutputhandlerStorerecordsEditor', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var editor = new StoreRecordsEditor($scope, ctrl, $attrs);
            editor.initializeController();
        },
        controllerAs: 'storeCtrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Mediation_Generic/Directives/MainExtensions/OutPutHandler/Templates/OutPutHandlerStoreRecordsEditorTemplate.html'
    };

    function StoreRecordsEditor($scope, ctrl, $attrs) {
        var dataStoreSelectorAPI;
        var dataStoreSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        $scope.scopeModel = {};
        $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
            dataStoreSelectorAPI = api;
            dataStoreSelectorReadyDeferred.resolve();
        };
        this.initializeController = initializeController;
        function initializeController() {            
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var dataStoreSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                dataStoreSelectorReadyDeferred.promise.then(function () {
                    var slectorPayload = {
                        selectedIds: payload!=undefined && payload.data!=undefined && payload.data.DataRecordStorageId || undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(dataStoreSelectorAPI, slectorPayload, dataStoreSelectorLoadDeferred);
                });

                return dataStoreSelectorLoadDeferred.promise;
               
            };

            api.getData = function () {

                return {
                    $type: 'Mediation.Generic.MainExtensions.MediationOutputHandlers.StoreRecordsOutputHandler, Mediation.Generic.MainExtensions',
                    DataRecordStorageId: dataStoreSelectorAPI.getSelectedIds()

                };
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }
    }
}]);