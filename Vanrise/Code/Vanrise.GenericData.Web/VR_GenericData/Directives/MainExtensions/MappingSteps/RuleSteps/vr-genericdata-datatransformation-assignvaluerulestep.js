'use strict';
app.directive('vrGenericdataDatatransformationAssignvaluerulestep', ['UtilsService','VR_GenericData_GenericRuleTypeConfigAPIService','VRUIUtilsService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new assignvaluerulestepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/Templates/AssignValueRuleStepTemplate.html';
            }

        };

        function assignvaluerulestepCtor(ctrl, $scope) {
            var ruleTypeName = "GenericRuleMapping";
            var ruleTypeEntity;

            var ruleStepCommonDirectiveAPI;
            var ruleStepCommonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.onRuleStepCommonReady = function (api)
                {
                    ruleStepCommonDirectiveAPI = api;
                    ruleStepCommonDirectiveReadyPromiseDeferred.resolve();
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var loadRuleStepCommonDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        ruleStepCommonDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payload = { ruleTypeName: ruleTypeName };
                            VRUIUtilsService.callDirectiveLoad(ruleStepCommonDirectiveAPI, payload, loadRuleStepCommonDirectivePromiseDeferred);
                        });

                    promises.push(loadRuleStepCommonDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                }
               
                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignValueRuleStep, Vanrise.GenericData.Transformation.MainExtensions",
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);