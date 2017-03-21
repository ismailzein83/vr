'use strict';
app.directive('vrAccountbalanceAccounttypeSourcefieldsSelector', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "=",
                isdisabled: "=",
                selectedvalues: '=',
                normalColNum: '@',
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];
                var ctor = new SourceFieldSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;
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
            var label = "Field";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Fields";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                   +'<vr-select on-ready="ctrl.onSelectorReady"'
                   + '  selectedvalues="ctrl.selectedvalues"'
                   + '  onselectionchanged="ctrl.onselectionchanged"'
                   + '  datasource="ctrl.datasource"`'
                   + ' ' + hideremoveicon 
                   + '  datavaluefield="FieldName"'
                   + '  datatextfield="FieldTitle"'
                   + '  ' + multipleselection
                   + '  isrequired="ctrl.isrequired"'
                   + '  label="' + label + '"'
                   + ' entityName="' + label + '"'
                   + '  >'
                   + '</vr-select>'
                   + '<vr-columns>';
        }

        function SourceFieldSelectorCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var filter;
            var selectorAPI;
            var context;
            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(defineAPI());
                    }
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    ctrl.datasource.length = 0;
                    var selectedIds;
                    var filter;
                    var sourceId;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        context = payload.context;
                        sourceId = payload.sourceId
                    } 
                    if (context != undefined && context.getSourceFieldsInfo != undefined && sourceId != undefined)
                    {
                        ctrl.datasource = context.getSourceFieldsInfo(sourceId);
                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'FieldName', attrs, ctrl);
                        }
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('FieldName', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;
            }
        }

        return directiveDefinitionObject;

    }]);