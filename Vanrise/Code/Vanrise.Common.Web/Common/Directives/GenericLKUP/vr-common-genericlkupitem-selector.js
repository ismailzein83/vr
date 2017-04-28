(function (app) {

    'use strict';

    GenericLKUPItemSelectorDirective.$inject = ['VR_Common_GnericLKUPAPIService', 'UtilsService', 'VRUIUtilsService'];

    function GenericLKUPItemSelectorDirective(VR_Common_GnericLKUPAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                ismultipleselection: '@',
                isdisabled: '@',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined && $attrs.ismultipleselection != null)
                    ctrl.selectedvalues = [];

                //ctrl.isrequired = ($attrs.isrequired != undefined);

                var genericLKUPItemSelector = new GenericLKUPItemSelector(ctrl, $scope, $attrs);
                genericLKUPItemSelector.initializeController();
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

        function GenericLKUPItemSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.fieldTitle = "GenericLKUP Item";
                if (attrs.ismultipleselection != undefined) {
                    ctrl.fieldTitle = "GenericLKUP Items";
                }
                if (attrs.customlabel != undefined) {
                    ctrl.fieldTitle = attrs.customlabel;
                }
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var directiveAPI = {};

                directiveAPI.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var selectedIds;
                    var filter;
                    if (payload) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        if (payload.businessEntityDefinitionId != undefined)
                        {
                            if (filter == undefined)
                                filter = {};
                            filter.BusinessEntityDefinitionId = payload.businessEntityDefinitionId;
                        }
                        if (payload.fieldTitle != undefined)
                            ctrl.fieldTitle = payload.fieldTitle;
                    }
                    ctrl.datasource.length = 0;
                    return VR_Common_GnericLKUPAPIService.GetGenericLKUPItemsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'GenericLKUPItemId', attrs, ctrl);
                            }
                        }
                    });
                };

                directiveAPI.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('GenericLKUPItemId', attrs, ctrl);
                };

                return directiveAPI;
            }
        }

        function getDirectiveTemplate(attrs) {

            var ismultipleselection = "";
            var label = "GenericLKUP Item";

            if (attrs.ismultipleselection != undefined) {
                label = "GenericLKUP Items";
                ismultipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                   + '<vr-label>{{ctrl.fieldTitle}}</vr-label>'
                + '<vr-select on-ready="ctrl.onSelectorReady"'
                    + ' datasource="ctrl.datasource"'

                    + ' selectedvalues="ctrl.selectedvalues"'
                    + ' onselectionchanged="ctrl.onselectionchanged"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="GenericLKUPItemId"'
                    + ' datatextfield="Name"'
                    + ismultipleselection
                    + ' vr-disabled="ctrl.isdisabled"'
                    + ' isrequired="ctrl.isrequired">'

                + '</vr-select>'
            + '</vr-columns>';
        }

        return directiveDefinitionObject;
    }

    app.directive('vrCommonGenericlkupitemSelector', GenericLKUPItemSelectorDirective);

})(app);
