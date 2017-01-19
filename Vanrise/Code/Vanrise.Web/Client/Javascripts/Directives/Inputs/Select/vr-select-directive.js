﻿(function (app) {

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
                onblurdropdown: '=',
                ismultipleselection: '@',
                hideselectedvaluessection: '@',
                datavaluefield: '@',
                datatextfield: '@',
                datadisabledfield: '@',
                hidefilterbox: '@',
                datasource: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                onaddclicked: "=",
                hint: '@',
                haspermission: '=',
                hasviewpermission: '=',
                limitcharactercount:'='
            },
            controller: function ($scope, $element, $attrs) {
                if (rootScope == undefined)
                    rootScope = $scope.$root;

                var controller = this;
                controller.filtername = '';
                controller.readOnly = utilsService.isContextReadOnly($scope) || $attrs.readonly != undefined;

                controller.validate = function () {
                    return VRValidationService.validate(controller.selectedvalues, $scope, $attrs);
                };
                if (controller.limitcharactercount == undefined)
                    controller.limitcharactercount = 2;

                controller.boundDataSource = [];
                $scope.effectiveDataSource = [];
                var itemsToAddToSource;
                
                $scope.$watchCollection('ctrl.datasource', function (newValue, oldValue) {
                    fillEffectiveDataSourceFromItems(getdatasource());
                });

                $scope.$watchCollection('ctrl.data', function (newValue, oldValue) {
                    fillEffectiveDataSourceFromItems(getdatasource());
                });

                controller.hideAddButton = false;
                if (controller.haspermission != undefined && typeof (controller.haspermission) == 'function') {
                    controller.haspermission().then(function (isAllowed) {
                        if (!isAllowed)
                            controller.hideAddButton = true;
                    });
                }

                controller.showViewButton = true;
                if (controller.hasviewpermission != undefined && typeof (controller.hasviewpermission) == 'function') {
                    controller.hasviewpermission().then(function (isAllowed) {
                        if (!isAllowed)
                            controller.showViewButton = false;
                    });
                }

                controller.searchLocal = function () {
                    var filteredItems = [];
                    var allDataItems = getdatasource(); 
                    for (var i = 0; i < allDataItems.length; i++) {
                        var propValue = allDataItems[i][controller.datatextfield]; 
                        if (propValue != undefined && propValue.toLowerCase().indexOf(controller.filtername.toLowerCase()) >= 0)
                            filteredItems.push(allDataItems[i]);
                    }
                    fillEffectiveDataSourceFromItems(filteredItems);
                };

                var isAddingPage = false;
                function addPageToBoundDataSource() {
                    if (isAddingPage)
                        return;
                    isAddingPage = true;
                    setTimeout(function () {
                        $scope.$apply(function () {
                            var addedItems = 0;
                            for (var i = controller.boundDataSource.length; i < $scope.effectiveDataSource.length && addedItems < 20; i++) {
                                controller.boundDataSource.push($scope.effectiveDataSource[i]);
                                addedItems++;
                            }
                            isAddingPage = false;
                        });
                    });
                }

                //Configuration
                angular.extend(this, {
                    ValidationMessagesEnum: validationMessagesEnum,
                    //limitcharactercount: $attrs.limitcharactercount,
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

                function adjustTooltipPosition(e) {
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
                }

                function fillEffectiveDataSourceFromItems(items)
                {
                    $scope.effectiveDataSource.length = 0;
                    controller.boundDataSource.length = 0;
                    for (var i = 0; i < items.length; i++) {
                        $scope.effectiveDataSource.push(items[i]);
                    }
                    addPageToBoundDataSource();
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
                function getObjectDisabled(item) {
                    if (controller.datadisabledfield) return getObjectProperty(item, controller.datadisabledfield);
                    return false;
                };

                function findExsite(item) {
                    return utilsService.getItemIndexByVal(controller.selectedvalues, getObjectValue(item), controller.datavaluefield);
                }

                function onViewHandler(obj) {
                    var onViewHandler = $scope.$parent.$eval($attrs.onviewclicked);
                    onViewHandler(obj);
                }

                function includeOnViewHandler() {
                    var onViewHandler = $scope.$parent.$eval($attrs.onviewclicked);
                    return (onViewHandler != undefined && typeof (onViewHandler) == 'function');
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
                    return controller.selectedvalues.length === 0 || controller.readOnly ? 'single-col-checklist' : 'double-col-checklist';
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
                    getObjectDisabled: getObjectDisabled,
                    findExsite: findExsite,
                    clearFilter: clearFilter,
                    selectFirstItem: selectFirstItem,
                    getLabel: getLabel,
                    getSelectedSectionClass: getSelectedSectionClass,
                    onAddhandler: onAddhandler,
                    includeOnAddHandler: includeOnAddHandler,
                    onViewHandler: onViewHandler,
                    includeOnViewHandler: includeOnViewHandler
                });
                var api = {};
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
                };
                api.setLimitCharacterCount = function (count) {
                    controller.limitcharactercount = count;
                };
                api.loadDataSource = function (nameFilter) {
                    return controller.datasource(nameFilter).then(function (items) {
                        controller.setdatasource(items);
                        return items;
                    });
                };
                if (controller.onReady != null) {
                    controller.onReady(api);
                }
                //Exports
                setTimeout(function () {
                    $('div[name=' + $attrs.id + ']').on('show.bs.dropdown', function () {

                        vrSelectSharedObject.onOpenDropDown($attrs.id);

                        setTimeout(function () {
                            $('#filterInput').focus();
                            var lastScrollTop;
                            $element.find("#divDataSourceContainer").scroll(function () {
                                
                                var scrollTop = $(this).scrollTop();
                                var scrollPercentage = 100 * scrollTop / ($element.find('#divDataSourceBody').height() - $(this).height());

                                if (scrollTop > lastScrollTop) {
                                    if (scrollPercentage > 80)
                                        addPageToBoundDataSource();
                                }
                                lastScrollTop = scrollTop;
                            });
                        }, 1);
                        var selfHeight = $(this).height();
                        var selfOffset = $(this).offset();
                        var basetop = selfOffset.top - $(window).scrollTop() + selfHeight;

                        var heigth = $(this).parents('.vr-pager-container').length > 0 ? 235 : 200;

                        if ((innerHeight - 100) - basetop < heigth) {
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
                        if (controller.onblurdropdown != null) {
                            controller.onblurdropdown()
                        }
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
                        if ((innerHeight - 100) - basetop < heigth) {
                            top = basetop - (heigth + (selfHeight * 2.7));
                            if (isRemoteLoad()) {
                                top = top - 35;
                            }
                            if (controller.hidefilterbox != undefined)
                                top = top + 30;
                            if (controller.readOnly && controller.isRemoteLoad())
                                top = top + 30;

                        }
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
                        var buttonTemplate = '<button ' + tabindex + ' class="btn btn-default dropdown-toggle vr-dropdown-select" style="' + (noborder ? 'border:none' : '') + '" type="button" data-toggle="dropdown" '
                                            + ' aria-expanded="true"  ' + validateButtonClass + '>'
                                            + '<span style="float: left; margin: 0px;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;display: inline-block;width:calc(100% - 11px ); " ng-style="!ctrl.isHideRemoveIcon() ? {\'width\':\'calc(100% - 11px)\'}:{\'width\':\'100%\'} " >{{ctrl.getLabel()}}</span>'
                                            + (noCaret === true ? '' : '<span ng-if="!ctrl.readOnly || ctrl.isMultiple()" class="caret vr-select-caret"></span>')
                                            + '</button><span ng-hide="ctrl.isHideRemoveIcon() || ctrl.readOnly"  ng-if="!ctrl.isMultiple() &&  ctrl.selectedvalues != undefined && ctrl.selectedvalues.length != 0  "  class="glyphicon glyphicon-remove hand-cursor vr-select-remove"  aria-hidden="true" ng-click="ctrl.clearAllSelected($event,true);"></span>';
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
                                $(div).css({ "display": "inline-block", "width": "calc(100% - 15px)", "margin-right": "-3px" });
                            }
                        };
                        getInputeStyle();

                        //baseDirService.addScopeValidationMethods(ctrl, iAttrs.id, formCtrl);

                        ctrl.clearAllSelected = function (e, isSingle) {
                            ctrl.muteAction(e);
                            ctrl.selectedvalues = [];
                            ctrl.selectedvalues.length = 0;
                            if (isSingle != undefined) {                               
                                //$('.dropdown-menu').hide();
                                ctrl.selectedvalues = undefined;
                            }
                               
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
                            if (ctrl.getObjectDisabled(item) == true || ctrl.readOnly)
                                return;
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
                            if (ctrl.filtername.length > (ctrl.limitcharactercount - 1) && typeof ctrl.datasource !== 'undefined' && ctrl.datasource != null) {
                                var promise = ctrl.datasource(ctrl.filtername);
                                if (promise != null && promise.then != undefined) {
                                    ctrl.showloading = true;
                                    promise.then(function (items) {
                                        ctrl.setdatasource(items);                                      
                                    }).finally(function () {
                                        ctrl.showloading = false;
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

