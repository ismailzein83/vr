'use strict';

app.directive('vrWhsDealDealdefinitionSelector', ['UtilsService', '$compile', 'VRUIUtilsService', 'WhS_Deal_DealDefinitionAPIService',
    function (UtilsService, $compile, VRUIUtilsService, WhS_Deal_DealDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                type: "=",
                onReady: '=',
                label: "@",
                ismultipleselection: "@",
                hideselectedvaluessection: '@',
                onselectionchanged: '=',
                isrequired: '=',
                isdisabled: "=",
                selectedvalues: "=",
                hidelabel: '@',
                normalColNum: '@',
                hideremoveicon: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];
                var ctor = new directiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
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
                return getTemplate(attrs);
            }

        };
        function getTemplate(attrs) {
            var label;
            if (attrs.hidelabel == undefined)
                label = attrs.ismultipleselection != undefined ? 'label="Deals"' : 'label="Deal"';

            var disabled = "";
            if (attrs.isdisabled)
                disabled = "vr-disabled='true'";

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";
            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            var hideselectedvaluessection = "";
            if (attrs.hideselectedvaluessection != undefined)
                hideselectedvaluessection = "hideselectedvaluessection";

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) 
                multipleselection = "ismultipleselection";

            return '<vr-columns colnum="{{ctrl.normalColNum}}" ' + disabled + '  > <vr-select ' + multipleselection + ' datasource="ctrl.datasource" isrequired="ctrl.isrequired" ' + hideselectedvaluessection + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" datatextfield="Name" datavaluefield="DealId"'
                   + 'entityname="Deal" ' + label + ' ' + hideremoveicon + '></vr-select> </vr-columns>';

        }
        function directiveCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('DealId', $attrs, ctrl);
                };

                api.load = function (payload) {
                    ctrl.datasource.length = 0;

                    var filter;
                    var selectedIds;
                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    if (filter == undefined)
                        filter = {};

                    var serializedFilter = {};
                    if (filter != undefined)
                        serializedFilter = UtilsService.serializetoJson(filter);

                    return WhS_Deal_DealDefinitionAPIService.GetDealDefinitionInfo(serializedFilter).then(function (response) {
                        angular.forEach(response, function (itm) {
                            ctrl.datasource.push(itm);
                        });
                        if (selectedIds != undefined)
                            VRUIUtilsService.setSelectedValues(selectedIds, 'DealId', $attrs, ctrl);

                    });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);

