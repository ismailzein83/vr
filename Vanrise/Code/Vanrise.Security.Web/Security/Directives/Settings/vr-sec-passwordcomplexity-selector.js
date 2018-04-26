'use strict';

app.directive('vrSecPasswordcomplexitySelector', ['UtilsService', 'VRUIUtilsService', 'VR_Sec_PasswordComplexityEnum',
    function (UtilsService, VRUIUtilsService, VR_Sec_PasswordComplexityEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                hideremoveicon: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new Ctor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function Ctor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var selectedIds;
                    var filter;
                    var setDefaultValue;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        setDefaultValue = payload.setDefaultValue;
                    }
                    var datalist = UtilsService.getArrayEnum(VR_Sec_PasswordComplexityEnum);

                    if (datalist != null) {
                        for (var i = 0; i < datalist.length; i++) {
                            ctrl.datasource.push(datalist[i]);
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                        }
                      
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Complexity Level";

            if (attrs.ismultipleselection != undefined) {
                label = "Complexity Levels";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = ' hideremoveicon ';

            return     '<vr-select ' + multipleselection + ' datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" '
                            + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" '
                            + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                        '</vr-select>';
        }
    }]);