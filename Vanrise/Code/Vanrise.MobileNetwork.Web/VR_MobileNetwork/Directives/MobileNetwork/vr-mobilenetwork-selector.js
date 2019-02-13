'use strict';

app.directive('vrMobilenetworkSelector', ['VRUIUtilsService', 'VR_MobileNetwork_MobileNetworkAPIService', 'UtilsService', '$filter', function (VRUIUtilsService, VR_MobileNetwork_MobileNetworkAPIService, UtilsService, $filter) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            onselectionchanged: '=',
            isrequired: "=",
            isdisabled: "=",
            selectedvalues: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var ctor = new MobileNetworkSelector(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'mobileNetworkSelectorCtrl',
        bindToController: true,
        template: function (element, attrs) {
            return getSelectorTemplate(attrs);
        }
    };

    function MobileNetworkSelector(ctrl, $scope, attrs) {

        this.initializeController = initializeController;

        var directiveAPI;
        var directiveReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};
            
            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                directiveReadyDeferred.resolve();

                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                directiveAPI.clearDataSource();

                var filter;
                var selectedIds;

                if (payload != undefined) {
                    filter = payload.filter;
                    selectedIds = payload.selectedIds;
                }

                var serializedFilter = {};
                if (filter != undefined)
                    serializedFilter = UtilsService.serializetoJson(filter);

                return VR_MobileNetwork_MobileNetworkAPIService.GetMobileNetworksInfo(serializedFilter).then(function (response) {
                    ctrl.datasource.length = 0;
                    if (response) {
                        var data = $filter('orderBy')(response, 'NetworkName');
                        for (var i = 0; i < data.length; i++) {
                            ctrl.datasource.push(data[i]);
                        }
                    }

                    if (selectedIds) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'MobileNetworkId', attrs, ctrl);
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('MobileNetworkId', attrs, ctrl);
            };

            if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }
    }
    function getSelectorTemplate(attrs) {

        var multipleselection = '';

        var label = 'Mobile Network';
        if (attrs.ismultipleselection != undefined) {
            label = 'Mobile Networks';
            multipleselection = 'ismultipleselection';
        }

        if (attrs.customlabel != undefined) 
            label = attrs.customlabel;

        var hideremoveicon = '';
        if (attrs.hideremoveicon != undefined)
            hideremoveicon = 'hideremoveicon';

        return '<vr-columns colnum="{{mobileNetworkSelectorCtrl.normalColNum}}">'
            + '<vr-select on-ready="scopeModel.onDirectiveReady"'
            + ' datasource="mobileNetworkSelectorCtrl.datasource"'
            + ' selectedvalues="mobileNetworkSelectorCtrl.selectedvalues"'
            + ' onselectionchanged="mobileNetworkSelectorCtrl.onselectionchanged"'
            + ' datavaluefield="MobileNetworkId" datatextfield="NetworkName"'
            + ' ' + multipleselection
            + ' isrequired="mobileNetworkSelectorCtrl.isrequired"'
            + ' ' + hideremoveicon
            + ' label="' + label + '"'
            + ' vr-disabled="mobileNetworkSelectorCtrl.isdisabled"'
            + '</vr-select>'
            + '</vr-columns>';
    }
}]);