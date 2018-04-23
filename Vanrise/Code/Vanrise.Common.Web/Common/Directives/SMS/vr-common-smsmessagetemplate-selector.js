'use strict';

app.directive('vrCommonSmsmessagetemplateSelector', ['UtilsService', 'VRUIUtilsService', 'VRCommon_SMSMessageTemplateAPIService',

function (UtilsService, VRUIUtilsService, VRCommon_SMSMessageTemplateAPIService) {
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
            normalColNum: '@',
            customvalidate: '='
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var smsMessageTemplateSelector = new SMSMessageTemplateSelector(ctrl, $scope, $attrs);
            smsMessageTemplateSelector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function SMSMessageTemplateSelector(ctrl, $scope, attrs) {

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

                var selectedIds;
                var filter;
                
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    filter = { SMSMessageTypeId: payload.filter };
                }

                return VRCommon_SMSMessageTemplateAPIService.GetSMSMessageTemplatesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                    selectorAPI.clearDataSource();
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }
                        
                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'SMSMessageTemplateId', attrs, ctrl);
                        }
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('SMSMessageTemplateId', attrs, ctrl);
            };


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getTemplate(attrs) {

        var multipleselection = "";
        var label = "SMS Message Template";
        var hideremoveicon = '';

        if (attrs.ismultipleselection != undefined) {
            label = "SMS Message Templates";
            multipleselection = "ismultipleselection";
        }
        if (attrs.customlabel != undefined)
            label = attrs.customlabel;

        if (attrs.hideremoveicon != undefined)
            hideremoveicon = 'hideremoveicon';

        return '<vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="SMSMessageTemplateId" isrequired="ctrl.isrequired" label="' + label +
                   '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label +
                   '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
               '</vr-select>';
    }

}]);