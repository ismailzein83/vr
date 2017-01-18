'use strict';

app.directive('retailVoiceRuleinternationalidentification', ['UtilsService', 'VRUIUtilsService',
function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new didInternationalIdentification(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Retail_Voice/Directives/MainExtensions/InternationalIdentification/Templates/RuleInternationalIdentification.html'
    };


    function didInternationalIdentification(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;
        $scope.scopeModel = {};

        var genericRuleDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var genericRuleDefinitionSelectorDirectiveApi;

        function initializeController() {
            $scope.scopeModel.onGenericRuleDefinitionSelectorDirectiveReady = function (api) {
                genericRuleDefinitionSelectorDirectiveApi = api;
                genericRuleDefinitionSelectorReadyPromiseDeferred.resolve();
            }
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                var loadGenericRuleDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                genericRuleDefinitionSelectorReadyPromiseDeferred.promise.then(function () {
                    var genericRuleDefinitionPayload;
                    if (payload != undefined) {
                        genericRuleDefinitionPayload = { selectedIds: payload.RuleDefinitionId };
                    }
                    VRUIUtilsService.callDirectiveLoad(genericRuleDefinitionSelectorDirectiveApi, genericRuleDefinitionPayload, loadGenericRuleDefinitionSelectorPromiseDeferred);
                });
                promises.push(loadGenericRuleDefinitionSelectorPromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var obj = {
                    $type: "Retail.Voice.MainExtensions.RuleInternationalIdentification, Retail.Voice.MainExtensions",
                    RuleDefinitionId: genericRuleDefinitionSelectorDirectiveApi.getSelectedIds()
                };
                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);