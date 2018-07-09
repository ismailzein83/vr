(function (app) {

    'use strict';

    LKUPBusinessEntitySelector.$inject = ['VR_GenericData_LKUPBEDefinitionAPIService', 'VR_GenericData_LKUPBusinessEntityAPIService', 'UtilsService', 'VRUIUtilsService'];

    function LKUPBusinessEntitySelector(VR_GenericData_LKUPBEDefinitionAPIService, VR_GenericData_LKUPBusinessEntityAPIService, UtilsService, VRUIUtilsService) {
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
                customlabel: "@",
                normalColNum: '@'
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
                };
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
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var filter;
                    var selectedIds;
                    var businessEntityDefinitionId;
                    var promises = [];

                    if (payload != undefined) {
                        ctrl.fieldTitle = payload.fieldTitle;
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;

                        var getLKUPBERuntimeInfoPromise = getLKUPBusinessEntityRuntimeInfo();
                        promises.push(getLKUPBERuntimeInfoPromise);

                        function getLKUPBusinessEntityRuntimeInfo() {
                            return VR_GenericData_LKUPBEDefinitionAPIService.GetLookUpBESelectorRuntimeInfo(businessEntityDefinitionId).then(function (response) {

                                if (response != undefined) {

                                    if (attrs.ismultipleselection != undefined) {
                                        if (response.SelectorPluralTitle != undefined) ctrl.fieldTitle = response.SelectorPluralTitle;
                                    } else {
                                        if (response.SelectorSingularTitle != undefined) ctrl.fieldTitle = response.SelectorSingularTitle;
                                    }
                                }

                            });

                        }
                    }

                    var getLKUPBusinessEntityInfoPromise = GetLKUPBusinessEntityInfo();
                    promises.push(getLKUPBusinessEntityInfoPromise);

                    function GetLKUPBusinessEntityInfo() {
                        return VR_GenericData_LKUPBusinessEntityAPIService.GetLKUPBusinessEntityInfo(businessEntityDefinitionId, UtilsService.serializetoJson(filter)).then(function (response) {

                            selectorAPI.clearDataSource();
                            if (response) {
                                for (var i = 0; i < response.length; i++) {
                                    ctrl.datasource.push(response[i]);
                                }
                            }

                            if (selectedIds) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'LKUPBusinessEntityId', attrs, ctrl);
                            }
                        });

                    }

                    return UtilsService.waitMultiplePromises(promises);

                };



                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('LKUPBusinessEntityId', attrs, ctrl);
                };

                return api;
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = '{{ctrl.fieldTitle}}';
            if (attrs.ismultipleselection != undefined) {
                label = '{{ctrl.fieldTitle}}';
                multipleselection = 'ismultipleselection';
            }

            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : null;

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                    + '<vr-label>' + label + '</vr-label>'
                    + '<vr-select on-ready="ctrl.onSelectorReady"'
                    + ' datasource="ctrl.datasource"'
                    + ' selectedvalues="ctrl.selectedvalues"'
                    + ' onselectionchanged="ctrl.onselectionchanged"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="LKUPBusinessEntityId"'
                    + ' datatextfield="Name"'
                    + ' ' + multipleselection
                    + ' ' + hideselectedvaluessection
                    + ' isrequired="ctrl.isrequired"'
                    + ' ' + hideremoveicon
                + '</vr-select></vr-columns>';
        }
    }

    app.directive('vrGenericdataLookupbusinessentitySelector', LKUPBusinessEntitySelector);

})(app);