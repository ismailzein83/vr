'use strict';

app.directive('vrNotificationNotificationtypesettingSelector', ['UtilsService', 'VRUIUtilsService', 'VR_Notification_VRNotificationTypeAPIService',
    function (UtilsService, VRUIUtilsService, VR_Notification_VRNotificationTypeAPIService) {
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
                normalColNum: '@',
                customvalidate: '=',
                isloading: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new VRNotificationTypeSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function VRNotificationTypeSelectorCtor(ctrl, $scope, attrs) {
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
                    var selectIfSingleItem;
                    selectorAPI.clearDataSource();

                    var selectedIds;
                    var filter;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        selectIfSingleItem = payload.selectIfSingleItem;
                    }
                    if (filter == undefined)
                        filter = {};

                    return VR_Notification_VRNotificationTypeAPIService.GetVRNotificationTypeSettingsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'Id', attrs, ctrl);
                            }
                            else if (selectIfSingleItem == true) {
                                selectorAPI.selectIfSingleItem();
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('Id', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Notification Type";

            if (attrs.ismultipleselection != undefined) {
                label = "Notification Types";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var htmlLabel;
            if (attrs.hidelabel == undefined)
                htmlLabel = "label = '" + label + "' ";

            var haschildcolumns = attrs.haschildcolumns != undefined ? " haschildcolumns " : "";

            return '<vr-columns colnum="{{ctrl.normalColNum}}" ' + haschildcolumns + ' >'
                        + '<span vr-loader="ctrl.isloading">'
                            + '<vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="Id" isrequired="ctrl.isrequired" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" '
                                + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label
                                + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate" '
                                + htmlLabel + ' >'
                            + '</vr-select>'
                        + '</span>'
                   + '</vr-columns>';
        }
    }]);