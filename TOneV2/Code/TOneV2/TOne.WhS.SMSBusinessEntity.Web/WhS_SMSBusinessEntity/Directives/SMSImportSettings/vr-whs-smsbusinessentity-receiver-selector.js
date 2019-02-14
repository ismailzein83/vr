'use strict';

app.directive('vrWhsSmsbusinessentityReceiverSelector', ['UtilsService', 'VRUIUtilsService', 'WhS_SMSBE_ReceiverIdentificationEnum',
    function (UtilsService, VRUIUtilsService, WhS_SMSBE_ReceiverIdentificationEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                isrequired: '=',
                normalColNum: '@',
                customvalidate: '=',
                label: '@',
                withoutnormalization: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var receiverSelector = new ReceiverSelectorCtor(ctrl, $scope, $attrs);
                receiverSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {

                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Receiver";
            var entityName = "Receiver";

            if (attrs.ismultipleselection != undefined) {
                label = "Receivers";
                multipleselection = "ismultipleselection";
            }
            if (attrs.label != undefined)
                label = attrs.label;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                '<vr-select ' + multipleselection + ' datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" label="' + label +
                '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + entityName +
                '" customvalidate="ctrl.customvalidate">' +
                '</vr-select>' +
                '</vr-columns>';
        }

        function ReceiverSelectorCtor(ctrl, $scope, attrs) {

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

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }

                    var receiverIdentifications = UtilsService.getArrayEnum(WhS_SMSBE_ReceiverIdentificationEnum);

                    if (attrs.withoutnormalization == undefined) {
                        ctrl.datasource = receiverIdentifications;
                    }
                    else {
                        for (var index = 0; index < receiverIdentifications.length; index++) {
                            var currentReceiverIdentification = receiverIdentifications[index];
                            if (!currentReceiverIdentification.isNormalized)
                                ctrl.datasource.push(currentReceiverIdentification);
                        }
                    }

                    if (selectedIds != undefined)
                        ctrl.selectedvalues = UtilsService.getEnum(WhS_SMSBE_ReceiverIdentificationEnum, 'value', selectedIds);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
    }]);