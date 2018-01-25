(function (app) {

    'use strict';

    GenricdataBeSerialnumberEditor.$inject = ['VR_GenericData_GenericRuleTypeConfigAPIService', 'UtilsService', 'VRUIUtilsService'];

    function GenricdataBeSerialnumberEditor(VR_GenericData_GenericRuleTypeConfigAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SerialNumberCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/GenericBESerialNumber/Templates/SerialNumberEditorTemplate.html"
        };

        function SerialNumberCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

           
            function initializeController() {
                var serialNumberParts = [];
                $scope.scopeModel = {};
               
                $scope.scopeModel.openSerialNumberPatternHelper = function () {
                   
                };
            }

            function getDirectiveAPI() {
                var api = {};
                var businessEntityDefinitionId;
                api.load = function (payload) {
                    if (payload != undefined) {
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                    }

                    return VR_GenericData_GenericRuleTypeConfigAPIService.GetSerialNumberPartDefinitionsInfo(businessEntityDefinitionId).then(function (response) {
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
                return api;
            }

            function getContext() {

                return {
                    getSerialNumberParts: function () {
                        return serialNumberParts;
                    }
                };
            }
        }

        
    }

    app.directive('vrGenricdataBeSerialnumberEditor', GenricdataBeSerialnumberEditor);

})(app);