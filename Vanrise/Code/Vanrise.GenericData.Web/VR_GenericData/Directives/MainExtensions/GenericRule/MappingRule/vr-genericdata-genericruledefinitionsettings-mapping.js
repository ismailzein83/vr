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
                    var fieldTypeSelectivePayload = {};

                    if (payload != undefined) {
                        ctrl.fieldTitle = payload.FieldTitle;
                        fieldTypeSelectivePayload = payload.FieldType;
                    }

                    var fieldTypeSelectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    VRUIUtilsService.callDirectiveLoad(fieldTypeSelectiveAPI, fieldTypeSelectivePayload, fieldTypeSelectiveLoadDeferred);
                    return fieldTypeSelectiveLoadDeferred.promise;
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.Transformation.Entities.MappingRuleDefinitionSettings, Vanrise.GenericData.Transformation.Entities',
                        FieldTitle: ctrl.fieldTitle,
                        FieldType: fieldTypeSelectiveAPI.getData()
                    };
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataGenericruledefinitionsettingsMapping', MappingRuleDefinitionSettingsDirective);

})(app);