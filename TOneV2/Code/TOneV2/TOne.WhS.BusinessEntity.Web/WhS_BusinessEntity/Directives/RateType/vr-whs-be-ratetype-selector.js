'use strict';
app.directive('vrWhsBeRatetypeSelector', ['WhS_BE_RateTypeAPIService', 'WhS_BE_RateTypeService', 'UtilsService', '$compile', 'VRUIUtilsService', function (WhS_BE_RateTypeAPIService, WhS_BE_RateTypeService, UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            type: "=",
            onReady: '=',
            label: "@",
            ismultipleselection: "@",
            hideselectedvaluessection: '@',
            onselectionchanged: '=',
            isrequired: '@',
            isdisabled: "=",
            selectedvalues: "=",
            showaddbutton: '@',
            hidelabel: '@',
            onselectitem: "=",
            ondeselectitem: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];
            ctrl.datasource = [];


           
            ctrl.datasource = [];
            var ctor = new rateTypeCtor(ctrl, $scope, WhS_BE_RateTypeAPIService, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        template: function (element, attrs) {
            return getTemplate(attrs);
        }

    };
    function getTemplate(attrs) {
        var label;
        if (attrs.hidelabel == undefined)
            label = 'label="RateType"';
        var disabled = "";
        if (attrs.isdisabled)
            disabled = "vr-disabled='true'"

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        var hideselectedvaluessection = "";
        if (attrs.hideselectedvaluessection != undefined)
            hideselectedvaluessection = "hideselectedvaluessection";
        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="ctrl.addNewRateType"';

        var multipleselection = "";
        if (attrs.ismultipleselection != undefined)
            multipleselection = "ismultipleselection"

        return ' <vr-select ' + multipleselection + ' datasource="ctrl.datasource" ' + required + ' ' + hideselectedvaluessection + ' selectedvalues="ctrl.selectedvalues" ' + disabled + ' onselectionchanged="ctrl.onselectionchanged" datatextfield="Name"  on-ready="ctrl.onSelectorReady" datavaluefield="RateTypeId" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"'
                   + 'entityname="RateType" ' + label + ' ' + addCliked + ' ></vr-select>';
       
    }
    function rateTypeCtor(ctrl, $scope, WhS_BE_RateTypeAPIService, $attrs) {
        var selectorApi;
        function initializeController() {
            ctrl.addNewRateType = function () {
                var onRateTypeAdded = function (rateTypeObj) {
                    ctrl.datasource.push(rateTypeObj)
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(rateTypeObj);
                    else
                        ctrl.selectedvalues = rateTypeObj;
                };
                WhS_BE_RateTypeService.addRateType(onRateTypeAdded);
            }

            ctrl.onSelectorReady = function (api) {
                selectorApi = api;
                defineAPI();
            }
        }

        function defineAPI() {
            var api = {};

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('RateTypeId', $attrs, ctrl);
            }
            api.load = function (payload) {

                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                return WhS_BE_RateTypeAPIService.GetAllRateTypes().then(function (response) {
                    selectorApi.clearDataSource();
                    angular.forEach(response, function (item) {
                        ctrl.datasource.push(item);

                    });
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'RateTypeId', $attrs, ctrl);

                });
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

