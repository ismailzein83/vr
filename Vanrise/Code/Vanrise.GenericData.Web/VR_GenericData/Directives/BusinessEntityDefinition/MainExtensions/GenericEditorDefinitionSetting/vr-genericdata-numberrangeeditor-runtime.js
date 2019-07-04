"use strict";

app.directive("vrGenericdataNumberrangeeditorRuntime", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new NumberRangeEditorRuntimeSetting($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/NumberRangeEditorRuntimeSettingTemplate.html"
        };

        function NumberRangeEditorRuntimeSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var definitionSettings;

         

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.numbersValidation = function () {
                    if (Number($scope.scopeModel.fromNumber) > Number($scope.scopeModel.toNumber))
                        return "From number must be less than to number";
                    return null;
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        definitionSettings = payload.definitionSettings;
                        if (payload.settings != undefined) {
                            $scope.scopeModel.fromNumber = payload.settings.FromNumber;
                            $scope.scopeModel.toNumber = payload.settings.ToNumber;
                            $scope.scopeModel.prefix = payload.settings.Prefix;
                            $scope.scopeModel.suffix = payload.settings.Suffix;
                        }
                        
                    }

                    return UtilsService.waitPromiseNode({ promises: promises });
                };


                api.setData = function (dicData) {
                    var rangeVariableName = definitionSettings.RangeVariableName;
                    if (rangeVariableName == undefined || rangeVariableName == '')
                        rangeVariableName = "GeneratedRangeInfo";
                    dicData[rangeVariableName] = {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericEditorDefinitionSetting.NumberRangeGenericEditorDefinitionSettings,Vanrise.GenericData.MainExtensions",
                        FromNumber: $scope.scopeModel.fromNumber,
                        ToNumber: $scope.scopeModel.toNumber,
                        Prefix: $scope.scopeModel.prefix,
                        Suffix: $scope.scopeModel.suffix
                    }
                };

                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);