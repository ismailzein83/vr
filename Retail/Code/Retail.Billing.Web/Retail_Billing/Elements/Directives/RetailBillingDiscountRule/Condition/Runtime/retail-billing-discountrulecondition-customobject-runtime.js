(function (app) {

    'use strict';

    DiscountRuleConditionCustomObjectRuntime.$inject = ['UtilsService', 'VRUIUtilsService'];

    function DiscountRuleConditionCustomObjectRuntime(UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DiscountRuleConditionCustomObjectRuntimeDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingDiscountRule/Condition/Runtime/Templates/DiscountRuleConditionRuntimeTemplate.html"
        };

        function DiscountRuleConditionCustomObjectRuntimeDirectiveCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var recordFilterConditionDirectiveAPI;
            var recordFilterConditionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRecordFilterConditionDirectiveReady = function (api) {
                    recordFilterConditionDirectiveAPI = api;
                    recordFilterConditionDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var fieldValue;
                    var fieldTitle;
                    var allFieldValuesByName;

                    if (payload != undefined) {
                        fieldValue = payload.fieldValue;
                        fieldTitle = payload.fieldTitle;
                        allFieldValuesByName = payload.allFieldValuesByName;
                    }

                    var promises = [];
                    promises.push(loadRecordFilterConditionDirective());

                    function loadRecordFilterConditionDirective() {
                        var loadRecordFilterConditionDirectiveDeferred = UtilsService.createPromiseDeferred();

                        recordFilterConditionDirectiveReadyDeferred.promise.then(function () {

                            var recordFilterConditionDirectivePayload = {
                                fieldValue: fieldValue,
                                fieldTitle: fieldTitle,
                                allFieldValuesByName: allFieldValuesByName
                            };
                            VRUIUtilsService.callDirectiveLoad(recordFilterConditionDirectiveAPI, recordFilterConditionDirectivePayload, loadRecordFilterConditionDirectiveDeferred);
                        });

                        return loadRecordFilterConditionDirectiveDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.getData = function () {
                    return recordFilterConditionDirectiveAPI.getData();
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                    if (recordFilterConditionDirectiveAPI != undefined && recordFilterConditionDirectiveAPI.onFieldValueChanged != undefined && typeof (recordFilterConditionDirectiveAPI.onFieldValueChanged) == "function")
                        return recordFilterConditionDirectiveAPI.onFieldValueChanged(allFieldValuesByFieldNames);

                    return UtilsService.waitPromiseNode({ promises: [] });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('retailBillingDiscountruleconditionCustomobjectRuntime', DiscountRuleConditionCustomObjectRuntime);
})(app);