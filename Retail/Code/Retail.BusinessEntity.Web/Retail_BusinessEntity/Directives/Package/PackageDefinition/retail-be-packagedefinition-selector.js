'use strict';

app.directive('retailBePackagedefinitionSelector', ['Retail_BE_PackageDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', function (Retail_BE_PackageDefinitionAPIService, UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: '@',
            selectedvalues: '=',
            onselectionchanged: '=',
            onselectitem: '=',
            ondeselectitem: '=',
            isrequired: '=',
            hideremoveicon: '@',
            normalColNum: '@',
            customvalidate: '='
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var accountPartDefinitionSelector = new PackageDefinitionSelector(ctrl, $scope, $attrs);
            accountPartDefinitionSelector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function PackageDefinitionSelector(ctrl, $scope, attrs) {

        this.initializeController = initializeController;

        var selectorAPI;

        function initializeController() {
            ctrl.onSelectorReady = function (api) {
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
                var accountBEDefinitionId;
                var includeHiddenPackageDefinitions;

                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    filter = payload.filter;
                    accountBEDefinitionId = payload.accountBEDefinitionId;
                    includeHiddenPackageDefinitions = payload.includeHiddenPackageDefinitions;
                }
                if (filter == undefined)
                    filter = {};
                filter.AccountBEDefinitionId = accountBEDefinitionId;
                filter.IncludeHiddenPackageDefinitions = includeHiddenPackageDefinitions;

                return Retail_BE_PackageDefinitionAPIService.GetPackageDefinitionsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'PackageDefinitionId', attrs, ctrl);
                        }
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('PackageDefinitionId', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getTemplate(attrs) {

        var multipleselection = "";
        var label = "Package Definition";

        if (attrs.ismultipleselection != undefined) {
            label = "Package Definitions";
            multipleselection = "ismultipleselection";
        }
        if (attrs.customlabel != undefined)
            label = attrs.customlabel;

        return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="PackageDefinitionId" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate"></vr-select></vr-columns>';
    }

}]);