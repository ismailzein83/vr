
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
                var businessEntityDefinitionId;

                if (payload != undefined) {

                    if (payload.fieldTitle != undefined) {
                        ctrl.label = payload.fieldTitle;
                    }

                    businessEntityDefinitionId = payload.businessEntityDefinitionId;

                    selectedIds = payload.selectedIds;
                    filter = payload.filter;
                }

                return getOperatorsWithLocalInfo(attrs, ctrl, businessEntityDefinitionId, selectedIds);
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('AccountId', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getOperatorsWithLocalInfo(attrs, ctrl, businessEntityDefinitionId, selectedIds) {
        return Retail_BE_ICX_OperatorsWithLocalAPIService.GetOperatorsWithLocalInfo(businessEntityDefinitionId).then(function (response) {
            angular.forEach(response, function (item) {
                ctrl.datasource.push(item);
            });
            if (selectedIds != undefined)
                VRUIUtilsService.setSelectedValues(selectedIds, 'AccountId', attrs, ctrl);
        });
    }

    return directiveDefinitionObject;
}]);