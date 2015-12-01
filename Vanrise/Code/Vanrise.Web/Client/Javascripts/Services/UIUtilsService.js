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
                if (selectedIds) {
                    for (var i = 0; i < selectedIds.length; i++) {
                        var selectedValue = UtilsService.getItemByVal(ctrl.datasource, selectedIds[i], idProperty);
                        if (selectedValue != null)
                            ctrl.selectedvalues.push(selectedValue);
                    }
                }
                else
                    ctrl.selectedvalues = [];
                
            } else {
                if (selectedIds) {
                    var selectedValue = UtilsService.getItemByVal(ctrl.datasource, selectedIds, idProperty);
                    if (selectedValue != null)
                        ctrl.selectedvalues = selectedValue;
                }
                else
                    ctrl.selectedvalues = undefined;
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

        function defineGridDrillDownTabs(drillDownDefinitions, gridAPI, gridMenuActions) {
            return new GridDrillDownTabs(drillDownDefinitions, gridAPI, gridMenuActions);
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


function GridDrillDownTabs(drillDownDefinitions, gridAPI, gridMenuActions) {

    initialize();
    function initialize() {
        for (var i = 0; i < drillDownDefinitions.length; i++) {
            var drillDownDefinition = drillDownDefinitions[i];
            defineDrillDownDefinitionMembers(drillDownDefinition);
            if (drillDownDefinition.parentMenuActions != undefined) {
                for (var j = 0; j < drillDownDefinition.parentMenuActions.length; j++) {
                    defineDrillDownMenuAction(drillDownDefinition.parentMenuActions[j]);
                }
            }
        }
    }

    function defineDrillDownDefinitionMembers(drillDownDefinition) {
        drillDownDefinition.setTabSelected = function (dataItem) {
            gridAPI.expandRow(dataItem);
            var tabIndex = drillDownDefinitions.indexOf(drillDownDefinition);
            var drillDownTab = dataItem.drillDownExtensionObject.drillDownDirectiveTabs[tabIndex];
            if (drillDownTab.tabObject == undefined)
                drillDownTab.tabObject = {};
            drillDownTab.tabObject.isSelected = true;
        };
    }

    function defineDrillDownMenuAction(drillDownDefinition) {
        var menuAction = {
            name: drillDownDefinition.name,
            clicked: drillDownDefinition.clicked
        };
        gridMenuActions.push(menuAction);
    }

    function setDrillDownExtensionObject(dataItem) {
        dataItem.drillDownExtensionObject = {};
        dataItem.drillDownExtensionObject.drillDownDirectiveTabs = [];
        for (var i = 0; i < drillDownDefinitions.length; i++) {
            addDrillDownTab(drillDownDefinitions[i], dataItem);
        }
    }

    function addDrillDownTab(drillDownDefinition, dataItem) {
        var drillDownDirectiveTab = {};
        drillDownDirectiveTab.title = drillDownDefinition.title;
        drillDownDirectiveTab.directive = drillDownDefinition.directive;
        drillDownDirectiveTab.loadDirective = function (directiveAPI) {
            return drillDownDefinition.loadDirective(directiveAPI, dataItem);
        }
        dataItem.drillDownExtensionObject.drillDownDirectiveTabs.push(drillDownDirectiveTab);
    }

    return {
        setDrillDownExtensionObject: setDrillDownExtensionObject
    }
}