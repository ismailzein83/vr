
appControllers.directive('vrGenericdataFieldtypeTextTexttypeSelector', ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_FieldTextTextTypeEnum',
    function (VRNotificationService, UtilsService, VRUIUtilsService, VR_GenericData_FieldTextTextTypeEnum) {
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
                isdisabled: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var typeSelector = new TextTypeSelector(ctrl, $scope, $attrs);
                typeSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {

                return getTextTypeTemplate(attrs);
            }
        };

        function getTextTypeTemplate(attrs) {

            var label = "Text Type";

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select  on-ready="scopeModel.onSelectorReady" datatextfield="description" datavaluefield="value" label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Text Type" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' isrequired="ctrl.isrequired"></vr-select></vr-columns>';

        }


        function TextTypeSelector(ctrl, $scope, attrs) {


            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) { //payload is an object that has selectedids and filter
                    selectorAPI.clearDataSource();
                    var selectedIds;
                    ctrl.datasource = [];

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }
                    ctrl.datasource = UtilsService.getArrayEnum(VR_GenericData_FieldTextTextTypeEnum);
                   
                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };
                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;

    }]);