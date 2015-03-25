'use strict';

app.directive('vrDropdown', ['DropdownService', 'BaseDirService', function (DropdownService, BaseDirService) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            options:'=',
            datatextfield: '@',
            datavaluefield: '@',
            selectlbl: '@',
            placeholder: '@',
            selectplaceholder: '@',
            entityname: '@',
            onselectionchange: '&',
            type: '@',
            multipleselection: '@',
            onsearch: '&',
            limitplaceholder: '@',
            limitcharactercount: '@'
        },
        controller: function () {
            var controller = this;

            this.filtername = '';
            this.showloading = false;
            
            this.singleSelection = function () {
                return DropdownService.isSingleSelection(controller.type);
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
                return BaseDirService.findExsite(controller.options.selectedvalues, controller.getObjectValue(item), controller.datavaluefield);
            };

            this.muteAction = function (e) {
                BaseDirService.muteAction(e);
            };

            this.clearFilter = function (e) {
                controller.muteAction(e);
                controller.filtername = '';
            };

            this.getSelectText = function () {

                if (controller.singleSelection()) {
                    if (controller.options.lastselectedvalue !== undefined || controller.options.lastselectedvalue !== '') {
                        return controller.getObjectText(controller.options.lastselectedvalue);
                    }
                    return controller.placeholder;
                }
                
                var selectedVal =[];
                for (var i = 0; i < controller.options.selectedvalues.length; i++) {
                    selectedVal.push(controller.getObjectText(controller.options.selectedvalues[i]));
                    if (i == 2) break;
                    }
                var s = DropdownService.getSelectText( controller.options.selectedvalues.length, selectedVal, controller.placeholder, controller.selectplaceholder);
                return s;
            };

            this.getUlClass = function () {
                if (controller.options.selectedgroup !== undefined && !controller.options.selectedgroup) return 'single-col-checklist';
                return controller.options.selectedvalues.length == 0 ? 'single-col-checklist' : 'double-col-checklist';
            };

            this.isSelectedGroup = function() {
                if (controller.options.selectedgroup !== undefined && !controller.options.selectedgroup) return false;
                if (controller.options.selectedvalues.length > 0) return true;
                return false;
            };

            this.getLastSelectedValue = function () {
                if (controller.options.lastselectedvalue !== undefined){
                    var x = controller.getObjectText(controller.options.lastselectedvalue);
                    if (x == undefined) return controller.getObjectText(controller.options.datasource[0]);
                    else return x;
                }
                else
                    return controller.getObjectText(controller.options.datasource[0]);
            };

            },
        controllerAs: 'ctrl',
        bindToController: true,
            compile: function (element, attrs) {

                angular.element(element.children()[0]).on('show.bs.dropdown', function (e) {
                    $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
            });

            angular.element(element.children()[0]).on('hide.bs.dropdown', function (e) {
                $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
            });

            attrs = DropdownService.setDefaultAttributes(attrs);

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                    $scope.clearDatasource = function () {
                        if (ctrl.options.datasource == undefined) return;
                            ctrl.options.datasource =[];
                            ctrl.options.datasource.length = 0;
                    };

                    $scope.clearAllSelected = function (e) {
                        ctrl.muteAction(e);
                        ctrl.options.selectedvalues = [];
                        ctrl.options.selectedvalues.length = 0;
                    };

                    var selectval = function (e, item) {
                        
                        if (ctrl.singleSelection()) {
                            
                                ctrl.options.selectedvalues =[];
                                            ctrl.options.selectedvalues.length = 0;
                                            ctrl.options.selectedvalues.push(item);
                                }
                                else {
                                            ctrl.muteAction(e);
                                            var index = null;
                                            try {
                                                index = BaseDirService.findExsite(ctrl.options.selectedvalues, ctrl.getObjectValue(item), ctrl.datavaluefield);
                                }
                                            catch (ex) {

                                }
                                            if (index >= 0)
                                                ctrl.options.selectedvalues.splice(index, 1);
                                else
                                                ctrl.options.selectedvalues.push(item);
                            }
                            };

                    $scope.selectValue = function (e, item) {
                        selectval(e, item);
                        ctrl.options.lastselectedvalue = item;
                            if (typeof (ctrl.onselectionchange()) !== "undefined") {
                                    var item = ctrl.onselectionchange()(ctrl.options.selectedvalues, ctrl.options.lastselectedvalue, ctrl.options.datasource);
                                    if (item !== undefined) {
                                        ctrl.options.lastselectedvalue = item;
                                        selectval(null, item);
                                    }

                                }
                            };

                    $scope.search = function () {
                        $scope.clearDatasource();
                        if (ctrl.filtername.length > (ctrl.limitcharactercount -1)) {
                            ctrl.showloading = true;
                            ctrl.onsearch() (ctrl.filtername).then(function (items) {
                                ctrl.options.datasource = items;
                                ctrl.showloading = false;
                                }, function (msg) {
                                    console.log(msg);
                        });
                    }
                    };

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