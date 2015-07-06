DataSourceEditorController.$inject = ['$scope', 'DataSourceAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function DataSourceEditorController($scope, DataSourceAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {

    var editMode;
    var dataSourceId;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        dataSourceId = undefined;

        if (parameters != undefined && parameters != null) {
            dataSourceId = parameters.dataSourceId;
        }


        if (dataSourceId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {
        $scope.SaveDataSource = function () {
            if (editMode) {
                return updateDataSource();
            }
            else {
                return insertDataSource();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.adapterTypes = [];
        $scope.dataSourceAdapter = {};

    }

    function load() {

        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadAdapters]).finally(function () {
            if (editMode) {
                getDataSource();
            }
            else {
                $scope.selectedAdapterType = UtilsService.getItemByVal($scope.adapterTypes, 1, "AdapterTypeId");
            }

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isGettingData = false;
        });


    }

    function getDataSource() {
        return DataSourceAPIService.GetDataSource(dataSourceId)
           .then(function (response) {
               fillScopeFromDataSourceObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function loadAdapters() {
        return DataSourceAPIService.GetDataSourceAdapterTypes().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.adapterTypes.push(item);
            });
        });
    }

    function buildDataSourceObjFromScope() {

        var dataSourceObject = {
            DataSourceId: (dataSourceId != null) ? dataSourceId : 0,
            AdapterTypeId: $scope.selectedAdapterType.AdapterTypeId,
            Settings: { Adapter: $scope.dataSourceAdapter.getData() }
        };
        return dataSourceObject;
    }

    function fillScopeFromDataSourceObj(dataSourceObject) {
        $scope.selectedAdapterType = UtilsService.getItemByVal($scope.adapterTypes, dataSourceObject.AdapterTypeId, "AdapterTypeId");
        $scope.dataSourceAdapter.data = dataSourceObject.Settings.Adapter;
    }

    function insertDataSource() {
        $scope.issaving = true;
        var dataSourceObject = buildDataSourceObjFromScope();
        return DataSourceAPIService.AddDataSource(dataSourceObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Data Source", response)) {
                if ($scope.onDataSourceAdded != undefined)
                    $scope.onDataSourceAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });

    }

    function updateDataSource() {
        var dataSourceObject = buildDataSourceObjFromScope();
        DataSourceAPIService.UpdateDataSource(dataSourceObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Data Source", response)) {
                if ($scope.onDataSourceUpdated != undefined)
                    $scope.onDataSourceUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }
}
appControllers.controller('Integration_DataSourceEditorController', DataSourceEditorController);
