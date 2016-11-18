(function (app) {

    'use strict';

    DataStoreConfigSelectorDirective.$inject = ['VR_GenericData_DataStoreAPIService', 'UtilsService', 'VRUIUtilsService'];

    function DataStoreConfigSelectorDirective(VR_GenericData_DataStoreAPIService, UtilsService, VRUIUtilsService) {
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

                var dataStoreConfigSelector = new DataStoreConfigSelector(ctrl, $scope, $attrs);
                dataStoreConfigSelector.initializeController();
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

        function DataStoreConfigSelector(ctrl, $scope, attrs) {
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
                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }

                    return VR_GenericData_DataStoreAPIService.GetDataStoreConfigs().then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }
                       
                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'ExtensionConfigurationId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ExtensionConfigurationId', attrs, ctrl);
                };

                return api;
            }
            //function onAddRuleDefinition() {
            //    var onRuleDefinitionAdded = function (ruleDefinitionObj) {
            //        ctrl.datasource.push(ruleDefinitionObj);
            //        if (attrs.ismultipleselection != undefined)
            //            ctrl.selectedvalues.push(ruleDefinitionObj);
            //        else
            //            ctrl.selectedvalues = ruleDefinitionObj;
            //    };
            //    VR_GenericData_GenericRuleDefinitionService.addGenericRuleDefinition(onRuleDefinitionAdded, specificTypeName);
            //}
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'Data Store Config';
            if (attrs.ismultipleselection != undefined) {
                label = 'Data Store Configs';
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
                   // + ' onaddclicked="ctrl.onAddRuleDefinition"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="ExtensionConfigurationId"'
                    + ' datatextfield="Title"'
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

    app.directive('vrGenericdataDatastoreconfigSelector', DataStoreConfigSelectorDirective);

})(app);