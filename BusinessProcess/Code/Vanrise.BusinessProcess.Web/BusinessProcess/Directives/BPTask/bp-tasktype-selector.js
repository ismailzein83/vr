'use strict';
app.directive('bpTasktypeSelector', ['BusinessProcess_BPTaskTypeAPIService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPTaskTypeService',
    function (BusinessProcess_BPTaskTypeAPIService, UtilsService, VRUIUtilsService, BusinessProcess_BPTaskTypeService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                isdisabled: "=",
                onselectionchanged: '=',
                isrequired: "@",
                selectedvalues: '=',
                normalColNum: '@'

            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];


                $scope.addNewBPTaskType = function () {
                    var onTaskTypeAdded = function (taskTypeObj) {
                        ctrl.datasource.push(taskTypeObj);
                        if ($attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(taskTypeObj);
                        else
                            ctrl.selectedvalues = taskTypeObj;
                    };
                    BusinessProcess_BPTaskTypeService.addBPTaskType(onTaskTypeAdded);
                };

                var ctor = new taskTypeCtor(ctrl, $scope, $attrs);
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
            var label = "Task Type";
            if (attrs.ismultipleselection != undefined) {
                label = "Task Types";
                multipleselection = "ismultipleselection";
            }

            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewBPTaskType"';

            return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                + '<vr-select ' + multipleselection + ' ' + addCliked + ' datatextfield="Name" datavaluefield="BPTaskTypeId" '
                + required + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"   onselectionchanged="ctrl.onselectionchanged" on-ready="onSelectorReady"></vr-select>'
                + '</vr-columns>';
        }

        function taskTypeCtor(ctrl, $scope, $attrs) {
            var selectorAPI;
            function initializeController() {
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('BPTaskTypeId', $attrs, ctrl);
                };
                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var selectedIds;
                    var selectIfSingleItem;
                    var filter;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        selectIfSingleItem = payload.selectifsingleitem;
                        filter = payload.filter;
                    }
                    return BusinessProcess_BPTaskTypeAPIService.GetBPTaskTypesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        angular.forEach(response, function (item) {
                            ctrl.datasource.push(item);
                        });
                        if (selectedIds != undefined)
                            VRUIUtilsService.setSelectedValues(selectedIds, 'BPTaskTypeId', $attrs, ctrl);
                        else if (selectedIds == undefined && selectIfSingleItem)
                            selectorAPI.selectIfSingleItem();

                    });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);