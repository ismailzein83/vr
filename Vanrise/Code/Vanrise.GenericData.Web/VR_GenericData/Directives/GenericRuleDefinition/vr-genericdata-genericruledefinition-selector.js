(function (app) {

    'use strict';

    GenericRuleDefinitionSelectorDirective.$inject = ['VR_GenericData_GenericRuleDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function GenericRuleDefinitionSelectorDirective(VR_GenericData_GenericRuleDefinitionAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                isdisabled: "=",
                customlabel: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined) {
                    ctrl.selectedvalues = [];
                }

                var genericRuleDefinitionSelector = new GenericRuleDefinitionSelector(ctrl, $scope, $attrs);
                genericRuleDefinitionSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function GenericRuleDefinitionSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                }
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var filter;
                    var selectedIds;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinitionsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'GenericRuleDefinitionId', attrs, ctrl);
                        }
                    });
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('GenericRuleDefinitionId', attrs, ctrl);
                }

                return api;
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = "";

            var label = "Generic Rule Definition";
            if (attrs.ismultipleselection != undefined) {
                label = "Generic Rule Definitions";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="UserId" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="User" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>'
        }
    }

    app.directive('vrGenericdataGenericruledefinitionSelector', GenericRuleDefinitionSelectorDirective);

})(app);