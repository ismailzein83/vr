(function (appControllers) {

    "use strict";
    BusinessProcess_BPBusinessRuleSetService.$inject = ['VRModalService'];
    function BusinessProcess_BPBusinessRuleSetService(VRModalService) {

        function addBusinessRuleSet(onBusinessRuleSetAdded) {
            var settings = {};
            var parameters = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBusinessRuleSetAdded = onBusinessRuleSetAdded;
            };
            var editor = '/Client/Modules/BusinessProcess/Views/BPBusinessRule/BPBusinessRuleSetEditor.html';
            VRModalService.showModal(editor, parameters, settings);
        }
        function editBPBusinessRuleSet(bpBusinessRuleSetId, onBusinessRuleSetUpdated) {
            var modalSettings = {};
            var parameters = {
                bpBusinessRuleSetId: bpBusinessRuleSetId
            };
            var editor = '/Client/Modules/BusinessProcess/Views/BPBusinessRule/BPBusinessRuleSetEditor.html';

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBusinessRuleSetUpdated = onBusinessRuleSetUpdated;
            };
            VRModalService.showModal(editor, parameters, modalSettings);

        }

        return ({
            addBusinessRuleSet: addBusinessRuleSet,
            editBPBusinessRuleSet: editBPBusinessRuleSet
        });


    }
    appControllers.service('BusinessProcess_BPBusinessRuleSetService', BusinessProcess_BPBusinessRuleSetService);

})(appControllers);