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
                var obj = new GenericRuleGrid($scope, ctrl, $attrs);
                obj.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/GenericRule/Grid/Templates/RuleTreeCriteriaGridTemplate.html'
        };

        function GenericRuleGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridDrillDownTabsObj;
            var gridAPI;
            var accessibility;
            var criteriaFieldsToHide;

            function initializeController() {
                ctrl.criteriaFields = [];
                $scope.genericRules = [];
                $scope.showSettingColum = false;


                $scope.onGridReady = function (api) {
                    var drillDownDefinitions = VR_GenericData_GenericRule.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
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

                //defineMenuActions();
            }

            function getDirectiveAPI() {
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
                        ctrl.criteriaFields.length = 0;
                        if (response.CriteriaDefinition != undefined) {
                            for (var i = 0; i < response.CriteriaDefinition.Fields.length; i++) {
                                var criteriaFieldDefinition = response.CriteriaDefinition.Fields[i];
                                if (isCriteriaFieldVisible(criteriaFieldDefinition.FieldName)) {
                                    criteriaFieldDefinition.fieldValueIndex = i;
                                    ctrl.criteriaFields.push(criteriaFieldDefinition);
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

                return directiveAPI;
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

            // function defineMenuActions() {
            $scope.gridMenuActions = function () {
                return [{
                    name: 'Edit',
                    clicked: editGenericRule,
                    haspermission: hasEditGenericRulePermission
                }, {
                    name: 'View',
                    clicked: viewGenericRule,
                    haspermission: hasViewGenericRulePermission
                }
                ];
            };
            //  }

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