//"use strict";

//app.directive("vrGenericdataNumberrangeeditorRuntime", ["UtilsService", "VRUIUtilsService",
//    function (UtilsService, VRUIUtilsService) {

//        var directiveDefinitionObject = {
//            restrict: "E",
//            scope: {
//                onReady: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new NumberRangeEditorRuntimeSetting($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/NumberRangeEditorRuntimeSettingTemplate.html"
//        };

//        function NumberRangeEditorRuntimeSetting($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;
//            var definitionSettings;

         

//            function initializeController() {
//                $scope.scopeModel = {};

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];
//                    if (payload != undefined) {
//                        definitionSettings = payload.definitionSettings;
//                        if (payload.settings != undefined) {
//                            $scope.scopeModel.fromNumber = payload.settings.FromNumber;
//                            $scope.scopeModel.toNumber = payload.settings.ToNumber;
//                        }
                        
//                    }

//                    return UtilsService.waitPromiseNode({ promises: promises });
//                };


//                api.setData = function (dicData) {
//                    var rangeVariableName = definitionSettings.RangeVariableName;
//                    dicData[rangeVariableName] = {
//                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericEditorDefinitionSetting.NumberRangeGenericEditorDefinitionSettings,Vanrise.GenericData.MainExtensions",
//                        FromNumber: $scope.scopeModel.fromNumber,
//                        ToNumber: $scope.scopeModel.toNumber
//                    }
//                };

                
//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }
//        }

//        return directiveDefinitionObject;
//    }
//]);