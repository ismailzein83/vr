'use strict';
app.directive('vrAccountmanagerAccountmanagerSelector', ['VR_AccountManager_AccountManagerAPIService', 'UtilsService', 'VRUIUtilsService',
    function (VR_AccountManager_AccountManagerAPIService, UtilsService, VRUIUtilsService) {

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
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new accountManagerCtor(ctrl, $scope, $attrs);
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
                return getAccountManagerSelectorTemplate(attrs);
            }

        };


        function getAccountManagerSelectorTemplate(attrs) {

            var multipleselection = "";
            var label = "Account Manager";
            if (attrs.ismultipleselection != undefined) {
                label = "Account Manager";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="ctrl.onSelectorReady" datatextfield="UserName" datavaluefield="AccountManagerId" label="' + label + '" ' + '  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="AccountManager" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" isrequired="ctrl.isrequired" haspermission="ctrl.haspermission"></vr-select></vr-columns>';
        }

        function accountManagerCtor(ctrl, $scope, attrs) {

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

                    selectorAPI.clearDataSource();
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;

                    }

                    return VR_AccountManager_AccountManagerAPIService.GetAccountManagerInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++)
                                ctrl.datasource.push(response[i]);
                            if (selectedIds != undefined)
                                VRUIUtilsService.setSelectedValues(selectedIds, 'AccountManagerId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('AccountManagerId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);