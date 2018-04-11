(function (app) {
    'use strict';

    ExecuteTemplateBinaryRecordParserDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_DataParser_ParserTypeAPIService"];

    function ExecuteTemplateBinaryRecordParserDirective(UtilsService, VRNotificationService, VRUIUtilsService, VR_DataParser_ParserTypeAPIService) {
        return {
            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ExecuteTemplateBinaryRecordParser($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/ExecuteTemplateBinaryRecordParser.html"
        };

        function ExecuteTemplateBinaryRecordParser($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            
            var recordParserIdDirectiveAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onRecordParserIdDirectiveReady = function (api) {
                    recordParserIdDirectiveAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    promises.push(getParserTypes());

                    function getParserTypes() {
                        return VR_DataParser_ParserTypeAPIService.GetParserTypes().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) 
                                    $scope.scopeModel.templateConfigs.push(response[i]);                                   
                                if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.RecordParserTemplateId != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig = 
                                         UtilsService.getItemByVal($scope.scopeModel.templateConfigs, payload.extendedSettings.RecordParserTemplateId, 'ParserTypeId');
                                }
                            }
                        });
                    }
                   return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers.ExecuteTemplateRecordParser, Vanrise.DataParser.MainExtensions",
                        RecordParserTemplateId: $scope.scopeModel.selectedTemplateConfig.ParserTypeId
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }

    app.directive('vrDataparserExecuteTemplateBinaryRecordParserSettings', ExecuteTemplateBinaryRecordParserDirective);

})(app);