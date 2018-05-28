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
            return '<vr-button type="PersonalizationSave"  data-onclick="ctrl.savePersonalizationSettings" vr-disabled="ctrl.disabelSaveSettings()"></vr-button>' +
                    '<vr-button type="PersonalizationReset"  data-onclick="ctrl.resetPersonalizationSettings" vr-disabled="ctrl.disabelResetSettings()" ></vr-button>';
        }
    };

    function Personalization($scope, ctrl, $attrs) {

        this.initializeController = initializeController;
        var context;
        var uniqueKeys;
        ctrl.currentPersonilizationItems = [];
        var globalPersonilizationItems;

        ctrl.savePersonalizationSettings = function () {
            var changedItems = getEntityPersonalizationChangedItems();
            if (changedItems.length == 0)
                VRNotificationService.showInformation("No settings has been changed.");
            else {
                var onSavePesonilization = function (checkedEntityUniqueNames, allUsers) {
                    var checkedItemsSettingsInputs = buildEntityUniqueInputs(checkedEntityUniqueNames);
                    return UpdateEntityPersonalization(checkedItemsSettingsInputs, allUsers);
                };
                openPersonilizationEditor("Save Settings", changedItems, onSavePesonilization, "Save to All Users");
            }
        };

        ctrl.resetPersonalizationSettings = function () {
            var itemsObj = getLoadedEntityPersonalizationItems();
            if (itemsObj.items.length == 0)
                VRNotificationService.showInformation("No settings has been loaded.");
            else {
                var onSavePesonilization = function (checkedEntityUniqueNames, allUsers) {
                    return ResetEntityPersonalization(checkedEntityUniqueNames, allUsers);
                };
                openPersonilizationEditor("Reset to Default", itemsObj.items, onSavePesonilization, "Reset to All Users", itemsObj.justUser);
            }
        };

        ctrl.disabelSaveSettings = function () {
            var changedItems = getEntityPersonalizationChangedItems();
            return changedItems.length == 0;
        };

        ctrl.disabelResetSettings = function () {
            var itemsObj = getLoadedEntityPersonalizationItems();
            return itemsObj.items.length == 0;
        };
        function openPersonilizationEditor(title, changedItems, onSavePesonilization, userOptionsLabel, justUser) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.title = title;
                modalScope.onSavePesonilization = function (checkedEntityUniqueNames, saveAsGlobal) {
                    if (checkedEntityUniqueNames.length == 0)
                        return;
                    else if (onSavePesonilization != undefined && typeof (onSavePesonilization) == 'function') {
                        onSavePesonilization(checkedEntityUniqueNames, saveAsGlobal);
                    }
                };
            };
            var parameters = {
                changedItems: changedItems,
                userOptionsLabel: userOptionsLabel,
                justUser: justUser
            };
            VRModalService.showModal('/Client/Modules/Common/Directives/Personalization/Templates/PersonilizationEditor.html', parameters, settings);
        }



        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {
            };
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
                currentContext = {
                };
            return currentContext;
        }

        function buildEntityUniqueKeys(uniqueKeys) {
            var entityUniqueKeys = [];
            for (var i = 0; i < uniqueKeys.length; i++)
                entityUniqueKeys.push(uniqueKeys[i].EntityUniqueName);
            return entityUniqueKeys;
        }

        function UpdateEntityPersonalization(checkedItemsSettingsInputs, allUsers) {
            var input = {
                EntityPersonalizationInputs: checkedItemsSettingsInputs,
                AllUsers: allUsers
            };

            return VR_Common_EntityPersonalizationAPIService.UpdateEntityPersonalization(input).then(function (response) {
                GetCurrentUserEntityPersonalization(buildEntityUniqueKeys(uniqueKeys));
            }).catch(function (error) {
                VRNotificationService.notifyException(error, null);
            });
        }

        function ResetEntityPersonalization(resetEntityUniqueNames, allUsers) {
            var input = {
                EntityUniqueNames: resetEntityUniqueNames,
                AllUsers: allUsers
            };
            return VR_Common_EntityPersonalizationAPIService.DeleteEntityPersonalization(input).then(function (response) {
                GetCurrentUserEntityPersonalization(buildEntityUniqueKeys(uniqueKeys));
            }).catch(function (error) {
                VRNotificationService.notifyException(error, null);
            });
        }

        function GetCurrentUserEntityPersonalization(entityUniqueKeys) {
            return VR_Common_EntityPersonalizationAPIService.GetCurrentUserEntityPersonalization(entityUniqueKeys).then(function (response) {
                var userEntityPersonalizations = [];
                ctrl.currentPersonilizationItems = response.UserEntityPersonalizations;
                globalPersonilizationItems = response.GlobalEntityPersonalizations;
                if (response != null) {
                    userEntityPersonalizations = response.UserEntityPersonalizations;
                }
                return userEntityPersonalizations;
            });
        }

        function buildEntityUniqueInputs(checkedEntityUniqueNames) {
            var changedItems = getContext().getPersonalizationItems();
            var inputs = [];
            for (var i = 0; i < changedItems.length; i++) {
                if (checkedEntityUniqueNames.indexOf(changedItems[i].EntityUniqueName) > -1)
                    inputs.push(changedItems[i]);
            }
            return inputs;
        }


        function getEntityPersonalizationChangedItems() {
            var changedItems = [];
            var newPersonilizationItems = getContext().getPersonalizationItems();
            for (var i = 0; i < uniqueKeys.length; i++) {
                var entityUniqueName = uniqueKeys[i].EntityUniqueName;
                var currentItem = UtilsService.getItemByVal(ctrl.currentPersonilizationItems, entityUniqueName, "EntityUniqueName");
                var newItem = UtilsService.getItemByVal(newPersonilizationItems, entityUniqueName, "EntityUniqueName");
                if (currentItem != null && newItem != null)
                    newItem.IsGlobal = currentItem.IsGlobal;


                if (currentItem == null && newItem == null) {
                    continue;
                }
                if (currentItem == null && newItem != null && newItem.ExtendedSetting != null) {
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

        function getLoadedEntityPersonalizationItems() {

            var items = [];
            var filterItems = ctrl.currentPersonilizationItems;
            var hasGlobal = globalPersonilizationItems && globalPersonilizationItems.length > 0;
            var hasUser;

            if (filterItems != undefined && filterItems.length > 0) {
                for (var i = 0; i < filterItems.length; i++) {
                    var item = UtilsService.getItemByVal(uniqueKeys, filterItems[i].EntityUniqueName, "EntityUniqueName");
                    if (item != null) {
                        if (filterItems[i].IsGlobal == false)
                            hasUser = true;
                        items.push(item);
                    }
                }
            }

            var justUser = null;
            if (hasGlobal == true && !hasUser)
                justUser = false;
            if (hasUser == true && !hasGlobal)
                justUser = true;

            return {
                items: items,
                justUser: justUser
            };
        }
    }

    return directiveDefinitionObject;

}]);


