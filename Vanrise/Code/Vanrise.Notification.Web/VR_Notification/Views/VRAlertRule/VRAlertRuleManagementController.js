(function (appControllers) {

    "use strict";

    VRAlertRuleManagementController.$inject = ['$scope', 'VR_Notification_VRAlertRuleService', 'VR_Notification_VRAlertRuleAPIService', 'UtilsService', 'VRUIUtilsService'];

    function VRAlertRuleManagementController($scope, VR_Notification_VRAlertRuleService, VR_Notification_VRAlertRuleAPIService, UtilsService, VRUIUtilsService) {

        var gridAPI;
        var vrAlertRuleTypeSelectorAPI;
        var vrAlertRuleTypeSelectoReadyDeferred = UtilsService.createPromiseDeferred();
        var vrAlertRuleStatusSelectorAPI;
        var vrAlertRuleStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        load();


        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            
            $scope.scopeModel.onAlertRuleTypeSelectorReady = function (api) {
                vrAlertRuleTypeSelectorAPI = api;
                vrAlertRuleTypeSelectoReadyDeferred.resolve();
            };

            $scope.scopeModel.onAlertRuleStatusSelectorReady = function (api) { 
                vrAlertRuleStatusSelectorAPI = api;
                vrAlertRuleStatusSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.add = function () {
                var onVRAlertRuleAdded = function (addedVRAlertRule) {
                    gridAPI.onVRAlertRuleAdded(addedVRAlertRule);
                };

                VR_Notification_VRAlertRuleService.addVRAlertRule(onVRAlertRuleAdded);
            };

            $scope.scopeModel.hasAddVRAlertRulePermission = function () {
                return VR_Notification_VRAlertRuleAPIService.HasAddVRAlertRulePermission()
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }
       
        
    
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadVRAlertRuleTypeSelector, loadVRAlertRuleStatusSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }
        function loadVRAlertRuleTypeSelector() {
            var vrAlertRuleTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            vrAlertRuleTypeSelectoReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(vrAlertRuleTypeSelectorAPI, undefined, vrAlertRuleTypeSelectorLoadDeferred);
            });
            return vrAlertRuleTypeSelectorLoadDeferred.promise;
        }
        function loadVRAlertRuleStatusSelector() {
            var vrAlertRuleStatusSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            vrAlertRuleStatusSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(vrAlertRuleStatusSelectorAPI, undefined, vrAlertRuleStatusSelectorLoadDeferred);
            });
            return vrAlertRuleStatusSelectorLoadDeferred.promise;
        }
        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                RuleTypeIds: vrAlertRuleTypeSelectorAPI.getSelectedIds(),
                Statuses: vrAlertRuleStatusSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('VR_Notification_VRAlertRuleManagementController', VRAlertRuleManagementController);

})(appControllers);