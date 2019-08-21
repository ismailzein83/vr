(function (app) {

    'use strict';

    manufactorySelector.$inject = ['Demo_Module_ManufactoryAPIService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_ManufactoryService'];

    function manufactorySelector(Demo_Module_ManufactoryAPIService, UtilsService, VRUIUtilsService, Demo_Module_ManufactoryService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                hideremoveicon: '@',
                normalColNum: '@',
                isdisabled: '=',
                includeviewhandler: '@',
                showaddbutton: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new manufactorySelectorCtor($scope, $attrs, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (elemment, attrs) {
                return getManufactoryTemplate(attrs);
            }
        };

        function manufactorySelectorCtor($scope, attrs, ctrl) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onViewIconClicked = function (manufactory) {
                    Demo_Module_ManufactoryService.viewManufactory(manufactory.Id);
                };

                $scope.scopeModel.onAddIconClicked = function () {
                    var onManufactoryAddedFromSelector = function (manufactory) {
                        ctrl.datasource.push(manufactory);
                    };

                    Demo_Module_ManufactoryService.addManufactory(onManufactoryAddedFromSelector);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var selectedIds;
                    var filter;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    return Demo_Module_ManufactoryAPIService.GetManufactoriesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'Id', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('Id', attrs, ctrl);
                };

                if (ctrl.onReady != undefined) {
                    ctrl.onReady(api);
                }
            }
        }

        function getManufactoryTemplate(attrs) {

            var label = 'Manufactory';
            var multipleselection = '';
            if (attrs.ismultipleselection != undefined) {
                label = 'Manufactories';
                multipleselection = 'ismultipleselection';
            }

            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = 'hideremoveicon';
            }

            var onviewclicked = '';
            if (attrs.includeviewhandler != undefined) {
                onviewclicked = 'onviewclicked="scopeModel.onViewIconClicked"';
            }

            var onAddCliked = '';
            if (attrs.showaddbutton != undefined) {
                onAddCliked = 'onaddclicked="scopeModel.onAddIconClicked"';
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + '<vr-select '
                + ' vr-disabled="ctrl.isdisabled" '
                + ' on-ready="scopeModel.onSelectorReady" '
                + ' datasource="ctrl.datasource" '
                + ' isrequired="ctrl.isrequired" '
                + ' label="' + label + '" '
                + ' ' + multipleselection + ' '
                + ' ' + hideremoveicon + ' '
                + ' ' + onviewclicked + ' '
                + ' ' + onAddCliked + ' '
                + ' datatextfield="Name" '
                + ' datavaluefield="Id" '
                + ' entityname="Manufactory" '
                + ' selectedvalues="ctrl.selectedvalues" '
                + ' onselectionchanged="ctrl.onselectionchanged" '
                + ' onselectitem="ctrl.onselectitem" '
                + ' ondeselectitem="ctrl.ondeselectitem" >'
                + '</vr-select>'
                + '</vr-columns>';

        }
    }

    app.directive('demoModuleManufactorySelector', manufactorySelector);

})(app);