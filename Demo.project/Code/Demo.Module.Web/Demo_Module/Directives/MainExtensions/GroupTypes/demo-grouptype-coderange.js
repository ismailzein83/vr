"use strict";

app.directive("demoGrouptypeCoderange", ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.range = [];
            var groupTypeCodeRange = new GroupTypeCodeRange(ctrl, $scope);
            groupTypeCodeRange.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/MainExtensions/GroupTypes/Templates/GroupTypeCodeRange.html"
    };

    function GroupTypeCodeRange(ctrl, $scope) {
        this.initCtrl = initCtrl;

        function initCtrl() {
            defineAPI();
            function contains(a, obj) {
                for (var i = 0; i < a.length; i++) {
                    if (a[i].range === obj) {
                        return true;
                    }
                }
                return false;
            }
            ctrl.disabledrange = true;
            ctrl.onRangeValueChange = function (value) {
                ctrl.disabledrange = (contains(ctrl.range, value) || value == undefined);
            }

            ctrl.addRangeOption = function () {
                var range = ctrl.rangevalue;
                ctrl.range.push({
                    range: range
                });
                ctrl.rangevalue = undefined;
                ctrl.disabledrange = true;
            };

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    ctrl.range = [];
                    if (payload != undefined && payload.selectedIds != undefined)
                        for (var j = 0; j < payload.selectedIds.length; j++) {
                            ctrl.range.push({
                                range: payload.selectedIds[j]
                            });
                        }
                };


                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.GroupTypeCodeRange, Demo.Module.MainExtension",
                        SelectedIds: UtilsService.getPropValuesFromArray(ctrl.range, "range")
                    };
                };

                if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }
    }
}]);
