'use strict';

app.directive('vrDropdown', ['DropdownService', 'BaseDirService', function (DropdownService, BaseDirService) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            datasource: '=',
            datatextfield: '@',
            datavaluefield: '@',
            selectlbl: '@',
            placeholder: '@',
            selectplaceholder: '@',
            entityname: '@',
            onselectionchange: '&',
            type: '@',
            selectedvalues: '=',
            lastselectedvalue: '=',
            multipleselection: '@',
            onsearch: '&',
            limitplaceholder: '@',
            limitcharactercount:'@'
        },
        controller: function () {
            var controller = this;
            this.selectedValues = [];
            this.filtername = '';
            this.showloading = false;

            this.singleSelection = function () {
                return DropdownService.isSingleSelection(controller.type);
            };

            this.getDatasource = function () {
                return controller.datasource;
            };

            this.getObjectProperty = function (item, property) {
                return BaseDirService.getObjectProperty(item, property);
            };

            this.getObjectText = function (item) {
                return controller.getObjectProperty(item, controller.datatextfield);
            };

            this.getObjectValue = function (item) {
                return controller.getObjectProperty(item, controller.datavaluefield);
            };

            this.findExsite = function (item) {
                return BaseDirService.findExsite(controller.selectedValues, controller.getObjectValue(item), controller.datavaluefield);
            };

            this.muteAction = function (e) {
                BaseDirService.muteAction(e);
            };

            this.selectValue = function (e, c) {
                    var index = null;
                    controller.muteAction(e);
                    try {
                        var index = controller.selectedValues.indexOf(c);
                    }
                    catch (e) {

                    }
                    if (index >= 0)
                        controller.selectedValues.splice(index, 1);
                    else
                        controller.selectedValues.push(c);
                controller.onselectionchange()(controller.selectedValues);
            };

            this.clearAllSelected = function (e) {
                controller.muteAction(e);
                controller.selectedValues = [];
                controller.selectedValues.length = 0;
                controller.onselectionchange()(controller.selectedValues);
            };

            this.clearFilter = function (e) {
                controller.muteAction(e);
                controller.filtername = '';
            };

            this.getSelectText = function () {

                var selectedVal = [];

                for (var i = 0 ; i < controller.selectedValues.length ; i++)
                {
                    selectedVal.push(controller.getObjectText(controller.selectedValues[i]));
                    if (i == 2) break;
                }
                return DropdownService.getSelectText(controller.singleSelection(),controller.selectedValues.length, selectedVal, controller.placeholder, controller.selectplaceholder);
            };

            this.getUlClass = function () {
                return controller.selectedValues.length == 0 ? 'single-col-checklist' : 'double-col-checklist';
            };
    
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

            $('.dropdown').on('show.bs.dropdown', function (e) {
                $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
            });

            $('.dropdown').on('hide.bs.dropdown', function (e) {
                $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
            });

            attrs = DropdownService.setDefaultAttributes(attrs);

            return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {
                    $scope.clearDatasource = function () {
                        if (ctrl.datasource == undefined) return;
                            ctrl.datasource =[];
                            ctrl.datasource.length = 0;
                    };

                    $scope.search = function () {
                        $scope.clearDatasource();
                        if (ctrl.filtername.length > (ctrl.limitcharactercount -1)) {
                            ctrl.showloading = true;
                            ctrl.onsearch() (ctrl.filtername).then(function (items) {
                                ctrl.datasource = items;
                                ctrl.showloading = false;
                                }, function (msg) {
                                    console.log(msg);
                            });
                        }
                    };

                    $scope.refreshOutput = function () {

                        if (ctrl.selectedvalues == undefined && ctrl.lastselectedvalue == undefined) return;

                        if (ctrl.selectedvalues != undefined)
                        {
                            ctrl.selectedvalues = [];
                            ctrl.selectedvalues.length = 0;
                        }
                        if (ctrl.lastselectedvalue != undefined)
                            ctrl.lastselectedvalue = '';
                        
                        angular.forEach(ctrl.selectedValues, function (value, key) {
                            if (typeof value !== 'undefined') {
                                var temp = angular.copy(value);
                                if (ctrl.lastselectedvalue != undefined)  ctrl.lastselectedvalue = temp;
                                if (ctrl.selectedvalues != undefined) ctrl.selectedvalues.push(temp);
                            }
                        });
                    }
                }
            }
        },
        templateUrl: function (element, attrs) {
            if (attrs.type == undefined) return DropdownService.dTemplate;
            else return DropdownService.getTemplateByType(attrs.type);
        }
    };

    return directiveDefinitionObject;

}]);