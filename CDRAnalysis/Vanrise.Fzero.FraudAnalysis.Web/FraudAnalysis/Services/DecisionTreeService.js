(function (appControllers) {

    'use strict';

    DecisionTreeService.$inject = ['VRModalService'];

    function DecisionTreeService(VRModalService) {
        return ({
            addCondition: addCondition,
            editCondition: editCondition,
            addLabel: addLabel,
            editLabel: editLabel
        });

        function addCondition(onConditionAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onConditionAdded = onConditionAdded;
            };
            var parameters = {
            };

            VRModalService.showModal('/Client/Modules/FraudAnalysis/Views/DecisionTree/DecisionTreeConditionEditor.html', parameters, modalSettings);
        }

        function editCondition(conditionObj, onConditionUpdated) {
            var modalParameters = {
                conditionObj: conditionObj
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onConditionUpdated = onConditionUpdated;
            };

            VRModalService.showModal('/Client/Modules/FraudAnalysis/Views/DecisionTree/DecisionTreeConditionEditor.html', modalParameters, modalSettings);
        }

        function addLabel(onLabelAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onLabelAdded = onLabelAdded;
            };
            var parameters = {
            };

            VRModalService.showModal('/Client/Modules/FraudAnalysis/Views/DecisionTree/DecisionTreeLabelEditor.html', parameters, modalSettings);
        }

        function editLabel(labelObj, onLabelUpdated) {
            var modalParameters = {
                labelObj: labelObj
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onLabelUpdated = onLabelUpdated;
            };

            VRModalService.showModal('/Client/Modules/FraudAnalysis/Views/DecisionTree/DecisionTreeLabelEditor.html', modalParameters, modalSettings);
        }
    };

    appControllers.service('Fzero_FraudAnalysis_DecisionTreeService', DecisionTreeService);

})(appControllers);
