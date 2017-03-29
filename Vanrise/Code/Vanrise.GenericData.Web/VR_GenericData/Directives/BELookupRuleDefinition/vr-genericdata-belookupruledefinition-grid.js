(function (app) {

    'use strict';

    BELookupRuleDefinitionGridDirective.$inject = ['VR_GenericData_BELookupRuleDefinitionAPIService', 'VR_GenericData_BELookupRuleDefinitionService', 'VRNotificationService', 'VRUIUtilsService'];

    function BELookupRuleDefinitionGridDirective(VR_GenericData_BELookupRuleDefinitionAPIService, VR_GenericData_BELookupRuleDefinitionService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var beLookupRuleDefinitionGrid = new BELookupRuleDefinitionGrid($scope, ctrl, $attrs);
                beLookupRuleDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BELookupRuleDefinition/Templates/BELookupRuleDefinitionGridTemplate.html'
        };

        function BELookupRuleDefinitionGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController()
            {
                $scope.scopeModel = {};
                $scope.scopeModel.dataSource = [];

                $scope.scopeModel.onGridReady = function (api)
                {
                    gridAPI = api;
                    var drillDownDefinitions = VR_GenericData_BELookupRuleDefinitionService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady)
                {
                    return VR_GenericData_BELookupRuleDefinitionAPIService.GetFilteredBELookupRuleDefinitions(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
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

            function defineAPI()
            {
                var api = {};

                api.load = function (query)
                {
                    return gridAPI.retrieveData(query);
                };

                api.onBELookupRuleDefinitionAdded = function (addedBELookupRuleDefinition)
                {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedBELookupRuleDefinition);
                    gridAPI.itemAdded(addedBELookupRuleDefinition);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions()
            {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editBELookupRuleDefinition,
                    haspermission: hasEditPermission
                }];

                function editBELookupRuleDefinition(dataItem) {
                    var onBELookupRuleDefinitionUpdated = function (updatedBELookupRuleDefinition) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(updatedBELookupRuleDefinition);
                        gridAPI.itemUpdated(updatedBELookupRuleDefinition);
                    };
                    VR_GenericData_BELookupRuleDefinitionService.editBELookupRuleDefinition(dataItem.Entity.BELookupRuleDefinitionId, onBELookupRuleDefinitionUpdated);
                }

                function hasEditPermission() {
                    return VR_GenericData_BELookupRuleDefinitionAPIService.HasEditBELookupRuleDefinitionPermission();
                }
            }
        }
    }

    app.directive('vrGenericdataBelookupruledefinitionGrid', BELookupRuleDefinitionGridDirective);

})(app);