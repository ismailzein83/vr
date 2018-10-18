(function (app) {

    'use strict';

    RuleTreeCriteriaGrid.$inject = ['VR_GenericData_GenericRuleAPIService', 'VR_GenericData_GenericRuleDefinitionAPIService', 'VR_GenericData_GenericRule', 'VRUIUtilsService', 'UtilsService', 'VRNotificationService'];

    function RuleTreeCriteriaGrid(VR_GenericData_GenericRuleAPIService, VR_GenericData_GenericRuleDefinitionAPIService, VR_GenericData_GenericRule, VRUIUtilsService, UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericRuleGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/GenericRule/Grid/Templates/RuleTreeCriteriaGridTemplate.html'
        };

        function GenericRuleGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accessibility;
            var criteriaFieldsToHide;

            var gridAPI;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.criteriaFields = [];
                $scope.genericRules = [];
                $scope.showSettingColum = false;

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_GenericData_GenericRule.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                    defineAPI();
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_GenericRuleAPIService.GetFilteredGenericRules(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {

                var directiveAPI = {};

                directiveAPI.loadGrid = function (query) {

                    var promises = [];

                    accessibility = query.accessibility;
                    criteriaFieldsToHide = query.criteriaFieldsToHide;

                    var getDefinitionPromise = VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(query.RuleDefinitionId);
                    promises.push(getDefinitionPromise);

                    var retrieveDataDeferred = UtilsService.createPromiseDeferred();
                    promises.push(retrieveDataDeferred.promise);

                    getDefinitionPromise.then(function (response) {
                        $scope.criteriaFields.length = 0;
                        if (response.CriteriaDefinition != undefined) {
                            for (var i = 0; i < response.CriteriaDefinition.Fields.length; i++) {
                                var criteriaFieldDefinition = response.CriteriaDefinition.Fields[i];
                                if (isCriteriaFieldVisible(criteriaFieldDefinition.FieldName)) {
                                    criteriaFieldDefinition.fieldValueIndex = i;
                                    $scope.criteriaFields.push(criteriaFieldDefinition);
                                }
                            }
                        }
                        if (response.SettingsDefinition != undefined && response.SettingsDefinition.GridSettingTitle != undefined) {
                            $scope.settingTitle = response.SettingsDefinition.GridSettingTitle;
                        }
                        else {
                            $scope.settingTitle = 'Settings';
                        }
                        $scope.showSettingColum = true;
                        gridAPI.retrieveData(query).then(function () { retrieveDataDeferred.resolve(); }).catch(function (error) { retrieveDataDeferred.reject(error); });
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                directiveAPI.onGenericRuleAdded = function (addedGenericRule) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedGenericRule);
                    gridAPI.itemAdded(addedGenericRule);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(directiveAPI);
                }
            }

            function isCriteriaFieldVisible(criteriaFieldName) {
                if (criteriaFieldsToHide == undefined)
                    return true;
                for (var i = 0; i < criteriaFieldsToHide.length; i++) {
                    if (criteriaFieldName == criteriaFieldsToHide[i])
                        return false;
                }
                return true;
            }

            function defineMenuActions() {
                $scope.gridMenuActions = function () {
                    return [{
                        name: 'Edit',
                        clicked: editGenericRule,
                        haspermission: hasEditGenericRulePermission
                    }, {
                        name: 'View',
                        clicked: viewGenericRule,
                        haspermission: hasViewGenericRulePermission
                    }];
                };
            }

            function hasEditGenericRulePermission(genericRule) {
                return VR_GenericData_GenericRuleAPIService.DoesUserHaveEditAccess(genericRule.Entity.DefinitionId);
            }

            function hasViewGenericRulePermission(genericRule) {
                return VR_GenericData_GenericRuleAPIService.DoesUserHaveEditAccess(genericRule.Entity.DefinitionId).then(function (response) {
                    return !response;
                });
            }

            function editGenericRule(genericRule) {
                var onGenericRuleUpdated = function (updatedGenericRule) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedGenericRule);
                    gridAPI.itemUpdated(updatedGenericRule);
                };
                VR_GenericData_GenericRule.editGenericRule(genericRule.Entity.RuleId, genericRule.Entity.DefinitionId, onGenericRuleUpdated, accessibility);
            }

            function viewGenericRule(genericRule) {
                VR_GenericData_GenericRule.viewGenericRule(genericRule.Entity.RuleId, genericRule.Entity.DefinitionId, accessibility);
            }

            function deleteGenericRule(genericRule) {
                var onGenericRuleDeleted = function () {
                    gridAPI.itemDeleted(genericRule);
                };
                VR_GenericData_GenericRule.deleteGenericRule($scope, genericRule.Entity, onGenericRuleDeleted);
            }
        }
    }

    app.directive('vrGenericdataGenericruleGridRuletreecriteria', RuleTreeCriteriaGrid);
})(app);