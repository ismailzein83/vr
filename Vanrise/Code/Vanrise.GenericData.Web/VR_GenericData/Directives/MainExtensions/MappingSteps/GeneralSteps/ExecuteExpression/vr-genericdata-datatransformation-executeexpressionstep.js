'use strict';
app.directive('vrGenericdataDatatransformationExecuteexpressionstep', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService','VR_GenericData_DataTransformationDefinitionAPIService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService, VR_GenericData_DataTransformationDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new ExecuteExpressionStepCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/ExecuteExpression/Templates/ExecuteExpressionStepTemplate.html';
            }

        };

        function ExecuteExpressionStepCtor(ctrl, $scope) {
            var stepPayload;
            var expressionDirectiveReadyAPI;
            var expressionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onExpressionReady = function (api) {
                    expressionDirectiveReadyAPI = api;
                    expressionDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    stepPayload = payload;
                    var promises = [];

                    var loadExpressionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                    expressionDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadSource = { context: payload.context };
                        if (payload.stepDetails != undefined)
                            payloadSource.selectedRecords = payload.stepDetails.Expression;
                        VRUIUtilsService.callDirectiveLoad(expressionDirectiveReadyAPI, payloadSource, loadExpressionDirectivePromiseDeferred);
                    });
                    promises.push(loadExpressionDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.ExecuteExpressionStep, Vanrise.GenericData.Transformation.MainExtensions",
                        Expression: expressionDirectiveReadyAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);