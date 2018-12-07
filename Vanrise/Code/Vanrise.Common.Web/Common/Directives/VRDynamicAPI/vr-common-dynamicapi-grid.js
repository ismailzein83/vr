"use strict";
app.directive("vrCommonDynamicapiGrid", ["UtilsService", "VRNotificationService", "VRCommon_DynamicAPIService","VRCommon_VRDynamicAPIAPIService" ,"VRUIUtilsService", "VRCommon_ObjectTrackingService",
    function (UtilsService, VRNotificationService, VRCommon_DynamicAPIService,VRCommon_VRDynamicAPIAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var vrDynamicAPIGrid = new VRDynamicAPIGrid($scope, ctrl, $attrs);
            vrDynamicAPIGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Common/Directives/VRDynamicAPI/Templates/DynamicAPIGridTemplate.html"
    };

    function VRDynamicAPIGrid($scope, ctrl) {

        var gridApi;
        var vrDynamicAPIModuleId;
        var gridDrillDownTabsObj;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.vrDynamicAPIs = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                var drillDownDefinitions = VRCommon_DynamicAPIService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridApi, $scope.scopeModel.gridMenuActions);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }
                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (payload) {
                        vrDynamicAPIModuleId = payload.VRDynamicAPIModuleId;
                        var query = payload;
                        return gridApi.retrieveData(query);
                    };

                    return directiveApi;
                }
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return VRCommon_VRDynamicAPIAPIService.GetFilteredVRDynamicAPIs(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                $scope.scopeModel.vrDynamicAPIs.push(response.Data[i]);
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            $scope.scopeModel.AddVRDynamicAPI = function () {

                var onVRDynamicAPIAdded = function (vrDynamicAPI) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(vrDynamicAPI);
                    $scope.scopeModel.vrDynamicAPIs.push(vrDynamicAPI);
                };
                VRCommon_DynamicAPIService.addVRDynamicAPI(onVRDynamicAPIAdded, vrDynamicAPIModuleId);
            };

            defineMenuActions();
        }

        function defineMenuActions() {

            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editVRDynamicAPI,
                haspermission: hasEditVRDynamicAPIPermission
            }];
        }

        function editVRDynamicAPI(vrDynamicAPI) {

            var onVRDynamicAPIUpdated = function (vrDynamicAPI) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(vrDynamicAPI);
                gridApi.itemUpdated(vrDynamicAPI); 
            };
            VRCommon_DynamicAPIService.editVRDynamicAPI(vrDynamicAPI.VRDynamicAPIId, onVRDynamicAPIUpdated, vrDynamicAPIModuleId);
        }

        function hasEditVRDynamicAPIPermission() {
            return VRCommon_VRDynamicAPIAPIService.HasEditVRDynamicAPIPermission();
        }
    }

    return directiveDefinitionObject;
}]);
