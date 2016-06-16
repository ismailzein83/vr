(function (appControllers) {

    "use strict";

    SwitchEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function SwitchEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var switchEntity;
        $scope.scopeModel = {};
        $scope.scopeModel.isEditMode = false;

        var switchSettingsAPI;
        var switchSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                switchEntity = parameters.switchEntity.Entity;
            }
            $scope.scopeModel.isEditMode = (switchEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel.onSwitchDirectiveReady = function (api) {
                switchSettingsAPI = api;
                switchSettingsReadyDeferred.resolve();
            }

            $scope.scopeModel.save = function () {
                if ($scope.scopeModel.isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            //$scope.scopeModel.isLoading = true;

            loadAllControls();

        }

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSwitchSettingsDirective]).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
        

            function setTitle() {
                if ($scope.scopeModel.isEditMode && switchEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor("Switch: " + switchEntity.Name);
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("");
            }

            function loadSwitchSettingsDirective() {
                var loadSwitchSettingsDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                switchSettingsReadyDeferred.promise.then(function () {
                    var payload = {
                        switchSettings: switchEntity != undefined ? switchEntity.Settings : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(switchSettingsAPI, payload, loadSwitchSettingsDirectivePromiseDeferred);
                });
                return loadSwitchSettingsDirectivePromiseDeferred.promise;
            }

            function loadStaticData() {
                if (switchEntity == undefined)
                    return;

                $scope.scopeModel.name = switchEntity.Name;
            }


        function buildSwitchObjectFromScope() {
            var service = {
                Name: $scope.scopeModel.name,
            };
            return service;
        }

        function insert() {
            var switchObj = buildSwitchObjectFromScope();
            if ($scope.onServiceAdded != undefined)
                $scope.onServiceAdded(switchObj);
            $scope.modalContext.closeModal();
        }

        function update() {
            var switchObj = buildSwitchObjectFromScope();
            if ($scope.onServiceUpdated != undefined)
                $scope.onServiceUpdated(switchObj);
            $scope.modalContext.closeModal();

        }


    }

    appControllers.controller('Retail_BE_SwitchEditorController', SwitchEditorController);
})(appControllers);
