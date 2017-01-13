'use strict';

app.directive('retailBeRecurringchargedefinitionGrid', ['VRNotificationService', 'Retail_BE_RecurringChargeDefinitionAPIService', 'Retail_BE_RecurringChargeDefinitionService',
    function (VRNotificationService, Retail_BE_RecurringChargeDefinitionAPIService, Retail_BE_RecurringChargeDefinitionService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new recurringChargeDefinitionGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/RecurringChargeDefinition/Templates/RecurringChargeDefinitionGridTemplate.html'
        };

        function recurringChargeDefinitionGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.recurringChargeDefinitions = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_RecurringChargeDefinitionAPIService.GetFilteredRecurringChargeDefinitions(dataRetrievalInput).then(function (response) {
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

                api.onRecurringChargeDefinitionAdded = function (addedRecurringChargeDefinition) {
                    gridAPI.itemAdded(addedRecurringChargeDefinition);
                };

                api.onRecurringChargeDefinitionUpdated = function (updatedRecurringChargeDefinition) {
                    gridAPI.itemUpdated(updatedRecurringChargeDefinition);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editRecurringChargeDefinition,
                    haspermission: hasEditRecurringChargeDefinitionPermission
                });
            }
            function editRecurringChargeDefinition(item) {
                var onRecurringChargeDefinitionUpdated = function (updatedRecurringChargeDefinition) {
                    gridAPI.itemUpdated(updatedRecurringChargeDefinition);
                };

                Retail_BE_RecurringChargeDefinitionService.editRecurringChargeDefinition(item.Entity.RecurringChargeDefinitionId, onRecurringChargeDefinitionUpdated);
            }
            function hasEditRecurringChargeDefinitionPermission() {
                return Retail_BE_RecurringChargeDefinitionAPIService.HasUpdateRecurringChargeDefinitionPermission()
            }
        }
    }]);
