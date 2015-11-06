'use strict';
app.directive('vrWhsBeSupplierzoneSelector', ['WhS_BE_SupplierZoneAPIService', 'UtilsService','VRUIUtilsService',
    function (WhS_BE_SupplierZoneAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "@",
                supplierid: "=",
                selectedvalues: '=',
                hidetitle:'@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new supplierZoneCtor(ctrl, $scope, $attrs);
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
                return getBeSupplierZoneTemplate(attrs);
            }

        };


        function getBeSupplierZoneTemplate(attrs) {
           
            var multipleselection = "";
            var label = 'label="Supplier Zone"';
            if (attrs.ismultipleselection != undefined)
            {
                multipleselection = "ismultipleselection";
                label = 'label="Supplier Zones"';
            }
            if (attrs.hidetitle != undefined) {
                label = "";
            }
            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            return '<div>'
               +  '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="SupplierZoneId"'
            + required + ' datasource="ctrl.searchSupplierZones" selectedvalues="ctrl.selectedvalues"'  +  label + 'onselectionchanged="ctrl.onselectionchanged" entityName="Supplier Zone"></vr-select>'
            + '</div>'
        }

        function supplierZoneCtor(ctrl, $scope, $attrs) {

            function initializeController() {

                ctrl.searchSupplierZones = function (searchValue) {
                    var filter = {
                        SupplierId: ctrl.supplierid,
                    }
                    return WhS_BE_SupplierZoneAPIService.GetSupplierZoneInfo(UtilsService.serializetoJson(filter), searchValue);
                }
                ctrl.supplierZones = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                   
                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload;
                    }
                    if (selectedIds != undefined) {
                        ctrl.datasource = [];
                        return WhS_BE_SupplierZoneAPIService.GetSupplierZoneInfoByIds(UtilsService.serializetoJson(selectedIds)).then(function (response) {
                        angular.forEach(response, function (item) {
                            ctrl.datasource.push(item);
                            
                        });
                        
                        VRUIUtilsService.setSelectedValues(selectedIds, 'SupplierZoneId', $attrs, ctrl);
                       
                    });
                  }
                   
                }
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SupplierZoneId', $attrs, ctrl);
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);