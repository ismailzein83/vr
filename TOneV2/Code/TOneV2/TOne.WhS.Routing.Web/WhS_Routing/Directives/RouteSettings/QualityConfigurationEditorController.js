(function (appControllers) {

    "use strict";

    qualityConfigurationEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Routing_QualityConfigurationAPIService'];

    function qualityConfigurationEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_Routing_QualityConfigurationAPIService) {

        var isEditMode;
        var qualityConfigurationEntity;
        var qualityConfigurationNames; //for validation

        var timePeriodAPI;
        var timePeriodReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                qualityConfigurationEntity = parameters.qualityConfigurationEntity;
                qualityConfigurationNames = parameters.qualityConfigurationNames;
            }

            isEditMode = (qualityConfigurationEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.qualityConfigurationFields = [];
            $scope.scopeModel.qualityConfigurationSignsFields = buildQualityConfigurationSignsFields();

            $scope.scopeModel.onTimeperiodReady = function (api) {
                timePeriodAPI = api;
                timePeriodReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.qualityConfigurationFieldClicked = function (measure) {
                if ($scope.scopeModel.expression == undefined)
                    $scope.scopeModel.expression = measure.Expression;
                else
                    $scope.scopeModel.expression += " " + measure.Expression;
            };

            $scope.scopeModel.validateQualityConfigurationName = function () {
                if ($scope.scopeModel.qualityConfigurationyName == undefined || qualityConfigurationNames == undefined)
                    return null;

                //for EditMode
                if (qualityConfigurationEntity != undefined && qualityConfigurationEntity.Name == $scope.scopeModel.qualityConfigurationyName)
                    return null;

                for (var i = 0; i < qualityConfigurationNames.length; i++) {
                    var qualityConfigurationName = qualityConfigurationNames[i];
                    if ($scope.scopeModel.qualityConfigurationyName.toLowerCase() == qualityConfigurationName.toLowerCase())
                        return 'Same Quality Configuration Name Exists';
                }
                return null;
            };

            $scope.scopeModel.validateExpression = function () {
                if ($scope.scopeModel.expression == undefined)
                    return null;

                if ($scope.scopeModel.expression.indexOf("context.GetMeasureValue(") == -1)
                    return "Expression should contain at least one Measure!!"

                return null;
            };

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
                        VRUIUtilsService.callDirectiveLoad(timePeriodAPI, directivePayload, loadTimeperiodPromiseDeferred);
                    });
                return loadTimeperiodPromiseDeferred.promise;
            }

            function loadQualityConfigurationFields() {
                return WhS_Routing_QualityConfigurationAPIService.GetQualityConfigurationFields().then(function (response) {
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

        function insertQualityConfiguration() {
            var qualityConfigurationObject = buildQualityConfigurationObjectFromScope(UtilsService.guid());

            return WhS_Routing_QualityConfigurationAPIService.TryCompileQualityConfigurationExpression(qualityConfigurationObject.Expression).then(function (response) {
                if (response) {
                    if ($scope.onQualityConfigurationAdded != undefined)
                        $scope.onQualityConfigurationAdded(qualityConfigurationObject);
                    $scope.modalContext.closeModal();
                } else {
                    VRNotificationService.showError("Expression Validation Error. Check Log");
                }
            });
        }
        function updateQualityConfiguration() {
            var qualityConfigurationObject = buildQualityConfigurationObjectFromScope($scope.scopeModel.qualityConfigurationId);

            return WhS_Routing_QualityConfigurationAPIService.TryCompileQualityConfigurationExpression(qualityConfigurationObject.Expression).then(function (response) {
                if (response) {
                    if ($scope.onQualityConfigurationUpdated != undefined)
                        $scope.onQualityConfigurationUpdated(qualityConfigurationObject);
                    $scope.modalContext.closeModal();
                } else {
                    VRNotificationService.showError("Expression Validation Error. Check Log");
                }
            });
        }

        function buildQualityConfigurationSignsFields() {
            return [{
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
        }
        function buildQualityConfigurationObjectFromScope(qualityConfigurationId) {
            var obj = {
                Name: $scope.scopeModel.qualityConfigurationyName,
                TimePeriod: timePeriodAPI.getData(),
                Expression: $scope.scopeModel.expression,
                QualityConfigurationId: qualityConfigurationId
            };

            return obj;
        }
    }

    appControllers.controller('WhS_Routing_QualityConfigurationEditorController', qualityConfigurationEditorController);
})(appControllers);
