(function (appControllers) {

    "use strict";

    AnalyticPermanentFilterEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','VR_Analytic_AnalyticTableAPIService'];

    function AnalyticPermanentFilterEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Analytic_AnalyticTableAPIService) {

        var permanentFilterSettingsAPI;
        var permanentFilterSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        var analyticTableId;
        var analyticTablePermanentFilter;
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

            $scope.scopeModel.onPermanentFilterSettingsDirectiveReady = function (api) {
                permanentFilterSettingsAPI = api;
                permanentFilterSettingsReadyDeferred.resolve();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.savePermanentFilter = function () {
                saveAnalyticTablePermanentFilter();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            setTitle();
            loadStaticData();
            var rootPromiseNode = {
                promises: [loadAnalyticTableData()],
                getChildNode: function () {
                    return { promises: [loadPermanentFilterSettingsDirective()] };
                }
            };

            function setTitle() {
                $scope.title = 'Analytic Permanent Filter Editor';
            }
            function loadStaticData() {

            }
            function loadAnalyticTableData() {
                return VR_Analytic_AnalyticTableAPIService.GetTableById(analyticTableId).then(function (response) {
                    if (response != undefined && response.PermanentFilter != undefined) {
                        analyticTablePermanentFilter = response.PermanentFilter.Settings;
                    }
                });
            }
            function loadPermanentFilterSettingsDirective() {
                var loadPermanentFilterSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                permanentFilterSettingsReadyDeferred.promise.then(function () {
                    var payLoad = {
                        analyticTableId: analyticTableId,
                        settings: analyticTablePermanentFilter
                    };

                    VRUIUtilsService.callDirectiveLoad(permanentFilterSettingsAPI, payLoad, loadPermanentFilterSettingsPromiseDeferred);
                });
                return loadPermanentFilterSettingsPromiseDeferred.promise;
            }

            return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function buildObjectFromScope() {
            var permanentFilter = {
                Settings: permanentFilterSettingsAPI.getData()
            };
            return permanentFilter;
        }

        function saveAnalyticTablePermanentFilter() {
            $scope.scopeModel.isLoading = true;
            var permanentFilter = buildObjectFromScope();
            var input = {
                AnalyticTableId: analyticTableId,
                PermanentFilter: permanentFilter
            };
            return VR_Analytic_AnalyticTableAPIService.SaveAnalyticTablePermanentFilter(input).then(function (response) {
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

    appControllers.controller('VR_Analytic_AnalyticPermanentFilterEditorController', AnalyticPermanentFilterEditorController);
})(appControllers);
