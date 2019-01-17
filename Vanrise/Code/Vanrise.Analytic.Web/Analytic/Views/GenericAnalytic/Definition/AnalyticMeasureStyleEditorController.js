(function (appControllers) {

    "use strict";

    AnalyticMeasureStyleEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','VR_Analytic_AnalyticTableAPIService'];

    function AnalyticMeasureStyleEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Analytic_AnalyticTableAPIService) {

        var measureStyleGridAPI;
        var measureStyleGridReadyDeferred = UtilsService.createPromiseDeferred();

        var isEditMode;
        var analyticTableId;

        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                analyticTableId = parameters.analyticTableId;
            }

        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onMeasureStyleGridReady = function (api) {
                measureStyleGridAPI = api;
                measureStyleGridReadyDeferred.resolve();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.saveMeasureStyles = function () {
                saveAnalyticTableMeasureStyles();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadMeasureStyleGrid]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

            function setTitle() {
                    $scope.title = UtilsService.buildTitleForAddEditor('Analytic Measure Styles Editor');
            }

            function loadStaticData() {

            }

            function loadMeasureStyleGrid() {
                var loadMeasureStyleGridPromiseDeferred = UtilsService.createPromiseDeferred();
                measureStyleGridReadyDeferred.promise.then(function () {
                    var payLoad = {
                        analyticTableId: analyticTableId
                    };

                    VRUIUtilsService.callDirectiveLoad(measureStyleGridAPI, payLoad, loadMeasureStyleGridPromiseDeferred);
                });
                return loadMeasureStyleGridPromiseDeferred.promise;
            }

        }

        function buildObjectFromScope() {
            var measureStyle = {
                MeasureStyleRules: measureStyleGridAPI.getData()
            };
            return measureStyle;
        }

        function saveAnalyticTableMeasureStyles() {
            $scope.scopeModel.isLoading = true;
            var measureStyle = buildObjectFromScope();
            var input = {
                AnalyticTableId: analyticTableId,
                MeasureStyles: measureStyle
            };
           
            return VR_Analytic_AnalyticTableAPIService.SaveAnalyticTableMeasureStyles(input).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Analytic Table', response, 'Name')) {
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

    }

    appControllers.controller('VR_Analytic_AnalyticMeasureStyleEditorController', AnalyticMeasureStyleEditorController);
})(appControllers);
