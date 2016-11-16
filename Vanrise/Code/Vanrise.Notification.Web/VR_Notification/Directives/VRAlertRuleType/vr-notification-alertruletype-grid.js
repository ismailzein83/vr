'use strict';

app.directive('vrNotificationAlertruletypeGrid', ['VR_Notification_VRAlertRuleTypeAPIService', 'VR_Notification_VRAlertRuleTypeService', 'VRNotificationService',
    function (VR_Notification_VRAlertRuleTypeAPIService, VR_Notification_VRAlertRuleTypeService, VRNotificationService) {
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

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.vrAlertRuleType = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Notification_VRAlertRuleTypeAPIService.GetFilteredVRAlertRuleTypes(dataRetrievalInput).then(function (response) {
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
                    gridAPI.itemUpdated(updatedVRAlertRuleType);
                };

                VR_Notification_VRAlertRuleTypeService.editVRAlertRuleType(vrAlertRuleTypeItem.Entity.VRAlertRuleTypeId, onVRAlertRuleTypeUpdated);
            }
            function hasEditVRAlertRuleTypePermission() {
                return VR_Notification_VRAlertRuleTypeAPIService.HasEditVRAlertRuleTypePermission();
            }
        }
}]);
