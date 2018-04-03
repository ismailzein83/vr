'use strict';

app.directive('vrCommonMailmessagetemplateSelector', ['VRCommon_VRMailMessageTemplateAPIService', 'UtilsService', 'VRUIUtilsService', 'VRCommon_VRMailMessageTemplateService',

    function (VRCommon_VRMailMessageTemplateAPIService, UtilsService, VRUIUtilsService, VRCommon_VRMailMessageTemplateService) {
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
                customvalidate: '=',
                customlabel: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];


                var mailMessageTemplateSelector = new MailMessageTemplateSelector(ctrl, $scope, $attrs);
                mailMessageTemplateSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function MailMessageTemplateSelector(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var filter;
            $scope.addNewMailMessageTemplate = function () {
                var mailMessageTypeId = filter != undefined && filter.VRMailMessageTypeId ? filter.VRMailMessageTypeId : undefined;
                var onMailMessageTemplateAdded = function (mailMessageObj) {
                    ctrl.datasource.push(mailMessageObj.Entity);
                    if (attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(mailMessageObj.Entity);
                    else
                        ctrl.selectedvalues = mailMessageObj.Entity;
                };
                VRCommon_VRMailMessageTemplateService.addMailMessageTemplate(onMailMessageTemplateAdded, mailMessageTypeId);
            };
            ctrl.haspermission = function () {
                return VRCommon_VRMailMessageTemplateAPIService.HasAddMailMessageTemplatePermission();
            };

            var selectorAPI;
            var selectFirstItem;
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


                    if (payload != undefined) {
                        selectFirstItem = payload.selectFirstItem;
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    return VRCommon_VRMailMessageTemplateAPIService.GetMailMessageTemplatesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'VRMailMessageTemplateId', attrs, ctrl);
                            } else if (selectFirstItem && ctrl.datasource.length > 0) {
                                var defaultValue = attrs.ismultipleselection != undefined ? [ctrl.datasource[0].VRMailMessageTemplateId] : ctrl.datasource[0].VRMailMessageTemplateId;
                                VRUIUtilsService.setSelectedValues(defaultValue, 'VRMailMessageTemplateId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('VRMailMessageTemplateId', attrs, ctrl);
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = 'label= "Mail Message Template"';
            if (attrs.ismultipleselection != undefined) {
                label = 'label= "Mail Message Templates"';
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = 'label= "'+attrs.customlabel+'"';

            if (attrs.hidelabel != undefined) {
                label = "";
            }

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewMailMessageTemplate"';

            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = 'hideremoveicon';

            return '<vr-select ' + multipleselection + ' ' + addCliked + ' ' + hideremoveicon + ' datatextfield="Name" datavaluefield="VRMailMessageTemplateId" isrequired="ctrl.isrequired" ' + label +
                       '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" ' +
                       '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" customvalidate="ctrl.customvalidate" haspermission="ctrl.haspermission">' +
                   '</vr-select>';
        }

    }]);