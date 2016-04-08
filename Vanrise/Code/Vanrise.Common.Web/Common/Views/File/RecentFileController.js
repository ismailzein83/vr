(function (appControllers) {

    'use strict';

    RecentFileController.$inject = ['$scope', 'VRCommon_FileAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function RecentFileController($scope, VRCommon_FileAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var moduleType;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                moduleType = parameters.moduleType;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.recentFiles = [];

            $scope.scopeModel.onGridReady = function (api) {
                var query = { ModuleType: moduleType };
                api.retrieveData(query);
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_FileAPIService.GetFilteredRecentFiles(dataRetrievalInput).then(function (response) {
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            $scope.scopeModel.validateGrid = function () {
                var fileCount = 0;
                for (var i = 0; i < $scope.scopeModel.recentFiles.length; i++) {
                    var file = $scope.scopeModel.recentFiles[i];
                    if (file.isSelected)
                        fileCount++;
                }
                if (fileCount != 1)
                    return 'Select one file';
                return null;
            };

            $scope.scopeModel.select = function () {
                var selectedFile = UtilsService.getItemByVal($scope.scopeModel.recentFiles, true, 'isSelected');
                $scope.onRecentFileSelected(selectedFile.FileId);
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.cancel = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            UtilsService.waitMultipleAsyncOperations([setTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            $scope.title = 'Recent Files';
        }
    }

    appControllers.controller('VRCommon_RecentFileController', RecentFileController);

})(appControllers);