(function (appControllers) {
    'use strict';

    genericRuleManagementController.$inject = ['$scope', 'VRNavigationService', 'VR_GenericData_GenericRuleAPIService', 'VR_GenericData_GenericRuleDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRDateTimeService'];

    function genericRuleManagementController($scope, VRNavigationService, VR_GenericData_GenericRuleAPIService, VR_GenericData_GenericRuleDefinitionAPIService, UtilsService, VRUIUtilsService, VRNotificationService, VRDateTimeService) {

        var gridAPI;

        var genericRuleDefinitionAPI;
        var genericRuleDefinitionReadyDeferred = UtilsService.createPromiseDeferred();

        var context;
        var ruleDefinition;

        var searchDirectiveAPI;
        var searchDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var criteriaDefinitions;
        loadParameters();
        defineScope();

        var promises = [];

        var loadCriteriaDefinitionConfigsPromise = loadCriteriaDefinitionConfigs();
        promises.push(loadCriteriaDefinitionConfigsPromise);

        UtilsService.waitMultiplePromises(promises).then(function () {
            load();
        });

        var viewId;

        function loadCriteriaDefinitionConfigs() {
            $scope.isLoading = true;
            return VR_GenericData_GenericRuleDefinitionAPIService.GetCriteriaDefinitionConfigs().then(function (response) {
                criteriaDefinitions = response;
            });
        };

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null) {
                viewId = parameters.viewId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.haveAddPermission = false;
            $scope.scopeModel.gridloadded = false;
            $scope.scopeModel.ruleSupportsUpload = false;

            $scope.onGenericRuleDefinitionSelectorDirectiveReady = function (api) {
                genericRuleDefinitionAPI = api;
                genericRuleDefinitionReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var defFilter = {
                    RuleDefinitionId: genericRuleDefinitionAPI.getSelectedIds(),
                    EffectiveDate: $scope.scopeModel.effectiveDate,
                    Description: $scope.scopeModel.description,
                    ReportName: ruleDefinition.Name
                };
                gridAPI.loadGrid(defFilter);
            };

            $scope.onGenericRuleDefinitionSelectorSelectionChange = function () {
                if (genericRuleDefinitionAPI.getSelectedIds() != undefined) {
                    $scope.scopeModel.gridloadded = false;
                    loadAllControls().then(function () {
                        $scope.scopeModel.gridloadded = true;
                        hasAddGenericRulePermission();
                        doesRuleSupportUpload();
                    });
                }
            };

            $scope.search = function () {
                var filter = searchDirectiveAPI.getSearchObject();

                filter.RuleDefinitionId = genericRuleDefinitionAPI.getSelectedIds();
                filter.EffectiveDate = $scope.scopeModel.effectiveDate;
                filter.Description = $scope.scopeModel.description;

                return gridAPI.loadGrid(filter);
            };

            $scope.onSearchDirectiveReady = function (api) {
                searchDirectiveAPI = api;
                searchDirectiveReadyDeferred.resolve();
            }

            $scope.addGenericRule = function () {
                var onGenericRuleAdded = function (ruleObj) {
                    gridAPI.onGenericRuleAdded(ruleObj);
                };

                var addRuleObj = { onGenericRuleAdded: onGenericRuleAdded };
                searchDirectiveAPI.addGenericRule(addRuleObj);
            };

            $scope.uploadGenericRules = function () {
                var uploadRulesObj = { context: getContext() };
                searchDirectiveAPI.uploadGenericRules(uploadRulesObj);
            };

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {
                    };
                return currentContext;
            }
        }

        function load() {
            $scope.isLoading = true;
            loadGenericRuleDefinition();
        }

        function loadGenericRuleDefinition() {
            var loadGenericRuleDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            genericRuleDefinitionReadyDeferred.promise.then(function () {
                var payLoad;
                payLoad = {
                    filter: {
                        Filters: [{
                            $type: "Vanrise.GenericData.Business.GenericRuleDefinitionViewFilter, Vanrise.GenericData.Business",
                            ViewId: viewId
                        }]
                    },
                    selectfirstitem: true
                };
                VRUIUtilsService.callDirectiveLoad(genericRuleDefinitionAPI, payLoad, loadGenericRuleDefinitionSelectorPromiseDeferred);
            });
            return loadGenericRuleDefinitionSelectorPromiseDeferred.promise.then(function () {
                $scope.hideGenericRuleDefinition = genericRuleDefinitionAPI.hasSingleItem();
            });
        }

        function loadAllControls() {
            $scope.isLoading = true;
            $scope.scopeModel.selectedRuleDefinitionType = undefined;

            function setStaticData() {
                $scope.scopeModel.effectiveDate = VRDateTimeService.getNowDateTime();
            }
            function loadFilters() {
                var promises = [];

                searchDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

                var getRuleDefinitionPromise = getRuleDefinition();
                promises.push(getRuleDefinitionPromise);

                var loadSearchDirectivePromise = loadSearchDirective();
                promises.push(loadSearchDirectivePromise);

                function loadSearchDirective() {
                    var loadSearchDirectiveDeferred = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultiplePromises([getRuleDefinitionPromise, searchDirectiveReadyDeferred.promise]).then(function () {
                        searchDirectiveReadyDeferred = undefined;
                        var searchDirectivePayload = { ruleDefinition: ruleDefinition };
                        VRUIUtilsService.callDirectiveLoad(searchDirectiveAPI, searchDirectivePayload, loadSearchDirectiveDeferred);
                    });

                    return loadSearchDirectiveDeferred.promise;
                };

                function getRuleDefinition() {
                    return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(genericRuleDefinitionAPI.getSelectedIds()).then(function (response) {
                        ruleDefinition = response;
                        $scope.scopeModel.selectedRuleDefinitionType = UtilsService.getItemByVal(criteriaDefinitions, ruleDefinition.CriteriaDefinition.ConfigId, "ExtensionConfigurationId");
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            }

            return UtilsService.waitMultipleAsyncOperations([setStaticData, loadFilters]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function hasAddGenericRulePermission() {
            return VR_GenericData_GenericRuleAPIService.DoesUserHaveAddAccess(genericRuleDefinitionAPI.getSelectedIds()).then(function (response) {
                $scope.scopeModel.haveAddPermission = response;
            });
        };

        function doesRuleSupportUpload() {
            return VR_GenericData_GenericRuleAPIService.DoesRuleSupportUpload(genericRuleDefinitionAPI.getSelectedIds()).then(function (response) {
                $scope.scopeModel.ruleSupportsUpload = response;
            });
        };
    }

    appControllers.controller('VR_GenericData_GenericRuleManagementController', genericRuleManagementController);

})(appControllers);
