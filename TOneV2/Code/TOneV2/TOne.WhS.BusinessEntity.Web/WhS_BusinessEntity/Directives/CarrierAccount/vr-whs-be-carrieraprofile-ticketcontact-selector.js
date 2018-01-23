'use strict';
app.directive('vrWhsBeCarrierprofileTicketcontactSelector', ['WhS_BE_CarrierProfileAPIService', 'UtilsService', 'VRUIUtilsService', 
    function (WhS_BE_CarrierProfileAPIService, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            onselectionchanged: '=',
            onblurdropdown: '=',
            isrequired: '=',
            selectedvalues: "=",
            hideremoveicon: "@",
            onselectitem: "=",
            ondeselectitem: "=",
            normalColNum: '@',
            customlabel: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            ctrl.datasource = [];
            var ctor = new TicketContactCtor(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function getTemplate(attrs) {

        var label = "Ticket Contact";
        if (attrs.ismultipleselection != undefined) 
            label = 'Ticket Contacts';
   
        if (attrs.customlabel != undefined)
            label = attrs.customlabel;

        var hideselectedvaluessection = "";
        if (attrs.hideselectedvaluessection != undefined)
            hideselectedvaluessection = "hideselectedvaluessection";

        var hideremoveicon = "";
        if (attrs.hideremoveicon != undefined)
            hideremoveicon = "hideremoveicon";
        var viewCliked = '';
      
       

        var ismultipleselection = "";
        if (attrs.ismultipleselection != undefined)
            ismultipleselection = "ismultipleselection";

        return '<vr-columns colnum="{{ctrl.normalColNum}}"> <vr-select   isrequired="ctrl.isrequired" on-ready="ctrl.onSelectorReady" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged"  onselectitem="ctrl.onselectitem"  ondeselectitem="ctrl.ondeselectitem" datatextfield="Name" datavaluefield="CarrierProfileTicketContactId" label="'
            + label + '" ' + hideselectedvaluessection + '  ' + hideremoveicon + ' ' + ismultipleselection + '  ></vr-select></vr-columns>';
    }

    function TicketContactCtor(ctrl, $scope, attrs) {

        var selectorApi;

        function initializeController() {       
            ctrl.onSelectorReady = function (api) {
                selectorApi = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var filter;
                var selectedIds;
                if (payload != undefined) {
                    filter = payload.filter;
                    selectedIds = payload.selectedIds;
                }
                var serializedFilter = {};
                if (filter != undefined)
                    serializedFilter = UtilsService.serializetoJson(filter);

                return WhS_BE_CarrierProfileAPIService.GetCarrierProfileTicketContactsInfo(serializedFilter).then(function (response) {
                    selectorApi.clearDataSource();
                    angular.forEach(response, function (itm) {
                        ctrl.datasource.push(itm);
                    });

                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'CarrierProfileTicketContactId', attrs, ctrl);
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('CarrierProfileTicketContactId', attrs, ctrl);
            };

            
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);