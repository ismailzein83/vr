"use strict";
app.service('Demo_Module_DescriptionService', ['VRModalService',
    function (VRModalService) {

        function addDescription(onDescriptionAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onDescriptionAdded = onDescriptionAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Directives/Description/Templates/DescriptionEditor.html', parameters, settings);
        }
        function editDescription(descriptionEntity, onDescriptionUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onDescriptionUpdated = onDescriptionUpdated;
            };
            var parameters = {
                descriptionEntity: descriptionEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/Demo_Module/Directives/Description/Templates/DescriptionEditor.html', parameters, settings);
        }

        return ({
            addDescription: addDescription,
            editDescription: editDescription

        });
    }]);
