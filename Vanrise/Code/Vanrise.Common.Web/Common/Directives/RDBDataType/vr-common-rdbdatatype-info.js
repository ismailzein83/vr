
appControllers.directive('vrCommonRdbdatatypeInfo', ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRCommon_RDBDataTypeAPIService',
    function (VRNotificationService, UtilsService, VRUIUtilsService, VRCommon_RDBDataTypeAPIService) {
        'use strict';

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
                hideremoveicon: '@',
                normalColNum: '@',
                isdisabled: '=',
                label: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var typeSelector = new TypeSelector(ctrl, $scope, $attrs);
                typeSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {

                return rdbDataTypeInfoTemplate(attrs);
            }
        };

        function rdbDataTypeInfoTemplate(attrs) {


            var multipleselection = "";
            var label = attrs.label;
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="scopeModel.onSelectorReady" onselectionchanged="scopeModel.onTypeChanged" datatextfield="Description" datavaluefield="Value" label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"  onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' isrequired="ctrl.isrequired"></vr-select></vr-columns>' +
                '<vr-columns colnum="{{ctrl.normalColNum/2}}" ng-if="ctrl.selectedvalues.RequireSize"><vr-textbox value="scopeModel.size" label="Size" type="number" isrequired="ctrl.selectedvalues.HasSizeRequired"></vr-textbox></vr-columns>' +
                '<vr-columns colnum="{{ctrl.normalColNum/2}}" ng-if="ctrl.selectedvalues.RequirePrecision"><vr-textbox value="scopeModel.precision" label="Precision" type="number" isrequired="ctrl.selectedvalues.HasPrecisionRequired"></vr-textbox></vr-columns>';

        }


        function TypeSelector(ctrl, $scope, attrs) {

            $scope.scopeModel = {};
            var selectorAPI;
            var typeSelectedPromise;
            function initializeController() {


                $scope.scopeModel = {};

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onTypeChanged = function (value) {
                    if (selectorAPI) {
                        if (value) {
                            if (typeSelectedPromise != undefined) {
                                typeSelectedPromise=undefined;
                            }
                            else {
                                $scope.scopeModel.size = undefined;
                                $scope.scopeModel.precision = undefined;
                            }
                        }
                    }
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) { //payload is an object that has selectedids and filter
                    selectorAPI.clearDataSource();
                    var selectedIds;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        $scope.scopeModel.size = payload.size;
                        $scope.scopeModel.precision = payload.precision;
                    }

                   return VRCommon_RDBDataTypeAPIService.GetRDBDataTypeInfo().then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                            if (selectedIds != undefined) {
                                typeSelectedPromise = UtilsService.createPromiseDeferred();
                                VRUIUtilsService.setSelectedValues(selectedIds, 'Value', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return {
                        Type: VRUIUtilsService.getIdSelectedIds('Value', attrs, ctrl),
                        Size: $scope.scopeModel.size,
                        Precision: $scope.scopeModel.precision
                    };
                };
                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;

    }]);