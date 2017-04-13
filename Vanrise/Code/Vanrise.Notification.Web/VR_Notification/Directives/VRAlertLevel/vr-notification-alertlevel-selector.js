'use strict';

app.directive('vrNotificationAlertlevelSelector', ['VR_Notification_AlertLevelAPIService', 'UtilsService', 'VRUIUtilsService',

    function (VR_Notification_AlertLevelAPIService, UtilsService, VRUIUtilsService) {
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

                var alertLevelSelector = new AlertLevelSelector(ctrl, $scope, $attrs);
                alertLevelSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function AlertLevelSelector(ctrl, $scope, attrs) {

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
                        filter = payload.filter;

                    }

                    return VR_Notification_AlertLevelAPIService.GetAlertLevelsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'VRAlertLevelId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('VRAlertLevelId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Alert Level";

            if (attrs.ismultipleselection != undefined) {
                label = "Alert Levels";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                     '<span vr-loader="ctrl.isloading">' +
                       '<vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="VRAlertLevelId" isrequired="ctrl.isrequired" label="' + label +
                           '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label +
                           '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate">' +
                       '</vr-select>' +
                     '</span>' +
                   '</vr-columns>';
        }

    }]);