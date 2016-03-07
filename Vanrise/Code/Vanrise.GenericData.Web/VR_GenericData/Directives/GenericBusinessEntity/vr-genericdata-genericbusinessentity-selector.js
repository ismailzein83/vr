(function (app) {

    'use strict';

    GenericBusinessEntitySelector.$inject = ['VR_GenericData_GenericBusinessEntityAPIService', 'UtilsService', 'VRUIUtilsService'];

    function GenericBusinessEntitySelector(VR_GenericData_GenericBusinessEntityAPIService, UtilsService, VRUIUtilsService) {
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

                var businessentitySelector = new BusinessentitySelector(ctrl, $scope, $attrs);
                businessentitySelector.initializeController();
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

        function BusinessentitySelector(ctrl, $scope, attrs) {
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
                    var businessEntityId;
                    if (payload != undefined) {
                        ctrl.fieldTitle = payload.fieldTitle;
                        businessEntityId = payload.businessEntityId;
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityInfo(businessEntityId, UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'GenericBusinessEntityId', attrs, ctrl);
                        }
                    });
                }
                api.getIfSingleItem = function () {
                    if (ctrl.datasource.length == 1)
                        return ctrl.datasource[0];
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('GenericBusinessEntityId', attrs, ctrl);
                }

                return api;
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = '{{ctrl.fieldTitle}}';
            if (attrs.ismultipleselection != undefined) {
                label = '{{ctrl.fieldTitle}}s';
                multipleselection = 'ismultipleselection';
            }

            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : null;

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

            return '<vr-select on-ready="ctrl.onSelectorReady"'
                    + ' datasource="ctrl.datasource"'
                    + ' selectedvalues="ctrl.selectedvalues"'
                    + ' onselectionchanged="ctrl.onselectionchanged"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="GenericBusinessEntityId"'
                    + ' datatextfield="Name"'
                    + ' ' + multipleselection
                    + ' ' + hideselectedvaluessection
                    + ' isrequired="ctrl.isrequired"'
                    + ' ' + hideremoveicon
                    + ' label="' + label + '"'
                    + ' entityName="' + label + '"'
                + '</vr-select>';
        }
    }

    app.directive('vrGenericdataGenericbusinessentitySelector', GenericBusinessEntitySelector);

})(app);