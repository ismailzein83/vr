(function (appControllers) {

    'use strict';

    genericRuleManagementController.$inject = ['$scope', 'VRNavigationService', 'VR_GenericData_GenericRuleAPIService', 'VR_GenericData_GenericRuleDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRDateTimeService'];

    function genericRuleManagementController($scope, VRNavigationService, VR_GenericData_GenericRuleAPIService, VR_GenericData_GenericRuleDefinitionAPIService, UtilsService, VRUIUtilsService, VRNotificationService, VRDateTimeService) {

        var viewId;
        var ruleDefinition;
        var criteriaDefinitionConfigs;
        var context;

        var gridAPI;

        var genericRuleDefinitionAPI;
        var genericRuleDefinitionReadyDeferred = UtilsService.createPromiseDeferred();

        var searchDirectiveAPI;
        var searchDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

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

            $scope.scopeModel.onGenericRuleDefinitionSelectorDirectiveReady = function (api) {
                genericRuleDefinitionAPI = api;
                genericRuleDefinitionReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                var defFilter = {
                    RuleDefinitionId: genericRuleDefinitionAPI.getSelectedIds(),
                    EffectiveDate: $scope.scopeModel.effectiveDate,
                    Description: $scope.scopeModel.description,
                    ReportName: ruleDefinition.Name
                };
                gridAPI.loadGrid(defFilter);
            };

            $scope.scopeModel.onSearchDirectiveReady = function (api) {
                searchDirectiveAPI = api;
                searchDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onGenericRuleDefinitionSelectorSelectionChange = function () {
                if (genericRuleDefinitionAPI.getSelectedIds() != undefined) {
                    $scope.scopeModel.gridloadded = false;
                    loadAllControls().then(function () {
                        $scope.scopeModel.gridloadded = true;
                        hasAddGenericRulePermission();
                        doesRuleSupportUpload();
                    });
                }
            };

            $scope.scopeModel.search = function () {
                var filter = searchDirectiveAPI.getSearchObject();
                filter.RuleDefinitionId = genericRuleDefinitionAPI.getSelectedIds();
                filter.EffectiveDate = $scope.scopeModel.effectiveDate;
                filter.Description = $scope.scopeModel.description;

                return gridAPI.loadGrid(filter);
            };

            $scope.scopeModel.addGenericRule = function () {
                var onGenericRuleAdded = function (ruleObj) {
                    gridAPI.onGenericRuleAdded(ruleObj);
                };

                var addRuleObj = { onGenericRuleAdded: onGenericRuleAdded };
                searchDirectiveAPI.addGenericRule(addRuleObj);
            };

            $scope.scopeModel.uploadGenericRules = function () {
                var uploadRulesObj = { context: getContext() };
                searchDirectiveAPI.uploadGenericRules(uploadRulesObj);
            };

        }
        function load() {
            $scope.isLoading = true;

            loadCriteriaDefinitionConfigs().then(function () {
                loadGenericRuleDefinition().catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadCriteriaDefinitionConfigs() {
            return VR_GenericData_GenericRuleDefinitionAPIService.GetCriteriaDefinitionConfigs().then(function (response) {
                criteriaDefinitionConfigs = response;
            });
        }

        function loadGenericRuleDefinition() {
            var genericRuleDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            genericRuleDefinitionReadyDeferred.promise.then(function () {
                var genericRuleDefinitionSelectorPayLoad = {
                    filter: {
                        Filters: [{
                            $type: "Vanrise.GenericData.Business.GenericRuleDefinitionViewFilter, Vanrise.GenericData.Business",
                            ViewId: viewId
                        }]
                    },
                    selectfirstitem: true
                };
                VRUIUtilsService.callDirectiveLoad(genericRuleDefinitionAPI, genericRuleDefinitionSelectorPayLoad, genericRuleDefinitionSelectorLoadDeferred);
            });

            return genericRuleDefinitionSelectorLoadDeferred.promise.then(function () {
                $scope.hideGenericRuleDefinition = genericRuleDefinitionAPI.hasSingleItem();
            });
        }

        function loadAllControls() {
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

                function getRuleDefinition() {
                    return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(genericRuleDefinitionAPI.getSelectedIds()).then(function (response) {
                        ruleDefinition = response;
                        $scope.scopeModel.selectedRuleDefinitionType = UtilsService.getItemByVal(criteriaDefinitionConfigs, ruleDefinition.CriteriaDefinition.ConfigId, "ExtensionConfigurationId");
                    });
                }
                function loadSearchDirective() {
                    var loadSearchDirectiveDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises([getRuleDefinitionPromise, searchDirectiveReadyDeferred.promise]).then(function () {
                        searchDirectiveReadyDeferred = undefined;
                        var searchDirectivePayload = { ruleDefinition: ruleDefinition };
                        VRUIUtilsService.callDirectiveLoad(searchDirectiveAPI, searchDirectivePayload, loadSearchDirectiveDeferred);
                    });

                    return loadSearchDirectiveDeferred.promise;
                };

                return UtilsService.waitMultiplePromises(promises);
            }

            return UtilsService.waitMultipleAsyncOperations([setStaticData, loadFilters]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function hasAddGenericRulePermission() {
            return VR_GenericData_GenericRuleAPIService.DoesUserHaveAddAccess(genericRuleDefinitionAPI.getSelectedIds()).then(function (response) {
                $scope.scopeModel.haveAddPermission = response;
            });
        }

        function doesRuleSupportUpload() {
            return VR_GenericData_GenericRuleAPIService.DoesRuleSupportUpload(genericRuleDefinitionAPI.getSelectedIds()).then(function (response) {
                $scope.scopeModel.ruleSupportsUpload = response;
            });
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleManagementController', genericRuleManagementController);
})(appControllers);