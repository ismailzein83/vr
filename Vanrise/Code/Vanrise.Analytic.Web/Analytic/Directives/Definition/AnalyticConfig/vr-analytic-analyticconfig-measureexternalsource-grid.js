"use strict";

app.directive("vrAnalyticAnalyticconfigMeasureexternalsourceGrid", ['VRNotificationService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigService', 'UtilsService', 'VR_Analytic_AnalyticItemConfigAPIService', 'VRUIUtilsService', function (VRNotificationService, VRModalService, VR_Analytic_AnalyticItemConfigService, UtilsService, VR_Analytic_AnalyticItemConfigAPIService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var analyticExternalSourceGrid = new AnalyticExternalSourceGrid($scope, ctrl, $attrs);
            analyticExternalSourceGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticConfig/Templates/AnalyticMeasureExternalSourceGridTemplate.html"
    };
    function AnalyticExternalSourceGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;
        var tableId;
        var itemType;

        function initializeController() {
            $scope.externalSources = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = VR_Analytic_AnalyticItemConfigService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        
                        if (query != undefined) {
                            tableId = query.TableId;
                            itemType = query.ItemType;
                        }
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onAnalyticExternalSourceAdded = function (externalSourceObj) {
                        
                        gridDrillDownTabsObj.setDrillDownExtensionObject(externalSourceObj);
                        gridAPI.itemAdded(externalSourceObj);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                
                return VR_Analytic_AnalyticItemConfigAPIService.GetFilteredAnalyticItemConfigs(dataRetrievalInput)
                    .then(function (response) {
                       
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };
            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editExternalSource
            }];
        }
        function editExternalSource(dataItem) {
            
            var onEditExternalSource = function (externalSourceObj) {
                
                gridDrillDownTabsObj.setDrillDownExtensionObject(externalSourceObj);
                gridAPI.itemUpdated(externalSourceObj);
            };
            VR_Analytic_AnalyticItemConfigService.editItemConfig(dataItem.Entity.AnalyticItemConfigId, onEditExternalSource, tableId, itemType);
        }

    }
    return directiveDefinitionObject;
}]);