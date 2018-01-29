(function (app) {

    'use strict';

    GenricdataBeSerialnumberEditor.$inject = ['VR_GenericData_GenericBESerialNumberService', 'VR_GenericData_GenericBESerialNumberAPIService', 'UtilsService', 'VRUIUtilsService'];

    function GenricdataBeSerialnumberEditor(VR_GenericData_GenericBESerialNumberService, VR_GenericData_GenericBESerialNumberAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope:{
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SerialNumberCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Runtime/MainExtensions/OnBeforeInsertHandler/GenericBESerialNumber/Templates/SNEditorTemplate.html"
        };

        function SerialNumberCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var serialNumberParts = [];

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.openSerialNumberPatternHelper = function () {
                    var onSetSerialNumberPattern = function (serialNumberPatternValue) {
                        if ($scope.scopeModel.serialNumberPattern == undefined)
                            $scope.scopeModel.serialNumberPattern = "";
                        $scope.scopeModel.serialNumberPattern += serialNumberPatternValue;
                    };
                    var context = getContext();
                    VR_GenericData_GenericBESerialNumberService.openSerialNumberPatternHelper(onSetSerialNumberPattern, context)
                };
                getDirectiveAPI();
            }

            function getDirectiveAPI() {
                var api = {};
                var businessEntityDefinitionId;
                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.serialNumberPattern = payload.data != undefined && payload.data.SerialNumberPattern || undefined;
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                    }

                    return VR_GenericData_GenericBESerialNumberAPIService.GetSerialNumberPartDefinitionsInfo(businessEntityDefinitionId).then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                serialNumberParts.push(response[i]);
                            }
                        }
                    });
                };

                api.getData = function () {
                    return {
                        serialNumberPattern: $scope.scopeModel.serialNumberPattern
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                return {
                    getSerialNumberParts: function () {
                        var serialNumberPartsInfo = new Array();
                        if (serialNumberParts != undefined)
                        {
                            for (var i = 0; i < serialNumberParts.length;i++)
                            {
                                serialNumberPartsInfo.push(serialNumberParts[i]);
                            }
                        }
                        return serialNumberPartsInfo;
                    }
                };
            }
        }

    }

    app.directive('vrGenricdataBeSnEditor', GenricdataBeSerialnumberEditor);

})(app);