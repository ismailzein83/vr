'use strict';

app.directive('vrActionbar', ['ActionBarDirService', 'MultiTranscludeService', 'UtilsService', 'VRNotificationService', 'VR_Common_EntityPersonalizationAPIService', 'VRModalService', 'VRLocalizationService', 'MobileService', function (ActionBarDirService, MultiTranscludeService, UtilsService, VRNotificationService, VR_Common_EntityPersonalizationAPIService, VRModalService, VRLocalizationService, MobileService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        scope: {
            showcollapsebutton: '=',
            showpersonalizationmenu: '=',
            issectioncollapsed: '=',
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.isMobile = MobileService.isMobile();
            var ctor = new ActionBarCtor($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        link: function (scope, elem, attr, ctrl, transcludeFn) {
            MultiTranscludeService.transclude(elem, transcludeFn);
        },
        controllerAs: 'actionBarCtrl',
        bindToController: true,
        templateUrl: function (element, attrs) {
            return ActionBarDirService.dTemplate;
        }
    };

    function ActionBarCtor($scope, ctrl, $attrs) {

        ctrl.showMenuOption = false;

        ctrl.buttonsList = [];

        ctrl.addButton = function (btn) {
            ctrl.buttonsList.push(btn);
        };

        ctrl.openAllActionPopup = function () {
            var modalSettings = {
                autoclose: true
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.actionBarCtrl = ctrl;
            };
            VRModalService.showModal("/Client/Javascripts/Directives/ActionBar/AllActionsModalPopup.html", null, modalSettings);
        };

        ctrl.getFirstTowButtons = function () {
            var firstTowList = [];
            for (var i = 0; i < ctrl.buttonsList.length ; i++) {
                var btn = ctrl.buttonsList[i];
                if (btn.showbutton == undefined ||  btn.showbutton == true)
                    firstTowList.push(btn);
                if (firstTowList.length == 2)
                    return firstTowList;
            }
            return firstTowList;
        };

        $scope.$on("hide-all-menu", function (event, args) {
            ctrl.showMenuOption = false;
            $('.vr-menu-list').removeClass("open-grid-menu");
            $(document).unbind('click', bindClickOutSidePersonalizationMenu);
        });

        ctrl.toggelOptionMenu = function (e) {
            var self = angular.element(e.currentTarget);
            var menu = self.parent().find('.vr-menu-list')[0];
            if (ctrl.showMenuOption == false) {
                setTimeout(function () {
                    var selfHeight = $(self).height();
                    var selfOffset = $(self).offset();
                    $(menu).css({ display: 'block' });
                    $(menu).addClass("open-option-menu");
                    var elleft = selfOffset.left - $(window).scrollLeft() + 14;
                    if (VRLocalizationService.isLocalizationRTL())
                        elleft = 'auto';
                    if ($(self).parents('.vr-datagrid-celltext').length > 0)
                        basetop -= 10;


                    $(menu).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + 5, left: elleft });
                    ctrl.showMenuOption = true;
                    $scope.$root.$digest();
                }, 1);
                $(document).bind("click", bindClickOutSidePersonalizationMenu);

            }
            else {
                ctrl.showMenuOption = false;
                $(menu).removeClass("open-option-menu");
                $(document).unbind('click', bindClickOutSidePersonalizationMenu);
            }
        };

        function bindClickOutSidePersonalizationMenu(e) {
            if (!$('menu-out-div').is(e.target) && $('menu-out-div').has(e.target).length === 0 && $('.open-grid-menu').has(e.target).length === 0) {
                $('menu-out-div').removeClass("open-option-menu");
                ctrl.showMenuOption = false;
                $scope.$root.$digest();
            }
        }

        setTimeout(function () {
            $('.vr-menu-list').parents('div').on('scroll', hidePersonalizationMenu);
            $(window).on('scroll', hidePersonalizationMenu);
        }, 1);

        function hidePersonalizationMenu() {
            var menu = $('.vr-menu-list');
            menu.css({ display: 'none' });
            $('menu-out-div').removeClass("open-option-menu");
            if (ctrl.showMenuOption == true) {
                ctrl.showMenuOption = false;
                $scope.$root.$digest();
            }
        };

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
                if (response != null) {
                    ctrl.currentPersonilizationItems = response.UserEntityPersonalizations;
                    globalPersonilizationItems = response.GlobalEntityPersonalizations;
                }
                return buildSettingsDataOutContext();
            });
        }

        function buildEntityUniqueInputs(checkedEntityUniqueNames) {
            var changedItems = typeof (getContext().getPersonalizationItems) == 'function' ? getContext().getPersonalizationItems() : [];
            var inputs = [];
            for (var i = 0; i < changedItems.length; i++) {
                if (checkedEntityUniqueNames.indexOf(changedItems[i].EntityUniqueName) > -1)
                    inputs.push(changedItems[i]);
            }
            return inputs;
        }


        function getEntityPersonalizationChangedItems() {
            var changedItems = [];
            var newPersonilizationItems = typeof (getContext().getPersonalizationItems) == 'function' ? getContext().getPersonalizationItems() : [];
            if (uniqueKeys == undefined)
                return changedItems;
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
        function buildSettingsDataOutContext() {
            var outContext = {
                getItemByUniqueName: function (uniqueName) {
                    var item = null;
                    var personilizationItemObject = UtilsService.getItemByVal(ctrl.currentPersonilizationItems, uniqueName, "EntityUniqueName");
                    if (personilizationItemObject != null)
                        item = personilizationItemObject.ExtendedSetting;
                    return item;
                }
            };
            return outContext;
        }


    }

    return directiveDefinitionObject;

}]);