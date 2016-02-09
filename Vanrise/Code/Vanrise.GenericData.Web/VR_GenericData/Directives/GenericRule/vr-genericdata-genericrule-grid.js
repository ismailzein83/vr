(function (app) {

    'use strict';

    GenericRuleGridDirective.$inject = ['VR_GenericData_GenericRuleDefinitionAPIService', 'VR_GenericData_GenericRuleDefinitionService', 'VRNotificationService'];

    function GenericRuleGridDirective(VR_GenericData_GenericRuleDefinitionAPIService, VR_GenericData_GenericRuleDefinitionService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericRuleDefinitionGrid = new GenericRuleDefinitionGrid($scope, ctrl, $attrs);
                genericRuleDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericRuleDefinition/Templates/GenericRuleDefinitionGridTemplate.html'
        };

        function GenericRuleDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.genericRuleDefinitions = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_GenericRuleDefinitionAPIService.GetFilteredGenericRuleDefinitions(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onGenericRuleDefinitionAdded = function (addedGenericRuleDefinition) {
                    gridAPI.itemAdded(addedGenericRuleDefinition);
                };

                return api;
            }

            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: 'Edit',
                    clicked: editGenericRuleDefinition,
                }];
            }

            function editGenericRuleDefinition(genericRuleDefinition) {
                var onGenericRuleDefinitionUpdated = function (updatedGenericRuleDefinition) {
                    gridAPI.itemUpdated(updatedGenericRuleDefinition);
                };
                VR_GenericData_GenericRuleDefinitionService.editGenericRuleDefinition(genericRuleDefinition.GenericRuleDefinitionId, onGenericRuleDefinitionUpdated);
            }
        }
    }

    app.directive('vrGenericdataGenericruleGrid', GenericRuleGridDirective);

})(app);