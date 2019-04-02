(function (app) {

    'use strict';

    ActionGroupSelector.$inject = ['UtilsService', 'VRUIUtilsService'];

    function ActionGroupSelector(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                selectedvalues: '=',
                ismultipleselection: "@",
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                ctrl.datasource = [];
                ctrl.selectedvalues;

                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var actionGroupSelector = new ActionGroupSelector($scope, ctrl, $attrs);
                actionGroupSelector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function ActionGroupSelector($scope, ctrl, attrs) {
            this.initializeController = initializeController;

            var context;

            var selectorAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var initialPromises = [];

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            if (payload != undefined) {
                                context = payload.context;
                                var selectedIds = payload.selectedIds;

                                ctrl.datasource = context.getActionGroupInfos();

                                if (selectedIds != undefined) {
                                    VRUIUtilsService.setSelectedValues(selectedIds, 'AccountActionGroupId', attrs, ctrl);
                                }
                            }

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('AccountActionGroupId', attrs, ctrl);
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }

        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Action Group'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }
            var hideremoveicon = (attrs.hideremoveicon != undefined && attrs.hideremoveicon != null) ? 'hideremoveicon' : null;

            var template = '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                + ' datasource="ctrl.datasource"'
                + ' selectedvalues="ctrl.selectedvalues"'
                + ' datavaluefield="AccountActionGroupId"'
                + ' datatextfield="Title"'
                + label
                + hideremoveicon
                + ' isrequired="ctrl.isrequired">'
                + '</vr-select>'
                + '</vr-columns>'
            return template;

        }
    }

    app.directive('retailBeAccountactiondefinitionActiongroupSelector', ActionGroupSelector);

})(app);