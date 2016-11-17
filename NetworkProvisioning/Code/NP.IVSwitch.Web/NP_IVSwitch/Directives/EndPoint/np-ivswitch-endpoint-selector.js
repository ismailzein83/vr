'use strict';

app.directive('npIvswitchEndpointSelector', ['NP_IVSwitch_EndPointAPIService', 'UtilsService', 'VRUIUtilsService', 'NP_IVSwitch_EndPointEnum',

    function (NP_IVSwitch_EndPointAPIService, UtilsService, VRUIUtilsService, NP_IVSwitch_EndPointEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var endPointSelector = new EndPointSelector(ctrl, $scope, $attrs);
                endPointSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function EndPointSelector(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
 
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var filter;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }
 

                    var EnumArray = UtilsService.getArrayEnum(NP_IVSwitch_EndPointEnum);
                    selectorAPI.clearDataSource();
                    if (EnumArray != null) {
                        ctrl.datasource = EnumArray;

                    }
                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    }
                
         


        };

        api.getSelectedIds = function () {
            return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
        };


        if (ctrl.onReady != null)
            ctrl.onReady(api);
    }
}

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "EndPoint Type";

            if (attrs.ismultipleselection != undefined) {
                label = "EndPoint Type";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                   '<vr-select ' + multipleselection + ' datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" label="' + label +
                       '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label +
                       '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate">' +
                   '</vr-select>' +
                   '</vr-columns>';
        }

}]);

    