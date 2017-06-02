'use strict';

app.directive('retailMultinetAddresstypeSelector', ['UtilsService', 'VRUIUtilsService', 'Retail_MultiNet_AddressTypeEnum',
    function (UtilsService, VRUIUtilsService, Retail_MultiNet_AddressTypeEnum) {
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

                var ctor = new AddressTypeCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function AddressTypeCtor(ctrl, $scope, attrs) {
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
                    var addressTypes = UtilsService.getArrayEnum(Retail_MultiNet_AddressTypeEnum);

                    if (addressTypes != null) {
                        for (var i = 0; i < addressTypes.length; i++) {
                            ctrl.datasource.push(addressTypes[i]);
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                        }
                        else if (setDefaultValue) {
                            VRUIUtilsService.setSelectedValues(addressTypes[0].value, 'value', attrs, ctrl);
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
            var label = "Address Type";

            if (attrs.ismultipleselection != undefined) {
                label = "Address Types";
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