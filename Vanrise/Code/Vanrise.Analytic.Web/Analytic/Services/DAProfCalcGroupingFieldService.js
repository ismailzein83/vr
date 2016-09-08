
(function (appControllers) {

    "use strict";

    DAProfCalcGroupingFieldService.$inject = ['VRModalService'];

    function DAProfCalcGroupingFieldService(VRModalService) {

        function addDAProfCalcGroupingField(context, onDAProfCalcGroupingFieldAdded) {
            var settings = {};

            var parameters = {
                context: context
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDAProfCalcGroupingFieldAdded = onDAProfCalcGroupingFieldAdded
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/DataAnalysis/ProfilingAndCalculation/DAProfCalcGroupingFieldEditor.html', parameters, settings);
        };

        function editDAProfCalcGroupingField(daProfCalcGroupingFieldObj, context, onDAProfCalcGroupingFieldUpdated) {
            var settings = {};

            var parameters = {
                daProfCalcGroupingField: daProfCalcGroupingFieldObj,
                context: context
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDAProfCalcGroupingFieldUpdated = onDAProfCalcGroupingFieldUpdated;
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/DataAnalysis/ProfilingAndCalculation/DAProfCalcGroupingFieldEditor.html', parameters, settings);
        }


        return {
            addDAProfCalcGroupingField: addDAProfCalcGroupingField,
            editDAProfCalcGroupingField: editDAProfCalcGroupingField
        };
    }

    appControllers.service('VR_Analytic_DAProfCalcGroupingFieldService', DAProfCalcGroupingFieldService);

})(appControllers);