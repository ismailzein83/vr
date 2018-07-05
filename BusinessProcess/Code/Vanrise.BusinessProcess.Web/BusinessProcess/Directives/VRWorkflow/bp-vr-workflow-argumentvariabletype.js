'use strict';

app.directive('businessprocessVrWorkflowArgumentvariabletype', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;

                var ctor = new ArgumentVariableTypeCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowArgumentVariableTypeTemplate.html"
        };

        function ArgumentVariableTypeCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var variableTypeSelectorAPI;
            var variableTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onVariableTypeSelectorReady = function (api) {
                    variableTypeSelectorAPI = api;
                    variableTypeSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var argumentVariableType;

                    if(payload != undefined){
                        argumentVariableType = payload.argumentVariableType;
                    }

                    var variableTypeSelectorLoadPromise = getVariableTypeSelectorLoadPromise();
                    promises.push(variableTypeSelectorLoadPromise);

                    function getVariableTypeSelectorLoadPromise() {

                        var variableTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        variableTypeSelectorReadyDeferred.promise.then(function () {

                            var variableTypeSelectorPayload = { selectIfSingleItem: true };
                            if (argumentVariableType != undefined) {
                                variableTypeSelectorPayload.variableType = argumentVariableType;
                            }
                            VRUIUtilsService.callDirectiveLoad(variableTypeSelectorAPI, variableTypeSelectorPayload, variableTypeSelectorLoadDeferred);
                        });

                        return variableTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return variableTypeSelectorAPI.getData();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);