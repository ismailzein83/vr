
app.directive('vrSelect', ['DropdownService', 'BaseDirService', 'ValidationMessagesEnum', function (DropdownService, BaseDirService, ValidationMessagesEnum) {

    var directiveDefinitionObject = {

        require: '^form',
        restrict: 'E',
        scope: {
            label: '@',
            entityname: '@',
            ismultiplesection: '@',
            hideselectedvaluessection: '@',
            datavaluefield: '@',
            datatextfield: '@',
            hidefilterbox:'@',
            isrequired: '@',
            validator: '@',
            datasource: '=',
            selectedvalues: '=',
            onselectionchanged: '='
        },
        controller: function ($scope, $element, $attrs) {
            var controller = this;
            
            this.ValidationMessagesEnum = ValidationMessagesEnum;

            this.filtername = '';
            this.showloading = false;

            this.remoteFilter = function () {
                if(typeof(controller.datasource) == 'function') return true;
                return false;
            };

            this.enableFilter = function () {
                if (controller.hidefilterbox) return false;
                if (typeof (controller.datasource) == 'function') return true;
                if (ismultiplesection) return true;
                return false;
            };

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
                return BaseDirService.findExsite(controller.selectedvalues, controller.getObjectValue(item), controller.datavaluefield);
            };

            this.muteAction = function (e) {
                BaseDirService.muteAction(e);
            };

            this.clearFilter = function (e) {
                controller.muteAction(e);
                controller.filtername = '';
            };

            this.getLastSelectedValue = function () {

                if (controller.selectedvalues == undefined) return null;

                if (controller.selectedvalues.length > 0)
                    return controller.selectedvalues[controller.selectedvalues.length - 1];
                return null;
            };

            this.getSelectText = function () {

                if (controller.singleSelection()) {
                    if (controller.getLastSelectedValue() !== undefined || controller.getLastSelectedValue() !== '') {
                        var x = controller.getObjectText(controller.getLastSelectedValue());
                        if (x !== undefined)
                            return x;
                    }
                    return controller.placeholder;
                }

                var selectedVal = [];
                for (var i = 0; i < controller.selectedvalues.length; i++) {
                    selectedVal.push(controller.getObjectText(controller.selectedvalues[i]));
                    if (i == 2) break;
                }
                var s = DropdownService.getSelectText(controller.selectedvalues.length, selectedVal, controller.placeholder, controller.selectplaceholder);
                return s;
            };

            this.selectedSectionVisible = function () {
                if (controller.hideselectedvaluessection) return false;
                if (controller.selectedvalues == undefined) return false;
                if (controller.selectedvalues.length > 0) return true;
                return false;
            };

            this.getSelectedSectionClass = function () {
                return controller.selectedvalues.length == 0 ? 'single-col-checklist' : 'double-col-checklist';
            };



            this.tooltip = false;

            this.showtooltip = function () {
                controller.tooltip = true;
            };

            this.hidetooltip = function () {
                controller.tooltip = false;
            };

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

            function validate() {
                if (attrs.isrequired == undefined && attrs.validator == undefined) return false;
                return true;
            }

            function onLoad(){
                
                var divDropdown = angular.element(element[0].querySelector('.dropdown'));

                attrs.id = BaseDirService.guid();
                divDropdown.attr('name', attrs.id);

                var validateButtonClass = '';
                if (validate()) {
                    validateButtonClass = 'ng-class="isvalidcomp()"';
                    divDropdown.attr('ng-model', 'ctrl.selectedvalues.length');
                    divDropdown.attr('ng-mouseenter', 'ctrl.showtooltip();');
                    divDropdown.attr('ng-mouseleave', 'ctrl.hidetooltip();');

                    if (attrs.isrequired !== undefined) {
                        if (DropdownService.isSingleSelection(attrs.type)) {
                            divDropdown.attr('vr-validation-value', '');
                        }
                        else {
                            divDropdown.attr('vr-validation-array', '');
                        }
                    }

                    if (attrs.customvalidate !== undefined) {
                        divDropdown.attr('vr-validation-custom', 'ctrl.customvalidate');
                    }

                    var validationTemplate = '<div  class="tooltip-error disable-animations " ng-show="isvisibleTooltip()" ng-messages="getErrorObject()">'
                    + '<div ng-message="requiredarray">{{ ctrl.ValidationMessagesEnum.required }}</div>'
                    + '<div ng-message="requiredvalue">{{ ctrl.ValidationMessagesEnum.required }}</div>'
                    + '<div ng-message="customvalidation">{{ customMessage.custom }}</div></div>';

                    divDropdown.append(validationTemplate);
                }

                if (DropdownService.isMenu(attrs.type)) {

                    var labelTemplate = '<label  id="dropdownMenuType" class="dropdown-toggle" style="padding-top:6px" data-toggle="dropdown" aria-expanded="true">'
                    + '<label class="hand-cursor" style="display: inline-block; min-width: 100px; font-size: 12px;'
                    + ' border-width: 0px 0px 1px 1px;border-style: solid;border-color: #F0F0F0; border-bottom-left-radius: 4px; padding: 5px; ">'
                    + '{{ctrl.getLabel()}}<span style="float:right;top:8px;position:relative" class="caret"></span></label> </label>';


                    angular.element(element[0].querySelector('.dropdown-container1')).attr('style', 'position: relative;');
                    angular.element(element[0].querySelector('.dropdown-container2')).attr('style', 'position: absolute;right: -10px;top: -16px;');
                    divDropdown.prepend(labelTemplate);
                    angular.element(element[0].querySelector('.dropdown-menu')).attr('class', 'menu-right');
                }
                else {

                    var buttonTemplate = '<button class="btn btn-default dropdown-toggle" style="width:100%;" type="button" data-toggle="dropdown" '
                                        + ' aria-expanded="true"  ' + validateButtonClass + '>'
                                        + '<span style="float: left; margin: 0px; ">{{ctrl.getSelectText()}}</span>'
                                        + '<span style="position:absolute;top:13px;right:5px" class="caret"></span></button>';
                    divDropdown.prepend(buttonTemplate);
                }

                if (attrs.ismultiplesection !== undefined ) {
                    divDropdown.attr('class', 'single-col-checklist');
                }
                else {
                    if (attrs.hideselectedvaluessection)  divDropdown.attr('class', 'single-col-checklist');
                    else divDropdown.attr('ng-class', 'ctrl.getSelectedSectionClass()');
                }

            }

            onLoad();

            
            setTimeout(function () {
                $('div[name=' + attrs.id + ']').on('show.bs.dropdown', function (e) {
                    $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
                });

                $('div[name=' + attrs.id + ']').on('hide.bs.dropdown', function (e) {
                    $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
                });
            }, 100)


            attrs = DropdownService.setDefaultAttributes(attrs);

            return {
                pre: function ($scope, iElem, iAttrs, formCtrl) {

                    var ctrl = $scope.ctrl;

                    $scope.clearDatasource = function () {
                        if (ctrl.datasource == undefined) return;
                        ctrl.datasource = [];
                        ctrl.datasource.length = 0;
                    };

                    $scope.clearAllSelected = function (e) {
                        ctrl.muteAction(e);
                        ctrl.selectedvalues = [];
                        ctrl.selectedvalues.length = 0;
                    };

                    var selectval = function (e, item) {

                        if (ctrl.singleSelection()) {
                            ctrl.selectedvalues = [];
                            ctrl.selectedvalues.length = 0;
                            ctrl.selectedvalues.push(item);
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

                    $scope.selectValue = function (e, item) {
                        selectval(e, item);
                        ctrl.getLastSelectedValue() = item;
                        if (typeof (ctrl.onselectionchange()) !== "undefined") {
                            var item = ctrl.onselectionchange()(ctrl.selectedvalues, ctrl.getLastSelectedValue(), ctrl.datasource);
                            if (item !== undefined) {
                                ctrl.getLastSelectedValue() = item;
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
                    }

                    $scope.isvisibleTooltip = function () {
                        if (isValidElem()) return false;
                        return ctrl.tooltip;
                    };

                    $scope.isvalidcomp = function () {

                        if (isValidElem()) return validationClass.valid;

                        return validationClass.invalid;
                    }

                    $scope.search = function () {
                        $scope.clearDatasource();
                        if (ctrl.filtername.length > (ctrl.limitcharactercount - 1)) {
                            ctrl.showloading = true;
                            ctrl.onsearch()(ctrl.filtername).then(function (items) {
                                ctrl.datasource = items;
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
            return DropdownService.dSelectTemplate;
        }
    };

    return directiveDefinitionObject;

}]);