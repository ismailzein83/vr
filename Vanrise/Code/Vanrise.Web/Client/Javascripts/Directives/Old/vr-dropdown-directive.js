'use strict';
app.directive('vrDropdown', ['DropdownService', 'BaseDirService','ValidationMessagesEnum' , function (DropdownService, BaseDirService,ValidationMessagesEnum) {

    var directiveDefinitionObject = {

        require: '^form',
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
            limitcharactercount: '@',
            customvalidate: '&',
            onReady: '&',
        },
        controller: function ($scope, $element, $attrs) {
            var controller = this;
            this.api = { };
            this.customMessage = '';

            this.ValidationMessagesEnum =ValidationMessagesEnum;

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
                        var x = controller.getObjectText(controller.options.lastselectedvalue);
                        if (x !== undefined)
                            return x;
                    }
                    return controller.placeholder;
                }

                var selectedVal = [];
                for (var i = 0; i < controller.options.selectedvalues.length; i++) {
                    selectedVal.push(controller.getObjectText(controller.options.selectedvalues[i]));
                    if (i == 2) break;
                }
                var s = DropdownService.getSelectText(controller.options.selectedvalues.length, selectedVal, controller.placeholder, controller.selectplaceholder);
                return s;
            };

            this.getUlClass = function () {
                if (controller.options.selectedgroup !== undefined && !controller.options.selectedgroup) return 'single-col-checklist';
                return controller.options.selectedvalues.length == 0 ? 'single-col-checklist' : 'double-col-checklist';
            };

            this.isSelectedGroup = function () {
                if (controller.options.selectedgroup !== undefined && !controller.options.selectedgroup) return false;
                if (controller.options.selectedvalues.length > 0) return true;
                return false;
            };

            this.getLastSelectedValue = function () {
                if (controller.options.lastselectedvalue !== undefined) {
                    var x = controller.getObjectText(controller.options.lastselectedvalue);
                    if (x == undefined) return controller.getObjectText(controller.options.datasource[0]);
                    else return x;
                }
                else
                    return controller.getObjectText(controller.options.datasource[0]);
            };

            if (typeof (controller.onReady()) !== "undefined")
                controller.onReady()(controller.api);

            this.tooltip = false;
              
            this.showtooltip = function() {
                controller.tooltip = true;
            };

            this.hidetooltip = function() {
                controller.tooltip = false;
            };

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

            attrs.id = BaseDirService.guid();
            angular.element(element.children()[0]).attr('name', attrs.id);
            var divError = angular.element(element[0].querySelector('.tooltip-error'));
            if(attrs.isrequired !== undefined) {
                if (DropdownService.isSingleSelection(attrs.type)) {
                        angular.element(element.children()[0]).attr('vr-validation-value', '');
                    }
                    else {
                        angular.element(element.children()[0]).attr('vr-validation-array', '');
                    } 
                }


            if (attrs.customvalidate !== undefined) {
                angular.element(element.children()[0]).attr('vr-validation-custom', 'ctrl.customvalidate');
            }
            setTimeout(function () {
                $('div[name=' + attrs.id + ']').on('show.bs.dropdown', function (e) {
                    $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
                });

                $('div[name=' + attrs.id + ']').attr('name', attrs.id).on('hide.bs.dropdown', function (e) {
                    $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
                });
            }, 100);
            setTimeout(function () {
                //alert($('.drop-down-inside-modal').closest('.modal-body').length)
                if ($('div[name=' + attrs.id + ']').closest('.modal-body').length > 0) {

                    $('div[name=' + attrs.id + ']').on('click', '.dropdown-toggle', function (event) {

                        var self = $(this);
                        var selfHeight = $(this).parent().height();
                        var selfWidth = $(this).parent().width();
                        var selfOffset = $(self).offset();
                        var selfOffsetRigth = $(document).width() - selfOffset.left - selfWidth;
                        var dropDown = self.parent().find('ul');
                        $(dropDown).css({ position: 'fixed', top: selfOffset.top + selfHeight, left: 'auto' });
                    });

                    var fixDropdownPosition = function () {
                        $('.drop-down-inside-modal').find('.dropdown-menu').hide();

                    };

                    $(".modal-body").unbind("scroll");
                    $(".modal-body").scroll(function () {
                        fixDropdownPosition();
                    });
                    $(window).resize(function () {
                        fixDropdownPosition();
                    });
                }
            }, 1000);

           
          
            attrs = DropdownService.setDefaultAttributes(attrs);

            return {
                pre: function ($scope, iElem, iAttrs, formCtrl) {

                    var ctrl = $scope.ctrl;

                    $scope.clearDatasource = function () {
                        if (ctrl.options.datasource == undefined) return;
                        ctrl.options.datasource = [];
                        ctrl.options.datasource.length = 0;
                    };

                    $scope.clearAllSelected = function (e) {
                        ctrl.muteAction(e);
                        ctrl.options.lastselectedvalue = '';
                        ctrl.options.selectedvalues = [];
                        ctrl.options.selectedvalues.length = 0;
                    };

                    var selectval = function (e, item) {

                        if (ctrl.singleSelection()) {
                            ctrl.options.lastselectedvalue = '';
                            ctrl.options.selectedvalues = [];
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

                    var validationClass = {
                        invalid: "required-inpute", valid: ''
                    };
                    var index = -1;

                    var isValidElem = function () {
                        if (iAttrs.isrequired !== undefined || iAttrs.customvalidate !== undefined)
                            return formCtrl[iAttrs.id].$valid;
                        return true;
                    };

                    $scope.getErrorObject = function () {
                        return formCtrl[iAttrs.id].$error;
                    };

                    $scope.isvisibleTooltip = function () {
                        if (isValidElem()) return false;
                        return ctrl.tooltip;
                    };

                    $scope.isvalidcomp = function () {

                        if (isValidElem()) return validationClass.valid;

                        return validationClass.invalid;

                        //if (ctrl.options.validationGroup == undefined) {
                        //    ctrl.options.validationGroup = [];
                        //    index = ctrl.options.validationGroup.push(isvalid) - 1;
                        //}
                        //else {
                        //    if (index != -1)
                        //        ctrl.options.validationGroup[index] = isvalid;
                        //    else
                        //        index = ctrl.options.validationGroup.push(isvalid) - 1;
                        //}

                    };

                    $scope.search = function () {
                        $scope.clearDatasource();
                        if (ctrl.filtername.length > (ctrl.limitcharactercount - 1)) {
                            ctrl.showloading = true;
                            ctrl.onsearch()(ctrl.filtername).then(function (items) {
                                ctrl.options.datasource = items;
                                ctrl.showloading = false;
                            }, function (msg) {
                                console.log(msg);
                            });
                        }
                    };
                }
            };
        },
            templateUrl: function (element, attrs) {
                if (attrs.type == undefined) return DropdownService.dTemplate;
                else return DropdownService.getTemplateByType(attrs.type);
        }
    };

    return directiveDefinitionObject;

}]);