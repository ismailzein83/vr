'use strict';

app.directive('retailTelesUsersSelector', ['Retail_Teles_UserAPIService', 'UtilsService', 'VRUIUtilsService', function (Retail_Teles_UserAPIService, UtilsService, VRUIUtilsService) {
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

            var usersSelector = new UsersSelector(ctrl, $scope, $attrs);
            usersSelector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function UsersSelector(ctrl, $scope, attrs) {

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
                var promises = [];

                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;
                var vrConnectionId;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    vrConnectionId = payload.vrConnectionId;
                    var siteId = payload.siteId;
                    if (payload.filter != undefined)
                        filter = payload.filter;

                    return Retail_Teles_UserAPIService.GetUsersInfo(vrConnectionId, siteId, UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'TelesUserId', attrs, ctrl);
                            }
                        }
                    });
                }
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('TelesUserId', attrs, ctrl);
            };
            api.clearDataSource = function () {
                selectorAPI.clearDataSource();
                ctrl.datasource.length = 0;
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getTemplate(attrs) {

        var multipleselection = "";
        var label = "User";

        if (attrs.ismultipleselection != undefined) {
            label = "Users";
            multipleselection = "ismultipleselection";
        }

        return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="TelesUserId" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate"></vr-select></vr-columns>';
    }

}]);