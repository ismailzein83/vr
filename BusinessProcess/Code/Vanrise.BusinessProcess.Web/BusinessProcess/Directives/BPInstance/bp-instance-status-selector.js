'use strict';
app.directive('businessprocessBpInstanceStatusSelector', ['BusinessProcess_BPDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'BPInstanceStatusEnum',
function (BusinessProcess_BPDefinitionAPIService, UtilsService, VRUIUtilsService, BPInstanceStatusEnum) {

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
            isdisabled: "=",
            showaddbutton: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var ctor = new bpInstanceStatusCtor(ctrl, $scope, $attrs);
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
            return getBPInstanceStatusTemplate(attrs);
        }

    };


    function getBPInstanceStatusTemplate(attrs) {

        var multipleselection = "";
        var label = "Status";
        if (attrs.ismultipleselection != undefined) {
            multipleselection = "ismultipleselection";
        }


        var addCliked = '';


        return '<div>'
            + '<vr-select ' + multipleselection + '  datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Status" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
            + '</div>';
    }

    function bpInstanceStatusCtor(ctrl, $scope, attrs) {

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var selectedIds;
                var serializedFilter = {};
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    if (payload.filter != undefined) {
                        serializedFilter = UtilsService.serializetoJson(payload.filter);
                    }
                }
                return getBPInstanceStatusInfo(attrs, ctrl, selectedIds, serializedFilter);
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getBPInstanceStatusInfo(attrs, ctrl, selectedIds, serializedFilter) {
        var response = UtilsService.getArrayEnum(BPInstanceStatusEnum);

        ctrl.datasource.length = 0;
        angular.forEach(response, function (itm) {
            ctrl.datasource.push(itm);
        });

        if (selectedIds != undefined) {
            VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
        }

    }
    return directiveDefinitionObject;
}]);