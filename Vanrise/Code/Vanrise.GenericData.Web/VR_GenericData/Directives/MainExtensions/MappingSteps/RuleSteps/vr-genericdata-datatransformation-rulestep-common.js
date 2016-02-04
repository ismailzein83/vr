'use strict';
app.directive('vrGenericdataDatatransformationRulestepCommon', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService','VR_GenericData_GenericRuleDefinitionAPIService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService, VR_GenericData_GenericRuleDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new rulestepCommonCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/Templates/RuleStepCommonTemplate.html';
            }

        };

        function rulestepCommonCtor(ctrl, $scope) {
            var ruleTypeEntity;

            var ruleDefinitionDirectiveAPI;
            var ruleDefinitionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.ruleFieldsMappings = [];
                $scope.onRuleDefinitionReady = function (api) {
                    ruleDefinitionDirectiveAPI = api;
                    ruleDefinitionDirectiveReadyPromiseDeferred.resolve();
                }
                $scope.onRuleSelectionChanged = function () {
                    if ($scope.selectedRuleDefinition != undefined)
                    {
                        $scope.isLoadingMappingData = true;
                        loadRuleDefinition($scope.selectedRuleDefinition.GenericRuleDefinitionId).then(function (response) {
                            console.log(response);
                            if (response.CriteriaDefinition.Fields)
                            {
                                for (var i = 0; i < response.CriteriaDefinition.Fields.length; i++) {
                                    $scope.ruleFieldsMappings.push({
                                        FieldName:response.CriteriaDefinition.Fields[i].FieldName,
                                        Title: response.CriteriaDefinition.Fields[i].Title
                                    });
                                }
                            }
                        }).finally(function () {
                            $scope.isLoadingMappingData = false;
                        });
                    }
                    
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined)
                    {
                        var promises = [];
                        var loadRuleDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        loadRuleType(payload.ruleTypeName).then(function () {
                            ruleDefinitionDirectiveReadyPromiseDeferred.promise.then(function () {
                                var payload = { filter: { RuleTypeId: ruleTypeEntity.GenericRuleTypeConfigId } };
                                VRUIUtilsService.callDirectiveLoad(ruleDefinitionDirectiveAPI, payload, loadRuleDefinitionDirectivePromiseDeferred);
                            });
                        })
                        promises.push(loadRuleDefinitionDirectivePromiseDeferred.promise);
                        return UtilsService.waitMultiplePromises(promises);
                    }
                   
                }
                function loadRuleType(ruleTypeName) {
                    return VR_GenericData_GenericRuleTypeConfigAPIService.GetGenericRuleTypeByName(ruleTypeName).then(function (response) {

                        ruleTypeEntity = response;
                    });
                }

               
                api.getData = function () {
                    return {
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function loadRuleDefinition(ruleDefinitionId) {
               return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(ruleDefinitionId);
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);