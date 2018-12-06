'use strict';

app.directive('vrCommonCustomCodeDynamicCode', ['VRUIUtilsService', 'VRNotificationService',  'VRCommon_VRNamespaceService',
    function (VRUIUtilsService, VRNotificationService,  VRCommon_VRNamespaceService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRCommonCustomCodeDynamicCodeDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRNamespace/MainExtensions/Templates/VRCustomCodeDynamicCode.html'
        };

        function VRCommonCustomCodeDynamicCodeDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.menuActions = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload!=undefined)
                        $scope.scopeModel.customCode = payload.vrCustomCodeSettingsEntity.CustomCode;
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Common.MainExtensions.VRDynamicCode.CustomCodeDynamicCodeSettings,Vanrise.Common.MainExtensions",
                        CustomCode: $scope.scopeModel.customCode
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

         
        }
    }]);
