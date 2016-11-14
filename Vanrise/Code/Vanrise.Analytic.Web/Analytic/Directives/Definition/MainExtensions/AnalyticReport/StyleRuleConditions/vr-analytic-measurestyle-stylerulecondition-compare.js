(function (app) {

    'use strict';

    MeasurestyleStyleruleconditionCompareDirective.$inject = ["UtilsService", 'VRUIUtilsService','VR_Analytic_CompareOperatorEnum'];

    function MeasurestyleStyleruleconditionCompareDirective(UtilsService, VRUIUtilsService, VR_Analytic_CompareOperatorEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var measurestyleStyleruleconditionCompare = new MeasurestyleStyleruleconditionCompare($scope, ctrl, $attrs);
                measurestyleStyleruleconditionCompare.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticReport/StyleRuleConditions/Templates/CompareConditionTemplate.html"

        };
        function MeasurestyleStyleruleconditionCompare($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();

            var directiveAPI;
            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.compareOperators = UtilsService.getArrayEnum(VR_Analytic_CompareOperatorEnum);
                $scope.scopeModel.selectedCompareOperator = VR_Analytic_CompareOperatorEnum.Equals;
                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directiveReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        mainPayload = payload;
                        context = payload.context;
                        $scope.scopeModel.editor = context != undefined ? context.getFieldTypeEditor(payload.FieldType.ConfigId) : undefined;
                        if(payload.RuleStyle !=undefined)
                            $scope.scopeModel.selectedCompareOperator = UtilsService.getItemByVal($scope.scopeModel.compareOperators, payload.RuleStyle.CompareOperator, "value");

                        var promises = [];

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            var payloadDirective = { fieldType: payload.FieldType, fieldValue: payload.RuleStyle != undefined ? payload.RuleStyle.CompareValue : undefined, fieldTitle: payload.Title };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, directiveLoadDeferred);
                        });
                        promises.push(directiveLoadDeferred.promise);
                        return UtilsService.waitMultiplePromises(promises);

                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {  
                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.StyleRuleConditions.CompareCondition, Vanrise.Analytic.MainExtensions ",
                        CompareOperator: $scope.scopeModel.selectedCompareOperator.value,
                        CompareValue: directiveAPI != undefined ? directiveAPI.getData() : undefined
                    };
                    return data;
                }
            }
        }
    }

    app.directive('vrAnalyticMeasurestyleStyleruleconditionCompare', MeasurestyleStyleruleconditionCompareDirective);

})(app);