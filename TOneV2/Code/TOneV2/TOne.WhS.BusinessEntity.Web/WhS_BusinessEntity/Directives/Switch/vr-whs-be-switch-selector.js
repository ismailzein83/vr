'use strict';

app.directive('vrWhsBeSwitchSelector', ['WhS_BE_SwitchAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SwitchAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                isdisabled: "=",
                onselectionchanged: '=',
                isrequired: "=",
                selectedvalues: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];
                var ctor = new switchCtor(ctrl, $scope, $attrs);
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

            var multipleselection = "";
            var label = "Switch";
            if (attrs.ismultipleselection != undefined) {
                label = "Switches";
                multipleselection = "ismultipleselection";
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="SwitchId" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"   onselectionchanged="ctrl.onselectionchanged" vr-disabled="ctrl.isdisabled"></vr-select>'
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
                    var filter;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    return WhS_BE_SwitchAPIService.GetSwitchesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        angular.forEach(response, function (item) {
                            ctrl.datasource.push(item);
                        });

                        if (selectedIds != undefined)
                            VRUIUtilsService.setSelectedValues(selectedIds, 'SwitchId', $attrs, ctrl);
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SwitchId', $attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);