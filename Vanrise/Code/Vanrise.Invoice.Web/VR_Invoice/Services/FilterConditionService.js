
app.service('VR_Invoice_FilterConditionService', ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService) {

        function addFilterCondition(onFilterConditionItemAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onFilterConditionItemAdded = onFilterConditionItemAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceGridSettings/MainExtensions/InvoiceFilterCondition/Templates/FilterConditionItemEditor.html', parameters, settings);
        }
        function editFilterCondition(filterConditionItemEntity, onFilterConditionItemUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onFilterConditionItemUpdated = onFilterConditionItemUpdated;
            };
            var parameters = {
                filterConditionItemEntity: filterConditionItemEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceGridSettings/MainExtensions/InvoiceFilterCondition/Templates/FilterConditionItemEditor.html', parameters, settings);
        }

        return ({
            addFilterCondition: addFilterCondition,
            editFilterCondition: editFilterCondition,
        });
    }]);
