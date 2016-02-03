(function (app) {

    'use strict';

    MappingRuleDefinitionSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function MappingRuleDefinitionSettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var mappingRuleDefinitionSettingsDirective = new MappingRuleDefinitionSettingsDirective($scope, ctrl, $attrs);
                mappingRuleDefinitionSettingsDirective.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/GenericRule/MappingRule/Templates/MappingRuleDefinitionSettingsTemplate.html"
        };

        function MappingRuleDefinitionSettingsDirective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var fieldTypeSelectiveAPI;

            function initializeController() {
                ctrl.onFieldTypeSelectiveReady = function (api) {
                    fieldTypeSelectiveAPI = api;
                    
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        ctrl.fieldName = payload.FieldName;
                    }

                    var fieldTypeSelectiveLoadDeferred = UtilsService.createPomiseDeferred();
                    VRUIUtilsService.callDirectiveLoad(fieldTypeSelectiveAPI, payload, fieldTypeSelectiveLoadDeferred);
                    return fieldTypeSelectiveLoadDeferred.promise;
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.Transformation.Entities.MappingRuleDefinitionSettings, Vanrise.GenericData.Transformation.Entities',
                        FieldName: ctrl.fieldName,
                        FieldType: fieldTypeSelectiveAPI.getData()
                    };
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataGenericruledefinitionsettingsMapping', MappingRuleDefinitionSettingsDirective);

})(app);