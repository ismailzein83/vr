'use strict';
app.directive('vrGenericdataDatatransformationIfstep', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new IfStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/If/Templates/IfStepTemplate.html';
            }

        };

        function IfStepCtor(ctrl, $scope) {
            var conditionDirectiveReadyAPI;
            var conditionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                ctrl.onConditionDirectiveReady = function (api) {
                    conditionDirectiveReadyAPI = api;
                    conditionDirectiveReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var loadConditionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                    conditionDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadRecordName = { context: payload.context, getArray: true };
                        if (payload != undefined && payload.stepDetails != undefined)
                            payloadRecordName.selectedRecords = payload.stepDetails.Condition;
                        VRUIUtilsService.callDirectiveLoad(conditionDirectiveReadyAPI, payloadRecordName, loadConditionDirectivePromiseDeferred);
                    });
                    promises.push(loadConditionDirectivePromiseDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.IfStep, Vanrise.GenericData.Transformation.MainExtensions",
                        Condition: conditionDirectiveReadyAPI.getData(),
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