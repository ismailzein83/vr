(function (appControllers) {

    'use stict';

    OperatorDeclaredInformationService.$inject = ['VRModalService'];

    function OperatorDeclaredInformationService(VRModalService) {
        return ({
            addOperatorDeclaredInformation: addOperatorDeclaredInformation,
            editOperatorDeclaredInformation: editOperatorDeclaredInformation
        });

        function addOperatorDeclaredInformation(onOperatorDeclaredInformationAdded) {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onOperatorDeclaredInformationAdded = onOperatorDeclaredInformationAdded;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Views/OperatorDeclaredInformation/OperatorDeclaredInformationEditor.html', null, settings);
        }

        function editOperatorDeclaredInformation(operatorDeclaredInformationObj, onOperatorDeclaredInformationUpdated) {
            var modalSettings = {
            };

            var parameters = {
                OperatorDeclaredInformationId: operatorDeclaredInformationObj.Entity.OperatorDeclaredInformationId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onOperatorDeclaredInformationUpdated = onOperatorDeclaredInformationUpdated;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Views/OperatorDeclaredInformation/OperatorDeclaredInformationEditor.html', parameters, modalSettings);
        }
    }

    appControllers.service('Demo_OperatorDeclaredInformationService', OperatorDeclaredInformationService);

})(appControllers);
