(function (app) {

    'use strict';

    DataRecordStorageGridDirective.$inject = ['VR_GenericData_DataRecordStorageAPIService', 'VR_GenericData_DataRecordStorageService', 'VRNotificationService', 'VRUIUtilsService'];

    function DataRecordStorageGridDirective(VR_GenericData_DataRecordStorageAPIService, VR_GenericData_DataRecordStorageService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordStorageGrid = new DataRecordStorageGrid($scope, ctrl, $attrs);
                dataRecordStorageGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/DataRecordStorage/Templates/DataRecordStorageGridTemplate.html'
        };

        function DataRecordStorageGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                ctrl.dataRecordStorages = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_GenericData_DataRecordStorageService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_DataRecordStorageAPIService.GetFilteredDataRecordStorages(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onDataRecordStorageAdded = function (addedDataRecordStorage) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedDataRecordStorage);
                    gridAPI.itemAdded(addedDataRecordStorage);
                };

                return api;
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editDataRecordStorage,
                    haspermission: hasEditDataRecordStorage 
                }];
            }

            function hasEditDataRecordStorage() {
                return VR_GenericData_DataRecordStorageAPIService.HasUpdateDataRecordStorage();
            }
            function editDataRecordStorage(dataRecordStorage) {
                var onDataRecordStorageUpdated = function (updatedDataRecordStorage) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedDataRecordStorage);
                    gridAPI.itemUpdated(updatedDataRecordStorage);
                };
                VR_GenericData_DataRecordStorageService.editDataRecordStorage(dataRecordStorage.Entity.DataRecordStorageId, onDataRecordStorageUpdated);
            }
        }
    }

    app.directive('vrGenericdataDatarecordstorageGrid', DataRecordStorageGridDirective);

})(app);