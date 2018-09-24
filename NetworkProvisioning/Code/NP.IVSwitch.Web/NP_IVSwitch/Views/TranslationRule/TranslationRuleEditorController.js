(function (appControllers) {

    "use strict";

    TranslationRuleEditorController.$inject = ['$scope', 'NP_IVSwitch_TranslationRuleAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'NP_IVSwitch_EngineTypeEnum', 'NP_IVSwitch_PrefixSignEnum', 'NP_IVSwitch_CLITypeEnum', 'VRUIUtilsService', 'VR_GenericData_GenericBusinessEntityAPIService', 'NP_IVSwitch_CLISourceEnum'];

    function TranslationRuleEditorController($scope, NP_IVSwitch_TranslationRuleAPIService, VRNotificationService, UtilsService, VRNavigationService, NP_IVSwitch_EngineTypeEnum, NP_IVSwitch_PrefixSignEnum, NP_IVSwitch_CLITypeEnum, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService, NP_IVSwitch_CLISourceEnum) {

        var isEditMode;

        var translationRuleId;
        var translationRuleEntity;
        var translationRuleEntityPromiseDeferred = UtilsService.createPromiseDeferred();

        var context;
        var isViewHistoryMode;


        var DNISPrefixSignAPI;
        var DNISPrefixSignReadyDeferred = UtilsService.createPromiseDeferred();

        var CLIPatternPrefixSignAPI;
        var CLIPatternPrefixSignReadyDeferred = UtilsService.createPromiseDeferred();

        var poolBasedCLIGroupAPI;
        var poolBasedCLIGroupReadyDeferred = UtilsService.createPromiseDeferred();

        var engineTypeSelectedPromiseDeferred;
        var CLITypeSelectedPromiseDeferred;

        var CLISourceAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                translationRuleId = parameters.TranslationRuleId;
                context = parameters.context;
            }

            isViewHistoryMode = (context != undefined && context.historyId != undefined);
            isEditMode = (translationRuleId != undefined);
        }
        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.CLIPatterns = [];
            $scope.scopeModel.engineTypes = UtilsService.getArrayEnum(NP_IVSwitch_EngineTypeEnum);
            $scope.scopeModel.prefixSigns = UtilsService.getArrayEnum(NP_IVSwitch_PrefixSignEnum);
            $scope.scopeModel.CLITypes = UtilsService.getArrayEnum(NP_IVSwitch_CLITypeEnum);
            $scope.scopeModel.CLISources = UtilsService.getArrayEnum(NP_IVSwitch_CLISourceEnum);
            $scope.scopeModel.isEditMode = isEditMode;
            $scope.hasSaveTranslationRulePermission = function () {
                if (isEditMode) {
                    return NP_IVSwitch_TranslationRuleAPIService.HasEditTranslationRulePermission();
                }
                else {
                    return NP_IVSwitch_TranslationRuleAPIService.HasAddTranslationRulePermission();
                }
            };


            $scope.scopeModel.onDNISPrefixSignReady = function (api) {
                DNISPrefixSignAPI = api;
                DNISPrefixSignReadyDeferred.resolve();
            };

            $scope.scopeModel.onEngineTypeSelectionChanged = function (engineTypeSelected) {
                if (engineTypeSelected != undefined) {
                    if (engineTypeSelectedPromiseDeferred != undefined) {
                        engineTypeSelectedPromiseDeferred.resolve();
                        engineTypeSelectedPromiseDeferred = undefined;
                    }
                    else {
                        DNISPrefixSignReadyDeferred.promise.then(function() {
                            DNISPrefixSignAPI.selectFirstItem();
                        });
                        CLIPatternPrefixSignReadyDeferred.promise.then(function() {
                            CLIPatternPrefixSignAPI.selectFirstItem();
                        });
                    }
                }
            };

            $scope.scopeModel.onCLIPatternPrefixSignReady = function (api) {
                CLIPatternPrefixSignAPI = api;
                CLIPatternPrefixSignReadyDeferred.resolve();
            };
            $scope.scopeModel.addCLIPattern = function () {
                if ($scope.scopeModel.currentCLIPattern != undefined) {
                    $scope.scopeModel.CLIPatterns.push($scope.scopeModel.currentCLIPattern);
                    $scope.scopeModel.currentCLIPattern = undefined;
                }
                else if ($scope.scopeModel.selectedPoolBasedCLIGroup!=undefined) {
                    $scope.scopeModel.isLoadingCLIPatterns = true;
                    var poolBasedCLIGroupDetailPromise = VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityDetail($scope.scopeModel.selectedPoolBasedCLIGroup.GenericBusinessEntityId, "94820cf8-c9a3-40a9-b90e-ba25a2a96d73");
                    poolBasedCLIGroupDetailPromise.then(function (response) {
                        if (response != undefined && response.FieldValues != undefined && response.FieldValues.CLIPatterns != undefined && response.FieldValues.CLIPatterns.Value != undefined && response.FieldValues.CLIPatterns.Value.length > 0) {
                            for (var i = 0; i < response.FieldValues.CLIPatterns.Value.length; i++) {
                                var cliPatternValue = response.FieldValues.CLIPatterns.Value[i].CLIPattern;
                                if (!UtilsService.contains($scope.scopeModel.CLIPatterns, cliPatternValue)) {
                                    $scope.scopeModel.CLIPatterns.push(cliPatternValue);
                                }
                            }
                        }
                    }).finally(function () {
                        $scope.scopeModel.isLoadingCLIPatterns = false;
                    });
                }
             
            };
            $scope.scopeModel.onCLISourceReady = function (api) {
                CLISourceAPI = api;
                CLISourceAPI.selectFirstItem();
            };

            $scope.scopeModel.onCLITypeSelectionChange = function (selectedCLIType) {
                if (selectedCLIType != undefined) {
                    if (CLITypeSelectedPromiseDeferred !=undefined) {
                        CLITypeSelectedPromiseDeferred.resolve();
                        CLITypeSelectedPromiseDeferred = undefined;
                    }
                    else {
                        if (selectedCLIType == NP_IVSwitch_CLITypeEnum.FixedCLI) {
                            CLIPatternPrefixSignReadyDeferred.promise.then(function () {
                                CLIPatternPrefixSignAPI.selectFirstItem();
                            });
                            $scope.scopeModel.poolBasedCLISettingsPrefix = undefined;
                            $scope.scopeModel.poolBasedCLISettingsDestination = undefined;
                            $scope.scopeModel.poolBasedCLISettingsRandMin = undefined;
                            $scope.scopeModel.poolBasedCLISettingsRandMax = undefined;
                            $scope.scopeModel.poolBasedCLISettingsDisplayName = undefined;
                            $scope.scopeModel.CLIPatterns.length = 0;
                         }
                         else {
                            $scope.scopeModel.fixedCLISettingsSelectedPrefixSign = undefined;
                            $scope.scopeModel.CLIPattern = undefined;
                        }
                    }
                }
                else {
                    translationRuleEntityPromiseDeferred.promise.then(function () {
                        if (translationRuleEntity != undefined && translationRuleEntity.CLIType == undefined) {
                            if (CLITypeSelectedPromiseDeferred != undefined) {
                                CLITypeSelectedPromiseDeferred.resolve();
                                CLITypeSelectedPromiseDeferred = undefined;
                            }
                        }
                    });
                }
            };

            $scope.scopeModel.onRemoveCLIPattern = function (dataItem) {
                var index = $scope.scopeModel.CLIPatterns.indexOf(dataItem);
                if (index >-1)
                    $scope.scopeModel.CLIPatterns.splice(index, 1);
            };

            $scope.scopeModel.onPoolBasedCLIGroupSelectorReady = function (api) {
                poolBasedCLIGroupAPI = api;
                poolBasedCLIGroupReadyDeferred.resolve();
            };

            $scope.scopeModel.clearAllCLIPatterns = function() {
                $scope.scopeModel.CLIPatterns.length=0;
            };

            $scope.scopeModel.onCLISourceSelectionChange = function (cliSourceSelected) {
                if (cliSourceSelected != undefined) {
                    if (cliSourceSelected == NP_IVSwitch_CLISourceEnum.CLIGroup) {
                        loadPoolBasedCLIGroupSelector();
                        $scope.scopeModel.currentCLIPattern = undefined;
                    }
                    else if (cliSourceSelected == NP_IVSwitch_CLISourceEnum.Specific) {
                        $scope.scopeModel.selectedPoolBasedCLIGroup = undefined;
                    }
                }
            };

            $scope.scopeModel.validateCLIPatterns = function () {
                if ($scope.scopeModel.CLIPatterns.length == 0)
                    return "At least one CLI Pattern must be added.";
                return null;
            };

            $scope.scopeModel.validateCurrentCLIPattern = function () {
                if (UtilsService.contains($scope.scopeModel.CLIPatterns, $scope.scopeModel.currentCLIPattern))
                    return "The CLI Pattern already exists.";
                return null;
            };
            $scope.scopeModel.disableCLIPatternAddButton = function () {
                return ($scope.scopeModel.currentCLIPattern == undefined && $scope.scopeModel.selectedPoolBasedCLIGroup ==undefined) || UtilsService.contains($scope.scopeModel.CLIPatterns, $scope.scopeModel.currentCLIPattern);
            };

            $scope.scopeModel.validateRandMin = function () {
                if (!$scope.scopeModel.poolBasedCLISettingsRandMin && !$scope.scopeModel.poolBasedCLISettingsRandMax)
                    return null;
                if ($scope.scopeModel.poolBasedCLISettingsRandMin && $scope.scopeModel.poolBasedCLISettingsRandMax && parseInt($scope.scopeModel.poolBasedCLISettingsRandMin) > parseInt($scope.scopeModel.poolBasedCLISettingsRandMax) )
                    return 'Rand Min must be less than Rand Max.';
                return null;
            };

            $scope.scopeModel.save = function () {
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

            if (isEditMode) {
                engineTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                CLITypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                getTranslationRule().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else if (isViewHistoryMode) {
                getTranslationRuleHistory().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    vrNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });

            }
            else {
                loadAllControls();
            }


        }
        function getTranslationRuleHistory() {
            return NP_IVSwitch_TranslationRuleAPIService.GetTranslationRuleHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                translationRuleEntity = response;
                translationRuleEntityPromiseDeferred.resolve();
            });
        }
        function getTranslationRule() {
            return NP_IVSwitch_TranslationRuleAPIService.GetTranslationRule(translationRuleId).then(function (response) {
                translationRuleEntity = response;
                translationRuleEntityPromiseDeferred.resolve();
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var translationRuleName = (translationRuleEntity != undefined) ? translationRuleEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(translationRuleName, 'Translation Rule');
                }
                else if (isViewHistoryMode && translationRuleEntity != undefined)
                    $scope.title = "View Translation Rule: " + translationRuleEntity.Name;
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Translation Rule');
                }
            }
            function loadStaticData() {
                if (translationRuleEntity == undefined)
                    return;
                $scope.scopeModel.name = translationRuleEntity.Name;
                $scope.scopeModel.dnisPattern = translationRuleEntity.DNISPattern;
                $scope.scopeModel.CLIPattern = translationRuleEntity.CLIPattern;
                $scope.scopeModel.selectedEngineType=UtilsService.getItemByVal($scope.scopeModel.engineTypes,translationRuleEntity.EngineType,'value');
                $scope.scopeModel.selectedPrefixSign=UtilsService.getItemByVal($scope.scopeModel.prefixSigns, translationRuleEntity.DNISPatternSign, 'value');
                $scope.scopeModel.dnisPattern = translationRuleEntity.DNISPattern;
                if (translationRuleEntity.CLIType!=undefined) {
                    $scope.scopeModel.selectedCLIType = UtilsService.getItemByVal($scope.scopeModel.CLITypes, translationRuleEntity.CLIType, 'value');
                }
                if (translationRuleEntity.FixedCLISettings != undefined) {
                    $scope.scopeModel.fixedCLISettingsSelectedPrefixSign = UtilsService.getItemByVal($scope.scopeModel.prefixSigns, translationRuleEntity.FixedCLISettings.CLIPatternSign,'value');
                    $scope.scopeModel.CLIPattern = translationRuleEntity.FixedCLISettings.CLIPattern;
                }
                if (translationRuleEntity.PoolBasedCLISettings!=undefined) {
                    $scope.scopeModel.poolBasedCLISettingsPrefix = translationRuleEntity.PoolBasedCLISettings.Prefix;
                    $scope.scopeModel.poolBasedCLISettingsDestination = translationRuleEntity.PoolBasedCLISettings.Destination;
                    $scope.scopeModel.poolBasedCLISettingsRandMin = translationRuleEntity.PoolBasedCLISettings.RandMin!=-1? translationRuleEntity.PoolBasedCLISettings.RandMin : null;
                    $scope.scopeModel.poolBasedCLISettingsRandMax = translationRuleEntity.PoolBasedCLISettings.RandMax!=0 ? translationRuleEntity.PoolBasedCLISettings.RandMax : null;
                    $scope.scopeModel.poolBasedCLISettingsDisplayName = translationRuleEntity.PoolBasedCLISettings.DisplayName;
                    if (translationRuleEntity.PoolBasedCLISettings.CLIPatterns != null && translationRuleEntity.PoolBasedCLISettings.CLIPatterns.length>0) {
                         for (var i = 0; i < translationRuleEntity.PoolBasedCLISettings.CLIPatterns.length; i++) {
                             $scope.scopeModel.CLIPatterns.push(translationRuleEntity.PoolBasedCLISettings.CLIPatterns[i]);
                         }
                    }
                   
                }
            }
        }
        function loadPoolBasedCLIGroupSelector() {
            var poolBasedCLIGroupSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            poolBasedCLIGroupReadyDeferred.promise.then(function () {
                var payload = {
                    businessEntityDefinitionId: "94820cf8-c9a3-40a9-b90e-ba25a2a96d73",
                    selectIfSingleItem: true
                };
                VRUIUtilsService.callDirectiveLoad(poolBasedCLIGroupAPI, payload, poolBasedCLIGroupSelectorLoadDeferred);
            });
            return poolBasedCLIGroupSelectorLoadDeferred.promise;
        }
        function insert() {
            $scope.scopeModel.isLoading = true;

            return NP_IVSwitch_TranslationRuleAPIService.AddTranslationRule(buildTranslationRuleObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Translation Rule', response, 'Name')) {

                    if ($scope.onTranslationRuleAdded != undefined)
                        $scope.onTranslationRuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return NP_IVSwitch_TranslationRuleAPIService.UpdateTranslationRule(buildTranslationRuleObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Translation Rule', response, 'Name')) {

                    if ($scope.onTranslationRuleUpdated != undefined) {
                        $scope.onTranslationRuleUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
                translationRuleEntity = undefined;
            });
        }

        function buildTranslationRuleObjFromScope() {
            var fixedCLISettings;
            var poolBasedCLISettings;
            if ($scope.scopeModel.selectedCLIType != undefined) {
                if ($scope.scopeModel.selectedCLIType.value == 0) {
                    fixedCLISettings = {
                        $type: "NP.IVSwitch.Entities.FixedCLISettings, NP.IVSwitch.Entities",
                        CLIPatternSign: $scope.scopeModel.selectedEngineType.value == 0 ? $scope.scopeModel.fixedCLISettingsSelectedPrefixSign.value : undefined,
                        CLIPattern: $scope.scopeModel.CLIPattern
                    };
                }
                else {
                    poolBasedCLISettings = {
                        $type: "NP.IVSwitch.Entities.PoolBasedCLISettings, NP.IVSwitch.Entities",
                        Prefix: $scope.scopeModel.poolBasedCLISettingsPrefix,
                        Destination: $scope.scopeModel.poolBasedCLISettingsDestination,
                        RandMin: $scope.scopeModel.poolBasedCLISettingsRandMin,
                        RandMax: $scope.scopeModel.poolBasedCLISettingsRandMax,
                        DisplayName: $scope.scopeModel.poolBasedCLISettingsDisplayName,
                        CLIPatterns: $scope.scopeModel.CLIPatterns
                    };
                }
            }
            return {
                TranslationRuleId: translationRuleEntity != undefined ? translationRuleEntity.TranslationRuleId : undefined,
                Name: $scope.scopeModel.name,
                EngineType: $scope.scopeModel.selectedEngineType.value,
                DNISPatternSign:$scope.scopeModel.selectedEngineType.value==0? $scope.scopeModel.selectedPrefixSign.value : undefined,
                DNISPattern: $scope.scopeModel.dnisPattern,
                CLIType: $scope.scopeModel.selectedCLIType!=undefined ? $scope.scopeModel.selectedCLIType.value : null,
                FixedCLISettings: $scope.scopeModel.selectedCLIType != undefined && $scope.scopeModel.selectedCLIType.value == 0 ? fixedCLISettings : undefined,
                PoolBasedCLISettings: $scope.scopeModel.selectedCLIType != undefined && $scope.scopeModel.selectedCLIType.value == 1 ? poolBasedCLISettings : undefined
            };
        }
    }

    appControllers.controller('NP_IVSwitch_TranslationRuleEditorController', TranslationRuleEditorController);

})(appControllers);