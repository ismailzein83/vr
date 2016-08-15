(function (app) {

    "use strict";

    function vrSelectDirective(selectService, baseDirService, validationMessagesEnum, utilsService, VRValidationService) {

        var openedDropDownIds = [], rootScope;
        var vrSelectSharedObject = {
            onOpenDropDown: function (idAttribute) {
                rootScope.$apply(function () {
                    openedDropDownIds.push(idAttribute);
                });
            },
            onCloseDropDown: function (idAttribute) {
                var index = openedDropDownIds.indexOf(idAttribute);
                if (index >= 0) {
                    openedDropDownIds.splice(index, 1);
                    setTimeout(function () {
                        rootScope.$apply(function () {
                        });
                    }, 300);
                }
            },
            isDropDownOpened: function (idAttribute) {
                return openedDropDownIds.indexOf(idAttribute) >= 0;
            }
        };

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                label: '@',
                entityname: '@',
                onReady: '=',
                ismultipleselection: '@',
                hideselectedvaluessection: '@',
                datavaluefield: '@',
                datatextfield: '@',
                hidefilterbox: '@',
                datasource: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                onaddclicked: "=",
                hint: '@',
                haspermission: '='
            },
            controller: function ($scope, $element, $attrs) {
                if (rootScope == undefined)
                    rootScope = $scope.$root;

                var controller = this;

                controller.validate = function () {
                    return VRValidationService.validate(controller.selectedvalues, $scope, $attrs);
                };

                controller.boundDataSource = [];
                var itemsToAddToSource;
                $scope.$watch('isDropDownOpened', function (newValue, oldValue) {
                    $scope.effectiveDataSource = getdatasource();
                    refreshBoundDataSource();
                });

                $scope.$watchCollection('effectiveDataSource', function (newValue, oldValue) {
                    refreshBoundDataSource();
                });

                controller.hideAddButton = false;
                if (controller.haspermission != undefined && typeof (controller.haspermission) == 'function') {
                    controller.haspermission().then(function (isAllowed) {
                        if (!isAllowed)
                            controller.hideAddButton = true;
                    });
                }

                function refreshBoundDataSource()
                {
                    if (!$scope.isDropDownOpened)
                        return;
                    controller.boundDataSource.length = 0;
                    
                    if ($scope.effectiveDataSource != undefined) {
                        itemsToAddToSource = [];
                        for (var i = 0; i < $scope.effectiveDataSource.length; i++)
                            itemsToAddToSource.push($scope.effectiveDataSource[i]);
                        addBatchItemsToSource();
                    }
                }

                function addBatchItemsToSource() {
                    var numberOfItems = 10;
                    for (var i = 0; i < numberOfItems; i++) {
                        if (itemsToAddToSource.length > 0) {
                            controller.boundDataSource.push(itemsToAddToSource[0]);
                            itemsToAddToSource.splice(0, 1);
                        }
                    }
                    
                    if (itemsToAddToSource.length > 0) {
                        setTimeout(function () {
                            addBatchItemsToSource();
                            $scope.$apply(function () {

                            });
                        }, 250);
                    }
                }

                //Configuration
                angular.extend(this, {
                    ValidationMessagesEnum: validationMessagesEnum,
                    limitcharactercount: $attrs.limitcharactercount,
                    limitHeight: (($attrs.limitheight != undefined) ? 'limit-height' : ''),
                    selectlbl: $attrs.selectlbl,
                    filtername: '',
                    showloading: false,
                    data: [],
                    hint: (($attrs.hint != undefined) ? $attrs.hint : undefined)
                });
                //Configuration

                function isDropDownOpened() {
                    $scope.isDropDownOpened = vrSelectSharedObject.isDropDownOpened($attrs.id);
                    return $scope.isDropDownOpened;
                }

                function isHideRemoveIcon() {
                    return $attrs.hideremoveicon != undefined;
                }

                function isMultiple() {
                    return selectService.isMultiple($attrs);
                }

                function isRemoteLoad() {
                    if (typeof (controller.datasource) == 'function') return true;
                    return false;
                }

                function isEnFilter() {
                    var isEnable = false;
                    if (isMultiple()) isEnable = true;
                    if (isRemoteLoad()) isEnable = true;
                    if (controller.hidefilterbox === "" || controller.hidefilterbox) isEnable = false;
                    return isEnable;
                }

                function isContainerVisible() {
                    if (isMultiple()) return true;
                    if (isRemoteLoad()) return true;
                    if (isEnFilter()) return true;
                    return false;
                }

                function getdatasource() {
                    if (isRemoteLoad()) return controller.data;
                    return controller.datasource;
                }

                function withLocalFiter() {
                    return !(controller.hidefilterbox === "" || controller.hidefilterbox);
                }

                

                

                function  adjustTooltipPosition(e) {
                    setTimeout(function () {
                        var self = angular.element(e.currentTarget);
                        var selfHeight = $(self).height();
                        var selfOffset = $(self).offset();
                        var tooltip = self.parent().find('.tooltip-info')[0];
                        $(tooltip).css({ display: 'block' });
                        var innerTooltip = self.parent().find('.tooltip-inner')[0];
                        var innerTooltipArrow = self.parent().find('.tooltip-arrow')[0];
                        var innerTooltipWidth = parseFloat(($(innerTooltip).width() / 2) + 2.5);
                        $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 15, left: selfOffset.left - innerTooltipWidth });
                        $(innerTooltipArrow).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 10, left: selfOffset.left });

                    }, 1);
                }

                function setdatasource(datasource) {
                    if (isRemoteLoad()) controller.data = datasource;
                    else controller.datasource = datasource;
                    $scope.effectiveDataSource = getdatasource();
                    refreshBoundDataSource();
                }

                function muteAction(e) {
                    baseDirService.muteAction(e);
                }

                function onClickLi(e) {
                    if (isMultiple()) muteAction(e);
                }

                function selectedSectionVisible() {
                    if (controller.hideselectedvaluessection === "" || controller.hideselectedvaluessection) return false;
                    if (controller.selectedvalues == undefined) return false;
                    if (controller.selectedvalues.length > 0 && isMultiple()) return true;
                    return false;
                }

                function getObjectProperty(item, property) {
                    return baseDirService.getObjectProperty(item, property);
                }

                function getObjectText(item) {
                    if (controller.datatextfield) return getObjectProperty(item, controller.datatextfield);
                    return item;
                }

                function getObjectValue(item) {
                    if (controller.datavaluefield) return getObjectProperty(item, controller.datavaluefield);
                    return item;
                };

                function findExsite(item) {
                    return utilsService.getItemIndexByVal(controller.selectedvalues, getObjectValue(item), controller.datavaluefield);
                }

                function onAddhandler() {
                    var onAddHandler = $scope.$parent.$eval($attrs.onaddclicked);
                    onAddHandler();
                }

                function includeOnAddHandler() {
                    var onAddHandler = $scope.$parent.$eval($attrs.onaddclicked);
                    return (onAddHandler != undefined && typeof (onAddHandler) == 'function');
                }
                function clearFilter(e) {
                    muteAction(e);
                    controller.filtername = '';
                }

                function selectFirstItem() {
                    controller.selectedvalues = [];
                    controller.selectedvalues.length = 0;
                    controller.selectedvalues.push(getdatasource()[0]);
                    return getObjectText(getdatasource()[0]);
                }

                function getLabel() {

                    if (!isMultiple()) {

                        var lastValue;
                        if (Object.prototype.toString.call(controller.selectedvalues) === '[object Array]')
                            lastValue = baseDirService.getLastItem(controller.selectedvalues);
                        else
                            lastValue = controller.selectedvalues;

                        if (lastValue == null) {

                            if ($attrs.placeholder)
                                return $attrs.placeholder;

                            return selectFirstItem();
                        }

                        var x = getObjectText(lastValue);
                        if (x !== undefined)
                            return x;
                    }

                    var selectedVal = [];
                    for (var i = 0; i < controller.selectedvalues.length; i++) {
                        selectedVal.push(getObjectText(controller.selectedvalues[i]));
                        if (i === 2) break;
                    }
                    var s = selectService.getSelectText(controller.selectedvalues.length, selectedVal, $attrs.placeholder, $attrs.selectplaceholder);
                    return s;
                }

                function getSelectedSectionClass() {
                    if (!selectedSectionVisible()) return 'single-col-checklist';
                    return controller.selectedvalues.length === 0 ? 'single-col-checklist' : 'double-col-checklist';
                }
                
                //Exports
                angular.extend(this, {
                    isDropDownOpened: isDropDownOpened,
                    isHideRemoveIcon: isHideRemoveIcon,
                    isContainerVisible: isContainerVisible,
                    isMultiple: isMultiple,
                    isRemoteLoad: isRemoteLoad,
                    isEnFilter: isEnFilter,
                    getdatasource: getdatasource,
                    withLocalFiter: withLocalFiter,
                    //getInputeStyle: getInputeStyle,
                    adjustTooltipPosition: adjustTooltipPosition,
                    setdatasource: setdatasource,
                    onClickLi: onClickLi,
                    muteAction: muteAction,
                    selectedSectionVisible: selectedSectionVisible,
                    getObjectText: getObjectText,
                    getObjectValue: getObjectValue,
                    findExsite: findExsite,
                    clearFilter: clearFilter,
                    selectFirstItem: selectFirstItem,
                    getLabel: getLabel,
                    getSelectedSectionClass: getSelectedSectionClass,
                    onAddhandler: onAddhandler,
                    includeOnAddHandler: includeOnAddHandler
                });
                var api = {}
                api.clearDataSource = function () {
                    if (controller.isRemoteLoad()) {
                        controller.filtername = "";
                        controller.data.length = 0;
                    }

                    else
                        controller.datasource.length = 0;

                    if (controller.selectedvalues != undefined) {
                        if (controller.selectedvalues.length != undefined)
                            controller.selectedvalues.length = 0;
                        else
                            controller.selectedvalues = undefined;
                    }
                }
                if (controller.onReady != null) {
                    controller.onReady(api);
                }
                //Exports
                setTimeout(function () {
                    $('div[name=' + $attrs.id + ']').on('show.bs.dropdown', function () {
                        
                        vrSelectSharedObject.onOpenDropDown($attrs.id);

                        setTimeout(function () { $('#filterInput').focus(); }, 1);
                        var selfHeight = $(this).height();
                        var selfOffset = $(this).offset();
                        var basetop = selfOffset.top - $(window).scrollTop() + selfHeight;
                       
                        var heigth = $(this).parents('.vr-pager-container').length > 0 ? 235 : 200;

                        if (innerHeight - basetop < heigth) {
                            var div = $(this).find('.dropdown-menu');
                            var height = div.css({
                                display: "block"
                            }).height();

                            div.css({
                                overflow: "hidden",
                                marginTop: height,
                                height: 0
                            }).animate({
                                marginTop: 0,
                                height: height
                            }, 1000, function () {
                                $(this).css({
                                    display: "",
                                    overflow: "",
                                    height: "",
                                    marginTop: ""
                                });
                            });
                           
                        }
                        else
                           $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
                    });

                    $('div[name=' + $attrs.id + ']').attr('name', $attrs.id).on('hide.bs.dropdown', function () {
                        
                        $('#filterInput').blur();

                        vrSelectSharedObject.onCloseDropDown($attrs.id);
                        $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
                    });
                }, 100);
                setTimeout(function () {
                   // if ($('div[name=' + $attrs.id + ']').parents('.modal-body').length > 0) {

                        $('div[name=' + $attrs.id + ']').on('click', '.dropdown-toggle', function () {

                            var self = $(this);
                            var selfHeight = $(this).parent().height();
                            var selfOffset = $(self).offset();
                            var dropDown = self.parent().find('ul');
                            var top = 0;
                            var basetop = selfOffset.top - $(window).scrollTop() + selfHeight;
                            var baseleft = selfOffset.left - $(window).scrollLeft();
                         
                            var heigth = $(this).parents('.vr-pager-container').length > 0 ? 245 : 200;
                            if (innerHeight - basetop < heigth)
                                top = basetop - (heigth + (selfHeight * 2.7));
                            else
                                top = selfOffset.top - $(window).scrollTop() + selfHeight;

                            $(dropDown).css({ position: 'fixed', top: top, left: baseleft });
                        });

                        $('div[name=' + $attrs.id + ']').parents('div').scroll(function () {
                            fixDropdownPosition();
                        });
                        $(window).scroll(function () {
                            fixDropdownPosition();
                        });
                        $(window).resize(function () {
                            fixDropdownPosition();
                        });

                  //  }

                }, 1);
                var fixDropdownPosition = function () {
                    $('.drop-down-inside-modal').find('.dropdown-menu').hide();
                    $('.drop-down-inside-modal').removeClass("open");

                };
                $scope.$on('start-drag', function (event, args) {
                    fixDropdownPosition();
                });
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
               

                function onLoad() {

                    selectService.setAttributes(attrs);

                    var divDropdown = angular.element(element[0].querySelector('.dropdown'));
                    var ulDropdown = angular.element(element[0].querySelector('.dropdown-menu'));
                    var tabindex = '';
                    attrs.id = baseDirService.generateHTMLElementName();
                    divDropdown.attr('name', attrs.id);

                    if ($(divDropdown).parents().hasClass('divDisabled') == true) {
                        tabindex = 'tabindex=-1'
                    }
                    //attrs.id = baseDirService.prepareDirectiveHTMLForValidation({}, divDropdown, undefined, divDropdown);

                    var validateButtonClass = '';
                    if (selectService.validate(attrs)) {
                        //validateButtonClass = 'ng-class="ctrl.isValidComp()"';

                        //var validationOptions = {};
                        //divDropdown.attr('ng-model', 'ctrl.selectedvalues');

                        //if (attrs.isrequired !== undefined) {
                        //    if (selectService.isMultiple(attrs)) {
                        //        validationOptions.requiredArray = true;
                        //        divDropdown.attr('ng-model', 'ctrl.selectedvalues.length');
                        //    }
                        //    else {
                        //        validationOptions.requiredValue = true;
                        //    }
                        //}
                        //if (attrs.customvalidate !== undefined)
                        //    validationOptions.customValidation = true;



                        attrs.id = baseDirService.generateHTMLElementName();
                        divDropdown.attr('name', attrs.id);
                        //attrs.id = baseDirService.prepareDirectiveHTMLForValidation(validationOptions, divDropdown, undefined, divDropdown);
                        //var validationTemplate = baseDirService.getValidationMessageTemplate(true, true, false, true);
                        //divDropdown.append(validationTemplate);
                    }
                    var lblTemplate;
                    if (selectService.isMenu(attrs.type)) {
                        lblTemplate = '<label  id="dropdownMenuType" class="dropdown-toggle" style="padding-top:6px" data-toggle="dropdown" aria-expanded="true">'
                            + '<label class="hand-cursor" style="display: inline-block; min-width: 100px; font-size: 12px;'
                            + ' border-width: 0px 0px 1px 1px;border-style: solid;border-color: #F0F0F0; border-bottom-left-radius: 4px; padding: 5px; ">By '
                            + '{{ctrl.getLabel()}}<span style="float:right;top:8px;position:relative" class="caret"></span></label> </label>';
                        angular.element(element[0].querySelector('.dropdown-container1')).attr('style', 'position: relative;');
                        angular.element(element[0].querySelector('.dropdown-container2')).attr('style', 'position: absolute;right: -10px;top: -16px;');
                        divDropdown.prepend(lblTemplate);
                        divDropdown.parent().parent().addClass('menu-right-con');
                        ulDropdown.addClass('menu-right');
                    }
                    else if (selectService.isActionBarTop(attrs.type)) {
                        lblTemplate = ' <label  id="dropdownMenuType" class="dropdown-toggle" style="padding-top:6px" data-toggle="dropdown" aria-expanded="true">'
                            + '<label class="hand-cursor" style="color: #FFF;">{{ctrl.getLabel()}}</span></label> </label>';
                        divDropdown.prepend(lblTemplate);
                    }
                    else {
                        var noCaret = attrs.nocaret != undefined;
                        var noborder = attrs.noborder != undefined;
                        var buttonTemplate = '<button ' + tabindex + ' class="btn btn-default dropdown-toggle" style="width:100%;text-align: left;' + (noborder ? 'border:none' : '') + '" type="button" data-toggle="dropdown" '
                                            + ' aria-expanded="true"  ' + validateButtonClass + '>'
                                            + '<span style="float: left; margin: 0px;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;display: inline-block;width:calc(100% - 11px ); " ng-style="!ctrl.isHideRemoveIcon() ? {\'width\':\'calc(100% - 11px)\'}:{\'width\':\'100%\'} " >{{ctrl.getLabel()}}</span>'
                                            + (noCaret === true ? '' : '<span style="position:absolute;top:13px;right:5px" class="caret"></span>')
                                            + '</button><span ng-hide="ctrl.isHideRemoveIcon()"  ng-if="!ctrl.isMultiple() &&  ctrl.selectedvalues != undefined && ctrl.selectedvalues.length != 0  "  class="glyphicon glyphicon-remove hand-cursor" style="position: absolute;right: 15px;top: 10px;font-size: 11px;" aria-hidden="true" ng-click="ctrl.clearAllSelected($event,true);"></span>';
                        divDropdown.prepend(buttonTemplate);
                    }

                    var labelTemplate = '';
                    if (attrs.label != undefined)
                        labelTemplate = '<vr-label>' + attrs.label + '</vr-label>';
                    angular.element(element[0].querySelector('.dropdown-container2')).prepend(labelTemplate);

                    if (attrs.ismultipleselection !== undefined) {
                        if (attrs.hideselectedvaluessection) ulDropdown.addClass('single-col-checklist');
                        else ulDropdown.attr('ng-class', 'ctrl.getSelectedSectionClass()');
                    }
                    if (attrs.openup !== undefined) {
                        ulDropdown.addClass('menu-to-top');
                    }
                }

                onLoad();
                return {
                    pre: function ($scope, iElem, iAttrs) {

                        var ctrl = $scope.ctrl;

                        var getInputeStyle = function () {
                            var div = iElem.find('div[validator-section]')[0];
                            if (iAttrs.hint != undefined) {
                                $(div).css({ "display": "inline-block", "width": "calc(100% - 15px)", "margin-right": "-3px" })
                            };
                        }
                        getInputeStyle();

                        //baseDirService.addScopeValidationMethods(ctrl, iAttrs.id, formCtrl);

                        ctrl.clearAllSelected = function (e, isSingle) {
                            ctrl.muteAction(e);
                            ctrl.selectedvalues = [];
                            ctrl.selectedvalues.length = 0;
                            if (isSingle != undefined)
                                ctrl.selectedvalues = undefined;
                        };

                        function selectItem(e, item) {

                            if (!ctrl.isMultiple()) {

                                if (ctrl.onselectitem && typeof (ctrl.onselectitem) == 'function') {
                                    ctrl.onselectitem(item);

                                }
                                if (ctrl.ondeselectitem && typeof (ctrl.ondeselectitem) == 'function') {
                                    ctrl.ondeselectitem(ctrl.selectedvalues);
                                }
                                ctrl.selectedvalues = item;

                            }
                            else {
                                ctrl.muteAction(e);
                                var index = null;
                                try {
                                    index = baseDirService.findExsite(ctrl.selectedvalues, ctrl.getObjectValue(item), ctrl.datavaluefield);
                                }
                                catch (ex) {

                                }
                                if (index >= 0) {
                                    if (ctrl.ondeselectitem && typeof (ctrl.ondeselectitem) == 'function') {
                                        ctrl.ondeselectitem(item);
                                    }
                                    ctrl.selectedvalues.splice(index, 1);

                                }

                                else {
                                    if (ctrl.onselectitem && typeof (ctrl.onselectitem) == 'function') {
                                        ctrl.onselectitem(item);

                                    }
                                    ctrl.selectedvalues.push(item);
                                }

                            }
                        }

                        ctrl.selectValue = function (e, item) {
                            selectItem(e, item);
                        };



                        $scope.$watch(function () {

                            if (ctrl.isMultiple()) {
                                return ctrl.selectedvalues.length;
                            }

                            return ctrl.selectedvalues;

                        }, function () {
                            if (ctrl.onselectionchanged && typeof (ctrl.onselectionchanged) == 'function') {
                                var item = ctrl.onselectionchanged(ctrl.selectedvalues, ctrl.getdatasource());
                                if (item !== undefined) {
                                    selectItem(null, item);
                                }
                            }
                        });

                        ctrl.search = function () {
                            ctrl.setdatasource([]);
                            if (!ctrl.isRemoteLoad()) return;
                            if (ctrl.filtername.length > (iAttrs.limitcharactercount - 1) && typeof ctrl.datasource !== 'undefined' && ctrl.datasource != null) {
                                if (ctrl.datasource(ctrl.filtername).then != undefined) {

                                    ctrl.showloading = true;
                                    ctrl.datasource(ctrl.filtername).then(function (items) {
                                        ctrl.setdatasource(items);
                                        ctrl.showloading = false;
                                    }, function (msg) {
                                        console.log(msg);
                                    });

                                }

                            }

                        };
                    }
                }
            },
            templateUrl: function () {
                return selectService.dTemplate;
            }
        };

        return directiveDefinitionObject;

    }

    vrSelectDirective.$inject = ['SelectService', 'BaseDirService', 'ValidationMessagesEnum', 'UtilsService', 'VRValidationService'];

    app.directive('vrSelect', vrSelectDirective);

})(app);

