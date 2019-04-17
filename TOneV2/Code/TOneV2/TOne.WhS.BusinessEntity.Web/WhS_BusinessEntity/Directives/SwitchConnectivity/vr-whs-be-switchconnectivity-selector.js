'use strict';

app.directive('vrWhsBeSwitchconnectivitySelector', ['WhS_BE_SwitchConnectivityAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SwitchConnectivityAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                isdisabled: "=",
                onselectionchanged: '=',
                isrequired: '=',
                selectedvalues: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.label = "Switch Connectivity";
                if ($attrs.ismultipleselection != undefined) {
                    ctrl.label = "Switch Connectivities";
                }

                var ctor = new switchCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="SwitchConnectivityId" isrequired="ctrl.isrequired" '
                + ' label="{{ctrl.label}}" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"   onselectionchanged="ctrl.onselectionchanged" vr-disabled="ctrl.isdisabled"></vr-select>'
                + '</vr-columns>';
        }

        function switchCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;

                        if (payload.fieldTitle != undefined) {
                            ctrl.label = payload.fieldTitle;
                        }
                    }

                    return WhS_BE_SwitchConnectivityAPIService.GetSwitcheConnectivitiesInfo().then(function (response) {
                        angular.forEach(response, function (item) {
                            ctrl.datasource.push(item);
                        });

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'SwitchConnectivityId', $attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SwitchConnectivityId', $attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);