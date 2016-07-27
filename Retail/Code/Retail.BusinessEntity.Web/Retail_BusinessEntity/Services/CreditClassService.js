
(function (appControllers) {

    'use stict';

    CreditClassService.$inject = ['VRModalService'];

    function CreditClassService(VRModalService) {

        function addCreditClass(onCreditClassAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onCreditClassAdded = onCreditClassAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/CreditClass/CreditClassEditor.html', null, settings);
        };

        function editCreditClass(creditClassId, onCreditClassUpdated) {
            var settings = {};

            var parameters = {
                creditClassId: creditClassId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCreditClassUpdated = onCreditClassUpdated;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/CreditClass/CreditClassEditor.html', parameters, settings);
        }


        return {
            addCreditClass: addCreditClass,
            editCreditClass: editCreditClass
        };
    }

    appControllers.service('Retail_BE_CreditClassService', CreditClassService);

})(appControllers);