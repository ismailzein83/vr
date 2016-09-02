(function (appControllers) {

    'use stict';

    OperatorDeclaredInfoService.$inject = ['VRModalService', 'VRNotificationService'];

    function OperatorDeclaredInfoService(VRModalService, VRNotificationService)
    {
        function addOperatorDeclaredInfo(onOperatorDeclaredInfoAdded)
        {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onOperatorDeclaredInfoAdded = onOperatorDeclaredInfoAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/OperatorDeclaredInfo/OperatorDeclaredInfoEditor.html', null, settings);
        };

        function editOperatorDeclaredInfo(operatorDeclaredInfoId, onOperatorDeclaredInfoUpdated)
        {
            var parameters = {
                operatorDeclaredInfoId: operatorDeclaredInfoId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onOperatorDeclaredInfoUpdated = onOperatorDeclaredInfoUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/OperatorDeclaredInfo/OperatorDeclaredInfoEditor.html', parameters, settings);
        };

        function addOperatorDeclaredInfoItem(onOperatorDeclaredInfoItemAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onOperatorDeclaredInfoItemAdded = onOperatorDeclaredInfoItemAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/OperatorDeclaredInfo/OperatorDeclaredInfoItemEditor.html', null, settings);
        };

        function editOperatorDeclaredInfoItem(operatorDeclaredInfoItem, onOperatorDeclaredInfoItemUpdated) {
            var parameters = {
                operatorDeclaredInfoItem: operatorDeclaredInfoItem
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onOperatorDeclaredInfoItemUpdated = onOperatorDeclaredInfoItemUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/OperatorDeclaredInfo/OperatorDeclaredInfoItemEditor.html', null, settings);
        };
        return {
            addOperatorDeclaredInfo: addOperatorDeclaredInfo,
            editOperatorDeclaredInfo: editOperatorDeclaredInfo,
            addOperatorDeclaredInfoItem: addOperatorDeclaredInfoItem,
            editOperatorDeclaredInfoItem: editOperatorDeclaredInfoItem

        };
    }

    appControllers.service('Retail_BE_OperatorDeclaredInfoService', OperatorDeclaredInfoService);

})(appControllers);