(function (appControllers) {

    "use strict";
    VRUIUtilsService.$inject = ['VRNotificationService', 'UtilsService'];

    function VRUIUtilsService(VRNotificationService, UtilsService) {

        function loadDirective(scope, directiveAPI, loaderProperty) {
            var promise = directiveAPI.load();
            if (promise != undefined) {
                scope[loaderProperty] = true;
                promise.catch(function (error) {
                    VRNotificationService.notifyException(error, scope);
                }).finally(function () {
                    scope[loaderProperty] = false;
                });
            }
        }

        function callDirectiveLoad(directiveAPI, directiveLoadPayload, loadPromiseDeferred) {
            UtilsService.convertToPromiseIfUndefined(directiveAPI.load(directiveLoadPayload)).then(function () {
                if (loadPromiseDeferred != undefined)
                    loadPromiseDeferred.resolve();
            }).catch(function (error) {
                if (loadPromiseDeferred != undefined)
                    loadPromiseDeferred.reject(error);
            });
        }

        function callDirectiveLoadOrResolvePromise(scope, directiveAPI, directiveLoadPayload, setLoader, readyPromiseDeferred) {
            if (readyPromiseDeferred != undefined)
                readyPromiseDeferred.resolve();
            else {
                setLoader(true);
                UtilsService.convertToPromiseIfUndefined(directiveAPI.load(directiveLoadPayload))
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    }).finally(function () {
                        setLoader(false);
                    });
            }
        }

        function getIdSelectedIds(idProperty, attrs, ctrl) {

            if (attrs.ismultipleselection != undefined) {
                return UtilsService.getPropValuesFromArray(ctrl.selectedvalues, idProperty);
            }
            else if (ctrl.selectedvalues != undefined) {
                return ctrl.selectedvalues[idProperty];
            }

            return undefined;
        }

        function setSelectedValues(selectedIds, idProperty, attrs, ctrl) {
            if (attrs.ismultipleselection != undefined) {
                for (var i = 0; i < selectedIds.length; i++) {
                    var selectedValue = UtilsService.getItemByVal(ctrl.datasource, selectedIds[i], idProperty);
                    if (selectedValue != null)
                        ctrl.selectedvalues.push(selectedValue);
                }
            }
            else {
                var selectedValue = UtilsService.getItemByVal(ctrl.datasource, selectedIds, idProperty);
                if (selectedValue != null)
                    ctrl.selectedvalues = selectedValue;
            }
        }

        function getSettingsFromDirective(scope, directiveAPI, templateProperty) {
            if (scope[templateProperty] != undefined) {
                var settings = directiveAPI.getData();
                settings.ConfigId = scope[templateProperty].TemplateConfigID;
                return settings;
            }
            else
                return null;
        }

        function defineGridDrillDownTabs(drillDownDefinitions, gridAPI, gridMenuActions, setMenuActionsOnDataItem) {
            return new GridDrillDownTabs(UtilsService, drillDownDefinitions, gridAPI, gridMenuActions, setMenuActionsOnDataItem);
        }

        return ({
            loadDirective: loadDirective,
            callDirectiveLoad: callDirectiveLoad,
            callDirectiveLoadOrResolvePromise: callDirectiveLoadOrResolvePromise,
            getIdSelectedIds: getIdSelectedIds,
            setSelectedValues: setSelectedValues,
            getSettingsFromDirective: getSettingsFromDirective,
            defineGridDrillDownTabs: defineGridDrillDownTabs
        });
    }

    appControllers.service('VRUIUtilsService', VRUIUtilsService);
})(appControllers);


