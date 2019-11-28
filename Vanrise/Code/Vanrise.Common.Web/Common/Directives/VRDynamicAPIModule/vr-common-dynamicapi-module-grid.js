﻿"use strict";
app.directive("vrCommonDynamicapiModuleGrid", ["UtilsService", "VRNotificationService", "VRCommon_VRDynamicAPIAPIService", "VRCommon_DynamicAPIModuleService", "VRCommon_VRDynamicAPIModuleAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService", "VR_GenericData_GenericBusinessEntityService",
    function (UtilsService, VRNotificationService, VRCommon_VRDynamicAPIAPIService, VRCommon_DynamicAPIModuleService, VRCommon_VRDynamicAPIModuleAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService, VR_GenericData_GenericBusinessEntityService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrDynamicAPIModuleGrid = new VRDynamicAPIModuleGrid($scope, ctrl, $attrs);
                vrDynamicAPIModuleGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/VRDynamicAPIModule/Templates/DynamicAPIModuleGridTemplate.html"
        };

        function VRDynamicAPIModuleGrid($scope, ctrl) {

            var gridApi;
            var gridDrillDownTabsObj;

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.vrDynamicAPIModules = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;

                    var drillDownDefinitions = [];
                    AddVRDynamicAPIModuleDrillDown();

                    function AddVRDynamicAPIModuleDrillDown() {
                        var drillDownDefinition = {};

                        drillDownDefinition.title = "Dynamic APIs";
                        drillDownDefinition.directive = "vr-common-dynamicapi-grid";
                        drillDownDefinition.haspermission = function () {
                            return VRCommon_VRDynamicAPIAPIService.HasGetFilteredVRDynamicAPIsPermission();
                        };
                        drillDownDefinition.loadDirective = function (directiveAPI, vrDynamicAPIModuleItem) {
                            vrDynamicAPIModuleItem.vrDynamicAPIModuleGridAPI = directiveAPI;
                            var payload = {
                                VRDynamicAPIModuleId: vrDynamicAPIModuleItem.VRDynamicAPIModuleId
                            };
                            return vrDynamicAPIModuleItem.vrDynamicAPIModuleGridAPI.load(payload);
                        };
                        drillDownDefinitions.push(drillDownDefinition);
                        drillDownDefinitions.push(VRCommon_DynamicAPIModuleService.getDrillDownDefinition()[0]);
                    }
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridApi, $scope.scopeModel.gridMenuActions);


                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveApi());
                    }

                    function getDirectiveApi() {
                        var directiveApi = {};

                        directiveApi.load = function (payload) {
                            var query = payload.query;

                            return gridApi.retrieveData(query);
                        };

                        directiveApi.onVRDynamicAPIModuleAdded = function (vrDynamicAPIModule) {
                            gridApi.itemAdded(vrDynamicAPIModule);
                            gridDrillDownTabsObj.setDrillDownExtensionObject(vrDynamicAPIModule);

                        };
                        return directiveApi;
                    }
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                    return VRCommon_VRDynamicAPIModuleAPIService.GetFilteredVRDynamicAPIModules(dataRetrievalInput)
                        .then(function (response) {
                            if (response && response.Data) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
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
                    clicked: editVRDynamicAPIModule,
                    haspermission: hasEditVRDynamicAPIModulePermission
                },
                {
                    name: "Compile Project",
                    clicked: compileDevProject,
                }];
            }

            function editVRDynamicAPIModule(vrDynamicAPIModule) {
                var onVRDynamicAPIModuleUpdated = function (vrDynamicAPIModule) {
                    gridApi.itemUpdated(vrDynamicAPIModule);
                    gridDrillDownTabsObj.setDrillDownExtensionObject(vrDynamicAPIModule);

                };
                VRCommon_DynamicAPIModuleService.editVRDynamicAPIModule(vrDynamicAPIModule.VRDynamicAPIModuleId, onVRDynamicAPIModuleUpdated);
            }

            function hasEditVRDynamicAPIModulePermission() {
                return VRCommon_VRDynamicAPIModuleAPIService.HasEditVRDynamicAPIModulePermission();
            }

            function compileDevProject(vrDynamicAPIModule) {
                if (vrDynamicAPIModule.DevProjectId == undefined)
                    return;
                VR_GenericData_GenericBusinessEntityService.CompileDevProject(vrDynamicAPIModule.DevProjectId);
            }

        }
        return directiveDefinitionObject;
    }]);
