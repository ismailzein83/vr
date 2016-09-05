
(function (appControllers) {

    "use strict";

    DAProfCalcAggregationFieldService.$inject = ['VRModalService'];

    function DAProfCalcAggregationFieldService(VRModalService) {

        function addDAProfCalcAggregationField(context, onDAProfCalcAggregationFieldAdded) {
            var settings = {};

            var parameters = {
                context: context
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDAProfCalcAggregationFieldAdded = onDAProfCalcAggregationFieldAdded
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/DataAnalysis/ProfilingAndCalculation/DAProfCalcAggregationFieldEditor.html', parameters, settings);
        };

        function editDAProfCalcAggregationField(daProfCalcAggregationFieldObj, context, onDAProfCalcAggregationFieldUpdated) {
            var settings = {};

            var parameters = {
                daProfCalcAggregationField: daProfCalcAggregationFieldObj,
                context: context
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDAProfCalcAggregationFieldUpdated = onDAProfCalcAggregationFieldUpdated;
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/DataAnalysis/ProfilingAndCalculation/DAProfCalcAggregationFieldEditor.html', parameters, settings);
        }


        return {
            addDAProfCalcAggregationField: addDAProfCalcAggregationField,
            editDAProfCalcAggregationField: editDAProfCalcAggregationField
        };
    }

    appControllers.service('VR_Analytic_DAProfCalcAggregationFieldService', DAProfCalcAggregationFieldService);

})(appControllers);