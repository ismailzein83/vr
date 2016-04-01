﻿"use strict";

app.directive("vrRulesNormalizenumbersettings", ["VR_Rules_NormalizationRuleAPIService", "UtilsService", "VRNotificationService","VRUIUtilsService", function (VR_Rules_NormalizationRuleAPIService, UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObj = {
        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: "/Client/Modules/VR_Rules/Directives/NormalizationRule/Templates/NormalizeNumberSettingsDirectiveTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        // public members

        this.initializeController = initializeController;

        var isNotRequired = false;

        function initializeController() {
            ctrl.templates = [];
            ctrl.selectedActionTemplate = undefined;
            ctrl.disableAddButton = true;
            ctrl.datasource = [];

            ctrl.onActionTemplateChanged = function () {
                ctrl.disableAddButton = (ctrl.selectedActionTemplate == undefined);
            };
            ctrl.isValid = function () {
                if (isNotRequired === true)
                    return null;
                if (ctrl.datasource.length > 0)
                    return null;
                return "You must add at least one action";
            }
            ctrl.addFilter = function () {
                var dataItem = {
                    id: ctrl.datasource.length + 1,
                    configId: ctrl.selectedActionTemplate.TemplateConfigID,
                    editor: ctrl.selectedActionTemplate.Editor,
                    name: ctrl.selectedActionTemplate.Name
                };

                dataItem.onDirectiveReady = function (api) {
                    dataItem.directiveAPI = api;
                    var setLoader = function (value) { ctrl.isLoadingDirective = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, undefined, setLoader);
                };

                ctrl.datasource.push(dataItem);
                ctrl.selectedActionTemplate = undefined;
            };
            ctrl.removeFilter = function (dataItem) {
                var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.id, 'id');
                ctrl.datasource.splice(index, 1);
            };
            defineAPI();
        }

        // private members

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var settings;

                if (payload != undefined)
                {
                    settings = payload.settings;
                    if (payload.isNotRequired != undefined) { isNotRequired = payload.isNotRequired; }
                }

                var promises = [];

                var filterItems;
                if (settings != undefined && settings.Actions != undefined) {
                    filterItems = [];
                    for (var i = 0; i < settings.Actions.length; i++) {
                        var filterItem = {
                            payload: settings.Actions[i],
                            readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                            loadPromiseDeferred: UtilsService.createPromiseDeferred()
                        };
                        promises.push(filterItem.loadPromiseDeferred.promise);
                        filterItems.push(filterItem);
                    }
                }

                var loadTemplatesPromise = VR_Rules_NormalizationRuleAPIService.GetNormalizeNumberActionSettingsTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.templates.push(item);
                    });

                    if (filterItems != undefined) {
                        for (var i = 0; i < filterItems.length; i++) {
                            addFilterItemToGrid(filterItems[i]);
                        }
                    }
                });

                promises.push(loadTemplatesPromise);

                function addFilterItemToGrid(filterItem) {
                    var matchItem = UtilsService.getItemByVal(ctrl.templates, filterItem.payload.ConfigId, "TemplateConfigID");
                    if (matchItem == null)
                        return;

                    var dataItem = {
                        id: ctrl.datasource.length + 1,
                        configId: matchItem.TemplateConfigID,
                        editor: matchItem.Editor,
                        name: matchItem.Name
                    };
                    var dataItemPayload = filterItem.payload;

                    dataItem.onDirectiveReady = function (api) {
                        dataItem.directiveAPI = api;
                        filterItem.readyPromiseDeferred.resolve();
                    };

                    filterItem.readyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, filterItem.loadPromiseDeferred);
                        });

                    ctrl.datasource.push(dataItem);

                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var data;
                if (ctrl.datasource.length > 0) {
                    data = {
                        Actions: getActions()
                    };
                }
                return data;
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                ctrl.onReady(api);
            }

            function getActions() {
                var actions = [];

                angular.forEach(ctrl.datasource, function (item) {

                    var action = item.directiveAPI.getData();
                    action.ConfigId = item.configId;

                    actions.push(action);
                });

                return actions;
            }
        }

    }

    return directiveDefinitionObj;

}]);