function GridDrillDownTabs(UtilsService, drillDownDefinitions, gridAPI, gridMenuActions, setMenuActionsOnDataItem) {

    initialize();
    function initialize() {
        var drillDownDefinitionId = 0;
        for (var i = 0; i < drillDownDefinitions.length; i++) {
            var drillDownDefinition = drillDownDefinitions[i];
            drillDownDefinition.drillDownDefinitionId = drillDownDefinitionId++;
            defineDrillDownDefinitionMembers(drillDownDefinition);
            if (!setMenuActionsOnDataItem) {
                if (drillDownDefinition.parentMenuActions != undefined) {
                    for (var j = 0; j < drillDownDefinition.parentMenuActions.length; j++) {
                        var menuAction = createMenuAction(drillDownDefinition.parentMenuActions[j]);
                        gridMenuActions.push(menuAction);
                    }
                }
            }
        }
    }

    function defineDrillDownDefinitionMembers(drillDownDefinition) {
        drillDownDefinition.setTabSelected = function (dataItem) {
            gridAPI.expandRow(dataItem);
            var drillDownTab = UtilsService.getItemByVal(dataItem.drillDownExtensionObject.drillDownDirectiveTabs, drillDownDefinition.drillDownDefinitionId, "drillDownDefinitionId");
            if (drillDownTab.tabObject == undefined)
                drillDownTab.tabObject = {};
            drillDownTab.tabObject.isSelected = true;
        };
    }

    function getDataItemMenuActions(dataItem) {
        var dataItemMenuActions = [];
        if (gridMenuActions != undefined) {
            for (var i = 0; i < gridMenuActions.length ; i++) {
                dataItemMenuActions.push(gridMenuActions[i]);
            }
        }
        for (var i = 0; i < drillDownDefinitions.length; i++) {
            var drillDownDefinition = drillDownDefinitions[i];
            if (drillDownDefinition.hideDrillDownFunction == undefined || !drillDownDefinition.hideDrillDownFunction(dataItem)) {
                if (drillDownDefinition.parentMenuActions != undefined) {
                    for (var j = 0; j < drillDownDefinition.parentMenuActions.length; j++) {
                        var menuAction = createMenuAction(drillDownDefinition.parentMenuActions[j]);
                        dataItemMenuActions.push(menuAction);
                    }
                }

            }
        }
        return dataItemMenuActions;
    }

    function createMenuAction(menuActionDefinition)
    {
        return {
            name: menuActionDefinition.name,
            clicked: menuActionDefinition.clicked,
            haspermission: menuActionDefinition.haspermission
        };
    }

    function setDrillDownExtensionObject(dataItem) {
        dataItem.drillDownExtensionObject = {};
        dataItem.drillDownExtensionObject.drillDownDirectiveTabs = [];
        for (var i = 0; i < drillDownDefinitions.length; i++) {
            setDrillDownTab(drillDownDefinitions[i], dataItem);
        }
        if (setMenuActionsOnDataItem) {
            dataItem.drillDownExtensionObject.menuActions = getDataItemMenuActions(dataItem);
        }
    }

    function setDrillDownTab(drillDownDefinition, dataItem) {
        if (drillDownDefinition.hideDrillDownFunction == undefined || !drillDownDefinition.hideDrillDownFunction(dataItem)) {
            addDrillDownTab(drillDownDefinition, dataItem);
        }
    }

    function addDrillDownTab(drillDownDefinition, dataItem) {
        var drillDownDirectiveTab = {};

        drillDownDirectiveTab.drillDownDefinitionId = drillDownDefinition.drillDownDefinitionId;
        drillDownDirectiveTab.title = drillDownDefinition.title;
        drillDownDirectiveTab.localizedtitle = drillDownDefinition.localizedtitle;
        drillDownDirectiveTab.directive = drillDownDefinition.directive;
        drillDownDirectiveTab.loadDirective = function (directiveAPI) {
            return drillDownDefinition.loadDirective(directiveAPI, dataItem);
        };
        drillDownDirectiveTab.setTabSelected = drillDownDefinition.setTabSelected;
        dataItem.drillDownExtensionObject.drillDownDirectiveTabs.push(drillDownDirectiveTab);
    }

    return {
        setDrillDownExtensionObject: setDrillDownExtensionObject
    };
}
