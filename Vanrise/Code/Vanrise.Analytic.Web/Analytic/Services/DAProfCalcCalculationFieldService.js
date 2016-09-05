
(function (appControllers) {

    "use strict";

    DAProfCalcCalculationFieldService.$inject = ['VRModalService'];

    function DAProfCalcCalculationFieldService(VRModalService) {

        function addDAProfCalcCalculationField(context, onDAProfCalcCalculationFieldAdded) {
            var settings = {};

            var parameters = {
                context: context
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDAProfCalcCalculationFieldAdded = onDAProfCalcCalculationFieldAdded
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/DataAnalysis/ProfilingAndCalculation/DAProfCalcCalculationFieldEditor.html', parameters, settings);
        };

        function editDAProfCalcCalculationField(daProfCalcCalculationFieldObj, context, onDAProfCalcCalculationFieldUpdated) {
            var settings = {};

            var parameters = {
                daProfCalcCalculationField: daProfCalcCalculationFieldObj,
                context: context
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDAProfCalcCalculationFieldUpdated = onDAProfCalcCalculationFieldUpdated;
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/DataAnalysis/ProfilingAndCalculation/DAProfCalcCalculationFieldEditor.html', parameters, settings);
        }


        return {
            addDAProfCalcCalculationField: addDAProfCalcCalculationField,
            editDAProfCalcCalculationField: editDAProfCalcCalculationField
        };
    }

    appControllers.service('VR_Analytic_DAProfCalcCalculationFieldService', DAProfCalcCalculationFieldService);

})(appControllers);