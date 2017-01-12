(function (app) {

    'use strict';

    FilterGroupAccountConditionDirective.$inject = ['Retail_BE_AccountTypeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function FilterGroupAccountConditionDirective(Retail_BE_AccountTypeAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FilterGroupAccountConditionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountCondition/MainExtensions/Templates/FilterGroupAccountConditionTemplate.html"
        };

        function FilterGroupAccountConditionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var accountFields;

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {}; 

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var accountConditionFilterGroup;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        accountConditionFilterGroup = payload.accountCondition;
                    }

                    var recordFilterDirectiveLoadPromise = getRecordFilterDirectiveLoadPromise();
                    promises.push(recordFilterDirectiveLoadPromise);


                    function getRecordFilterDirectiveLoadPromise() {
                        var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        recordFilterDirectiveReadyDeferred.promise.then(function () {

                            loadAccountFields().then(function () {

                                var recordFilterDirectivePayload = { context: buildContext() };
                                if (accountConditionFilterGroup != undefined) {
                                    recordFilterDirectivePayload.FilterGroup = accountConditionFilterGroup.FilterGroup
                                }
                                VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                            });
                        });

                        return recordFilterDirectiveLoadDeferred.promise;
                    }
                    function loadAccountFields() {

                        return Retail_BE_AccountTypeAPIService.GetGenericFieldDefinitionsInfo(accountBEDefinitionId).then(function (response) {
                            accountFields = response;
                        });
                    }
                    function buildContext() {
                        var context = {
                            getFields: function () {
                                var fields = [];
                                if (accountFields != undefined) {
                                    for (var i = 0; i < accountFields.length; i++) {
                                        var accountField = accountFields[i];

                                        fields.push({
                                            FieldName: accountField.Name,
                                            FieldTitle: accountField.Title,
                                            Type: accountField.FieldType,
                                        });
                                    }
                                }
                                return fields;
                            }
                        }
                        return context;
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountConditions.FilterGroupAccountCondition, Retail.BusinessEntity.MainExtensions",
                        FilterGroup: recordFilterDirectiveAPI.getData().filterObj
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeAccountconditionFiltergroup', FilterGroupAccountConditionDirective);

})(app);