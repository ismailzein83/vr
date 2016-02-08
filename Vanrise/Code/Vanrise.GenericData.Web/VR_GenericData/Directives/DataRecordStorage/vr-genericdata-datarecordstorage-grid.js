(function (app) {

    'use strict';

    DataRecordStorageGridDirective.$inject = ['VR_GenericData_DataRecordStorageAPIService', 'VR_GenericData_DataRecordStorageService', 'VRNotificationService'];

    function DataRecordStorageGridDirective(VR_GenericData_DataRecordStorageAPIService, VR_GenericData_DataRecordStorageService, VRNotificationService) {
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

            function initializeController() {
                ctrl.dataRecordStorages = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_DataRecordStorageAPIService.GetFilteredDataRecordStorages(dataRetrievalInput).then(function (response) {
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

                api.onGenericRuleDefinitionAdded = function (addedGenericRuleDefinition) {
                    gridAPI.itemAdded(addedGenericRuleDefinition);
                };

                return api;
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editDataRecordStorage,
                }];
            }

            function editDataRecordStorage(dataRecordStorage) {
                var onDataRecordStorageUpdated = function (updatedDataRecordStorage) {
                    gridAPI.itemUpdated(updatedDataRecordStorage);
                };
                VR_GenericData_DataRecordStorageService.editDataRecordStorage(dataRecordStorage.DataRecordStorageId, onDataRecordStorageUpdated);
            }
        }
    }

    app.directive('vrGenericdataDatarecordstorageGrid', DataRecordStorageGridDirective);

})(app);