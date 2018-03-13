"use strict";
app.service("VR_Analytic_DimensionMappingRulesService", ["VRModalService",
    function (VRModalService) {

        function addRule(onRuleAdded, params) {
            
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onRuleAdded = onRuleAdded;
            };
            var parameters = {
                
                context: params.context,
                tableId: params.tableId
            };
            VRModalService.showModal("/Client/Modules/Analytic/Directives/MainExtensions/MeasureExternalSource/AnalyticMeasureExternalSource/Templates/DimensionMappingRulesGridEditor.html", parameters, settings); // fix
        };
        function editRule(ruleEntity, onRuleUpdated, params) {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onRuleUpdated = onRuleUpdated;
            };
            var parameters = {
                
                context: params.context,
                tableId: params.tableId,
                ruleEntity: ruleEntity
            };
            VRModalService.showModal("/Client/Modules/Analytic/Directives/MainExtensions/MeasureExternalSource/AnalyticMeasureExternalSource/Templates/DimensionMappingRulesGridEditor.html", parameters, settings);
        };
        return {
            addRule: addRule,
            editRule: editRule
        }
    }]);