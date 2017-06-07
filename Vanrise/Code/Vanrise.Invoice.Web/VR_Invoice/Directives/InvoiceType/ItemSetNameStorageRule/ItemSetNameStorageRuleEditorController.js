(function (appControllers) {

    'use strict';

    ItemSetNameStorageRuleEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function ItemSetNameStorageRuleEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var itemSetNameStorageRule;

        var isEditMode;

        var itemSetNameStorageRuleSettingsAPI;
        var itemSetNameStorageRuleSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                itemSetNameStorageRule = parameters.itemSetNameStorageRule;
            }
            isEditMode = (itemSetNameStorageRule != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
         
            $scope.scopeModel.onItemSetNameStorageRuleSettingsReady = function (api) {
                itemSetNameStorageRuleSettingsAPI = api;
                itemSetNameStorageRuleSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateItemSetNameStorageRule() : addItemSetNameStorageRule();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function builItemSetNameStorageRuleObjFromScope() {
                return {
                    ItemSetNameStorageRuleId: itemSetNameStorageRule != undefined ? itemSetNameStorageRule.ItemSetNameStorageRuleId : UtilsService.guid(),
                    Name: $scope.scopeModel.itemSetNameStorageRuleName,
                    Settings: itemSetNameStorageRuleSettingsAPI.getData(),
                };
            }

            function addItemSetNameStorageRule() {
                var itemSetNameStorageRuleObj = builItemSetNameStorageRuleObjFromScope();
                if ($scope.onItemSetNameStorageRuleAdded != undefined) {
                    $scope.onItemSetNameStorageRuleAdded(itemSetNameStorageRuleObj);
                }
                $scope.modalContext.closeModal();
            }

            function updateItemSetNameStorageRule() {
                var itemSetNameStorageRuleObj = builItemSetNameStorageRuleObjFromScope();
                if ($scope.onItemSetNameStorageRuleUpdated != undefined) {
                    $scope.onItemSetNameStorageRuleUpdated(itemSetNameStorageRuleObj);
                }
                $scope.modalContext.closeModal();
            }
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
            function loadAllControls() {
                function setTitle() {
                    if (isEditMode && itemSetNameStorageRule != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(itemSetNameStorageRule.Name, 'Item SetName Storage Rule');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Item SetName Storage Rule');
                }
                function loadStaticData() {
                    if (itemSetNameStorageRule != undefined) {
                        $scope.scopeModel.itemSetNameStorageRuleName = itemSetNameStorageRule.Name;
                    }
                }
                function loadItemSetNameStorageRuleSettingsDirective() {
                    var itemSetNameStorageRuleSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    itemSetNameStorageRuleSettingsReadyPromiseDeferred.promise.then(function () {
                        var itemSetNameStorageRuleSettingsPayload = itemSetNameStorageRule != undefined ? { itemSetNameStorageRuleSettings: itemSetNameStorageRule.Settings } : undefined;
                        VRUIUtilsService.callDirectiveLoad(itemSetNameStorageRuleSettingsAPI, itemSetNameStorageRuleSettingsPayload, itemSetNameStorageRuleSettingsLoadPromiseDeferred);
                    });
                    return itemSetNameStorageRuleSettingsLoadPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadItemSetNameStorageRuleSettingsDirective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            }
        }

    }
    appControllers.controller('VR_Invoice_ItemSetNameStorageRuleEditorController', ItemSetNameStorageRuleEditorController);

})(appControllers);