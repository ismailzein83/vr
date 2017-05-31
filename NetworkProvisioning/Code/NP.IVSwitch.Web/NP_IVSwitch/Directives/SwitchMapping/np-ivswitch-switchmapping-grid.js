'use strict';

app.directive('npIvswitchSwitchmappingGrid', ['NP_IVSwitch_SwitchMappingAPIService', 'NP_IVSwitch_SwitchMappingService', 'VRNotificationService', 'NP_IVSwitch_CarrierAccountTypeEnum',
function (NP_IVSwitch_SwitchMappingAPIService, NP_IVSwitch_SwitchMappingService, VRNotificationService, NP_IVSwitch_CarrierAccountTypeEnum) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var switchMappingGrid = new SwitchMappingGrid($scope, ctrl, $attrs);
            switchMappingGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/NP_IVSwitch/Directives/SwitchMapping/Templates/SwitchMappingGridTemplate.html'
    };

    function SwitchMappingGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var gridAPI;
        function initializeController() {

            $scope.switchmappings = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
                    
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return NP_IVSwitch_SwitchMappingAPIService.GetFilteredSwitchMappings(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
                
            defineMenuActions();
        }

        function defineMenuActions() {

            $scope.gridMenuActions = function (dataItem) {
                

                var menuActions = [];
                var addEndPointMenu = {
                    name: "Link End Points",
                    clicked: linkEndPoints,
                    haspermission: hasLinkEndPointsPermission
                };
                var addRouteMenu = {
                    name: "Link Routes",
                    clicked: linkRoutes,
                    haspermission: hasLinkRoutesPermission
                };

                if (dataItem.CarrierAccountType == NP_IVSwitch_CarrierAccountTypeEnum.Exchange.value || dataItem.CarrierAccountType == NP_IVSwitch_CarrierAccountTypeEnum.Customer.value)
                    menuActions.push(addEndPointMenu);
                if (dataItem.CarrierAccountType == NP_IVSwitch_CarrierAccountTypeEnum.Exchange.value || dataItem.CarrierAccountType == NP_IVSwitch_CarrierAccountTypeEnum.Supplier.value)
                    menuActions.push(addRouteMenu);

                return menuActions;                 
            };
        }
        function linkEndPoints(dataItem) {
            var onEndPointLinked = function (updatedSwitchMapping) {
                gridAPI.itemUpdated(updatedSwitchMapping);
            };
            NP_IVSwitch_SwitchMappingService.linkEndPoints(dataItem.CarrierAccountId, onEndPointLinked);
        }

        function hasLinkEndPointsPermission() {
            return NP_IVSwitch_SwitchMappingAPIService.HasLinkEndPointsPermission();
        }
        function linkRoutes(dataItem) {
            var onRouteLinked = function (updatedSwitchMapping) {
                gridAPI.itemUpdated(updatedSwitchMapping);
            };
            NP_IVSwitch_SwitchMappingService.linkRoutes(dataItem.CarrierAccountId, onRouteLinked);
        }
        function hasLinkRoutesPermission() {
            return NP_IVSwitch_SwitchMappingAPIService.HasLinkRoutesPermission();

        }
           
    }
}]);

