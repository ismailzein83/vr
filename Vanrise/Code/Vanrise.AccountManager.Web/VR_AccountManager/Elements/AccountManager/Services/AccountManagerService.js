﻿(function (appControllers) {

    'use strict';

    AccountManagerService.$inject = ['VRModalService'];

    function AccountManagerService(VRModalService)
    {

        return ({
            addAssignmentDefinition: addAssignmentDefinition,
            editAssignmentDefinition: editAssignmentDefinition,
            addAccountManager: addAccountManager,
            editAccountmanager: editAccountmanager,
            addSubView: addSubView,
            editSubView: editSubView
        });
        function addAssignmentDefinition(onAssignmentDefinitionAdded)
       {
            var settings = {
            };

            var parameters = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onAssignmentDefinitionAdded = onAssignmentDefinitionAdded;
            };
            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AssignmentDefinitionEditor.html', parameters, settings);
        }
        function addSubView(onSubViewAdded,context) {
            var settings = {
            };

            var parameters = {
                context : context
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSubViewAdded = onSubViewAdded;
            };
            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/SubViewEditor.html', parameters, settings);
        }
        function editSubView(subViewEntity, onSubViewUpdated,subViews, context) {
            var parameters = {
                subViewEntity: subViewEntity,
                context:context
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSubViewUpdated = onSubViewUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/SubViewEditor.html', parameters, settings);
        }
        function addAccountManager(onAccountManagerAdded, accountDefinitionId) {
            var settings = {
            };

            var parameters = {
                accountManagerDefinitionId: accountDefinitionId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountManagerAdded = onAccountManagerAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AccountManagerEditor.html', parameters, settings);
        }
        function editAccountmanager(accountManagerObject, onAccountManagerUpdated) {
            var settings = {
            };
            var parameters = {
                accountManagerDefinitionId: accountManagerObject.AccountManagerDefinitionId,
                accountManagerId:accountManagerObject.AccountManagerId

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountManagerUpdated = onAccountManagerUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AccountManagerEditor.html', parameters, settings);
        }
        function editAssignmentDefinition(assignmentDefinitionEntity, onAssignmentDefinitionUpdated) {
            var parameters = {
                assignmentDefinitionEntity: assignmentDefinitionEntity
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAssignmentDefinitionUpdated = onAssignmentDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AssignmentDefinitionEditor.html', parameters, settings);
        }

    }

    appControllers.service('VR_AccountManager_AccountManagerService', AccountManagerService);

})(appControllers);
