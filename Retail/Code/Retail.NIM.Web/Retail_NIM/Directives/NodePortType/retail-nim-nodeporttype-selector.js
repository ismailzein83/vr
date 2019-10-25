"use strict";

app.directive("retailNimNodeporttypeSelector", ["Retail_NIM_NodePortTypeAPIService", "VRUIUtilsService",
    function (Retail_NIM_NodePortTypeAPIService, VRUIUtilsService) {
        "use strict";

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                hideremoveicon: '@',
                normalColNum: '@',
                isdisabled: '=',
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new NodePortTypeSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {
            var label = "Port Type";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Port Types";
                multipleselection = "ismultipleselection";
            }

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + '<vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + ' datatextfield="Name" datavaluefield="NodePortTypeId" isrequired="ctrl.isrequired" '
                + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Port Type" onselectitem="onselectitem" '
                + ' ondeselectitem = "ctrl.ondeselectitem"' + hideremoveicon + ' >'
                + '</vr-select>'
                + '</vr-columns>';
        }


        function NodePortTypeSelectorCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.hasSingleItem;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    var selectIfSingleItem;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        selectIfSingleItem = payload.selectifsingleitem;
                    }

                    return Retail_NIM_NodePortTypeAPIService.GetNodePortTypeInfo().then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'NodePortTypeId', attrs, ctrl);
                            } else if (selectedIds == undefined && selectIfSingleItem == true) {
                                selectorAPI.selectIfSingleItem();
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('NodePortTypeId', attrs, ctrl);
                };

                api.hasSingleItem = function () {
                    return ctrl.datasource.length == 1;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }]);