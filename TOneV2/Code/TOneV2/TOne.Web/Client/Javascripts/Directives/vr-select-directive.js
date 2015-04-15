﻿
app.directive('vrSelect', ['SelectService', 'BaseDirService', 'ValidationMessagesEnum', function (SelectService, BaseDirService, ValidationMessagesEnum) {

    var directiveDefinitionObject = {

        require: '^form',
        restrict: 'E',
        scope: {
            label: '@',
            entityname: '@',
            ismultipleselection: '@',
            hideselectedvaluessection: '@',
            datavaluefield: '@',
            datatextfield: '@',
            hidefilterbox:'@',
            isrequired: '@',
            customvalidate: '&',
            datasource: '=',
            selectedvalues: '=',
            onselectionchanged: '='
        },
        controller: function ($scope, $element, $attrs) {
            var controller = this;
            this.ValidationMessagesEnum = ValidationMessagesEnum;
            this.limitcharactercount = $attrs.limitcharactercount;
            this.selectlbl = $attrs.selectlbl;
            this.filtername = '';
            this.showloading = false;
            this.data = {};
            
            this.isContainerVisible = function () {
                if (controller.isMultiple()) return true;
                if (controller.isRemoteLoad()) return true;
                if (controller.isEnFilter()) return true;
                return false;
            };

            this.getdatasource = function () {
                if (controller.isRemoteLoad()) return controller.data;
                return controller.datasource;
            };

            this.setdatasource = function (datasource) {
                if (controller.isRemoteLoad()) controller.data = datasource;
                else controller.datasource = datasource;
            };

            this.isRemoteLoad = function () {
                if (typeof (controller.datasource) == 'function') return true;
                return false;
            };

            this.isEnFilter = function () {
                var isEnable = false;
                if (controller.isMultiple()) isEnable= true;
                if (controller.isRemoteLoad()) isEnable = true;
                if (controller.hidefilterbox == "" || controller.hidefilterbox ) isEnable = false;
                return isEnable;
            };

            this.onClickLi = function (e) {
                if (controller.isMultiple()) controller.muteAction(e);
            };

            this.isMultiple = function () {
                return SelectService.isMultiple($attrs);
            };

            this.selectedSectionVisible = function () {
                if (controller.hideselectedvaluessection == "" || controller.hideselectedvaluessection) return false;
                if (controller.selectedvalues == undefined) return false;
                if (controller.selectedvalues.length > 0 && controller.isMultiple()) return true;
                return false;
            };
            

            this.getObjectProperty = function (item, property) {
                return BaseDirService.getObjectProperty(item, property);
            };

            this.getObjectText = function (item) {
                if (controller.datatextfield) return controller.getObjectProperty(item, controller.datatextfield);
                return item;
            };

            this.getObjectValue = function (item) {
                if (controller.datavaluefield) return controller.getObjectProperty(item, controller.datavaluefield);
                return item;
            };

            this.findExsite = function (item) {
                return BaseDirService.findExsite(controller.selectedvalues, controller.getObjectValue(item), controller.datavaluefield);
            };

            this.muteAction = function (e) {
                BaseDirService.muteAction(e);
            };

            this.clearFilter = function (e) {
                controller.muteAction(e);
                controller.filtername = '';
            };

            this.selectFirstItem = function () {
                controller.selectedvalues = [];
                controller.selectedvalues.length = 0;
                controller.selectedvalues.push(controller.getdatasource()[0]);
                return controller.getObjectText(controller.getdatasource()[0]);
            };

            this.getLabel = function () {
                
                if (! controller.isMultiple()) {

                    var lastValue = null;
                    if (Object.prototype.toString.call( controller.selectedvalues ) === '[object Array]')
                        lastValue = BaseDirService.getLastItem(controller.selectedvalues);
                    else
                        lastValue = controller.selectedvalues;

                    if (lastValue == null) {

                        if ($attrs.placeholder)
                            return $attrs.placeholder;

                        return controller.selectFirstItem();
                    }

                    var x = controller.getObjectText(lastValue);
                    if (x !== undefined)
                        return x;
                }
                
                var selectedVal = [];
                for (var i = 0; i < controller.selectedvalues.length; i++) {
                    selectedVal.push(controller.getObjectText(controller.selectedvalues[i]));
                    if (i == 2) break;
                }
                var s = SelectService.getSelectText(controller.selectedvalues.length, selectedVal, $attrs.placeholder, $attrs.selectplaceholder);
                return s;

            };
            
            this.getSelectedSectionClass = function () {
                if (!controller.selectedSectionVisible()) return 'single-col-checklist';
                return controller.selectedvalues.length == 0 ? 'single-col-checklist' : 'double-col-checklist';
            };

            
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

            function onLoad(){

                SelectService.setAttributes(attrs);

                var divDropdown = angular.element(element[0].querySelector('.dropdown'));
                var ulDropdown = angular.element(element[0].querySelector('.dropdown-menu'));
                attrs.id = BaseDirService.prepareDirectiveHTMLForValidation({}, divDropdown, undefined, divDropdown);
               
                var validateButtonClass = '';
                if (SelectService.validate(attrs)) {
                    validateButtonClass = 'ng-class="ctrl.isValidComp()"';

                    var validationOptions = {};
                    divDropdown.attr('ng-model', 'ctrl.selectedvalues');

                    if (attrs.isrequired !== undefined) {
                        if (SelectService.isMultiple(attrs)) {
                            validationOptions.requiredArray = true;
                            divDropdown.attr('ng-model', 'ctrl.selectedvalues.length');
                        }
                        else {
                            validationOptions.requiredValue = true;
                        }
                    }
                    if (attrs.customvalidate !== undefined)
                        validationOptions.customValidation = true;

                   
                    
                    attrs.id = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, divDropdown, undefined, divDropdown);
                    var validationTemplate = BaseDirService.getValidationMessageTemplate(true, true, false, true);
                    divDropdown.append(validationTemplate);
                }

                
                if (SelectService.isMenu(attrs.type)) {

                    var lblTemplate = '<label  id="dropdownMenuType" class="dropdown-toggle" style="padding-top:6px" data-toggle="dropdown" aria-expanded="true">'
                    + '<label class="hand-cursor" style="display: inline-block; min-width: 100px; font-size: 12px;'
                    + ' border-width: 0px 0px 1px 1px;border-style: solid;border-color: #F0F0F0; border-bottom-left-radius: 4px; padding: 5px; ">'
                    + '{{ctrl.getLabel()}}<span style="float:right;top:8px;position:relative" class="caret"></span></label> </label>';


                    angular.element(element[0].querySelector('.dropdown-container1')).attr('style', 'position: relative;');
                    angular.element(element[0].querySelector('.dropdown-container2')).attr('style', 'position: absolute;right: -10px;top: -16px;');
                    divDropdown.prepend(lblTemplate);
                    ulDropdown.addClass('menu-right');
                }
                else {

                    var buttonTemplate = '<button class="btn btn-default dropdown-toggle" style="width:100%;" type="button" data-toggle="dropdown" '
                                        + ' aria-expanded="true"  ' + validateButtonClass + '>'
                                        + '<span style="float: left; margin: 0px; ">{{ctrl.getLabel()}}</span>'
                                        + '<span style="position:absolute;top:13px;right:5px" class="caret"></span></button>';
                    divDropdown.prepend(buttonTemplate);
                }

                var labelTemplate = '';
                if (attrs.label != undefined)
                    labelTemplate = '<vr-label>' + attrs.label + '</vr-label>';
                angular.element(element[0].querySelector('.dropdown-container2')).prepend(labelTemplate);

                if (attrs.ismultipleselection !== undefined ) {
                    if (attrs.hideselectedvaluessection) ulDropdown.addClass('single-col-checklist');
                    else ulDropdown.attr('ng-class', 'ctrl.getSelectedSectionClass()');
                }

             
                setTimeout(function () {
                    $('div[name=' + attrs.id + ']').on('show.bs.dropdown', function (e) {
                        $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
                    });

                    $('div[name=' + attrs.id + ']').attr('name', attrs.id).on('hide.bs.dropdown', function (e) {
                        $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
                    });
                }, 100)
                setTimeout(function () {
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
                        $('.drop-down-inside-modal').removeClass("open");

                    };

                    $(".modal-body").unbind("scroll");
                    $(".modal-body").scroll(function () {
                        fixDropdownPosition();
                    });
                    $(window).resize(function () {
                        fixDropdownPosition();
                    });
                      }
                }, 1000)



                //setTimeout(function () {
                //    if (divDropdown.closest('.modal-body').length > 0) {

                //        divDropdown.on('click', '.dropdown-toggle', function (event) {

                //            var self = $(this);
                //            var selfHeight = $(this).parent().height();
                //            var selfWidth = $(this).parent().width();
                //            var selfOffset = $(self).offset();
                //            var selfOffsetRigth = $(document).width() - selfOffset.left - selfWidth;
                //            ulDropdown.css({ position: 'fixed', top: selfOffset.top + selfHeight, left: 'auto' });
                //        });

                //        var fixDropdownPosition = function () {
                //            ulDropdown.hide();
                //            divDropdown.removeClass("open");
                //        };

                //        $(".modal-body").unbind("scroll");
                //        $(".modal-body").scroll(function () {
                //            fixDropdownPosition();
                //        });
                //        $(window).resize(function () {
                //            fixDropdownPosition();
                //        });
                //    }
                //}, 1000)

            }

            onLoad();
            return {
                pre: function ($scope, iElem, iAttrs, formCtrl) {

                    var ctrl = $scope.ctrl;

                    BaseDirService.addScopeValidationMethods(ctrl, iAttrs.id, formCtrl);

                    ctrl.clearAllSelected = function (e) {
                        ctrl.muteAction(e);
                        ctrl.selectedvalues = [];
                        ctrl.selectedvalues.length = 0;
                    };

                    var selectItem = function (e, item) {

                        if (! ctrl.isMultiple()) {
                            ctrl.selectedvalues = item;// [];
                            //ctrl.selectedvalues.length = 0;
                            //ctrl.selectedvalues.push(item);
                               
                        }
                        else {
                            ctrl.muteAction(e);
                            var index = null;
                            try {
                                index = BaseDirService.findExsite(ctrl.selectedvalues, ctrl.getObjectValue(item), ctrl.datavaluefield);
                            }
                            catch (ex) {

                            }
                            if (index >= 0)
                                ctrl.selectedvalues.splice(index, 1);
                            else
                                ctrl.selectedvalues.push(item);
                        }
                    };

                    ctrl.selectValue = function (e, item) {
                        selectItem(e, item);
                        if (ctrl.onselectionchanged && typeof (ctrl.onselectionchanged) == 'function') {
                            var item = ctrl.onselectionchanged(ctrl.selectedvalues, ctrl.getdatasource());
                            if (item !== undefined) {
                                selectItem(null, item);
                            }
                        }
                    };
                    
                    ctrl.search = function () {
                        ctrl.setdatasource([]);
                        if (!ctrl.isRemoteLoad()) return;
                        if (ctrl.filtername.length > (iAttrs.limitcharactercount - 1)) {
                            ctrl.showloading = true;
                            ctrl.datasource(ctrl.filtername).then(function (items) {
                                ctrl.setdatasource(items);
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
            return SelectService.dTemplate;
        }
    };

    return directiveDefinitionObject;

}]);