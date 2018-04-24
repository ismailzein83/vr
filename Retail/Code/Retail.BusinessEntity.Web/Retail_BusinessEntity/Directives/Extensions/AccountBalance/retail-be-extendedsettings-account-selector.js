'use strict';
app.directive('retailBeExtendedsettingsAccountSelector', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new accountsCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'accountCtrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {
            var multipleselection;
            if (attrs.ismultipleselection != undefined) {
                multipleselection = 'ismultipleselection="true"';
            }
            return '<retail-be-account-selector isrequired="accountCtrl.isrequired" normal-col-num="{{accountCtrl.normalColNum}}" ' + multipleselection + ' on-ready="accountCtrl.onDirectiveReady" ></retail-be-account-selector>';
        }

        function accountsCtor(ctrl, $scope, attrs) {

            var directiveReadyAPI;
            var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                ctrl.onDirectiveReady = function (api) {
                    directiveReadyAPI = api;
                    directiveReadyPromiseDeferred.resolve();
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var accountTypeId;
                    var extendedSettings;
                    var selectedIds;

                    if (payload != undefined) {
                        accountTypeId = payload.accountTypeId;
                        extendedSettings = payload.extendedSettings;
                        selectedIds = payload.selectedIds;
                    }

                    var promises = [];

                    var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(directiveLoadPromiseDeferred.promise);

                    directiveReadyPromiseDeferred.promise.then(function () {
                        var selectorPayload = {
                            filter: getAccountSelectorFilter(),
                            selectedIds: selectedIds
                        };
                        if (extendedSettings != undefined)
                            selectorPayload.AccountBEDefinitionId = extendedSettings.AccountBEDefinitionId;
                        VRUIUtilsService.callDirectiveLoad(directiveReadyAPI, selectorPayload, directiveLoadPromiseDeferred);
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = directiveReadyAPI.getSelectedIds();
                    return {
                        selectedIds: data,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getAccountSelectorFilter() {
                var filter = {};

                filter.Filters = [];

                var financialAccounts = {
                    $type: 'Retail.BusinessEntity.Business.AccountBalanceEnabledAccountFilter, Retail.BusinessEntity.Business',
                };
                filter.Filters.push(financialAccounts);
                return filter;
            }
            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);





'use strict';
app.directive('retailBeIcxOperatorswithlocalSelector', ['UtilsService', 'VRUIUtilsService', 'Retail_BE_ICX_OperatorsWithLocalAPIService',
function (UtilsService, VRUIUtilsService, Retail_BE_ICX_OperatorsWithLocalAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            onselectionchanged: '=',
            selectedvalues: '=',
            isrequired: "=",
            onselectitem: "=",
            ondeselectitem: "=",
            hideremoveicon: '@',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];

            ctrl.label = "Operator";
            if ($attrs.ismultipleselection != undefined) {
                ctrl.label = "Operators";
            }


            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var ctor = new operatorCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        template: function (element, attrs) {
            return getOperatorTemplate(attrs);
        }

    };

    function getOperatorTemplate(attrs) {

        var multipleselection = "";
        if (attrs.ismultipleselection != undefined) {
            multipleselection = "ismultipleselection";
        }

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns colnum="{{ctrl.normalColNum}}"    ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" ' + 'datavaluefield="AccountId" isrequired="ctrl.isrequired" label="{{ctrl.label}}" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" ' + 'onselectionchanged="ctrl.onselectionchanged" entityName="Operator" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '></vr-select></vr-columns>';
    }

    function operatorCtor(ctrl, $scope, attrs) {

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;

                if (payload != undefined) {

                    if (payload.fieldTitle != undefined) {
                        ctrl.label = payload.fieldTitle;
                    }

                    selectedIds = payload.selectedIds;
                    filter = payload.filter;
                }

                return getOperatorsWithLocalInfo(selectedIds);
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('AccountId', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getOperatorsWithLocalInfo(businessEntityDefinitionId, selectedIds) {
        return Retail_BE_ICX_OperatorsWithLocalAPIService.GetOperatorsWithLocalInfo(businessEntityDefinitionId).then(function (response)
        {
            angular.forEach(response, function (item) {
                ctrl.datasource.push(item);
            });
            if (selectedIds != undefined)
                VRUIUtilsService.setSelectedValues(selectedIds, 'AccountId', attrs, ctrl);
        });
    }

    return directiveDefinitionObject;
}]);