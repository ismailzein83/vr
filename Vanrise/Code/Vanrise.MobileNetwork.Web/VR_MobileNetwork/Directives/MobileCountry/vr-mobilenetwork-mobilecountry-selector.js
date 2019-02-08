'use strict';

app.directive('vrMobilenetworkMobilecountrySelector', ['VRUIUtilsService', 'VR_MobileNetwork_MobileCountryAPIService', 'UtilsService', function (VRUIUtilsService, VR_MobileNetwork_MobileCountryAPIService, UtilsService) {
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

            var ctor = new MobileCountrySelector(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'mobileCountrySelectorCtrl',
        bindToController: true,
        template: function (element, attrs) {
            return getSelectorTemplate(attrs);
        }
    };

    function MobileCountrySelector(ctrl, $scope, attrs) {

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

                return VR_MobileNetwork_MobileCountryAPIService.GetMobileCountriesInfo(serializedFilter).then(function (response) {
                    ctrl.datasource.length = 0;
                    if (response) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }
                    }

                    if (selectedIds) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'MobileCountryId', attrs, ctrl);
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('MobileCountryId', attrs, ctrl);
            };

            if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }
    }
    function getSelectorTemplate(attrs) {

        var multipleselection = '';

        var label = 'Mobile Country';
        if (attrs.ismultipleselection != undefined) {
            label = 'Mobile Countries';
            multipleselection = 'ismultipleselection';
        }

        if (attrs.customlabel != undefined) 
            label = attrs.customlabel;

        var hideremoveicon = '';
        if (attrs.hideremoveicon != undefined)
            hideremoveicon = 'hideremoveicon';

        return '<vr-columns colnum="{{mobileCountrySelectorCtrl.normalColNum}}">'
            + '<vr-select on-ready="scopeModel.onDirectiveReady"'
            + ' datasource="mobileCountrySelectorCtrl.datasource"'
            + ' selectedvalues="mobileCountrySelectorCtrl.selectedvalues"'
            + ' onselectionchanged="mobileCountrySelectorCtrl.onselectionchanged"'
            + ' datavaluefield="MobileCountryId" datatextfield="MobileCountryName"'
            + ' ' + multipleselection
            + ' isrequired="mobileCountrySelectorCtrl.isrequired"'
            + ' ' + hideremoveicon
            + ' label="' + label + '"'
            + ' vr-disabled="mobileCountrySelectorCtrl.isdisabled"'
            + '</vr-select>'
            + '</vr-columns>';
    }
}]);