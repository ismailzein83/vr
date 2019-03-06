'use strict';

app.directive('vrWhsBeSmsservicestypeSelector', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_SMSServiceTypeAPIService',
    function (UtilsService, VRUIUtilsService, WhS_BE_SMSServiceTypeAPIService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                onselectionchanged: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                ondeselectallitems: "=",
                selectedvalues: "=",
                onremoveitem: '=',
                isrequired: '=',
                isdisabled: '=',
                ismultipleselection: '@',
                normalColNum: '@',
                hideremoveicon: "@",
                customlabel: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new smsServiceTypeSelectorCtor(ctrl, $scope, $attrs);
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
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "SMS Service Type";
            if (attrs.ismultipleselection != undefined)
                multipleselection = "ismultipleselection";

            var hideselectall = "";
            if (attrs.hideselectall != undefined)
                hideselectall = "hideselectall";

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            var hideselectedvaluessection = "";
            if (attrs.hideselectedvaluessection != undefined)
                hideselectedvaluessection = "hideselectedvaluessection";

            var hidelabel = "";
            if (attrs.hidelabel != undefined)
                hidelabel = "hidelabel";

            var customlabel = '';
            if (attrs.customlabel != undefined)
                customlabel = 'customlabel="{{ctrl.customlabel}}"';

            var usefullcolumn = "";
            if (attrs.usefullcolumn != undefined)
                usefullcolumn = "usefullcolumn";

            return '<vr-select '
                + ' datatextfield = "Name"'
                + ' datavaluefield = "ID"'
                + ' label = "' + label
                + ' "datasource="ctrl.datasource"'
                + ' on-ready="ctrl.onSMSServiceTypeReady"'
                + ' selectedvalues = "ctrl.selectedvalues"'
                + ' onselectionchanged = "ctrl.onselectionchanged"'
                + ' onselectitem="ctrl.onselectitem" '
                + ' ondeselectitem = "ctrl.ondeselectitem" '
                + ' customvalidate = "ctrl.customvalidate" '
                + ' normal-col-num="{{ctrl.normalColNum}}"'
                +' isrequired = "ctrl.isrequired"' +
                + ' ' + hideselectedvaluessection
                + ' ' + hidelabel
                + ' ' + hideselectall
                + ' ' + customlabel
                + ' ' + usefullcolumn
                + ' ' + hideremoveicon
                + ' ' + multipleselection
                + '>'
                + '</vr-select>';
           
        }

        function smsServiceTypeSelectorCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                ctrl.onSMSServiceTypeReady = function (api) {
                    selectorAPI = api;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    var filter;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    return WhS_BE_SMSServiceTypeAPIService.GetSMSServicesTypesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                var smsServiceType = response[i];
                                ctrl.datasource.push(smsServiceType);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'ID', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ID', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);