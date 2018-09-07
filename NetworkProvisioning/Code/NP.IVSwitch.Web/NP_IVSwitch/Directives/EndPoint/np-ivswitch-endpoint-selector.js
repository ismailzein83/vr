'use strict';
app.directive('npIvswitchEndpointSelector', ['NP_IVSwitch_EndPointAPIService', 'NP_IVSwitch_EndPointService', 'VRNotificationService', 'VRUIUtilsService', 'UtilsService',
function (NP_IVSwitch_EndPointAPIService, NP_IVSwitch_EndPointService, VRNotificationService, VRUIUtilsService, UtilsService) {

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
            customvalidate:"="
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var ctor = new endPointCtor(ctrl, $scope, $attrs);
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

        var label = "End Point";
        if (attrs.ismultipleselection != undefined) {
            label = "End Points";
            multipleselection = "ismultipleselection";
        }

        if (attrs.customlabel != undefined)
            label = attrs.customlabel;

        return '<span vr-disabled="ctrl.isdisabled"><vr-select ' + multipleselection + '  datatextfield="Description" datavaluefield="EndPointId" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate"'
                + ' label="' + label + '"  datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" haspermission="ctrl.haspermission"></vr-select></span>'
    }

    function endPointCtor(ctrl, $scope, attrs) {

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

               return NP_IVSwitch_EndPointAPIService.GetEndPointsInfo(UtilsService.serializetoJson(filter)).then(function (response) {

                    if (response) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                            if (selectAll == true) {
                                selectedAllIds.push(response[i].EndPointId);
                            }
                        }

                    }

                    if (selectedIds) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'EndPointId', attrs, ctrl);
                    }
                    if (selectedAllIds) {
                        VRUIUtilsService.setSelectedValues(selectedAllIds, 'EndPointId', attrs, ctrl);
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('EndPointId', attrs, ctrl);
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);