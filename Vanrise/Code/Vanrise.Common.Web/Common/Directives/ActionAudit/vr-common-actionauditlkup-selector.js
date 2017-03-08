'use strict';
app.directive('vrCommonActionauditlkupSelector', ['VRCommon_ActionAuditLKUPAPIService', 'UtilsService', 'VRUIUtilsService', function (VRCommon_ActionAuditLKUPAPIService, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            onselectionchanged: '=',
            selectedvalues: '=',
            isrequired: "=",
            onselectitem: "=",
            ondeselectitem: "=",
            hideremoveicon: '@',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var stor = new SelectorCtor(ctrl, $scope, $attrs);
            stor.initializeController();
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
            return getSelectorTemplate(attrs);
        }

    };

    function getSelectorTemplate(attrs) {

        var multipleselection = "";
        var label = "Action Type";
        if (attrs.ismultipleselection != undefined) {
            label = "Action Types";
            multipleselection = "ismultipleselection";
        }

        if (attrs.customlabel != undefined)
            label = attrs.customlabel;
        

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns colnum="{{ctrl.normalColNum}}"    ><vr-select on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="VRActionAuditLKUPId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="VRActionAuditLKUP" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + '></vr-select></vr-columns>';
    }

    function SelectorCtor(ctrl, $scope, attrs) {

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;

                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    filter = payload.filter;
                }

                return getSelectorInfo(attrs, ctrl, selectedIds, filter);
            };

            api.getSelectedIds = function () {
                var id = VRUIUtilsService.getIdSelectedIds('VRActionAuditLKUPId', attrs, ctrl);
                alert(id);
                return id;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getSelectorInfo(attrs, ctrl, selectedIds, filter) {
        return VRCommon_ActionAuditLKUPAPIService.GetVRActionAuditLKUPInfo(UtilsService.serializetoJson(filter)).then(function (response) {
            ctrl.datasource.length = 0;
            angular.forEach(response, function (itm) {
                
                ctrl.datasource.push(itm);
            });

            if (selectedIds != undefined) {
                VRUIUtilsService.setSelectedValues(selectedIds, 'VRActionAuditLKUPId', attrs, ctrl);

            }
        });
    }

    return directiveDefinitionObject;
}]);