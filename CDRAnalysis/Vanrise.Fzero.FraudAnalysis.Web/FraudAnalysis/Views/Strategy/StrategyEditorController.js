(function (appControllers) {

    'use strict';

    StrategyEditorController.$inject = ['$scope', 'StrategyAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'CDRAnalysis_FA_SuspicionLevelEnum', 'VRCommon_HourEnum', 'VRUIUtilsService', 'CDRAnalysis_FA_PeriodEnum', 'CDRAnalysis_FA_ParametersService'];

    function StrategyEditorController($scope, StrategyAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService, CDRAnalysis_FA_SuspicionLevelEnum, VRCommon_HourEnum, VRUIUtilsService, CDRAnalysis_FA_PeriodEnum, CDRAnalysis_FA_ParametersService) {
        var isEditMode;

        var periodSelectorAPI;
        var periodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var strategyId;
        var strategyEntity;

        var filters;

        var strategyCriteriaDirectiveAPI;
        var strategyCriteriaDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var strategyParametersDirectiveAPI;
        var strategyParametersDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                strategyId = parameters.strategyId;
            }

            isEditMode = strategyId != undefined;
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onStrategyCriteriaReady = function (api)
            {
                strategyCriteriaDirectiveAPI = api;
                 strategyCriteriaDirectiveReadyDeferred.resolve();
            }

            $scope.hasSaveStrategyPermission = function () {
                if (isEditMode)
                    return StrategyAPIService.HasUpdateStrategyPermission();
                else
                    return StrategyAPIService.HasAddStrategyPermission();
            };

            $scope.scopeModel.onPeriodSelectorReady = function (api) {
                periodSelectorAPI = api;
                periodSelectorReadyDeferred.resolve();
            };
          
            $scope.scopeModel.onStrategyParametersReady = function(api)
            {
                strategyParametersDirectiveAPI = api;
                strategyParametersDirectiveReadyDeferred.resolve();
            }

            $scope.scopeModel.onPeriodSelectionChanged = function (selectedPeriod) {
                if (selectedPeriod != undefined) {
                    var setLoaderStrategyCriteria = function (value) { $scope.scopeModel.isLoadingStrategyCriteria = value };
                    var payloadStrategyCriteria = {
                        filter: { ExcludeHourly: selectedPeriod.Id == CDRAnalysis_FA_PeriodEnum.Hourly.value },
                        context : getContext()
                    };

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, strategyCriteriaDirectiveAPI, payloadStrategyCriteria, setLoaderStrategyCriteria, strategyCriteriaDirectiveReadyDeferred);
                }
            };

            $scope.scopeModel.saveStrategy = function () {
                if (isEditMode) {
                    return updateStrategy();
                }
                else {
                    return addStrategy();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getStrategy().then(function () {
                    loadAllControls().finally(function () {
                        strategyEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope.scopeModel);
                });
            }
            else {
                loadAllControls();
            }
        }

        function getStrategy() {
            return StrategyAPIService.GetStrategy(strategyId).then(function (response) {
                strategyEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticControls, loadPeriodSelector, loadStrategyCriteriaDirective, loadStrategyParametersDirective]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope.scopeModel);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(strategyEntity ? strategyEntity.Name : null, 'Strategy') : UtilsService.buildTitleForAddEditor('Strategy');
            }
            function loadStaticControls() {
                if (strategyEntity) {
                    $scope.scopeModel.name = strategyEntity.Name;
                    $scope.scopeModel.description = strategyEntity.Description;
                    if (strategyEntity.Settings != undefined)
                    {
                        $scope.scopeModel.isDefault = strategyEntity.Settings.IsDefault;
                        $scope.scopeModel.isEnabled = strategyEntity.Settings.IsEnabled;
                    }
                }
            }
            function loadPeriodSelector() {
                var periodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                periodSelectorReadyDeferred.promise.then(function () {
                    var periodSelectorPayload = strategyEntity && strategyEntity.Settings ? {
                        selectedIds: strategyEntity.Settings.PeriodId
                    } : null;
                    VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, periodSelectorPayload, periodSelectorLoadDeferred);
                });

                return periodSelectorLoadDeferred.promise;
            }
        }

        function loadStrategyCriteriaDirective() {
            var strategyCriteriaDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
            strategyCriteriaDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            strategyCriteriaDirectiveReadyDeferred.promise.then(function () {
                strategyCriteriaDirectiveReadyDeferred = undefined;
                var strategyCriteriaDirectivePayload = { context: getContext() };
                
                if (strategyEntity != undefined && strategyEntity.Settings !=undefined) {
                    strategyCriteriaDirectivePayload.strategyCriteria = strategyEntity.Settings.StrategySettingsCriteria;
                }
                VRUIUtilsService.callDirectiveLoad(strategyCriteriaDirectiveAPI, strategyCriteriaDirectivePayload, strategyCriteriaDirectiveLoadDeferred);
            });

            return strategyCriteriaDirectiveLoadDeferred.promise;
        }
        function loadStrategyParametersDirective() {
            var strategyParametersDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
            strategyParametersDirectiveReadyDeferred.promise.then(function () {
                var strategyParametersDirectivePayload = { context: getContext() };
                if (strategyEntity != undefined && strategyEntity.Settings != undefined) {
                    strategyParametersDirectivePayload.strategyParameters = strategyEntity.Settings.Parameters;
                }
                VRUIUtilsService.callDirectiveLoad(strategyParametersDirectiveAPI, strategyParametersDirectivePayload, strategyParametersDirectiveLoadDeferred);
            });

            return strategyParametersDirectiveLoadDeferred.promise;
        }

        function getContext() {
            var context = {
                getFilterHint: function (parameter) {
                    return strategyCriteriaDirectiveAPI.getFilterHint(parameter);
                },
                setParameterVisibility: function (visibility, parameters)
                {
                    strategyParametersDirectiveAPI.setParameterVisibility(visibility, parameters);
                }

            };
            return context;
        }
        function addStrategy() {
            var strategyObject = buildStrategyObjFromScope();
            $scope.scopeModel.isLoading = true;
            return StrategyAPIService.AddStrategy(strategyObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Strategy', response, 'Name')) {
                    if ($scope.onStrategyAdded != undefined && typeof $scope.onStrategyAdded == 'function')
                        $scope.onStrategyAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope.scopeModel);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateStrategy() {
            var strategyObject = buildStrategyObjFromScope();
            $scope.scopeModel.isLoading = true;

            return StrategyAPIService.UpdateStrategy(strategyObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Strategy', response, 'Name')) {
                    if ($scope.onStrategyUpdated != undefined && typeof $scope.onStrategyUpdated == 'function')
                        $scope.onStrategyUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope.scopeModel);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildStrategyObjFromScope() {
            var strategyObject = {
                Id: (strategyId != null) ? strategyId : 0,
                Name: $scope.scopeModel.name,
                Description: $scope.scopeModel.description,
                LastUpdatedOn: new Date(),
                Settings:{
                    IsDefault: $scope.scopeModel.isDefault,
                    IsEnabled: $scope.scopeModel.isEnabled,
                    PeriodId: periodSelectorAPI.getSelectedIds(),
                    Parameters: strategyParametersDirectiveAPI.getData(),
                    StrategySettingsCriteria:strategyCriteriaDirectiveAPI.getData()
                }
            };          
            return strategyObject;
        }
    }
    appControllers.controller('CDRAnalysis_FA_StrategyEditorController', StrategyEditorController);

})(appControllers);
