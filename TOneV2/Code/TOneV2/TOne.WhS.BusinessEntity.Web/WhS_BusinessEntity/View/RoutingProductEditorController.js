RoutingProductEditorController.$inject = ['$scope', 'RoutingProductAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

function RoutingProductEditorController($scope, RoutingProductAPIService, UtilsService, VRNotificationService, VRNavigationService) {

    var editMode;
    var routingProductId;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            routingProductId = parameters.routingProductId;
        }

        editMode = (routingProductId != undefined);
    }

    function defineScope() {
        $scope.SaveRoutingProduct = function () {
            if (editMode) {
                return updateRoutingProduct();
            }
            else {
                return insertRoutingProduct();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.saleZoneGroups = {};
        $scope.suppliersGroups = {};

        //$scope.adapterTypes = [];
        //$scope.executionFlows = [];
        //$scope.dataSourceAdapter = {};
        //$scope.dataSourceAdapter.argument = {};
        //$scope.dataSourceAdapter.adapterState = {};

        //$scope.schedulerTaskTrigger = {};
        //$scope.timeTriggerTemplateURL = undefined;

        //$scope.startEffDate = new Date();
        //$scope.endEffDate = undefined;
    }

    function load() {
        $scope.isGettingData = true;
        return UtilsService.waitMultipleAsyncOperations([loadSaleZoneGroups, loadSuppliersGroups]).then(function () {
            if (editMode) {
                getRoutingProduct();
            }
            else {
                $scope.isGettingData = false;
            }

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.isGettingData = false;
        });
    }

    function getRoutingProduct() {
        return DataSourceAPIService.GetDataSource(dataSourceId).then(function (routingProduct) {

            fillScopeFromRoutingProductObj(routingProduct);

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isGettingData = false;
        });
    }

    function loadSaleZoneGroups() {
        //return DataSourceAPIService.GetDataSourceAdapterTypes().then(function (response) {
        //    angular.forEach(response, function (item) {
        //        $scope.adapterTypes.push(item);
        //    });
        //});
    }

    function loadSuppliersGroups() {
        //return DataSourceAPIService.GetExecutionFlows().then(function (response) {
        //    angular.forEach(response, function (item) {
        //        $scope.executionFlows.push(item);
        //    });
        //});
    }

    function buildRoutingProductObjFromScope() {

        var routingProduct = {
            RoutingProductId: (routingProductId != null) ? routingProductId : 0,
            Name: $scope.routingProductName,
            Settings: { Zones: $scope.saleZoneGroups.getData(), Suppliers: $scope.suppliersGroup.getData()}
        };

        return routingProduct;
    }

    function fillScopeFromRoutingProductObj(routingProductObj) {
        $scope.routingProductName = routingProductObj.Name;

        $scope.saleZoneGroups.data = routingProductObj.Settings.Zones;

        if ($scope.saleZoneGroups.loadTemplateData != undefined)
            $scope.saleZoneGroups.loadTemplateData();

        $scope.suppliersGroup.data = routingProductObj.Settings.Suppliers;

        if ($scope.suppliersGroup.loadTemplateData != undefined)
            $scope.suppliersGroup.loadTemplateData();
    }

    function insertDataSource() {
        var routingProductObject = buildRoutingProductObjFromScope();
        return RoutingProductAPIService.AddRoutingProduct(routingProductObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Routing Product", response)) {
                if ($scope.onRoutingProductAdded != undefined)
                    $scope.onRoutingProductAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });

    }

    function updateDataSource() {
        var dataSourceObject = buildRoutingProductObjFromScope();
        RoutingProductAPIService.UpdateRoutingProduct(routingProductObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Data Source", response)) {
                if ($scope.onRoutingProductUpdated != undefined)
                    $scope.onRoutingProductUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }
}
appControllers.controller('WhS_BusinessEntity_RoutingProductEditorController', RoutingProductEditorController);
