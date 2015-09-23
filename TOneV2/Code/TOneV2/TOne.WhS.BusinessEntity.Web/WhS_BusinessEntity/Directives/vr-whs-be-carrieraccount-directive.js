'use strict';
app.directive('vrWhsBeCarrieraccount', ['WhS_BE_CarrierAccountAPIService', function ( WhS_BE_CarrierAccountAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            type: "=",
            label: "@",
            selectedvalues: "=",
            ismultipleselection:"@"
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            $scope.selectedCarrierValues = [];
            $scope.datasource = [];
            var beCarrierGroup = new BeCarrierGroup(ctrl, $scope.datasource, WhS_BE_CarrierAccountAPIService);
            beCarrierGroup.initializeController();
            $scope.onselectionvalueschanged = function () {
                ctrl.selectedvalues = $scope.selectedCarrierValues;

            }

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                    $scope.$watch('ctrl.selectedvalues.length', function () {
                        if (iAttrs.onselectionchanged != undefined) {
                            var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onselectionchanged);
                            if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                                onvaluechangedMethod();
                            }
                        }
                    });
                }
            }
        },
        template: function (element, attrs) {
            return getBeCarrierGroupTemplate(attrs);
        }

    };
    function getBeCarrierGroupTemplate(attrs) {
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
        if (attrs.ismultipleselection != undefined)
            return '<div style="display:inline-block;width: calc(100% - 18px);">'
                       + '<vr-label >' + label + '</vr-label>'
                   + ' <vr-select ismultipleselection datasource="datasource" selectedvalues="selectedCarrierValues" onselectionchanged="onselectionvalueschanged" datatextfield="Name" datavaluefield="CarrierAccountId"'
                   + 'entityname="' + label + '"></vr-select></div>'
                   + ' <span class="glyphicon glyphicon-th hand-cursor"  aria-hidden="true" ng-click="openTreePopup()"></span></div>';
        else
            return'<div><vr-label >' + label + '</vr-label>'
               + ' <vr-select datasource="datasource" selectedvalues="selectedCarrierValues" onselectionchanged="onselectionvalueschanged" datatextfield="Name" datavaluefield="CarrierAccountId"'
               + 'entityname="' + label + '"></vr-select></div>';
    }
    function BeCarrierGroup(ctrl, datasource, WhS_BE_CarrierAccountAPIService) {
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
            loadCarriers();
        }

        function loadCarriers() {

            return WhS_BE_CarrierAccountAPIService.GetCarrierAccounts(getCustomers, getSuppliers).then(function (response) {
                angular.forEach(response, function (itm) {
                    datasource.push(itm);
                });
            });
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

