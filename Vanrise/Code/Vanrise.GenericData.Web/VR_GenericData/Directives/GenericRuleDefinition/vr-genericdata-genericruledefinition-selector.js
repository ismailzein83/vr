(function (app) {

    'use strict';

    GenericRuleDefinitionSelectorDirective.$inject = ['VR_GenericData_GenericRuleDefinitionAPIService', 'UtilsService', 'VRUIUtilsService','VR_GenericData_GenericRuleDefinitionService'];

    function GenericRuleDefinitionSelectorDirective(VR_GenericData_GenericRuleDefinitionAPIService, UtilsService, VRUIUtilsService, VR_GenericData_GenericRuleDefinitionService) {
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
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

                var genericRuleDefinitionSelector = new GenericRuleDefinitionSelector(ctrl, $scope, $attrs);
                genericRuleDefinitionSelector.initializeController();
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

        function GenericRuleDefinitionSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var specificTypeName;
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
                    var selectfirstitem;

                    if (payload != undefined) {
                        if (payload.showaddbutton)
                            ctrl.onAddRuleDefinition = onAddRuleDefinition;
                        specificTypeName = payload.specificTypeName;
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                        selectfirstitem = payload.selectfirstitem != undefined && payload.selectfirstitem == true;
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
                        else if(selectfirstitem)
                        {
                            var defaultValue = attrs.ismultipleselection != undefined ? [ctrl.datasource[0].GenericRuleDefinitionId] : ctrl.datasource[0].GenericRuleDefinitionId;
                            VRUIUtilsService.setSelectedValues(defaultValue, 'GenericRuleDefinitionId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('GenericRuleDefinitionId', attrs, ctrl);
                };
                api.hasSingleItem = function () {
                    return ctrl.datasource.length == 1;
                };
                api.getDatasource = function () {
                    return ctrl.datasource;
                };

                return api;
            }
            function onAddRuleDefinition () {
                var onRuleDefinitionAdded = function (ruleDefinitionObj) {
                    ctrl.datasource.push(ruleDefinitionObj);
                    if (attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(ruleDefinitionObj);
                    else
                        ctrl.selectedvalues = ruleDefinitionObj;
                };
                VR_GenericData_GenericRuleDefinitionService.addGenericRuleDefinition(onRuleDefinitionAdded, specificTypeName);
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'Generic Rule Definition';
            if (attrs.ismultipleselection != undefined) {
                label = 'Generic Rule Definitions';
                multipleselection = 'ismultipleselection';
            }
            var datatextfield = "Name";
            if (attrs.showtitle != undefined)
                datatextfield ="Title";
            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }
            //var addCliked = '';
            //if (attrs.onaddclicked != undefined)
            //    addCliked = ' ';
            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : null;

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

            return '<div>'
                + '<vr-select on-ready="ctrl.onSelectorReady"'
                    + ' datasource="ctrl.datasource"'
                    + ' selectedvalues="ctrl.selectedvalues"'
                    + ' onselectionchanged="ctrl.onselectionchanged"'
                    + 'onaddclicked="ctrl.onAddRuleDefinition"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="GenericRuleDefinitionId"'
                    + ' datatextfield="'+ datatextfield + '"'
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

    app.directive('vrGenericdataGenericruledefinitionSelector', GenericRuleDefinitionSelectorDirective);

})(app);