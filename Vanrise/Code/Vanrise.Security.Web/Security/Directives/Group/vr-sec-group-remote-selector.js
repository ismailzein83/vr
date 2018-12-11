(function (app) {

    'use strict';

    GroupRemoteSelectorDirective.$inject = ['VR_Sec_GroupAPIService', 'UtilsService', 'VRUIUtilsService','VR_Sec_RemoteGroupAPIService'];

    function GroupRemoteSelectorDirective(VR_Sec_GroupAPIService, UtilsService, VRUIUtilsService, VR_Sec_RemoteGroupAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isdisabled: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var remoteGroupSelector = new RemoteGroupSelector(ctrl, $scope, $attrs);
                remoteGroupSelector.initializeController();
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
                return getGroupTemplate(attrs);
            }
        };

        function RemoteGroupSelector(ctrl, $scope, attrs) {
            var selectorApi;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    getDirectiveAPI();
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var connectionId;
                    var filter = null;
                    var selectedIds;
                    if (payload != undefined) {
                        connectionId = payload.connectionId;
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return VR_Sec_RemoteGroupAPIService.GetRemoteGroupInfo(connectionId,UtilsService.serializetoJson(filter)).then(function (response) {
                        ctrl.datasource.length = 0;

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'GroupId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('GroupId', attrs, ctrl);
                };

                if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }

        function getGroupTemplate(attrs) {
            var multipleselection = "";

            var label = "Group";
            if (attrs.ismultipleselection != undefined) {
                label = "Groups";
                multipleselection = "ismultipleselection";
            }

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="GroupId" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Group" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>';
        }

        return directiveDefinitionObject;
    }

    app.directive('vrSecGroupRemoteSelector', GroupRemoteSelectorDirective);

})(app);
