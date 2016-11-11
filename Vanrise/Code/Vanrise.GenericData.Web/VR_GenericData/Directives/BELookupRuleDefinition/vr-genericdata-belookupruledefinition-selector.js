(function (app) {

    'use strict';

    BELookupRuleDefinitionSelectorDirective.$inject = ['VR_GenericData_BELookupRuleDefinitionAPIService', 'VR_GenericData_BELookupRuleDefinitionService', 'UtilsService', 'VRUIUtilsService'];

    function BELookupRuleDefinitionSelectorDirective(VR_GenericData_BELookupRuleDefinitionAPIService, VR_GenericData_BELookupRuleDefinitionService, UtilsService, VRUIUtilsService)
    {
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
                isdisabled: "=",
                customlabel: "@",
            },
            controller: function ($scope, $element, $attrs)
            {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

                var beLookupRuleDefinitionSelector = new BELookupRuleDefinitionSelector(ctrl, $scope, $attrs);
                beLookupRuleDefinitionSelector.initializeController();
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

        function BELookupRuleDefinitionSelector(ctrl, $scope, attrs)
        {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController()
            {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI()
            {
                var api = {};

                api.load = function (payload)
                {
                    var selectedIds;
                    var filter = null;

                    if (payload != undefined) {
                        filter = {};
                        selectedIds = payload.selectedIds;
                    }

                    return VR_GenericData_BELookupRuleDefinitionAPIService.GetBELookupRuleDefinitionsInfo(UtilsService.serializetoJson(filter)).then(function (response)
                    {
                        selectorAPI.clearDataSource();

                        if (response != null)
                        {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'BELookupRuleDefinitionId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('BELookupRuleDefinitionId', attrs, ctrl);
                };

                api.clearDataSource = function () {
                    selectorAPI.clearDataSource();
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'Lookup Rule';
            if (attrs.ismultipleselection != undefined) {
                label = 'Lookup Rules';
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
                    + ' datavaluefield="BELookupRuleDefinitionId"'
                    + ' datatextfield="Name"'
                    + ' ' + multipleselection
                    + ' ' + hideselectedvaluessection
                    + ' isrequired="ctrl.isrequired"'
                    + ' ' + hideremoveicon
                    + ' vr-disabled="ctrl.isdisabled"'
                    + ' label="' + label + '"'
                    + ' entityName="' + label + '"'
                + '</vr-select>'
            + '</div>';
        }
    }

    app.directive('vrGenericdataBelookupruledefinitionSelector', BELookupRuleDefinitionSelectorDirective);

})(app);