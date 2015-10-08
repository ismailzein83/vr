'use strict';
app.directive('vrWhsBeCarrieraccount', ['WhS_BE_CarrierAccountAPIService', 'UtilsService', function (WhS_BE_CarrierAccountAPIService, UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            type: "=",
            onloaded: '=',
            label: "@",
            ismultipleselection: "@",
            onselectionchanged: '=',
            isrequired:'@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            $scope.selectedCarrierValues;
            if ($attrs.ismultipleselection != undefined)
                $scope.selectedCarrierValues = [];
            $scope.datasource = [];
            var beCarrierGroup = new BeCarrierGroup(ctrl, $scope, WhS_BE_CarrierAccountAPIService);
            beCarrierGroup.initializeController();
            $scope.onselectionchanged = function () {
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
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
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

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        if (attrs.ismultipleselection != undefined)
            return '<div style="display:inline-block;width: calc(100% - 18px);" vr-loader="isLoadingDirective">'
                       + '<vr-label >' + label + '</vr-label>'
                   + ' <vr-select ismultipleselection datasource="datasource" ' + required + ' selectedvalues="selectedCarrierValues" onselectionchanged="onselectionchanged" datatextfield="Name" datavaluefield="CarrierAccountId"'
                   + 'entityname="' + label + '"></vr-select></div>'
                   + ' <span class="glyphicon glyphicon-th hand-cursor"  aria-hidden="true" ng-click="openTreePopup()"></span></div>';
        else
            return '<div vr-loader="isLoadingDirective"><vr-label >' + label + '</vr-label>'
               + ' <vr-select datasource="datasource" selectedvalues="selectedCarrierValues" ' + required + ' onselectionchanged="onselectionchanged" datatextfield="Name" datavaluefield="CarrierAccountId"'
               + 'entityname="' + label + '"></vr-select></div>';
    }
    function BeCarrierGroup(ctrl, $scope, WhS_BE_CarrierAccountAPIService) {
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
            $scope.isLoadingDirective = true;
            return WhS_BE_CarrierAccountAPIService.GetCarrierAccounts(getCustomers, getSuppliers).then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.datasource.push(itm);
                });
            }).catch(function (error) {
                //TODO handle the case of exceptions
                        
            }).finally(function () {
                $scope.isLoadingDirective = false;
                defineAPI();
            });
        }
        function defineAPI() {
            var api = {};
            api.getData = function()
            {
                return $scope.selectedCarrierValues;
            }

            api.setData = function (selectedIds) {
                if ($attrs.ismultipleselection) {
                    for (var i = 0; i < selectedIds.length; i++) {
                        var selectedCarrierValue = UtilsService.getItemByVal($scope.datasource, selectedIds[i], "CarrierAccountId");
                        if (selectedCarrierValue != null)
                            $scope.selectedCarrierValues.push(selectedCarrierValue);
                    }
                } else {
                    var selectedCarrierValue = UtilsService.getItemByVal($scope.datasource, selectedIds, "CarrierAccountId");
                    if (selectedCarrierValue != null)
                        $scope.selectedCarrierValues=selectedCarrierValue;
                }
            }

            if (ctrl.onloaded != null)
                ctrl.onloaded(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

