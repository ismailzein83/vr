(function (app) {

    "use strict";

    function vrSelectDirective(selectService, baseDirService, validationMessagesEnum, utilsService, VRValidationService, $timeout, $rootScope, VRLocalizationService) {

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
                datatooltipfield: '@',
                datastylefield: '@',
                hidefilterbox: '@',
                datasource: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                ondeselectallitems: '=',
                onaddclicked: "=",
                hint: '@',
                haspermission: '=',
                hasviewpermission: '=',
                limitcharactercount: '=',
                lookandfeeltype: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var divDropdown = angular.element($element[0].querySelector('.dropdown'));
                $attrs.id = baseDirService.generateHTMLElementName();
                divDropdown.attr('name', $attrs.id);
                $scope.$on("$destroy", function () {
                    $(window).unbind('scroll', fixDropdownPosition);
                    $(window).unbind('resize', fixDropdownPosition);
                    $(window).off("resize.Viewport");
                    datasourceWatch();
                    dataWatch();
                });
                $element.on('$destroy', function () {
                    $('div[name=' + $attrs.id + ']').parents('div').unbind('scroll', fixDropdownPosition);
                });
                if (rootScope == undefined)
                    rootScope = $scope.$root;

                var controller = this;
                controller.filtername = '';
                controller.includeAdvancedSearch = $attrs.includeadvancedsearch != undefined;
                controller.readOnly = false;
                if ($attrs.stopreadonly == undefined)
                    controller.readOnly = utilsService.isContextReadOnly($scope) || $attrs.readonly != undefined;

                controller.validate = function () {
                    return VRValidationService.validate(controller.selectedvalues, $scope, $attrs);
                };
                if (controller.limitcharactercount == undefined)
                    controller.limitcharactercount = 2;

                controller.boundDataSource = [];
                $scope.effectiveDataSource = [];

                var datasourceWatch = $scope.$watchCollection('ctrl.datasource', function (newValue, oldValue) {
                    fillEffectiveDataSourceFromItems(getdatasource());
                });

                var dataWatch = $scope.$watchCollection('ctrl.data', function (newValue, oldValue) {
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
                        if (controller.datatextfield == undefined && propValue == undefined)
                            propValue = allDataItems[i].toString();
                        if (propValue != undefined && propValue.toLowerCase().indexOf(controller.filtername.toLowerCase()) >= 0)
                            filteredItems.push(allDataItems[i]);
                    }
                    fillEffectiveDataSourceFromItems(filteredItems);
                };
                controller.showSearchSection = false;
                controller.toggleAdvancedSearch = function (e) {
                    controller.showSearchSection = !controller.showSearchSection;
                };

                controller.cancelClickTrigger = function (e) {
                    controller.showSearchSection = false;
                    var onCancelHandler = $scope.$parent.$eval($attrs.oncancelhandler);
                    if (onCancelHandler != undefined && typeof (onCancelHandler) == 'function') {
                        var onCancelHandlerCallBackFunction = onCancelHandler;
                        if (onCancelHandlerCallBackFunction.then != undefined) {
                            controller.isloading = true;
                            return onCancelHandlerCallBackFunction.then(function (response) {
                                controller.isloading = false;
                            }).catch(function () {
                                controller.isloading = false;
                            });
                        }
                        else {
                            onCancelHandler();
                        }
                    }
                };
                controller.okClickTrigger = function (e) {
                    var onOkHandler = $scope.$parent.$eval($attrs.onokhandler);
                    controller.showSearchSection = false;
                    if (onOkHandler != undefined && typeof (onOkHandler) == 'function') {
                        var onOkHandlerCallBackFunction = onOkHandler;
                        if (onOkHandlerCallBackFunction.then != undefined) {
                            controller.isloading = true;
                            return onOkHandlerCallBackFunction.then(function (response) {
                                controller.isloading = false;
                            }).catch(function () {
                                controller.isloading = false;
                            });
                        }
                        else {
                            onOkHandler();
                        }
                    }
                };
                var isAddingPage = false;
                var found = false;
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
                            if (controller.isDropDownOpened())
                                markSelectedDataItem();
                        });
                    });
                }

                function markSelectedDataItem() {
                    if (found == true)
                        return;
                    var allDataItems = getdatasource();
                    if (!controller.isMultiple()) {
                        if (controller.selectedvalues != undefined) {
                            var index = controller.getObjectValue(controller.selectedvalues);
                            var item = utilsService.getItemByVal(allDataItems, index, controller.datavaluefield);
                            if ($('#' + index).offset() != undefined) {
                                found = true;
                                $('#' + index).find('a').first().addClass('mark-select-selected');
                                $('#divDataSourceContainer' + controller.id).first().stop().animate({
                                    scrollTop: parseInt($('#' + index).attr("dataindex")) * 21
                                }, 1);
                                $('.mark-select').mouseover(function () {
                                    $('.mark-select').removeClass('mark-select-selected');
                                });
                            }
                            else if (allDataItems.length < 300 && item != null && $('#' + index).offset() == undefined)
                                addPageToBoundDataSource();
                        }
                    }
                    else {
                        if (controller.selectedvalues.length > 0) {
                            var index = controller.getObjectValue(controller.selectedvalues[controller.selectedvalues.length - 1]);
                            var item = utilsService.getItemByVal(allDataItems, index, controller.datavaluefield);
                            if ($('#' + index).offset() != undefined) {
                                found = true;
                                $('#divDataSourceContainer' + controller.id).first().stop().animate({
                                    scrollTop: (parseInt($('#' + index).attr("dataindex")) * 24)
                                }, 1);
                                if (allDataItems.length > 20)
                                    addPageToBoundDataSource();
                            }
                            else if (allDataItems.length < 300 && item != null && $('#' + index).offset() == undefined)
                                addPageToBoundDataSource();
                        }
                    }
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

                function fillEffectiveDataSourceFromItems(items) {
                    $scope.effectiveDataSource.length = 0;
                    controller.boundDataSource.length = 0;
                    if (items != undefined) {
                        for (var i = 0; i < items.length; i++) {
                            $scope.effectiveDataSource.push(items[i]);
                        }
                    }
                    addPageToBoundDataSource();
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

                function getTooltipValue(item) {
                    if (controller.datatooltipfield)
                        return getObjectProperty(item, controller.datatooltipfield);
                };
                function getObjectDisabled(item) {
                    if (controller.datadisabledfield) return getObjectProperty(item, controller.datadisabledfield);
                    return false;
                };
                function getObjectStyle(item) {
                    if (controller.datastylefield)
                        return getObjectProperty(item, controller.datastylefield);
                };

                function findExsite(item) {
                    return utilsService.getItemIndexByVal(controller.selectedvalues, getObjectValue(item), controller.datavaluefield);
                }

                function isSelected(item) {
                    if (controller.selectedvalues != undefined)
                        return item[controller.datavaluefield] == controller.selectedvalues[controller.datavaluefield];
                }
                function onViewHandler(obj) {
                    hideAllOtherDropDown();
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
                    controller.filtername = '';
                    fillEffectiveDataSourceFromItems(getdatasource());
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
                    selectedSectionVisible: selectedSectionVisible,
                    getObjectText: getObjectText,
                    getObjectValue: getObjectValue,
                    getObjectDisabled: getObjectDisabled,
                    getObjectStyle: getObjectStyle,
                    getTooltipValue: getTooltipValue,
                    findExsite: findExsite,
                    clearFilter: clearFilter,
                    selectFirstItem: selectFirstItem,
                    getLabel: getLabel,
                    getSelectedSectionClass: getSelectedSectionClass,
                    onAddhandler: onAddhandler,
                    includeOnAddHandler: includeOnAddHandler,
                    onViewHandler: onViewHandler,
                    includeOnViewHandler: includeOnViewHandler,
                    isSelected: isSelected
                });

                var afterShowDropdown = function (id) {
                    var dropdown = $('div[name=' + id + ']');
                    var menuPosition = getDropDownDirection(id);
                    $('div[name=' + id + ']').find('.dropdown-menu').css({ position: 'fixed', top: menuPosition.top, left: menuPosition.left });
                    setTimeout(function () {
                        var lastScrollTop;
                        dropdown.find("#divDataSourceContainer" + id).scroll(function (e) {
                            var scrollTop = dropdown.find("#divDataSourceContainer" + id).scrollTop();
                            var scrollPercentage = 100 * scrollTop / (dropdown.find('#divDataSourceBody' + id).height() - dropdown.find("#divDataSourceContainer" + id).height());

                            if (scrollTop > lastScrollTop) {
                                if (scrollPercentage > 80)
                                    addPageToBoundDataSource();
                            }
                            lastScrollTop = scrollTop;

                        });
                        markSelectedDataItem();
                    }, 1);
                    var selfHeight = dropdown.height();
                    var selfOffset = dropdown.offset();
                    var basetop = selfOffset.top - $(window).scrollTop() + selfHeight;
                    var heigth = dropdown.parents('.vr-pager-container').length > 0 ? 235 : 200;
                    if (menuPosition.toTop) {
                        var dropdownMenu = dropdown.find('.dropdown-menu');
                        var height = dropdownMenu.css({ display: "block" }).height();
                        $element.find('.vr-select-add').css({ top: selfHeight + 16 });
                        dropdownMenu.css({ overflow: "hidden", marginTop: height, height: 0 }).animate({
                            marginTop: 0,
                            height: height
                        }, 1000, function () {
                            $(this).css({ display: "block", overflow: "", height: "", marginTop: "" });
                            $('div[name=' + id + ']').removeClass("changing-state");
                        });
                    }
                    else {
                        dropdown.find('.dropdown-menu').first().slideDown("slow", function () {
                            $('div[name=' + id + ']').removeClass("changing-state");
                        });
                    }

                };
                var afterHideDropdown = function (id) {
                    controller.filtername = '';
                    controller.searchLocal();
                    if (controller.isRemoteLoad())
                        controller.search();
                    $('div[name=' + id + ']').find('#filterInput').blur();
                    if (controller.onblurdropdown != null) {
                        controller.onblurdropdown();
                    }
                    vrSelectSharedObject.onCloseDropDown(id);
                    found = false;
                    controller.showSearchSection = false;
                };
                controller.focusFilterInput = function () {
                    setTimeout(function () {
                        $('.filter-input').focus();
                    }, 100);
                };
                function getDropDownDirection(id,getTopDirection) {
                    cleanUpPositionClass(id);
                    var self = $('div[name=' + id + ']').find('.dropdown-toggle').first();
                    var dropDown = $('div[name=' + id + ']').find('.dropdown-menu');
                    var selfHeight = $(self).parent().height();
                    var selfOffset = $(self).offset();
                    var initialtop = 0;
                    var basetop = selfOffset.top - $(window).scrollTop() + selfHeight;
                    var baseleft = selfOffset.left - $(window).scrollLeft();
                    var heigth = $('div[name=' + id + ']').parents('.vr-pager-container').length > 0 ? 235 : 200;
                    var toTopDirection = (innerHeight - 50) - basetop < heigth;
                    if (getTopDirection)
                        return { toTop: toTopDirection };
                    if (toTopDirection) {
                        initialtop = basetop - (heigth + (selfHeight * 2.9));
                        if (isRemoteLoad()) {
                            initialtop = initialtop - 35;
                        }
                        if (controller.hidefilterbox != undefined)
                            initialtop = initialtop + 30;
                        if (controller.readOnly && controller.isRemoteLoad())
                            initialtop = initialtop + 30;
                    }
                    else
                        initialtop = selfOffset.top - $(window).scrollTop() + selfHeight;
                    var ddleft = baseleft;

                    if (!VRLocalizationService.isLocalizationRTL()) {
                        if (innerWidth - baseleft < 226 && !controller.isMultiple()) {
                            ddleft = baseleft + $(self).parent().width() - $(dropDown).width();
                            $(dropDown).addClass('dropdown-top-right');

                        }
                        if ((innerWidth - baseleft < 444) && controller.isMultiple()) {
                            $(dropDown).addClass('dropdown-top-right');
                            if (innerWidth - baseleft < 226) {
                                ddleft = baseleft + $(self).parent().width() - $(dropDown).width();
                                if (controller.selectedSectionVisible())
                                    $(dropDown).addClass('top-max');
                                else
                                    $(dropDown).removeClass('top-max');
                            }
                        }
                        if ((innerWidth - baseleft < 468) && controller.includeAdvancedSearch) {
                            $(dropDown).find('.data-presentation').addClass('advance-search-top-right');
                            $(dropDown).find('.vr-select-advance-search').addClass('advance-search-top-right');
                        }
                    }
                    if (VRLocalizationService.isLocalizationRTL()) {
                        var ddwidth = $attrs.smallselect != undefined ? 150 : 226;
                        ddleft = baseleft + ($(self).parent().width() - ddwidth);
                        if ($(dropDown).parents('vr-pagination').length > 0) {
                            ddleft = baseleft - 39;
                        }
                        if (baseleft + $(self).parent().width() < ddwidth && !controller.isMultiple()) {
                            ddleft = baseleft > 0 && baseleft || 0;
                            $(dropDown).addClass('dropdown-top-left');
                        }
                        if (baseleft + $(self).parent().width() < 444 && controller.isMultiple()) {
                            $(dropDown).addClass('dropdown-top-left');
                            if ($(self).parent().width() < ddwidth) {
                                ddleft = baseleft;
                                if (controller.selectedSectionVisible() )
                                    $(dropDown).addClass('top-max');
                                else if(!controller.selectedSectionVisible() && $attrs.smallselect != undefined)
                                    ddleft = baseleft - 135;
                                else
                                    $(dropDown).removeClass('top-max');
                            }
                        }

                        if ( baseleft < 468  && controller.includeAdvancedSearch) {
                            $(dropDown).find('.data-presentation').addClass('advance-search-top-left');
                            $(dropDown).find('.vr-select-advance-search').addClass('advance-search-top-left');
                        }
                    }
                    return {
                        toTop: toTopDirection,
                        top: initialtop,
                        left: ddleft
                    };
                };
                function hideAllOtherDropDown(currentId) {
                    $rootScope.$broadcast("hide-all-menu");
                    var dropdowns = $('.dropdown-menu');
                    var len = dropdowns.length;
                    var i;
                    var self;
                    for (i = 0; i < len; i++) {
                        self = $(dropdowns[i]);
                        var id = self.parent().attr('name');
                        if (self.parent().hasClass('open-select') && currentId != id && self.parent().find('div[name=' + currentId + ']').length == 0) {
                            self.parent().removeClass('open-select');
                            afterHideDropdown(id);
                            if (currentId == undefined) {
                                $('div[name=' + id + ']').find('.dropdown-menu').hide();
                            }
                            else {
                                if (getDropDownDirection(id,true).toTop == true) {
                                    $('div[name=' + id + ']').find('.dropdown-menu').hide();
                                }
                                else {
                                    $('div[name=' + id + ']').find('.dropdown-menu').slideUp(300);
                                }
                            }
                        }
                    }
                };
                setTimeout(function () {
                    $('div[name=' + $attrs.id + ']').on('click', '.dropdown-toggle', function (event) {

                        hideAllOtherDropDown($attrs.id);
                        if ($('div[name=' + $attrs.id + ']').hasClass('changing-state'))
                            return;
                        $('div[name=' + $attrs.id + ']').addClass("changing-state");
                        if (!$('div[name=' + $attrs.id + ']').hasClass('open-select')) {
                            $('div[name=' + $attrs.id + ']').addClass("open-select");
                            vrSelectSharedObject.onOpenDropDown($attrs.id);
                            afterShowDropdown($attrs.id);
                            $(document).bind('click', boundDocumentSeclectOutside);
                        }
                        else {
                            var menuPosition = getDropDownDirection($attrs.id,true);
                            $('div[name=' + $attrs.id + ']').removeClass("open-select");
                            if (menuPosition.toTop == true) {
                                $('div[name=' + $attrs.id + ']').find('.dropdown-menu').hide(function () {
                                    $('div[name=' + $attrs.id + ']').removeClass("changing-state");
                                });
                            }
                            else {
                                $('div[name=' + $attrs.id + ']').find('.dropdown-menu').slideUp("slow", function () {
                                    $('div[name=' + $attrs.id + ']').removeClass("changing-state");
                                });
                            }
                            afterHideDropdown($attrs.id);
                            $(document).unbind('click', boundDocumentSeclectOutside);
                            event.stopPropagation();
                            return;
                        }
                        event.stopPropagation();
                    });
                }, 100);

                function boundDocumentSeclectOutside(e) {
                    var button = $('div[name=' + $attrs.id + ']');
                    if ($(e.target).attr('open-trriger') != undefined && $(e.target).attr('open-trriger') == $attrs.id)
                        return;
                    if ((!$(e.target).hasClass('inner-remove') && !$(e.target).hasClass('remove-selection') && !$(button).is(e.target) && $(button).has(e.target).length === 0) || ($(e.target).hasClass('single-select' + $attrs.id))) {
                        $(button).removeClass('open-select');
                        $('div[name=' + $attrs.id + ']').find('.dropdown-menu').first().slideUp("slow", function () {
                            afterHideDropdown($attrs.id);
                            $(document).unbind('click', boundDocumentSeclectOutside);
                        });
                    }
                }
                $('div[name=' + $attrs.id + ']').parents('div').on('scroll', fixDropdownPosition);
                $(window).on('scroll', fixDropdownPosition);
                $(window).on('resize', fixDropdownPosition);
                $scope.$on('start-drag', function (event, args) {
                    fixDropdownPosition();
                });
                $scope.$on('hide-all-select', function (event, args) {
                    fixDropdownPosition();
                });
                function fixDropdownPosition() {
                    hideAllOtherDropDown();
                }
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
                api.removeReadOnly = function (nameFilter) {
                    controller.readOnly = false;
                };
                api.selectItem = function (item) {
                    var value = $attrs.ismultipleselection != undefined ? [item] : item;
                    controller.selectedvalues = value;
                };
                api.selectFirstItem = function () {
                    api.selectItem(controller.datasource[0]);
                };
                api.selectIfSingleItem = function () {
                    if (controller.datasource.length == 1)
                        api.selectItem(controller.datasource[0]);
                };
                function cleanUpPositionClass(id) {
                    $('div[name=' + id + ']').find('.dropdown-menu').removeClass('dropdown-top-right');
                    $('div[name=' + id + ']').find('.dropdown-menu').removeClass('dropdown-top-left');
                    $('div[name=' + id + ']').find('.dropdown-menu').removeClass('top-max');
                }
                //api.openDropDown = function () {
                //    var event = $(window.event);
                //    if (event) {
                //        var target = event[0].srcElement;
                //        $(target).attr('open-trriger', $attrs.id);
                //    }
                //    if ($('div[name=' + $attrs.id + ']').hasClass('open-select') == false) {
                //        $timeout(function () {
                //            $('div[name=' + $attrs.id + ']').find('.dropdown-toggle').first().trigger("click");
                //        }, 1);
                //    }
                //};

                //api.closeDropDown = function () {
                //    if (!$('div[name=' + $attrs.id + ']').hasClass('open-select'))
                //        return;
                //    $timeout(function () {
                //        if ($('div[name=' + $attrs.id + ']').hasClass('open-select')) {
                //            $('div[name=' + $attrs.id + ']').find('.dropdown-toggle').first().trigger("click");
                //        }
                //    }, 1);
                //};
                if (controller.onReady != null) {
                    controller.onReady(api);
                }

            },
            controllerAs: 'ctrl',
            bindToController: true,
            transclude: true,
            compile: function (element, attrs) {


                function onLoad() {

                    selectService.setAttributes(attrs);

                    var divDropdown = angular.element(element[0].querySelector('.dropdown'));
                    var ulDropdown = angular.element(element[0].querySelector('.dropdown-menu'));
                    var tabindex = '';
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
                        var buttonTemplate = '<button ' + tabindex + ' class="btn btn-default dropdown-toggle vr-dropdown-select" style="' + (noborder ? 'border:none' : '') + '" type="button"  '
                                            + '   ' + validateButtonClass + '>'
                                            + '<span class="vanrise-inpute" style="float: left; margin: 0px;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;display: inline-block;width:calc(100% - 11px );" ng-style="!ctrl.isHideRemoveIcon() ? {\'width\':\'calc(100% - 11px)\'}:{\'width\':\'100%\'}" ng-class="ctrl.labelclass">{{ctrl.getLabel()}}</span>'
                                            + (noCaret === true ? '' : '<span ng-if="!ctrl.readOnly || ctrl.isMultiple()" class="caret vr-select-caret"></span>')
                                            + '</button><span ng-hide="ctrl.isHideRemoveIcon() || ctrl.readOnly"  ng-if="!ctrl.isMultiple() &&  ctrl.selectedvalues != undefined && ctrl.selectedvalues.length != 0  "  class="glyphicon glyphicon-remove hand-cursor vr-select-remove"  aria-hidden="true" ng-click="ctrl.clearAllSelected($event,true);"></span>';
                        divDropdown.prepend(buttonTemplate);
                    }

                    var labelTemplate = '';
                    if (attrs.label != undefined && attrs.hidelabel == undefined)
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
                if (attrs.lookandfeeltype == undefined)
                    onLoad();
                return {
                    pre: function ($scope, iElem, iAttrs) {
                        $scope.$on("$destroy", function () {
                            valueWatch();
                            classCollectionWatch();
                        });
                        var ctrl = $scope.ctrl;
                        ctrl.id = iAttrs.id;

                        var getInputeStyle = function () {
                            var div = iElem.find('div[validator-section]')[0];
                            if (iAttrs.hint != undefined) {
                                $(div).css({
                                    "display": "inline-block", "width": "calc(100% - 15px)", "margin-right": "-3px"
                                });
                            }
                        };
                        getInputeStyle();

                        //baseDirService.addScopeValidationMethods(ctrl, iAttrs.id, formCtrl);

                        ctrl.clearAllSelected = function (e, isSingle) {
                            ctrl.selectedvalues = [];
                            ctrl.selectedvalues.length = 0;
                            if (isSingle != undefined) {
                                //$('.dropdown-menu').hide();
                                ctrl.selectedvalues = undefined;
                            }

                            else {
                                if (ctrl.ondeselectallitems && typeof (ctrl.ondeselectallitems) == 'function') {
                                    ctrl.ondeselectallitems();
                                }
                            }

                        };

                        function selectItem(e, item, removeselection) {
                            if (!ctrl.isMultiple()) {

                                if (ctrl.onselectitem && typeof (ctrl.onselectitem) == 'function') {
                                    ctrl.onselectitem(item);

                                }
                                if (ctrl.ondeselectitem && typeof (ctrl.ondeselectitem) == 'function') {
                                    ctrl.ondeselectitem(ctrl.selectedvalues);
                                }
                                if (removeselection != undefined && removeselection == true && ctrl.selectedvalues != undefined && item[ctrl.datavaluefield] == ctrl.selectedvalues[ctrl.datavaluefield]) {
                                    ctrl.selectedvalues = undefined;
                                    return;
                                }

                                ctrl.selectedvalues = item;

                            }
                            else {
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
                            if (iElem.parents('.dropdown-menu').length > 0) {
                                $timeout(function () {
                                    $(iElem.find('.vr-select-remove')).addClass('inner-remove');
                                }, 10);
                            }
                        }

                        ctrl.selectValue = function (e, item, removeselection) {
                            if (ctrl.getObjectDisabled(item) == true || ctrl.readOnly)
                                return;
                            var onBeforeSelectionChanged = $scope.$parent.$eval(iAttrs.onbeforeselectionchanged);
                            if (onBeforeSelectionChanged != undefined && typeof (onBeforeSelectionChanged) == 'function') {
                                var onBeforeSelectionChangedPromise = onBeforeSelectionChanged();
                                if (onBeforeSelectionChangedPromise != undefined && onBeforeSelectionChangedPromise.then != undefined) {
                                    onBeforeSelectionChangedPromise.then(function (response) {
                                        if (response)
                                            selectItem(e, item);
                                        else
                                            return;
                                    }).catch(function () {
                                        return;
                                    });
                                }
                                else {
                                    selectItem(e, item);
                                }
                            }
                            else
                                selectItem(e, item, removeselection);
                        };




                        var valueWatch = $scope.$watch(function () {

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
                        var classCollectionWatch = $scope.$watchCollection('ctrl.selectedvalues', function (newValue, oldValue) {
                            ctrl.labelclass = "vr-select-watermark";
                            if (!ctrl.isMultiple() && newValue != undefined) {
                                ctrl.labelclass = "";
                                return;
                            }
                            if (ctrl.isMultiple() && newValue.length > 0) {
                                ctrl.labelclass = "";
                                return;
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
            templateUrl: function (element, attrs) {
                var temp = selectService.dTemplate;
                if (attrs.lookandfeeltype != undefined && attrs.lookandfeeltype == "toolbox")
                    temp = selectService.toolboxSelectTemplate;
                return temp;
            }
        };

        return directiveDefinitionObject;

    }

    vrSelectDirective.$inject = ['SelectService', 'BaseDirService', 'ValidationMessagesEnum', 'UtilsService', 'VRValidationService', '$timeout', '$rootScope', 'VRLocalizationService'];

    app.directive('vrSelect', vrSelectDirective);

})(app);

