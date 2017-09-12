'use strict';
app.directive('vrWhsBeCarrierprofileSelector', ['WhS_BE_CarrierProfileAPIService', 'UtilsService', '$compile', 'VRUIUtilsService','WhS_BE_CarrierProfileService',
function (WhS_BE_CarrierProfileAPIService, UtilsService, $compile, VRUIUtilsService, WhS_BE_CarrierProfileService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            onselectionchanged: '=',
            isrequired: "@",
            selectedvalues: '=',
            normalColNum: '@'

        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            ctrl.datasource = [];

            $scope.addNewCarrierProfile = function () {
                var onCarrierProfileAdded = function (carrierProfileObj) {
                    ctrl.datasource.push(carrierProfileObj.Entity);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(carrierProfileObj.Entity);
                    else
                        ctrl.selectedvalues = carrierProfileObj.Entity;
                };
                WhS_BE_CarrierProfileService.addCarrierProfile(onCarrierProfileAdded);
            };
            ctrl.haspermission = function () {
                return WhS_BE_CarrierProfileAPIService.HasAddCarrierProfilePermission();
            }; 
            var ctor = new carrierProfileCtor(ctrl, $scope, $attrs);
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
            return getTemplate(attrs);
        }

    };


    function getTemplate(attrs) {

        var multipleselection = "";
        if (attrs.ismultipleselection != undefined)
            multipleselection = "ismultipleselection";
        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewCarrierProfile"';

        return '<vr-columns colnum="{{ctrl.normalColNum}}"    > <vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="CarrierProfileId" '
        + required + ' label="Carrier Profile" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" haspermission="ctrl.haspermission"  onselectionchanged="ctrl.onselectionchanged" ' + addCliked + ' ></vr-select></vr-columns>'

    }

    function carrierProfileCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('CarrierProfileId', $attrs, ctrl);
            };
            api.load = function (payload) {

                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                return WhS_BE_CarrierProfileAPIService.GetCarrierProfilesInfo().then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.datasource.push(item);

                    });
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'CarrierProfileId', $attrs, ctrl);

                });

            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);