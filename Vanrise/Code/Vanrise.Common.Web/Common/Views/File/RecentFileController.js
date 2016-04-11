(function (appControllers) {

    'use strict';

    RecentFileController.$inject = ['$scope', 'VRCommon_FileAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function RecentFileController($scope, VRCommon_FileAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var moduleName;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                moduleName = parameters.moduleName;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.recentFiles = [];
            $scope.scopeModel.selectedFile;

            $scope.scopeModel.onGridReady = function (api) {
                var query = { ModuleName: moduleName };
                api.retrieveData(query);
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_FileAPIService.GetFilteredRecentFiles(dataRetrievalInput).then(function (response) {

                    if (response != null && response.Data != null) {
                        for (var i = 0; i < response.Data.length; i++) {
                            response.Data[i].onValueChanged = function (file) {
                                if (file.isSelected) {
                                    $scope.scopeModel.selectedFile = file;
                                    // Deselect other files
                                    for (var i = 0; i < $scope.scopeModel.recentFiles.length; i++) {
                                        var item = $scope.scopeModel.recentFiles[i];
                                        if (item.FileId != file.FileId) {
                                            item.isSelected = false;
                                        }
                                    }
                                }
                                else {
                                    $scope.scopeModel.selectedFile = undefined;
                                }
                            };
                        }
                    }

                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
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