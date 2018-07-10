(function (appControllers) {

    'use strict';

    fileNamePatternHelperController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService'];

    function fileNamePatternHelperController($scope, VRNavigationService, UtilsService, VRNotificationService) {

        var context;

        loadParameters();
        defineScope();
        load();
         
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.save = function (dataItem) {
                return addFileNamePattern(dataItem);
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function buildFileNamePatternObjFromScope(dataItem) {
                return "#" + dataItem.VariableName + "#";
            }
            function addFileNamePattern(dataItem) {
                var fileNamePatternValue = buildFileNamePatternObjFromScope(dataItem);
                if ($scope.onSetFileNamePattern != undefined) {
                    $scope.onSetFileNamePattern(fileNamePatternValue);
                }
                $scope.modalContext.closeModal();
            }
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
            function loadAllControls() {

                function setTitle() {
                    $scope.title = 'File Name Pattern Helper';
                }
                function loadStaticData() {
                    if (context != undefined) {
                        $scope.scopeModel.dataSource = context.getParts();
                    }
                }
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }
        }

    }
    appControllers.controller('VR_Analytic_AutomatedreportFileNamePatternHelperController', fileNamePatternHelperController);

})(appControllers);