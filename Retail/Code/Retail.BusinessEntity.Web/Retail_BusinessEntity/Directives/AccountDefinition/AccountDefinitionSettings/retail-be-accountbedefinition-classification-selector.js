'use strict';

app.directive('retailBeAccountbedefinitionClassificationSelector', ['UtilsService', 'VRUIUtilsService', 'Retail_BE_AccountBEDefinitionAPIService',

function (UtilsService, VRUIUtilsService, Retail_BE_AccountBEDefinitionAPIService) {
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
            normalColNum: '@',
            customvalidate: '='
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var classificationSelector = new ClassificationSelector(ctrl, $scope, $attrs);
            classificationSelector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function ClassificationSelector(ctrl, $scope, attrs) {

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

                var promises = [];
                var selectedIds;
                var filter;
                
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    filter = payload.AccountBEDefinitionId;
                }

                if (filter != undefined) {
                    return Retail_BE_AccountBEDefinitionAPIService.GetAccountBEDefinitionClassificationsInfo(filter).then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'Name', attrs, ctrl);
                            }
                        }
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('Name', attrs, ctrl);
            };


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getTemplate(attrs) {

        var multipleselection = "";
        var label = "Classification";
        var hideremoveicon = '';

        if (attrs.ismultipleselection != undefined) {
            label = "Classifications";
            multipleselection = "ismultipleselection";
        }
        if (attrs.customlabel != undefined)
            label = attrs.customlabel;
        var hidelabel = "";
        if (attrs.hidelabel != undefined)
            hidelabel = "hidelabel";

        if (attrs.hideremoveicon != undefined)
            hideremoveicon = 'hideremoveicon';

        return '<vr-columns colnum="{{ctrl.normalColNum}}"    ><vr-select ' + multipleselection + ' datatextfield="Title" datavaluefield="Name" isrequired="ctrl.isrequired" label="' + label +
                   '" datasource="ctrl.datasource"  ' + hidelabel + '  on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label +
                   '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
               '</vr-select></vr-columns>';
    }

}]);