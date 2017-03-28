(function (app) {

    'use strict';

    GenericRuleDefinitionGridDirective.$inject = ['VR_GenericData_GenericRuleDefinitionAPIService', 'VR_GenericData_GenericRuleDefinitionService', 'VRNotificationService', 'VRUIUtilsService'];

    function GenericRuleDefinitionGridDirective(VR_GenericData_GenericRuleDefinitionAPIService, VR_GenericData_GenericRuleDefinitionService, VRNotificationService, VRUIUtilsService) {
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
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.genericRuleDefinitions = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_GenericData_GenericRuleDefinitionService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_GenericRuleDefinitionAPIService.GetFilteredGenericRuleDefinitions(dataRetrievalInput).then(function (response) {
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

            function getDirectiveAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onGenericRuleDefinitionAdded = function (addedGenericRuleDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedGenericRuleDefinition);
                    gridAPI.itemAdded(addedGenericRuleDefinition);
                };

                return api;
            }

            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: 'Edit',
                    clicked: editGenericRuleDefinition,
                    haspermission: hasEditGenericRuleDefinitionPermission

                }];
            }
            function hasEditGenericRuleDefinitionPermission() {
                return VR_GenericData_GenericRuleDefinitionAPIService.HasUpdateGenericRuleDefinition();
            }
            function editGenericRuleDefinition(genericRuleDefinition) {
                var onGenericRuleDefinitionUpdated = function (updatedGenericRuleDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedGenericRuleDefinition);
                    gridAPI.itemUpdated(updatedGenericRuleDefinition);
                };
                VR_GenericData_GenericRuleDefinitionService.editGenericRuleDefinition(genericRuleDefinition.GenericRuleDefinitionId, onGenericRuleDefinitionUpdated);
            }
        }
    }

    app.directive('vrGenericdataGenericruledefinitionGrid', GenericRuleDefinitionGridDirective);

})(app);