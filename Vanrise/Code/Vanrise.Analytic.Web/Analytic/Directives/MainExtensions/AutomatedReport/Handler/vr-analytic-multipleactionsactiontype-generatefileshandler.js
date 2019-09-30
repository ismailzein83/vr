'use strict';

app.directive('vrAnalyticMultipleactionsactiontypeGeneratefileshandler', ['UtilsService', 'VR_Analytic_AutomatedReportHandlerSettingsAPIService', 'VR_Analytic_MultipleActionsActionTypeService',
    function (UtilsService, VR_Analytic_AutomatedReportHandlerSettingsAPIService, VR_Analytic_MultipleActionsActionTypeService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var multipleActionsHandler = new MultipleActionsActionType($scope, ctrl, $attrs);
                multipleActionsHandler.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Handler/Templates/MultipleActionsHandlerAutomatedReport.html'
        };

        function MultipleActionsActionType($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            var actionTypeSelectorAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onActionTypeSelectorReady = function (api) {
                    actionTypeSelectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.isAddActionButtonDisabled = function () {
                    if ($scope.scopeModel.selectedTemplateConfig != undefined)
                        return false;
                    else
                        return true;
                };

                $scope.scopeModel.addAction = function () {
                    var onActionAdded = function (action) {
                        $scope.scopeModel.datasource.push(action);
                    };

                    VR_Analytic_MultipleActionsActionTypeService.addAction($scope.scopeModel.selectedTemplateConfig, getContext(), onActionAdded);
                };

                $scope.scopeModel.removeAction = function (action) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, action.rowIndex, 'rowIndex');
                    if (index > -1)
                        $scope.scopeModel.datasource.splice(index, 1);
                };

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.datasource == undefined || $scope.scopeModel.datasource.length == 0)
                        return 'You should add at least one Action';

                    return null;
                };

                $scope.gridMenuActions = [{
                    name: 'Edit',
                    clicked: editAction
                }];
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    actionTypeSelectorAPI.clearDataSource();

                    var promises = [];
                    var actions = [];

                    if (payload != undefined) {
                        context = payload.context;

                        if (payload.actionType != undefined) {
                            actions = payload.actionType.Actions;
                        }
                    }

                    var getAutomatedReportActionTypesTemplateConfigsPromise = getAutomatedReportHandlerTemplateConfigs();

                    getAutomatedReportActionTypesTemplateConfigsPromise.then(function () {
                        loadActionsGrid();
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });

                    promises.push(getAutomatedReportActionTypesTemplateConfigsPromise);

                    function getAutomatedReportHandlerTemplateConfigs() {
                        return VR_Analytic_AutomatedReportHandlerSettingsAPIService.GetAutomatedReportHandlerActionTypesTemplateConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                            }
                        });
                    }

                    function loadActionsGrid() {
                        var actionsGridLoadDeferred = UtilsService.createPromiseDeferred();

                        if (actions != undefined) {
                            for (var i = 0; i < actions.length; i++) {
                                var currentAction = actions[i];
                                addActionActionTypeField(currentAction);
                                $scope.scopeModel.datasource.push(currentAction);
                            }
                        }
                        actionsGridLoadDeferred.resolve();
                        return actionsGridLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var actions = [];
                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++)
                        actions.push($scope.scopeModel.datasource[i]);

                    return {
                        $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers.MultipleActionsActionType, Vanrise.Analytic.MainExtensions",
                        Actions: actions
                    };
                };

                if (ctrl.onReady != undefined)
                    ctrl.onReady(api);
            }

            function editAction(actionObject) {
                var onActionUpdated = function (action) {
                    var index = $scope.scopeModel.datasource.indexOf(actionObject);
                    $scope.scopeModel.datasource[index] = action;
                };

                var actionTemplateConfigs = getActionTemplateConfigs(actionObject);
                VR_Analytic_MultipleActionsActionTypeService.editAction(actionObject, actionTemplateConfigs, getContext(), onActionUpdated);
            }

            function getActionTemplateConfigs(actionObject) {
                for (var i = 0; i < $scope.scopeModel.templateConfigs.length; i++) {
                    if ($scope.scopeModel.templateConfigs[i].ExtensionConfigurationId == actionObject.ConfigId) {
                        return $scope.scopeModel.templateConfigs[i];
                    }
                }
            }

            function addActionActionTypeField(action) {
                for (var i = 0; i < $scope.scopeModel.templateConfigs.length; i++) {
                    if ($scope.scopeModel.templateConfigs[i].ExtensionConfigurationId == action.ConfigId) {
                        action.ActionType = $scope.scopeModel.templateConfigs[i].Title;
                    }
                }
            }

            function getContext() {
                var currentContext = context;

                if (currentContext == undefined) {
                    currentContext = {};
                }

                return currentContext;
            }
        }

        return directiveDefinitionObject;
    }]);