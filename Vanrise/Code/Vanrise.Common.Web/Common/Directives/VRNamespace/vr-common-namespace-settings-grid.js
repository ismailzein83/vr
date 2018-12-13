'use strict';

app.directive('vrCommonNamespaceSettingsGrid', ['VRUIUtilsService', 'VRNotificationService',  'VRCommon_VRNamespaceService',
    function (VRUIUtilsService, VRNotificationService,  VRCommon_VRNamespaceService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRNamespaceSettingsGridDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRNamespace/Templates/VRNamespaceSettingsGridTemplate.html'
        };

        function VRNamespaceSettingsGridDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.vrNamespaceSettings = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.deleteDynamicCode = function (code) {
                    var index = $scope.scopeModel.vrNamespaceSettings.indexOf(code);
                    if (index > -1) {
                        $scope.scopeModel.vrNamespaceSettings.splice(index, 1);
                    }
                };
                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined )
                        for (var i = 0; i < payload.data.Codes.length; i++) {
                            $scope.scopeModel.vrNamespaceSettings.push({ Entity: payload.data.Codes[i] });
                        }
                };

                api.getData = function () {
                    var settings = [];
                    for (var j = 0; j < $scope.scopeModel.vrNamespaceSettings.length; j++) {
                        settings.push($scope.scopeModel.vrNamespaceSettings[j].Entity);
                    }
                    return settings;
                };

                $scope.scopeModel.addVRDynamicCode = function () {

                    var onVRDynamicCodeAdded = function (addedVRDynamicCode) {
                        $scope.scopeModel.vrNamespaceSettings.push({ Entity: addedVRDynamicCode });
                    };
                    VRCommon_VRNamespaceService.addVRDynamicCode(onVRDynamicCodeAdded);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editVRDynamicCode
                });
            }

            function editVRDynamicCode(vrDynamicCodeObj) {
                var onVRDynamicCodeUpdated = function (updatedVRDynamicCode) {
                    var index = $scope.scopeModel.vrNamespaceSettings.indexOf(vrDynamicCodeObj);
                    $scope.scopeModel.vrNamespaceSettings[index] = { Entity: updatedVRDynamicCode };
                };

                VRCommon_VRNamespaceService.editVRDynamicCode(onVRDynamicCodeUpdated, vrDynamicCodeObj.Entity);
            }

          
         
        }
    }]);
