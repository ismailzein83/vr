(function (app) {

    'use strict';

    BusinessEntityDefinitionSelectorDirective.$inject = ['VR_GenericData_BusinessEntityDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function BusinessEntityDefinitionSelectorDirective(VR_GenericData_BusinessEntityDefinitionAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectitem: "=",
                ondeselectitem: "=",
                onselectionchanged: '=',
                ismultipleselection: "@",
                isrequired: "=",
                customlabel: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

                var businessEntityDefinitionSelector = new BusinessEntityDefinitionSelector(ctrl, $scope, $attrs);
                businessEntityDefinitionSelector.initializeController();
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
                return getDirectiveTemplate(attrs);
            }
        };

        function BusinessEntityDefinitionSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {

                    var filter;
                    var selectedIds;
                    var selectFirstItem;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                        selectFirstItem = payload.selectFirstItem != undefined && payload.selectFirstItem == true;
                    }

                    return VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinitionsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'BusinessEntityDefinitionId', attrs, ctrl);
                        }
                        else if (selectFirstItem == true) {
                            var defaultValue = attrs.ismultipleselection != undefined ? [ctrl.datasource[0].Id] : ctrl.datasource[0].Id;
                            VRUIUtilsService.setSelectedValues(defaultValue, 'Id', attrs, ctrl);
                        }

                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('BusinessEntityDefinitionId', attrs, ctrl);
                };

                api.hasSingleItem = function () {
                    return ctrl.datasource.length == 1;
                };

                return api;
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'Business Entity Definition';
            if (attrs.ismultipleselection != undefined) {
                label = 'Business Entity Definitions';
                multipleselection = 'ismultipleselection';
            }

            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : null;

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

            return '<div>'
                + '<vr-select on-ready="ctrl.onSelectorReady"'
                    + ' datasource="ctrl.datasource"'
                    + ' selectedvalues="ctrl.selectedvalues"'
                    + ' onselectionchanged="ctrl.onselectionchanged"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="BusinessEntityDefinitionId"'
                    + ' datatextfield="Name"'
                    + ' ' + multipleselection
                    + ' ' + hideselectedvaluessection
                    + ' isrequired="ctrl.isrequired"'
                    + ' ' + hideremoveicon
                    + ' label="' + label + '"'
                    + ' entityName="' + label + '"'
                + '</vr-select>'
            + '</div>';
        }
    }

    app.directive('vrGenericdataBusinessentitydefinitionSelector', BusinessEntityDefinitionSelectorDirective);

})(app);