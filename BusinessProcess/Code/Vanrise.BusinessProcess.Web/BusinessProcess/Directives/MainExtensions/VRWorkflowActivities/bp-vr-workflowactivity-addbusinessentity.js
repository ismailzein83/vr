//'use strict';

//app.directive('businessprocessVrWorkflowactivityAddbusinessentity', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'BusinessProcess_VRWorkflowService',
//    function (UtilsService, VRUIUtilsService, VRNotificationService, BusinessProcess_VRWorkflowService) {

//        var directiveDefinitionObject = {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//                remove: '='
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new addBusinessEntity(ctrl, $scope, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowActivityAddBusinessEntityTemplate.html'
//        };

//        function addBusinessEntity(ctrl, $scope, $attrs) {

//            var settings;
//            var displayName;
//            var entityDefinitionId;
//            var isNew;
//            var context;

//            this.initializeController = initializeController;
//            function initializeController() {
//                $scope.scopeModel = {};

              
//                defineAPI();
//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];

             
//                    var editModeAction = {
//                        name: "Edit",
//                        clicked: openActivityEditor
//                    };

//                    if (payload != undefined) {
//                        if (payload.Settings != undefined) {
//                            isNew = payload.Settings.IsNew;
//                            entityDefinitionId = payload.Settings.EntityDefinitionId;
//                            displayName = payload.Settings.DisplayName;
//                            $scope.scopeModel.displayName = displayName;
//                            settings = payload.Settings.Settings;

//                        }

//                        if (payload.Context != null)
//                            context = payload.Context;

//                        if (payload.SetMenuAction != undefined)
//                            payload.SetMenuAction(editModeAction);

//                        if (isNew) {
//                            openActivityEditor();
//                        }
//                    }

//                    function openActivityEditor() {
//                        var onActivityUpdated = function (updatedObject) {
//                            entityDefinitionId = updatedObject.entityDefinitionId;
//                            displayName = updatedObject.displayName;
//                            settings = updatedObject.settings;
//                            isNew = false;
//                        };

//                        BusinessProcess_VRWorkflowService.openAddBusinessEntityEditor(buildObjectFromScope(), context, onActivityUpdated, ctrl.remove, isNew);
//                    }

//                    var rootPromiseNode = {
//                        promises: []
//                    };
//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {
//                    return buildObjectFromScope();
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }

//            function buildObjectFromScope() {
//                return {
//                    $type:"Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowAddBEActivity, Vanrise.BusinessProcess.MainExtensions",
//                    EntityDefinitionId: entityDefinitionId,
//                    DisplayName: displayName,
//                    Settings: settings,
//                };
//            }
//        }
//        return directiveDefinitionObject;
//    }]);