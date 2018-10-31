"use strict"
app.directive("demoModulePageRunTimeGrid", ["UtilsService", "VRNotificationService", "Demo_Module_PageRunTimeAPIService","Demo_Module_PageDefinitionAPIService", "Demo_Module_PageRunTimeService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_PageRunTimeAPIService,Demo_Module_PageDefinitionAPIService, Demo_Module_PageRunTimeService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveRunTimeObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var pageRunTimeGrid = new PageRunTimeGrid($scope, ctrl, $attrs);
            pageRunTimeGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/PageRunTime/Templates/PageRunTimeGridTemplate.html"
    };

    function PageRunTimeGrid($scope, ctrl) {

        var gridApi;
        var gridDrillDownTabsObj
        var pageDefinitionEntity;
        $scope.scopeModel = {};
        $scope.scopeModel.fields = [];

        this.initializeController = initializeController;

        function initializeController() {
            var subview
            $scope.scopeModel.pageRunTimes = [];

            function getPageDefinition(pageRunTimeItem) {
                var pageDefinitionId = pageRunTimeItem.PageDefinitionId;
                return Demo_Module_PageDefinitionAPIService.GetPageDefinitionById(pageDefinitionId).then(function (response) {
                    pageDefinitionEntity = response;
                });
            }

            $scope.scopeModel.onGridReady = function (api) {

                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};
                    
                    directiveApi.load = function (payload) {
                        $scope.scopeModel.fields = [];
                        var query = payload.query;  

                        function prepareSubviewDefinition(subview) {
                            var subviewDefinition = {};

                            subviewDefinition.title = subview.Title;
                            subviewDefinition.directive = subview.PageDefinitionSubViewSettings.RunTimeEditor;
                            subviewDefinition.loadDirective = function (directiveAPI, pageRunTimeItem) {

                                pageRunTimeItem.gridAPI = directiveAPI;
                                var payload = {};
                                payload.pageDefinitionSubViewSettings = subview.PageDefinitionSubViewSettings;
                                payload.pageRunTimeItem = pageRunTimeItem;
                                return pageRunTimeItem.gridAPI.load(payload);

                            };
                            subviewDefinitions.push(subviewDefinition);

                        }
                        if (payload.subviews != undefined) {
                            var subviewDefinitions = [];
                            for (var i = 0; i < payload.subviews.length; i++) {

                                 subview = payload.subviews[i];
                                 prepareSubviewDefinition(subview);
                                
                            }
                            gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(subviewDefinitions, gridApi, $scope.gridMenuActions);
                        }
                        if (payload.fields != undefined) {
                            for (var j = 0; j < payload.fields.length; j++) {
                                var field = payload.fields[j];
                                $scope.scopeModel.fields.push(field)
                            } console.log($scope.scopeModel.fields)
                        }
                        return gridApi.retrieveData(query);
                    };

                    directiveApi.onPageRunTimeAdded = function (pageRunTime) {
                        gridApi.itemAdded(pageRunTime);
                        if (gridDrillDownTabsObj != undefined) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(pageRunTime);
                        }
                    };
                    return directiveApi;
                };
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_PageRunTimeAPIService.GetFilteredPageRunTimes(dataRetrievalInput)
                .then(function (response) {
                    if (gridDrillDownTabsObj != undefined) {
                    if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                    }
                    onResponseReady(response);
                })
                .catch(function (error) {

                    VRNotificationService.notifyException(error, $scope);
                });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editPageRunTime,

            }];
        }

        function editPageRunTime(pageRunTime) {
            var onPageRunTimeUpdated = function (pageRunTime) {
                gridApi.itemUpdated(pageRunTime);
                if (gridDrillDownTabsObj != undefined) {

                    gridDrillDownTabsObj.setDrillDownExtensionObject(pageRunTime);
                }
            };

            Demo_Module_PageRunTimeService.editPageRunTime(pageRunTime.PageRunTimeId,pageRunTime.PageDefinitionId, onPageRunTimeUpdated);
        }
    }

    return directiveRunTimeObject;
}]);
