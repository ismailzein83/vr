"use strict";

app.directive("vrGenericFinancialaccountSelector", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "=",
                ismultipleselection: "@",
                isrequired: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new genericFinancialAccountSelectorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };
        function genericFinancialAccountSelectorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectedIds;

            var genericFinancialAccountSelectorApi;
            var genericFinancialAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGenericFinancialAccountSelectorReady = function (api) {
                    genericFinancialAccountSelectorApi = api;
                    genericFinancialAccountPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    promises.push(loadGenericFinancialAccountSelector());

                    function loadGenericFinancialAccountSelector() {
                        var genericFinancialAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        genericFinancialAccountPromiseDeferred.promise.then(function () {

                            var genericFinancialAccountSelectorPayload = {
                                businessEntityDefinitionId: payload.businessEntityDefinitionId,
                                selectedIds: payload.selectedIds
                            };
                            VRUIUtilsService.callDirectiveLoad(genericFinancialAccountSelectorApi, genericFinancialAccountSelectorPayload, genericFinancialAccountSelectorLoadDeferred);
                        });
                        return genericFinancialAccountSelectorLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getSelectedIds = function () {
                    return { selectedIds: genericFinancialAccountSelectorApi.getSelectedIds() };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };
        }

        function getTemplate(attrs) {
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }

            return '<vr-genericdata-genericbusinessentity-selector normal-col-num = "{{ctrl.normalColNum}}"   ' + multipleselection + ' isrequired="ctrl.isrequired" '
                + ' on-ready="scopeModel.onGenericFinancialAccountSelectorReady" customlabel="Financial Accounts" ></vr-genericdata-genericbusinessentity-selector> ';
        }

        return directiveDefinitionObject;
    }
]);