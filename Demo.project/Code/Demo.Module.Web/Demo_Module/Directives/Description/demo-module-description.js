"use strict";
app.directive("demoModuleDescription", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Demo_Module_DescriptionService",

function (UtilsService, VRNotificationService, VRUIUtilsService, Demo_Module_DescriptionService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope:
        {
            onReady: "=",
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new Descriptions($scope, ctrl, $attrs);
            ctor.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/Description/Templates/DescriptionTemplate.html"

    };

    function Descriptions($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var context;

        function initializeController() {
            $scope.scopeModel = {};
            ctrl.datasource = [];

            ctrl.addDescription = function () {
                var onDescriptionAdded = function (description) {
                    ctrl.datasource.push({ Entity: description });
                };

                Demo_Module_DescriptionService.addDescription(onDescriptionAdded, getContext());
            };


            ctrl.removeDescription = function (dataItem) {
                var index = ctrl.datasource.indexOf(dataItem);
                ctrl.datasource.splice(index, 1);
            };

            defineMenuActions();
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var descriptionsEntity;
                if (payload != undefined) {
                    descriptionsEntity = payload;
                    context = payload.context;
                    if (descriptionsEntity != undefined) {
                        for (var i = 0; i < descriptionsEntity.length; i++) {
                            var description = descriptionsEntity[i];
                            ctrl.datasource.push({ Entity: description });
                        }
                    }
                }

                var promises = [];
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var descriptions;
                if (ctrl.datasource != undefined) {
                    descriptions = [];
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        var currentItem = ctrl.datasource[i];
                        descriptions.push(currentItem.Entity);
                    }
                }
                return descriptions;
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

        function defineMenuActions() {
            var defaultMenuActions = [
            {
                name: "Edit",
                clicked: editDescription,
            }];

            $scope.gridMenuActions = function (dataItem) {
                return defaultMenuActions;
            };
        }

        function editDescription(descriptionObj) {
            var onDescriptionUpdated = function (description) {
                var index = ctrl.datasource.indexOf(descriptionObj);
                ctrl.datasource[index] = { Entity: description };
            };
            Demo_Module_DescriptionService.editDescription(descriptionObj.Entity, onDescriptionUpdated, getContext());
        }
    }

    return directiveDefinitionObject;
}
]);