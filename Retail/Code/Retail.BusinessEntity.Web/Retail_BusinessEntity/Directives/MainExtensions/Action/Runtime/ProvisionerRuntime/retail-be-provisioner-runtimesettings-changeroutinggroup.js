//(function (app) {

//    'use strict';

//    ProvisionerRuntimesettingsChangeRoutingGroupDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

//    function ProvisionerRuntimesettingsChangeRoutingGroupDirective(UtilsService, VRUIUtilsService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var changeRoutingGroupProvisionerRuntimeSetting = new ChangeRoutingGroupProvisionerRuntimeSetting($scope, ctrl, $attrs);
//                changeRoutingGroupProvisionerRuntimeSetting.initializeController();
//            },
//            controllerAs: "Ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Action/Runtime/ProvisionerRuntime/Templates/ChangeRoutingGroupProvisionerRuntimeSettingsTemplate.html"

//        };
//        function ChangeRoutingGroupProvisionerRuntimeSetting($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;
//            var mainPayload;
//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.destinationTypes = [{
//                    value: 0,
//                    description: "No Destination"
//                }, {
//                    value: 1,
//                    description: "All Destinations"
//                }, {
//                    value: 2,
//                    description: "Europe"
//                }, {
//                    value: 3,
//                    description: "Asia"
//                }];
//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    if (payload != undefined) {
//                        mainPayload = payload;
//                        if (payload.provisionerRuntimeEntity != undefined) {
//                            $scope.scopeModel.selectedDestinationType = UtilsService.getItemByVal($scope.scopeModel.destinationTypes, payload.provisionerRuntimeEntity.DestinationType, "value");
//                        }
//                    }

//                };

//                api.getData = getData;

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }

//                function getData() {
//                    var data = {
//                        $type: "Retail.BusinessEntity.MainExtensions.ChangeRoutingGroupProvisionerRuntimeSetting,Retail.BusinessEntity.MainExtensions",
//                        DestinationType: $scope.scopeModel.selectedDestinationType.value

//                    };
//                    return data;
//                }
//            }
//        }
//    }

//    app.directive('retailBeProvisionerRuntimesettingsChangeroutinggroup', ProvisionerRuntimesettingsChangeRoutingGroupDirective);

//})(app);