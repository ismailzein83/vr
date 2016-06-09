(function (appControllers) {

    'use strict';

    AnalyticItemActionService.$inject = ['VRModalService'];

    function AnalyticItemActionService(VRModalService) {
        return ({
            addItemAction: addItemAction,
            editItemAction: editItemAction
        });

        function addItemAction(onItemActionAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onItemActionAdded = onItemActionAdded;
            };
            var modalParameters = {
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticItemActionEditor.html', modalParameters, modalSettings);
        }

        function editItemAction(itemAction, onItemActionUpdated) {
            var modalParameters = {
                itemAction: itemAction
            };
            var modalSettings = {};


            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onItemActionUpdated = onItemActionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticItemActionEditor.html', modalParameters, modalSettings);
        }

    };

    appControllers.service('VR_Analytic_AnalyticItemActionService', AnalyticItemActionService);

})(appControllers);
