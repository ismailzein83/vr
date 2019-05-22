'use strict';

app.directive('businessprocessVrWorkflowactivityGetbusinessentity', ['UtilsService', 'BusinessProcess_VRWorkflowService',
    function (UtilsService, BusinessProcess_VRWorkflowService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                remove: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new getBusinessEntity(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowActivityGetBusinessEntityTemplate.html'
        };

        function getBusinessEntity(ctrl, $scope, $attrs) {

            var settings;
            var displayName;
            var entityDefinitionId;
            var isNew;
            var context;
            var entityId;

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
             
                    var editModeAction = {
                        name: "Edit",
                        clicked: openActivityEditor
                    };

                    if (payload != undefined) {
                        if (payload.Settings != undefined) {
                            isNew = payload.Settings.IsNew;
                            entityDefinitionId = payload.Settings.EntityDefinitionId;
                            displayName = payload.Settings.DisplayName;
                            $scope.scopeModel.displayName = displayName;
                            entityId = payload.Settings.EntityId;
                            settings = payload.Settings.Settings;
                        }

                        if (payload.Context != undefined)
                            context = payload.Context;

                        if (payload.SetMenuAction != undefined)
                            payload.SetMenuAction(editModeAction);

                        if (isNew) {
                            openActivityEditor();
                        }
                    }

                    function openActivityEditor() {
                        var onActivityUpdated = function (updatedObject) {
                            entityDefinitionId = updatedObject.entityDefinitionId;
                            displayName = updatedObject.displayName;
                            $scope.scopeModel.displayName = displayName;
                            settings = updatedObject.settings;
                            entityId = updatedObject.entityId;
                            isNew = false;
                        };

                        BusinessProcess_VRWorkflowService.openGetBusinessEntityEditor(buildObjectFromScope(), context, onActivityUpdated, ctrl.remove, isNew);
                    }

                    var rootPromiseNode = {
                        promises: []
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return buildObjectFromScope();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function buildObjectFromScope() {
                return {
                    $type:"Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowGetBEActivity, Vanrise.BusinessProcess.MainExtensions",
                    EntityDefinitionId: entityDefinitionId,
                    DisplayName: displayName,
                    EntityId: entityId,
                    Settings: settings,
                };
            }
        }
        return directiveDefinitionObject;
    }]);