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
        var globalPersonilizationItems;
        ctrl.personalizationMenu = [
            {
                name: "Save Settings",
                clicked: function () {
                    savePersonalizationSettings(true);
                }
            },
             {
                 name: "Reset to Default",
                 clicked: function () {
                     resetPersonalizationSettings(true);
                 }
             },
            {
                name: "Save as Global",
                clicked: function () {
                    savePersonalizationSettings(false);
                }
            },
            {
                name: "Reset Global",
                clicked: function () {
                    resetPersonalizationSettings(false);
                },
                disable: globalPersonilizationItems != undefined && globalPersonilizationItems.length > 0

            }
        ];

        function savePersonalizationSettings(isUser) {
            var title = isUser ? "Save User Settings" : "Save Global Settings";
            var changedItems = getEntityPersonalizationChangedItems();
            if (changedItems.length == 0)
                VRNotificationService.showInformation("No settings has been changed.");
            else {
                var onSavePesonilization = function (checkedEntityUniqueNames) {
                    var checkedItemsSettingsInputs = buildEntityUniqueInputs(checkedEntityUniqueNames);
                    return UpdateCurrentEntityPersonalization(isUser, checkedItemsSettingsInputs);
                };
                openPersonilizationEditor(title, changedItems, onSavePesonilization);
            }
        }

        function resetPersonalizationSettings(isUser) {
            var title = isUser ? "Reset User Settings" : "Reset Global Settings";
            var items = getLoadedEntityPersonalizationItems(isUser);
            if (items.length == 0)
                VRNotificationService.showInformation("No settings has been loaded.");
            else {
                var onSavePesonilization = function (checkedEntityUniqueNames) {
                    return ResetCurrentEntityPersonalization(isUser, checkedEntityUniqueNames);
                };
                openPersonilizationEditor(title, items, onSavePesonilization);
            }
        }

        function openPersonilizationEditor(title, changedItems, onSavePesonilization) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.title = title;
                modalScope.onSavePesonilization = function (checkedEntityUniqueNames) {
                    if (checkedEntityUniqueNames.length == 0)
                        return;
                    else if (onSavePesonilization != undefined && typeof (onSavePesonilization) == 'function') {
                        onSavePesonilization(checkedEntityUniqueNames);
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
                return GetCurrentUserEntityPersonalization(entityUniqueKeys);
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

        function UpdateCurrentEntityPersonalization(isUser, checkedItemsSettingsInputs) {
            var method = isUser ? "UpdateCurrentUserEntityPersonalization" : "UpdateGlobalEntityPersonalization";
            return VR_Common_EntityPersonalizationAPIService[method](checkedItemsSettingsInputs).then(function (response) {
                GetCurrentUserEntityPersonalization(buildEntityUniqueKeys(uniqueKeys));
            });
        }

        function ResetCurrentEntityPersonalization(isUser, resetEntityUniqueNames) {
            var method = isUser ? "DeleteCurrentUserEntityPersonalization" : "DeleteGlobalEntityPersonalization";
            return VR_Common_EntityPersonalizationAPIService[method](resetEntityUniqueNames).then(function (response) {
                GetCurrentUserEntityPersonalization(buildEntityUniqueKeys(uniqueKeys));
            });
        }

        function GetCurrentUserEntityPersonalization(entityUniqueKeys) {
            return VR_Common_EntityPersonalizationAPIService.GetCurrentUserEntityPersonalization(entityUniqueKeys).then(function (response) {
                var userEntityPersonalizations = [];
                currentPersonilizationItems = response.UserEntityPersonalizations;
                globalPersonilizationItems = response.GlobalEntityPersonalizations;
                if (response != null) {
                    userEntityPersonalizations = response.UserEntityPersonalizations;
                }
                return userEntityPersonalizations;
            });
        }

        function buildEntityUniqueInputs(checkedEntityUniqueNames) {
            var changedItems = getContext().getPersonalizationItems()
            var inputs = [];
            for (var i = 0 ; i < changedItems.length ; i++) {
                if (checkedEntityUniqueNames.indexOf(changedItems[i].EntityUniqueName) > -1)
                    inputs.push(changedItems[i]);
            }
            return inputs;
        }

        function getEntityPersonalizationChangedItems() {
            var changedItems = [];
            var newPersonilizationItems = getContext().getPersonalizationItems();
            for (var i = 0 ; i < uniqueKeys.length ; i++) {
                var entityUniqueName = uniqueKeys[i].EntityUniqueName;
                var currentItem = UtilsService.getItemByVal(currentPersonilizationItems, entityUniqueName, "EntityUniqueName");
                var newItem = UtilsService.getItemByVal(newPersonilizationItems, entityUniqueName, "EntityUniqueName");
                if (currentItem != null && newItem != null)
                    newItem.IsGlobal = currentItem.IsGlobal;


                if (currentItem == null && newItem == null) {
                    continue;
                }
                if (currentItem == null && newItem != null) {
                    changedItems.push(uniqueKeys[i]);
                    continue;
                }

                if (currentItem != null && currentItem.ExtendedSetting == null && newItem != null && newItem.ExtendedSetting != null) {
                    changedItems.push(uniqueKeys[i]);
                    continue;
                }

                if (currentItem != null && currentItem.ExtendedSetting != null && newItem != null && newItem.ExtendedSetting != null) {
                    var currentItemSettings = UtilsService.serializetoJson(currentItem.ExtendedSetting);
                    var newItemSettings = UtilsService.serializetoJson(newItem.ExtendedSetting);
                    if (newItemSettings != currentItemSettings)
                        changedItems.push(uniqueKeys[i]);
                }
            }
            return changedItems;
        }

        function getLoadedEntityPersonalizationItems(isUser) {
            var items = [];
            var filterItems = isUser == true ? currentPersonilizationItems : globalPersonilizationItems;
            if (filterItems != undefined && filterItems.length > 0) {
                for (var i = 0; i < filterItems.length; i++) {
                    var item = UtilsService.getItemByVal(uniqueKeys, filterItems[i].EntityUniqueName, "EntityUniqueName");
                    if (item != null) {
                        if (isUser == true && filterItems[i].IsGlobal == false)
                            items.push(item);
                        else if (isUser == false)
                            items.push(item);
                    }
                }
            }
            return items;
        }
    }

    return directiveDefinitionObject;

}]);


