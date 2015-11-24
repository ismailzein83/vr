"use strict";

app.directive("vrWhsBeAssignedcarriersGrid", ["VRNotificationService", "WhS_BE_AccountManagerAPIService",
function (VRNotificationService, WhS_BE_AccountManagerAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var assignedCarriersGrid = new AssignedCarriersGrid($scope, ctrl, $attrs);
            assignedCarriersGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/AccountManager/Templates/AssignedCarriersGrid.html"

    };

    function AssignedCarriersGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            ctrl.assignedCarriers = [];
            ctrl.gridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        if (query.ManagerId!=undefined)
                            ctrl.showGrid=true;
                        return gridAPI.retrieveData(query);
                    }
                    return directiveAPI;
                }
            };
            ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_AccountManagerAPIService.GetFilteredAssignedCarriers(dataRetrievalInput)
                .then(function (response) {
                    response.Data = getMappedAssignedCarriers(response.Data);
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
            function getMappedAssignedCarriers(assignedCarriers) {
                var mappedCarriers = [];

                angular.forEach(assignedCarriers, function (item) {

                    var gridObject = {
                        Entity: { CarrierAccountId: item.Entity.CarrierAccountId },
                        CarrierName: item.CarrierName,
                        IsCustomerAssigned: item.IsCustomerAssigned,
                        IsSupplierAssigned:  item.IsSupplierAssigned,
                        IsCustomerInDirect:(item.IsCustomerInDirect) ? ' (Indirect)':'',
                        IsSupplierInDirect: (item.IsSupplierInDirect) ? ' (Indirect)' : '',
                    };

                    mappedCarriers.push(gridObject);
                });

                return mappedCarriers;
            }

        }

      
    }

    return directiveDefinitionObject;

}]);
