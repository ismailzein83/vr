'use strict';
app.directive('vrCommonConnectionSelector', ['VRCommon_VRConnectionAPIService', 'UtilsService', 'VRUIUtilsService','VRCommon_VRConnectionService',
    function (VRCommon_VRConnectionAPIService, UtilsService, VRUIUtilsService, VRCommon_VRConnectionService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                isdisabled: "=",
                hideremoveicon: '@',
                normalColNum: '@',
                customlabel: '@',
                includeviewhandler: '@',
                showaddbutton:'@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new connectionCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getConnectionTemplate(attrs);
            }
        };


        function getConnectionTemplate(attrs) {

            var multipleselection = "";
            var label = "Connection";
            if (attrs.ismultipleselection != undefined) {
                label = "Connections";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon="";
            if (attrs.hideremoveicon !=undefined) {
                hideremoveicon="hideremoveicon";
            }

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="ctrl.addNewConnection"';

            var onviewclicked = "";
            if (attrs.includeviewhandler != undefined)
                onviewclicked = "onviewclicked='ctrl.onViewIconClicked'";

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="ctrl.onSelectorReady" datatextfield="Name" datavaluefield="VRConnectionId" label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Connection" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' isrequired="ctrl.isrequired" ' + onviewclicked + ' ' + addCliked+'></vr-select></vr-columns>';
        }

        function connectionCtor(ctrl, $scope, attrs) {
            var selectorAPI;

            var filter;

            function initializeController() {
                
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                ctrl.onViewIconClicked = function (connection) {
                    VRCommon_VRConnectionService.viewVRConnection(connection.VRConnectionId, true);
                };

                ctrl.addNewConnection = function () {

                    var connectionTypeIds = (filter != undefined && filter.ConnectionTypeIds != undefined && filter.ConnectionTypeIds.length > 0) ? filter.ConnectionTypeIds : undefined;

                    var connectionTypeIdsToLower;
                    if (connectionTypeIds != undefined && connectionTypeIds.length > 0) {
                        connectionTypeIdsToLower = [];
                        for (var i = 0; i < connectionTypeIds.length; i++)
                            connectionTypeIdsToLower.push(connectionTypeIds[i].toLowerCase());
                    }

                    var onVRConnectionAdded = function (connectionObj) {
                        var vrConnection = connectionObj.Entity;
                        ctrl.datasource.push(vrConnection);
                        if (attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(vrConnection);
                        else
                            ctrl.selectedvalues = vrConnection;
                    };

                    VRCommon_VRConnectionService.addVRConnection(onVRConnectionAdded, connectionTypeIds);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    return VRCommon_VRConnectionAPIService.GetVRConnectionInfos(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'VRConnectionId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('VRConnectionId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);