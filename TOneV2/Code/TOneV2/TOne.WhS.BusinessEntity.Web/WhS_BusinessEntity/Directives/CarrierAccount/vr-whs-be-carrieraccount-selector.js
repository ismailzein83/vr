'use strict';
app.directive('vrWhsBeCarrieraccountSelector', ['WhS_BE_CarrierAccountAPIService', 'UtilsService', '$compile', function (WhS_BE_CarrierAccountAPIService, UtilsService, $compile) {

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
            hideremoveicon: "@"
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            $scope.selectedCarrierValues;
            if ($attrs.ismultipleselection != undefined)
                $scope.selectedCarrierValues = [];
            $scope.datasource = [];
            var beCarrierGroup = new BeCarrierGroup(ctrl, $scope, WhS_BE_CarrierAccountAPIService, $attrs);
            beCarrierGroup.initializeController();
            $scope.onselectionchanged = function () {
                ctrl.selectedvalues = $scope.selectedCarrierValues;
                if (ctrl.onselectionchanged != undefined) {
                    var onvaluechangedMethod = $scope.$parent.$eval(ctrl.onselectionchanged);
                    if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                        onvaluechangedMethod();
                    }
                }

            }

        },
        controllerAs: 'ctrl',
        bindToController: true,
        link: function preLink($scope, iElement, iAttrs) {
            var ctrl = $scope.ctrl;
            $scope.$watch('ctrl.isdisabled', function () {
                var template = getBeCarrierGroupTemplate(iAttrs, ctrl);
                iElement.html(template);
                $compile(iElement.contents())($scope);
            });
        }

    };

    function getBeCarrierGroupTemplate(attrs, ctrl) {
        var label;
        if (attrs.label != undefined)
            label = attrs.label;
        else {
            if (attrs.type == "'Customer'")
                label = "Customers";
            else if (attrs.type == "'Supplier'")
                label = "Suppliers";
            else if (attrs.type == "'Both'")
                label = "Carriers";
        }
        var disabled = "";
        if (ctrl.isdisabled)
            disabled = "vr-disabled='true'"

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        var hideselectedvaluessection = "";
        if (attrs.hideselectedvaluessection != undefined)
            hideselectedvaluessection = "hideselectedvaluessection";

        var hideremoveicon = "";
        if (attrs.hideremoveicon != undefined)
            hideremoveicon = "hideremoveicon";

        if (attrs.ismultipleselection != undefined)
            return '<div style="display:inline-block;width: calc(100% - 18px);" vr-loader="isLoadingDirective">'
                       + '<vr-label >' + label + '</vr-label>'
                   + ' <vr-select ismultipleselection datasource="datasource" ' + required + ' ' + hideselectedvaluessection + ' selectedvalues="selectedCarrierValues" ' + disabled + ' onselectionchanged="onselectionchanged" datatextfield="Name" datavaluefield="CarrierAccountId"'
                   + 'entityname="' + label + '" ' + hideremoveicon + '></vr-select></div>'
                   + ' <span class="glyphicon glyphicon-th hand-cursor"  aria-hidden="true" ng-click="openTreePopup()"></span></div>';
        else
            return '<div vr-loader="isLoadingDirective"><vr-label >' + label + '</vr-label>'
               + ' <vr-select datasource="datasource" selectedvalues="selectedCarrierValues" ' + required + ' ' + hideselectedvaluessection + ' onselectionchanged="onselectionchanged"  ' + disabled + ' datatextfield="Name" datavaluefield="CarrierAccountId"'
               + 'entityname="' + label + '" ' + hideremoveicon + '></vr-select></div>';
    }

    function BeCarrierGroup(ctrl, $scope, WhS_BE_CarrierAccountAPIService, $attrs) {
        var getCustomers = false;
        var getSuppliers = false;
        if (ctrl.type == "Customer")
            getCustomers = true;
        else if (ctrl.type == "Supplier")
            getSuppliers = true;
        else if (ctrl.type == "Both") {
            getCustomers = true;
            getSuppliers = true;
        }
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.loadDir = function (selectedIds)
            {
                return WhS_BE_CarrierAccountAPIService.GetCarrierAccountsInfo(getCustomers, getSuppliers).then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.datasource.push(itm);
                    });

                    if (selectedIds != undefined)
                    {
                        if ($attrs.ismultipleselection != undefined) {
                            for (var i = 0; i < selectedIds.length; i++) {
                                var selectedCarrierValue = UtilsService.getItemByVal($scope.datasource, selectedIds[i], "CarrierAccountId");
                                if (selectedCarrierValue != null)
                                    $scope.selectedCarrierValues.push(selectedCarrierValue);
                            }
                        } else {
                            var selectedCarrierValue = UtilsService.getItemByVal($scope.datasource, selectedIds, "CarrierAccountId");
                            if (selectedCarrierValue != null)
                                $scope.selectedCarrierValues = selectedCarrierValue;
                        }
                    }
                });
            }

            api.load = function () {
                return WhS_BE_CarrierAccountAPIService.GetCarrierAccountsInfo(getCustomers, getSuppliers).then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.datasource.push(itm);
                    });
                });
            }

            api.getData = function ()
            {
                return $scope.selectedCarrierValues;
            }

            api.setData = function (selectedIds) {
                if ($attrs.ismultipleselection!=undefined) {
                    for (var i = 0; i < selectedIds.length; i++) {
                        var selectedCarrierValue = UtilsService.getItemByVal($scope.datasource, selectedIds[i], "CarrierAccountId");
                        if (selectedCarrierValue != null)
                            $scope.selectedCarrierValues.push(selectedCarrierValue);
                    }
                } else {
                    var selectedCarrierValue = UtilsService.getItemByVal($scope.datasource, selectedIds, "CarrierAccountId");
                    if (selectedCarrierValue != null)
                        $scope.selectedCarrierValues = selectedCarrierValue;
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

