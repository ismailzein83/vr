'use strict';

app.directive('npIvswitchTranslationruleGrid', ['NP_IVSwitch_TranslationRuleAPIService', 'NP_IVSwitch_TranslationRuleService', 'VRNotificationService', 'VRUIUtilsService', 'NP_IVSwitch_CLITypeEnum',
    function (NP_IVSwitch_TranslationRuleAPIService, NP_IVSwitch_TranslationRuleService, VRNotificationService, VRUIUtilsService, NP_IVSwitch_CLITypeEnum) {
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
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.translationRule = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return NP_IVSwitch_TranslationRuleAPIService.GetFilteredTranslationRules(dataRetrievalInput).then(function (response) {
                        if (response != undefined && response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var translationRule = response.Data[i];
                                NP_IVSwitch_TranslationRuleService.defineTranslationRuleViewTabs(translationRule, gridAPI);
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
                    NP_IVSwitch_TranslationRuleService.defineTranslationRuleViewTabs(addedTranslationRule, gridAPI);
                    gridAPI.itemAdded(addedTranslationRule);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [
                    {
                    name: 'Edit',
                    clicked: editTranslationRule,
                    haspermission: hasEditTranslationRulePermission
                    }, 
                    {
                        name: 'Delete',
                        clicked: deleteTranslationRule,
                        haspermission: hasDeleteTranslationRulePermission
                    }
                ];
            }
            function editTranslationRule(translationRuleItem) {
                var onTranslationRuleUpdated = function (updatedTranslationRule) {
                    NP_IVSwitch_TranslationRuleService.defineTranslationRuleViewTabs(updatedTranslationRule, gridAPI);
                    gridAPI.itemUpdated(updatedTranslationRule);
                };

                NP_IVSwitch_TranslationRuleService.editTranslationRule(translationRuleItem.TranslationRuleId, onTranslationRuleUpdated);
            }
            function hasEditTranslationRulePermission() {
                return NP_IVSwitch_TranslationRuleAPIService.HasEditTranslationRulePermission();
            }

            function deleteTranslationRule(translationRuleItem) {
                VRNotificationService.showConfirmation().then(function (response) {
                    if (response) {
                        NP_IVSwitch_TranslationRuleAPIService.DeleteTranslationRule(translationRuleItem.TranslationRuleId).then(function () {
                            gridAPI.itemDeleted(translationRuleItem);
                        });
                    }
                });
            }
            function hasDeleteTranslationRulePermission() {
                return NP_IVSwitch_TranslationRuleAPIService.HasDeleteTranslationRulePermission();
            }
        }
    }]);
