(function (appControllers) {

    "use strict";

    qualityConfigurationEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Routing_QualityConfigurationAPIService'];

    function qualityConfigurationEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_Routing_QualityConfigurationAPIService) {

        var isEditMode;
        var qualityConfigurationEntity;

        var timePeriodApi;
        var timePeriodReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
               qualityConfigurationEntity = parameters.qualityConfigurationEntity;
            }
            isEditMode = (qualityConfigurationEntity != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.qualityConfigurationFields = [];

            $scope.scopeModel.qualityConfigurationSignsFields = [{
                Name: '(',
                Title: '(',
                Expression: '(',
            }, {
                Name: ')',
                Title: ')',
                Expression: ')',
            }, {
                Name: '+',
                Title: '+',
                Expression: '+',
            }, {
                Name: '-',
                Title: '-',
                Expression: '-',
            }, {
                Name: '*',
                Title: '*',
                Expression: '*',
            }, {
                Name: '/',
                Title: '/',
                Expression: '/',
            }];

            $scope.scopeModel.saveQualityConfiguration = function () {
                if (isEditMode) {
                    return updateQualityConfiguration();
                }
                else
                    return insertQualityConfiguration();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onTimeperiodReady = function (api) {
                timePeriodApi = api;
                timePeriodReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.qualityConfigurationFieldClicked = function (measure) {
                if ($scope.scopeModel.expression == undefined)
                    $scope.scopeModel.expression = measure.Expression;
                else
                    $scope.scopeModel.expression += " " + measure.Expression;
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && qualityConfigurationEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(qualityConfigurationEntity.Name, "Quality Configuration");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Quality Configuration");
            }

            function loadStaticData() {
                if (isEditMode) {
                    $scope.scopeModel.qualityConfigurationyName = qualityConfigurationEntity.Name;
                    $scope.scopeModel.qualityConfigurationId = qualityConfigurationEntity.QualityConfigurationId;
                    $scope.scopeModel.expression = qualityConfigurationEntity.Expression;
                }
            }

            function loadTimeperiod() {
                var loadTimeperiodPromiseDeferred = UtilsService.createPromiseDeferred();
                timePeriodReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                        if (qualityConfigurationEntity != undefined && qualityConfigurationEntity.TimePeriod != undefined) {
                            directivePayload = {
                                timePeriod: qualityConfigurationEntity.TimePeriod
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(timePeriodApi, directivePayload, loadTimeperiodPromiseDeferred);
                    });
                return loadTimeperiodPromiseDeferred.promise;
            }

            function loadQualityConfigurationFields() {
                return WhS_Routing_QualityConfigurationAPIService.GetQualityConfigurationFields()
                    .then(function (response) {
                        if (response != undefined) {
                            for (var i = 0, length = response.length ; i < length ; i++) {
                                var responseItem = response[i];
                                $scope.scopeModel.qualityConfigurationFields.push({
                                    Name: responseItem.Name,
                                    Title: responseItem.Title,
                                    Expression: 'context.GetMeasureValue("' + responseItem.Name + '")'
                                });
                            };
                        }
                    });

            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTimeperiod, loadQualityConfigurationFields])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });

        }

        function buildQualityConfigurationObjectFromScope(qualityConfigurationId) {
            var obj = {
                Name: $scope.scopeModel.qualityConfigurationyName,
                TimePeriod: timePeriodApi.getData(),
                Expression: $scope.scopeModel.expression,
                QualityConfigurationId: qualityConfigurationId
            };
            return obj;
        }

        function insertQualityConfiguration() {
            var qualityConfigurationObject = buildQualityConfigurationObjectFromScope(UtilsService.guid());

            if ($scope.onQualityConfigurationAdded != undefined)
                $scope.onQualityConfigurationAdded(qualityConfigurationObject);
            $scope.modalContext.closeModal();
        }

        function updateQualityConfiguration() {
            var qualityConfigurationObject = buildQualityConfigurationObjectFromScope($scope.scopeModel.qualityConfigurationId);

            if ($scope.onQualityConfigurationUpdated != undefined)
                $scope.onQualityConfigurationUpdated(qualityConfigurationObject);
            $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('WhS_Routing_QualityConfigurationEditorController', qualityConfigurationEditorController);
})(appControllers);
