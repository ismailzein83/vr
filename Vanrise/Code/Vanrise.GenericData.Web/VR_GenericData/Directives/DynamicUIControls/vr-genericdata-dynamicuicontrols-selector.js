'use strict';
app.directive('vrGenericdataDynamiccontrolsSelector', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "@",
                selectedvalues: '=',
                hideremoveicon: "@",
                onselectitem: "=",
                ondeselectitem: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];
                var ctor = new selectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {
                    }
                }
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }

        };


        function getTemplate(attrs) {
            var multipleselection = "";
            var label = "";
            if (attrs.ismultipleselection != undefined) {
                label = "";
                multipleselection = "ismultipleselection";
            }
            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Text" datavaluefield="Value" onselectitem="ctrl.onselectitem"  ondeselectitem="ctrl.ondeselectitem"'
            + required + ' label="' + label + '" on-ready="ctrl.onSelectorReady" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" ' + hideremoveicon + '></vr-select>'
               + '</div>'
        }

        function selectorCtor(ctrl, $scope, $attrs) {

            var selectorApi;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineAPI();
                }
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorApi.clearDataSource();

                    var filter = {};
                    var selectedIds;
                    var fieldType;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                        fieldType = payload.fieldType;
                    }

                    if (fieldType != undefined)
                    {
                        for (var i = 0; i < fieldType.Choices.length; i++)
                            ctrl.datasource.push(fieldType.Choices[i]);

                        if (selectedIds != undefined)
                            VRUIUtilsService.setSelectedValues(selectedIds, 'Value', $attrs, ctrl);
                    }
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('Value', $attrs, ctrl);
                }

                api.getSelectedValues = function () {
                    return ctrl.selectedvalues;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);