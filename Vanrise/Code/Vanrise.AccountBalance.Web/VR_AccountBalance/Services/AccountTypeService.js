"use strict";
app.service('VR_AccountBalance_AccountTypeService', ['VRModalService',
    function (VRModalService) {

        function addSource(onSourceAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSourceAdded = onSourceAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_AccountBalance/Directives/MainExtensions/Account/Templates/VRAccountTypeSourceEditor.html', parameters, settings);
        }
        function editSource(sourceEntity, onSourceUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSourceUpdated = onSourceUpdated;
            };
            var parameters = {
                sourceEntity: sourceEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_AccountBalance/Directives/MainExtensions/Account/Templates/VRAccountTypeSourceEditor.html', parameters, settings);
        }
        function addGridColumn(onGridColumnAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onGridColumnAdded = onGridColumnAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_AccountBalance/Directives/MainExtensions/Account/Templates/VRAccountTypeGridColumnEditor.html', parameters, settings);
        }
        function editGridColumn(gridColumnEntity, onGridColumnUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onGridColumnUpdated = onGridColumnUpdated;
            };
            var parameters = {
                gridColumnEntity: gridColumnEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_AccountBalance/Directives/MainExtensions/Account/Templates/VRAccountTypeGridColumnEditor.html', parameters, settings);
        }
        function addBillingTransactionField(onBillingTransactionFieldAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onBillingTransactionFieldAdded = onBillingTransactionFieldAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_AccountBalance/Directives/MainExtensions/Account/MainExtensions/Templates/BillingTransactionFieldEditor.html', parameters, settings);
        }
        function editBillingTransactionField(billingTransactionFieldEntity, onBillingTransactionFieldUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onBillingTransactionFieldUpdated = onBillingTransactionFieldUpdated;
            };
            var parameters = {
                billingTransactionFieldEntity: billingTransactionFieldEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_AccountBalance/Directives/MainExtensions/Account/MainExtensions/Templates/BillingTransactionFieldEditor.html', parameters, settings);
        }
        return ({
            addSource: addSource,
            editSource: editSource,
            addGridColumn: addGridColumn,
            editGridColumn: editGridColumn,
            addBillingTransactionField:addBillingTransactionField,
            editBillingTransactionField: editBillingTransactionField
        });
    }]);
