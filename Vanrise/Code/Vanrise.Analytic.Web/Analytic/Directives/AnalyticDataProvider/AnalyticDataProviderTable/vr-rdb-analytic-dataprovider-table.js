(function (app) {

    'use strict';

    RdbAnalyticDataproviderTableSelector.$inject = ['UtilsService', 'VRUIUtilsService'];

    function RdbAnalyticDataproviderTableSelector(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var analyticDataProviderSettings = new AnalyticDataProviderSettings($scope, ctrl, $attrs);
                analyticDataProviderSettings.initializeController();
            },
            controllerAs: "providerTableCtrl",
            bindToController: true,
            template: function (element, attrs) {
                var template = '<vr-genericdata-datarecordstorage-selector on-ready="scopeModel.onDataRecordStorageSelectorReady" isrequired="true"' +
                    'selectedvalues = "scopeModel.selectedDataRecordStorages">' +
                    '</vr-genericdata-datarecordstorage-selector>';

                return template;
            }
        };

        function AnalyticDataProviderSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordStorageAPI;
            var dataRecordStorageReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                    dataRecordStorageAPI = api;
                    dataRecordStorageReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var selectedId;

                    if (payload != undefined && payload.analyticDataProviderTable != undefined) {
                        selectedId = payload.analyticDataProviderTable.RecordStorageId;
                    }
                    var dataRecordStorageLoadPromise = loadDataRecordStorage();
                    promises.push(dataRecordStorageLoadPromise);

                    function loadDataRecordStorage() {
                        var dataRecordStorageLoadDeferred = UtilsService.createPromiseDeferred();
                     
                        dataRecordStorageReadyPromiseDeferred.promise.then(function () {
                            var dataRecordStoragePayload = {
                                filters: [{
                                    $type: "Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageFilter, Vanrise.GenericData.RDBDataStorage"
                                }]
                            };

                            if (selectedId != undefined)
                                dataRecordStoragePayload.selectedIds = selectedId;
                   
                            VRUIUtilsService.callDirectiveLoad(dataRecordStorageAPI, dataRecordStoragePayload, dataRecordStorageLoadDeferred);
                        });
                        return dataRecordStorageLoadDeferred.promise;
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.RDBDataStorage.RecordStorageRDBAnalyticDataProviderTable, Vanrise.GenericData.RDBDataStorage',
                        RecordStorageId: dataRecordStorageAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrRdbAnalyticDataproviderTable', RdbAnalyticDataproviderTableSelector);

})(app);