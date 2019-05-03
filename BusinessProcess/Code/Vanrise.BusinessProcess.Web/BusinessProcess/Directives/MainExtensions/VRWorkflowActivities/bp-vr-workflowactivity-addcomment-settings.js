'use strict';

app.directive('businessprocessVrWorkflowactivityAddcommentSettings', ['UtilsService', 'VR_GenericData_DataRecordTypeAPIService','VR_GenericData_GenericBEDefinitionAPIService',
    function (UtilsService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_GenericBEDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                remove: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new addBusinessEntitySettings(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowActivityAddCommentSettingsTemplate.html'
        };

        function addBusinessEntitySettings(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.scopeModel.objectId = undefined;
                    $scope.scopeModel.content = undefined;
                    $scope.scopeModel.isSucceeded = undefined;
                    $scope.scopeModel.userId = undefined;

                    if (payload != undefined) {
                        $scope.scopeModel.context = payload.context;
                        var settings = payload.settings;

                        if (settings != undefined) {
                            $scope.scopeModel.objectId = settings.ObjectId;
                            $scope.scopeModel.content = settings.Content;
                            $scope.scopeModel.isSucceeded = settings.IsSucceeded;
                            $scope.scopeModel.userId = settings.UserId;
                      
                        }
                    }
                    return UtilsService.waitPromiseNode({promises:[]});
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.BEActivities.VRWorkflowAddCommentActivity, Vanrise.BusinessProcess.MainExtensions",
                        ObjectId: $scope.scopeModel.objectId,
                        Content: $scope.scopeModel.content,
                        IsSucceeded: $scope.scopeModel.isSucceeded,
                        UserId: $scope.scopeModel.userId,
                    };
                };
              
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


        }
        return directiveDefinitionObject;
    }]);