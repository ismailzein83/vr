'use strict';
app.directive('vrRuntimeSchedulertaskSelector', ['SchedulerTaskAPIService', 'UtilsService', '$compile', 'VRUIUtilsService',
function (SchedulerTaskAPIService, UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            isdisabled: "=",
            onselectionchanged: '=',
            isrequired: "@",
            selectedvalues: '='

        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            ctrl.datasource = [];
            var ctor = new scheduleCtor(ctrl, $scope, $attrs);
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
        if (attrs.ismultipleselection != undefined)
            multipleselection = "ismultipleselection";
        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";
        return '<div  vr-loader="isLoadingDirective">'
            + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="TaskId" '
            + required + ' label="Schedule" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" vr-disabled="ctrl.isdisabled"></vr-select>'
            + '</div>';
    }

    function scheduleCtor(ctrl, $scope, $attrs) {

        function initializeController() {
            defineAPI();
        }

        function fillDataSource(response, selectedIds) {
            angular.forEach(response, function (item) {
                ctrl.datasource.push(item);

            });

            if ($attrs.ismultipleselection == undefined) {
                ctrl.selectedvalues = response[0];
            }

            if (selectedIds != undefined)
                VRUIUtilsService.setSelectedValues(selectedIds, 'TaskId', $attrs, ctrl);
        }
        function defineAPI() {
            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('TaskId', $attrs, ctrl);
            };
            api.load = function (payload) {

                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                var isMySchedule;
                if (payload != undefined) {
                    isMySchedule = payload.isMySchedule;
                }

                if (isMySchedule == true) {
                    return SchedulerTaskAPIService.GetMySchedulesInfo().then(function (response) {
                        fillDataSource(response, selectedIds);
                    });
                }
                else {
                    return SchedulerTaskAPIService.GetSchedulesInfo().then(function (response) {
                        fillDataSource(response, selectedIds);
                    });
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);

        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);