"use strict";
app.directive("vrCommonDynamicapiGrid", ["UtilsService", "VRNotificationService", "VR_Dynamic_APIService","VRCommon_VRDynamicAPIAPIService" ,"VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, VR_Dynamic_APIService,VRCommon_VRDynamicAPIAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

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

        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.vrDynamicAPIs = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
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
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            $scope.scopeModel.AddVRDynamicAPI = function () {

                var onVRDynamicAPIAdded = function (vrDynamicAPI) {
                    $scope.scopeModel.vrDynamicAPIs.push(vrDynamicAPI);

                };
                VR_Dynamic_APIService.addVRDynamicAPI(onVRDynamicAPIAdded, vrDynamicAPIModuleId);
            };

            defineMenuActions();
        }

        function defineMenuActions() {

            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editVRDynamicAPI,
            }];
        }

        function editVRDynamicAPI(vrDynamicAPI) {

            var onVRDynamicAPIUpdated = function (vrDynamicAPI) {
                gridApi.itemUpdated(vrDynamicAPI); 
            };
            VR_Dynamic_APIService.editVRDynamicAPI(vrDynamicAPI.VRDynamicAPIId, onVRDynamicAPIUpdated, vrDynamicAPIModuleId);
        }
    }

    return directiveDefinitionObject;
}]);
