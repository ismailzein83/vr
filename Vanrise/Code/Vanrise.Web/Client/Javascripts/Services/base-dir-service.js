'use strict';

app.service('BaseDirService', ['ValidationMessagesEnum', function (ValidationMessagesEnum) {

    return ({
        getObjectProperty: getObjectProperty,
        muteAction: muteAction,
        findExsite: findExsite,
        guid: guid,
        getValidationMessageTemplate: getValidationMessageTemplate,
        generateHTMLElementName: generateHTMLElementName,
        addScopeValidationMethods: addScopeValidationMethods,
        prepareDirectiveHTMLForValidation: prepareDirectiveHTMLForValidation,
        getLastItem: getLastItem,
    });
    function getLastItem(arr) {
        if (arr == undefined) return null;
        if (arr.length > 0)
            return arr[arr.length - 1];
        return null;
    }

    function getObjectProperty(item, property) {
        if (item == undefined) return item;
        if (property && ('function' === typeof property)) {
            return property(item);
        }
        var arr = property.split('.');
        while (arr.length) {
            item = item[arr.shift()];
        }
        return item;
    }

    function muteAction(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    function findExsite(arr, value, attname) {
        var index = -1;
        for (var i = 0; i < arr.length; i++) {
            if (arr[i][attname] == value) {
                index = i
            }
        }
        return index;
    }

    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
              .toString(16)
              .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
          s4() + '-' + s4() + s4() + s4();
    }


    function escapeRegExp(string) {
        return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
    }
    function replaceAll(string, find, replace) {
        return string.replace(new RegExp(escapeRegExp(find), 'g'), replace);
    }

    function generateHTMLElementName() {
        return 'ElmntId_' + replaceAll(guid(), '-', '');
    }

    function getValidationMessageTemplate(requiredValue, requiredArray, invalidFormat, customValidation, invalidEmail, isNotSelect, withlabel, invalidNum) {
        var template = '';
        if (isNotSelect && withlabel)
            template = '<div  class="disable-animations tooltip-error fix-top" ng-style="ctrl.isVisibleTooltip() ? {\'display\':\'block\'} : {\'display\':\'none\'} " ng-messages="ctrl.getErrorObject()">';
        else
            template = '<div  class="disable-animations tooltip-error" ng-style="ctrl.isVisibleTooltip() ? {\'display\':\'block\'} : {\'display\':\'none\'} " ng-messages="ctrl.getErrorObject()">';

        if (requiredValue)
            template += '<div ng-message="requiredvalue">' + ValidationMessagesEnum.required + '</div>';
        if (requiredArray)
            template += '<div ng-message="requiredarray">' + ValidationMessagesEnum.required + '</div>';
        if (invalidFormat)
            template += '<div ng-message="invalidformat">' + ValidationMessagesEnum.invalidFormat + '</div>';
        if (invalidEmail)
            template += '<div ng-message="invalidemail">' + ValidationMessagesEnum.invalidEmail + '</div>';
        if (invalidNum)
            template += '<div ng-message="invalidnumber">' + ValidationMessagesEnum.invalidNumber + '</div>';
        if (customValidation)
            template += '<div ng-message="customvalidation">{{ ctrl.customMessage }}</div>';
        return template + '</div>';
    }

    function prepareDirectiveHTMLForValidation(validationOptions, elementToValidate, elementToStyleInvalid, elementToTriggerMessage) {
        var elementName = generateHTMLElementName();
        elementToValidate.attr('name', elementName);
        if (validationOptions.requiredValue)
            elementToValidate.attr('vr-validation-value', '');
        if (validationOptions.requiredArray)
            elementToValidate.attr('vr-validation-array', '');
        if (validationOptions.customValidation)
            elementToValidate.attr('vr-validation-custom', 'ctrl.customvalidate');
        if (validationOptions.emailValidation)
            elementToValidate.attr('vr-validation-email', '');
        if (validationOptions.numberValidation)
            elementToValidate.attr('vr-validation-number', '');

        elementToTriggerMessage.attr('ng-mouseenter', 'ctrl.showTooltip($event)');
        elementToTriggerMessage.attr('ng-mouseleave', 'ctrl.hideTooltip()');

        if (elementToStyleInvalid !== undefined) elementToStyleInvalid.attr('ng-class', 'ctrl.isValidComp()');
        return elementName;
    }

    function addScopeValidationMethods(ctrl, elementName, formCtrl) {
        var validationClass = {
            invalid: "required-inpute", valid: ''
        };

        var isValidElem = function () {
            return formCtrl[elementName].$valid;
        };

        ctrl.ValidationMessagesEnum = ValidationMessagesEnum;
        ctrl.tooltip = false;

        ctrl.showTooltip = function (e) {
            var self = angular.element(e.currentTarget);
            var selfHeight = $(self).height();
            var TophasLable = $(self).parent().attr('label') != undefined ? 0 : (($(self).parents('.dropdown-container2').length > 0)) ? -10 : - 15;
            var topVar = ($(self).parents('.dropdown-container2').length > 0) ? (selfHeight / 3) -5 : (selfHeight / 3);
            var selfWidth = $(self).width();
            var selfOffset = $(self).offset();
            var tooltip = self.parent().find('.tooltip-error')[0];
            $(tooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + topVar + TophasLable, left: selfOffset.left + selfWidth });
            ctrl.tooltip = true;
           
        };

        ctrl.hideTooltip = function () {
            ctrl.tooltip = false;
        };

        ctrl.getErrorObject = function () {
            return formCtrl[elementName].$error;
        };

        ctrl.isVisibleTooltip = function () {
            if (isValidElem()) return false;
            return ctrl.tooltip;
        };

        ctrl.isValidComp = function () {
            if (isValidElem()) return validationClass.valid;

            return validationClass.invalid;
        };


    }
}]);