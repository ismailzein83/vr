"use strict";

app.directive("vrAccountbalanceAccounttypeSources", ["UtilsService", "VRNotificationService", "VR_AccountBalance_AccountTypeService",
    function (UtilsService, VRNotificationService, VR_AccountBalance_AccountTypeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new Sources($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_AccountBalance/Directives/MainExtensions/Account/Templates/VRAccountTypeSources.html"

        };

        function Sources($scope, ctrl, $attrs) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one source.";
                };

                ctrl.addSource = function () {
                    var onSourceAdded = function (source) {
                        loadSourceFields(source);
                        ctrl.datasource.push({ Entity: source });
                    };
                    VR_AccountBalance_AccountTypeService.addSource(onSourceAdded, getContext());
                };

                ctrl.removeSource = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var sources;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        sources = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            sources.push(currentItem.Entity);
                        }
                    }
                    return sources;
                };

                api.load = function (payload) {

                    if (payload != undefined) {
                        context = payload.context;

                        if (payload.sources != undefined) {
                            for (var i = 0; i < payload.sources.length; i++) {
                                var source = payload.sources[i];
                                ctrl.datasource.push({ Entity: source });
                            }
                        }
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editSource,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editSource(sourceObj) {
                var onSourceUpdated = function (source) {
                    loadSourceFields(source);
                    var index = ctrl.datasource.indexOf(sourceObj);
                    ctrl.datasource[index] = { Entity: source };
                };
                VR_AccountBalance_AccountTypeService.editSource(sourceObj.Entity, onSourceUpdated, getContext());
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};

                return currentContext;
            }

            function loadSourceFields(source)
            {
               if (context != undefined && context.loadSourceFields != undefined)
                  context.loadSourceFields(source);
            }
        }

        return directiveDefinitionObject;

    }
]);