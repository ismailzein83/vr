"use strict";

app.directive("vrWhsBeSupplierzoneserviceGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SupplierZoneServiceAPIService", "WhS_BE_SupplierZoneService", "WhS_BE_SupplierEntityServiceSourceEnum", 'VRUIUtilsService',
function (UtilsService, VRNotificationService, WhS_BE_SupplierZoneServiceAPIService, WhS_BE_SupplierZoneService, WhS_BE_SupplierEntityServiceSourceEnum, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SupplierZoneServiceGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SupplierZoneService/Templates/SupplierZoneServiceGridTemplate.html"

    };

    function SupplierZoneServiceGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var supplierId;
        var effectiveOn;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.supplierZoneServices = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        supplierId = query.SupplierId;
                        effectiveOn = query.EffectiveOn;
                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SupplierZoneServiceAPIService.GetFilteredSupplierZoneServices(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {

                            var serviceViewerLoadPromises = [];

                            for (var i = 0 ; i < response.Data.length; i++) {
                                var dataItem = response.Data[i];
                                extendDataItem(dataItem);
                                serviceViewerLoadPromises.push(dataItem.serviceViewerLoadDeferred.promise);
                            }
                            UtilsService.waitMultiplePromises(serviceViewerLoadPromises);
                        }
                        onResponseReady(response);

                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }
        function defineMenuActions() {
            $scope.gridMenuActions = function (dataItem)
            {
                var menuActions;
                if (dataItem.ZoneEED == undefined)
                {
                    menuActions = [{
                        name: "Change",
                        clicked: editSupplierService,
                        haspermission: hasEditSupplierZoneServicePermission
                    }];
                }
                return menuActions;
            };
            
        }

        function editSupplierService(supplierServiceObj) {
            var onSupplierServiceUpdated = function (updatedObj) {
                extendDataItem(updatedObj);
                gridAPI.itemUpdated(updatedObj);
            };

            WhS_BE_SupplierZoneService.editSupplierService(supplierServiceObj, supplierId, effectiveOn, onSupplierServiceUpdated);
        }

        function hasEditSupplierZoneServicePermission(){
           return  WhS_BE_SupplierZoneServiceAPIService.HasUpdateSupplierZoneServicePermission();
        }

        function extendDataItem(dataItem) {

            defineServiceViewerProperties();
            defineIconProperties();

            function defineServiceViewerProperties() {

                dataItem.serviceViewerLoadDeferred = UtilsService.createPromiseDeferred();

                dataItem.onServiceViewerReady = function (api) {
                    dataItem.serviceViewerAPI = api;
                    var serviceViewerPayload = {
                        selectedIds: dataItem.Services
                    };
                    VRUIUtilsService.callDirectiveLoad(api, serviceViewerPayload, dataItem.serviceViewerLoadDeferred);
                };

            }
            function defineIconProperties() {
                if (dataItem.Source == WhS_BE_SupplierEntityServiceSourceEnum.Supplier.value)
                    dataItem.iconType = "Inherited From Supplier";
            }
        }
    }

    return directiveDefinitionObject;

}]);
