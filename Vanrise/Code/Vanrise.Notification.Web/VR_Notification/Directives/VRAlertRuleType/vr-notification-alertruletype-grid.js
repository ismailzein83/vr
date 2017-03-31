'use strict';

app.directive('vrNotificationAlertruletypeGrid', ['VR_Notification_VRAlertRuleTypeAPIService', 'VR_Notification_VRAlertRuleTypeService', 'VRNotificationService', 'VRUIUtilsService',
    function (VR_Notification_VRAlertRuleTypeAPIService, VR_Notification_VRAlertRuleTypeService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrAlertRuleTypeGrid = new VRAlertRuleTypeGrid($scope, ctrl, $attrs);
                vrAlertRuleTypeGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Notification/Directives/VRAlertRuleType/Templates/VRAlertRuleTypeGridTemplate.html'
        };

        function VRAlertRuleTypeGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.vrAlertRuleType = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_Notification_VRAlertRuleTypeService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Notification_VRAlertRuleTypeAPIService.GetFilteredVRAlertRuleTypes(dataRetrievalInput).then(function (response) {
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

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onVRAlertRuleTypeAdded = function (addedVRAlertRuleType) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedVRAlertRuleType);
                    gridAPI.itemAdded(addedVRAlertRuleType);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editVRAlertRuleType,
                    haspermission: hasEditVRAlertRuleTypePermission
                });
            }
            function editVRAlertRuleType(vrAlertRuleTypeItem) {
                var onVRAlertRuleTypeUpdated = function (updatedVRAlertRuleType) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedVRAlertRuleType);
                    gridAPI.itemUpdated(updatedVRAlertRuleType);
                };

                VR_Notification_VRAlertRuleTypeService.editVRAlertRuleType(vrAlertRuleTypeItem.Entity.VRAlertRuleTypeId, onVRAlertRuleTypeUpdated);
            }
            function hasEditVRAlertRuleTypePermission() {
                return VR_Notification_VRAlertRuleTypeAPIService.HasEditVRAlertRuleTypePermission();
            }
        }
}]);
