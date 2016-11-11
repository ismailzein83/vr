"use strict";

app.directive("vrWhsBeSupplierzoneserviceGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SupplierZoneServiceAPIService",
function (UtilsService, VRNotificationService, WhS_BE_SupplierZoneServiceAPIService) {

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
                       
                        return gridAPI.retrieveData(query);
                    }
                   
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SupplierZoneServiceAPIService.GetFilteredSupplierZoneServices(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0 ; i < response.Data.length; i++) {
                                addReadySericeApi(response.Data[i]);
                            }
                        }
                         onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        }

        var addReadySericeApi = function (dataItem) {
            dataItem.onServiceReady = function (api) {
                dataItem.ServieApi = api
                dataItem.ServieApi.load({ selectedIds: dataItem.Services });
            }
        };
    }

    return directiveDefinitionObject;

}]);
