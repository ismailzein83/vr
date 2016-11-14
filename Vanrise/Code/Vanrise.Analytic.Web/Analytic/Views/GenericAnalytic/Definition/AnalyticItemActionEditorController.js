(function (appControllers) {

    "use strict";

    AnalyticItemActionEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticReportAPIService', 'VR_Sec_MenuAPIService', 'Analytic_AnalyticService', 'VR_Analytic_AccessTypeEnum'];

    function AnalyticItemActionEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Analytic_AnalyticReportAPIService, VR_Sec_MenuAPIService, Analytic_AnalyticService, VR_Analytic_AccessTypeEnum) {

        var isEditMode;
        var itemActionEntity;

        var editorDirectiveAPI;
        var editorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                itemActionEntity = parameters.itemAction;
            }
            isEditMode = (itemActionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onEditorDirectiveReady = function (api) {
                editorDirectiveAPI = api;
                editorReadyDeferred.resolve();
            };

            $scope.scopeModel.saveAnalyticItemAction = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;
                loadAllControls();

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadEditorDirective]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

            function setTitle() {
                if (isEditMode && itemActionEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(itemActionEntity.Title, 'Grid Action');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Grid Action');
            }

            function loadEditorDirective() {
                var loadEditorDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                editorReadyDeferred.promise.then(function () {
                    var payLoad = {
                            itemAction: itemActionEntity
                        };
                    VRUIUtilsService.callDirectiveLoad(editorDirectiveAPI, payLoad, loadEditorDirectivePromiseDeferred);
                });
                return loadEditorDirectivePromiseDeferred.promise;
            }
        }

        function buildItemActionObjectFromScope() {
            var itemAction = editorDirectiveAPI != undefined ? editorDirectiveAPI.getData() : undefined;
            return itemAction;
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            var itemActionEntityObj = buildItemActionObjectFromScope();
            if ($scope.onItemActionAdded != undefined) {
                $scope.onItemActionAdded(itemActionEntityObj);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            $scope.scopeModel.isLoading = true;
            var itemActionEntityObj = buildItemActionObjectFromScope();
            if ($scope.onItemActionUpdated != undefined) {
                $scope.onItemActionUpdated(itemActionEntityObj);
            }
            $scope.modalContext.closeModal();
              
        }

    }

    appControllers.controller('VR_Analytic_AnalyticItemActionEditorController', AnalyticItemActionEditorController);
})(appControllers);
