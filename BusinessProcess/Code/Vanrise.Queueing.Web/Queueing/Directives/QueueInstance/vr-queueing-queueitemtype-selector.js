'use strict';
app.directive('vrQueueingQueueitemtypeSelector', ['VR_Queueing_QueueItemTypeAPIService', 'UtilsService', 'VRUIUtilsService',
    function (VR_Queueing_QueueItemTypeAPIService, UtilsService, VRUIUtilsService) {



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
                customlabel: "@"
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];




                var ctor = new executionFlowCtor(ctrl, $scope, $attrs);
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
                return getExecutionFlowTemplate(attrs);
            }

        };


        function getExecutionFlowTemplate(attrs) {

            var multipleselection = "";

            var label = "Item Type";
            if (attrs.ismultipleselection != undefined) {
                label = "Item Types";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewExecutionFlow"';

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Title" datavaluefield="Id" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Item Type" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>';
        }

        function executionFlowCtor(ctrl, $scope, attrs) {

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
                    var filter = {};
                    var selectedIds;

                    if (payload) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;


                    }

                    return VR_Queueing_QueueItemTypeAPIService.GetItemTypes(UtilsService.serializetoJson(filter)).then(function (response) {
                        ctrl.datasource.length = 0;

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'Id', attrs, ctrl);
                        }


                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('Id', attrs, ctrl);
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);