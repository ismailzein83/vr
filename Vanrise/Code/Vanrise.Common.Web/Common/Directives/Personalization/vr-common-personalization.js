"use strict";

"use strict";

app.directive("vrCommonPersonalization", ["UtilsService", "VRNotificationService", "VR_Common_EntityPersonalizationAPIService", "VRModalService",
function (UtilsService, VRNotificationService, VR_Common_EntityPersonalizationAPIService, VRModalService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new Personalization($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        template: function (element, attrs) {
            return '<vr-button type="Personalization" menuactions="ctrl.personalizationMenu" ></vr-button>'
        }
    };

    function Personalization($scope, ctrl, $attrs) {

        this.initializeController = initializeController;
        var context;
        var uniqueKeys;
        var currentPersonilizationItems;
        ctrl.personalizationMenu = [
            {
                name: "Save Settings",
                clicked: savePersonalizationSettings
            },
             {
                 name: "Reset to Default",
                 clicked: function () {

                 }
             },
            {
                name: "Save as Global",
                clicked: function () {

                }
            },
            {
                name: "Reset Global",
                clicked: function () {

                }
            }
        ];

        function savePersonalizationSettings() {
            var changedItems = getEntityPersonalizationChangedItems();
            if (changedItems.length == 0)
                VRNotificationService.showInformation("No user settings has been changed.");
            else
                openPersonilizationEditor("Save User Settings", changedItems);
        }

        function openPersonilizationEditor(title, changedItems) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.title = title;
                modalScope.onSavePesonilization = function (checkedEntityUniqueNames) {
                    if (checkedEntityUniqueNames.length == 0)
                        return;
                    else {
                        var checkedItemsSettingsInputs = buildEntityUniqueInputs(changedItems, checkedEntityUniqueNames);
                        VR_Common_EntityPersonalizationAPIService.UpdateCurrentUserEntityPersonalization(checkedItemsSettingsInputs).then(function (response) {
                           // currentPersonilizationItems = response;
                           // return response;
                        });
                    }
                };
            };
            var parameters = {
                changedItems: changedItems
            };
            VRModalService.showModal('/Client/Modules/Common/Directives/Personalization/Templates/PersonilizationEditor.html', parameters, settings);
        };

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                if (payload != undefined) {
                    uniqueKeys = payload.uniqueKeys;
                    var entityUniqueKeys;
                    if (uniqueKeys != undefined) {
                        entityUniqueKeys = buildEntityUniqueKeys(uniqueKeys);
                    }
                    context = payload.context;
                }
                return VR_Common_EntityPersonalizationAPIService.GetCurrentUserEntityPersonalization(entityUniqueKeys).then(function (response) {
                    currentPersonilizationItems = response;
                    return response;
                });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }

        function buildEntityUniqueKeys(uniqueKeys) {
            var entityUniqueKeys = [];
            for (var i = 0 ; i < uniqueKeys.length ; i++)
                entityUniqueKeys.push(uniqueKeys[i].EntityUniqueName);
            return entityUniqueKeys;
        }


        function buildEntityUniqueInputs(changedItems, checkedEntityUniqueNames) {
            var inputs = [];
            for (var i = 0 ; i < changedItems.length ; i++)
                if (checkedEntityUniqueNames.indexOf(changedItems[i].EntityUniqueName) > -1)
                    inputs.push(changedItems[i]);
            return inputs;
        }

        function getEntityPersonalizationChangedItems() {
            var changedItems = [];
            var newPersonilizationItems = getContext().getPersonalizationItems();
            for (var i = 0 ; i < uniqueKeys.length ; i++) {
                var entityUniqueName = uniqueKeys[i].EntityUniqueName;
                var currentItem = UtilsService.getItemByVal(currentPersonilizationItems, entityUniqueName, "EntityUniqueName");
                var newItem = UtilsService.getItemByVal(newPersonilizationItems, entityUniqueName, "EntityUniqueName");

                if (currentItem == null && newItem == null) {
                    continue;
                }

                if (currentItem.ExtendedSetting == null && newItem.ExtendedSetting == null) {
                    continue;
                }

                if (currentItem.ExtendedSetting != null && newItem.ExtendedSetting == null) {
                    changedItems.push(uniqueKeys[i]);
                }

                if (currentItem.ExtendedSetting == null && newItem.ExtendedSetting != null) {
                    changedItems.push(uniqueKeys[i]);
                }

                if (currentItem.ExtendedSetting != null && newItem.ExtendedSetting != null) {
                    var currentItemSettings = UtilsService.serializetoJson(currentItem.ExtendedSetting);
                    var newItemSettings = UtilsService.serializetoJson(newItem.ExtendedSetting);
                    if (newItemSettings != currentItemSettings)
                        changedItems.push(uniqueKeys[i]);
                }
            }
            return changedItems;
        }
    }

    return directiveDefinitionObject;

}]);


