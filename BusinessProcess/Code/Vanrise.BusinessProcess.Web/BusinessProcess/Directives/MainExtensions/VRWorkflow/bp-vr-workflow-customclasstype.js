'use strict';

app.directive('businessprocessVrWorkflowCustomclasstype', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new WorkflowCustomClassTypeDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflow/Templates/VRWorkflowCustomClassTypeTemplate.html"
        };

        function WorkflowCustomClassTypeDirectiveCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var customClassTypeDirectiveAPI;
            var customClassTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCustomClassTypeDirectiveReady = function (api) {
                    customClassTypeDirectiveAPI = api;
                    customClassTypeDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var customClassTypeDirectiveLoadPromise = getCustomClassTypeDirectivePromise();
                    promises.push(customClassTypeDirectiveLoadPromise);

                    function getCustomClassTypeDirectivePromise() {
                        var customClassTypeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        customClassTypeDirectiveReadyDeferred.promise.then(function () {
                            var customClassTypeDirectivePayload;
                            if (payload != undefined) {
                                customClassTypeDirectivePayload = payload.FieldType;
                            }
                            VRUIUtilsService.callDirectiveLoad(customClassTypeDirectiveAPI, customClassTypeDirectivePayload, customClassTypeDirectiveLoadDeferred);
                        });

                        return customClassTypeDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowVariableTypes.VRWorkflowCustomClassType, Vanrise.BusinessProcess.MainExtensions",
                        FieldType: customClassTypeDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);