"use strict";
app.directive("vrCommonDynamicapiMethodsGrid", ["UtilsService", "VRNotificationService", "VR_Dynamic_APIService", "VRCommon_VRDynamicAPIAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
    function (UtilsService, VRNotificationService, VR_Dynamic_APIService, VRCommon_VRDynamicAPIAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrDynamicAPIMethodsGrid = new VRDynamicAPIMethodsGrid($scope, ctrl, $attrs);
                vrDynamicAPIMethodsGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/VRDynamicAPI/Templates/DynamicAPIMethodsGridTemplate.html"
        };

        function VRDynamicAPIMethodsGrid($scope, ctrl) {

            var gridApi;

            this.initializeController = initializeController;

            function initializeController() {
                

                $scope.scopeModel = {};

                $scope.scopeModel.vrDynamicAPIMethods = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveApi());
                    }

                    function getDirectiveApi() {

                        var directiveApi = {};

                        directiveApi.load = function (payload) {
                            if (payload != undefined && payload.Methods != undefined && payload.Methods.length != 0) {

                                for (var j = 0; j < payload.Methods.length; j++) {

                                    $scope.scopeModel.vrDynamicAPIMethods.push({ Entity: payload.Methods[j] });
                                }
                            }
                        };

                        directiveApi.getData = function () {
                            var methods = [];

                            for (var j = 0; j < $scope.scopeModel.vrDynamicAPIMethods.length; j++) {

                                methods.push($scope.scopeModel.vrDynamicAPIMethods[j].Entity);
                            }

                            return methods;
                        };

                        return directiveApi;
                    }
                };

                $scope.scopeModel.AddVRDynamicAPIMethod = function () {

                    var onVRDynamicAPIMethodAdded = function (method) {
                        $scope.scopeModel.vrDynamicAPIMethods.push({ Entity: method });
                    };
                    VR_Dynamic_APIService.addVRDynamicAPIMethod(onVRDynamicAPIMethodAdded);

                };

                $scope.scopeModel.deleteMethod = function (method) {
                    var index = $scope.scopeModel.vrDynamicAPIMethods.indexOf(method);
                    if (index > -1) {
                        $scope.scopeModel.vrDynamicAPIMethods.splice(index, 1);
                    }
                };
                defineMenuActions();
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editVRDynamicAPIMethod,
                }];
            }

            function editVRDynamicAPIMethod(vrDynamicAPIMethod) {

                var index = $scope.scopeModel.vrDynamicAPIMethods.indexOf(vrDynamicAPIMethod);
                var onVRDynamicAPIMethodUpdated = function (method) {
                    $scope.scopeModel.vrDynamicAPIMethods[index] = { Entity: method };
                };
                VR_Dynamic_APIService.editVRDynamicAPIMethod(vrDynamicAPIMethod.Entity, onVRDynamicAPIMethodUpdated);
            }
        }

        return directiveDefinitionObject;
    }]);

