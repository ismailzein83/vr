'use strict';
app.directive('npIvswitchEndpointSelector',['NP_IVSwitch_EndPointAPIService', 'NP_IVSwitch_EndPointService', 'VRNotificationService', 'NP_IVSwitch_EndPointEnum', 'UtilsService',
function (NP_IVSwitch_EndPointAPIService, NP_IVSwitch_EndPointService, VRNotificationService, NP_IVSwitch_EndPointEnum, UtilsService) {

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
                onitemadded: "="
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

            return  '<vr-select ' + multipleselection + '  datatextfield="Description" datavaluefield="EndPointId" isrequired="ctrl.isrequired"'
                    + ' label="' + label + '"  datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="'+label+'" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" haspermission="ctrl.haspermission"></vr-select>'
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
                    var filter;
                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }
                    return NP_IVSwitch_EndPointAPIService.GetEndPointsInfo(UtilsService.serializetoJson(filter)).then(function (response) {

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'EndPointId', attrs, ctrl);
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