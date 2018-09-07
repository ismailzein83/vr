'use strict';
app.directive('npIvswitchRouteSelector', ['NP_IVSwitch_RouteAPIService', 'NP_IVSwitch_RouteService', 'VRNotificationService',  'UtilsService','VRUIUtilsService',
function (NP_IVSwitch_RouteAPIService, NP_IVSwitch_RouteService, VRNotificationService, UtilsService, VRUIUtilsService) {

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
            customlabel: "@",
            onitemadded: "=",
            customvalidate: "="
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var ctor = new RouteCtor(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getUserTemplate(attrs);
        }
    };


    function getUserTemplate(attrs) {

        var multipleselection = "";

        var label = "Route";
        if (attrs.ismultipleselection != undefined) {
            label = "Routes";
            multipleselection = "ismultipleselection";
        }

        if (attrs.customlabel != undefined)
            label = attrs.customlabel;

        var hidelabel = "";
        if (attrs.hidelabel != undefined)
            hidelabel = "hidelabel";

        return '<span vr-disabled="ctrl.isdisabled"><vr-select ' + multipleselection + ' customvalidate="ctrl.customvalidate" datatextfield="Description" datavaluefield="RouteId" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" ' + hidelabel + ' datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues "onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" haspermission="ctrl.haspermission"></vr-select></span>'
    }

    function RouteCtor(ctrl, $scope, attrs) {

        var selectorApi;

        function initializeController() {
            ctrl.onSelectorReady = function (api) {
                selectorApi = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                selectorApi.clearDataSource();
                var selectedIds;
                var selectedAllIds = [];
                var filter;
                var selectAll;
                if (payload != undefined) {
                    filter = payload.filter;
                    selectedIds = payload.selectedIds;
                    selectAll = payload.selectAll;
                }
                return NP_IVSwitch_RouteAPIService.GetRoutesInfo(UtilsService.serializetoJson(filter)).then(function (response) {

                    if (response) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                            if (selectAll == true) {
                                selectedAllIds.push(response[i].RouteId);
                            }
                        }
                    }

                    if (selectedIds) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'RouteId', attrs, ctrl);
                    }
                    if (selectedAllIds) {
                        VRUIUtilsService.setSelectedValues(selectedAllIds, 'RouteId', attrs, ctrl);
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('RouteId', attrs, ctrl);
            };
            api.clearDataSource=function()
            {
                selectorApi.clearDataSource();

            }
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);