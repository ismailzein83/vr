'use strict';

app.directive('npIvswitchTranslationruleGrid', ['NP_IVSwitch_TranslationRuleAPIService', 'NP_IVSwitch_TranslationRuleService', 'VRNotificationService', 'VRUIUtilsService',
    function (NP_IVSwitch_TranslationRuleAPIService, NP_IVSwitch_TranslationRuleService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var translationRuleGrid = new TranslationRuleGrid($scope, ctrl, $attrs);
                translationRuleGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/NP_IVSwitch/Directives/TranslationRule/Templates/TranslationRuleGridTemplate.html'
        };

        function TranslationRuleGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.translationRule = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = NP_IVSwitch_TranslationRuleService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return NP_IVSwitch_TranslationRuleAPIService.GetFilteredTranslationRules(dataRetrievalInput).then(function (response) {
                        if (response !=undefined && response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                     });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onTranslationRuleAdded = function (addedTranslationRule) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedTranslationRule);
                    gridAPI.itemAdded(addedTranslationRule);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editTranslationRule,
                    haspermission: hasEditTranslationRulePermission
                });
            }
            function editTranslationRule(translationRuleItem) {
                var onTranslationRuleUpdated = function (updatedTranslationRule) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedTranslationRule);
                    gridAPI.itemUpdated(updatedTranslationRule);
                };

                NP_IVSwitch_TranslationRuleService.editTranslationRule(translationRuleItem.Entity.TranslationRuleId, onTranslationRuleUpdated);
            }
            function hasEditTranslationRulePermission() {
                return NP_IVSwitch_TranslationRuleAPIService.HasEditTranslationRulePermission();
            }

        }
    }]);
