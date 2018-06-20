'use strict';

app.directive('vrSecEmailRemoteselector', ['UtilsService', 'VRUIUtilsService', 'VR_Sec_UserAPIService', 'VR_Sec_UserService',
    function (UtilsService, VRUIUtilsService, VR_Sec_UserAPIService, VR_Sec_UserService) {
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
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new EmailRemoteSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function EmailRemoteSelectorCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var connectionId;

            function initializeController() {

                $scope.addNewRemoteUser = function () {
                    var onRemoteUserAdded = function (userObj) {
                        ctrl.datasource.push(userObj.Entity);
                        if (attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(userObj.Entity);
                        else
                            ctrl.selectedvalues = userObj.Entity;
                    };
                    VR_Sec_UserService.addUser(onRemoteUserAdded, connectionId);
                };

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
                        connectionId = payload.connectionId;
                    }

                    return VR_Sec_UserAPIService.GetRemoteEmailInfo(connectionId).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'Email', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('Email', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Email";

            if (attrs.ismultipleselection != undefined) {
                label = "Emails";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = ' hideremoveicon ';

            var addClicked = '';
            if (attrs.showaddbutton != undefined)
                addClicked = 'onaddclicked="addNewRemoteUser"';

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                        '<vr-select ' + multipleselection + ' ' + addClicked + ' datatextfield="Email" datavaluefield="Id" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" '
                            + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" '
                            + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                        '</vr-select>' +
                    '</vr-columns>';
        }
    }]);