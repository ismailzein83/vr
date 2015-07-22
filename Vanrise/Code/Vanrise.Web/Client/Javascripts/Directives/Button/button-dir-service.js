'use strict';

app.service('ButtonDirService', ['BaseDirService', function (BaseDirService) {

    return ({
        getTemplate: getTemplate
    });

    function getTemplate(attrs) {
        var type = attrs.type;
        var buttonAttributes = getButtonAttributes(type);
        if (type == "Login") {            
            return '<button style="width:100%" type="button" class="btn btn-danger login-btn"'
            + 'aria-label="Left Align" ng-click="ctrl.onInternalClick()" ng-disabled="ctrl.isDisabled()">' + buttonAttributes.text
                + '<span style="padding-left:4px"  aria-hidden="true" ng-show="ctrl.showIcon()"></span>'
                + '<img src="../../Client/Javascripts/Directives/Button/images/loader-mask.gif" style="width:14px;margin-left:3px" ng-show="ctrl.showLoader()" />'
                + '</button>';

        }
        else if (attrs.standalone != undefined) {
            return '<div style="display:inline-block" title="' + buttonAttributes.text + '"'
           + ' aria-label="Left Align" ng-click="ctrl.onInternalClick()" ng-disabled="ctrl.isDisabled()">' 
               + '<span style="padding-left:4px;font-size:24px" class="' + buttonAttributes.class + ' hand-cursor" aria-hidden="true" ng-show="ctrl.showIcon()"></span>'
               + '<img src="Client/Javascripts/Directives/Button/images/loader-mask.gif" style="width:14px;margin-left:3px" ng-show="ctrl.showLoader()" />'
               + '</div>';
        }
        else {
            return '<button style="border-radius: 0px; border-color: transparent;  background-color: transparent; color: #FFF; " type="button" class="btn btn-default btncustom"'
            + 'aria-label="Left Align" ng-click="ctrl.onInternalClick()" ng-disabled="ctrl.isDisabled()">' + buttonAttributes.text
                + '<span style="padding-left:4px" class="' + buttonAttributes.class + ' aria-hidden="true" ng-show="ctrl.showIcon()"></span>'
                + '<img src="Client/Javascripts/Directives/Button/images/loader-mask.gif" style="width:14px;margin-left:3px" ng-show="ctrl.showLoader()" />'
                + '</button>';

        }
    }

    function getButtonAttributes(type) {
        switch (type) {
            case "Start":
                return {
                    text: "Start",
                    class: "glyphicon  glyphicon-play"
                };

            case "Reset":
                return {
                    text: "Reset",
                    class: "glyphicon  glyphicon-refresh"
                };
            case "Search":
                return {
                    text: "Search",
                    class: "glyphicon  glyphicon-search"
                };
            case "Add":
                return {
                    text: "Add",
                    class: "glyphicon  glyphicon-plus-sign"
                };
            case "Save":
                return {
                    text: "Save",
                    class: "glyphicon  glyphicon-floppy-disk"
                };
            case "Yes":
                return {
                    text: "Yes",
                    class: "glyphicon  glyphicon-floppy-disk"
                };
            case "Close":
                return {
                    text: "Close",
                    class: "glyphicon  glyphicon-remove-circle"
                };
            case "No":
                return {
                    text: "No",
                    class: "glyphicon  glyphicon-remove-circle"
                };
            case "Login":
                return {
                    text: "Login",
                    class: "glyphicon  glyphicon-log-in"
                };
            case "BreakInheritance":
                return {
                    text: "Break Inheritance",
                    class: "glyphicon  glyphicon-stop"
                };
            case "AllowInheritance":
                return {
                    text: "Allow Inheritance",
                    class: "glyphicon  glyphicon-play"
                };
            case 'ApplyChanges':
                return {
                    text: 'Apply Changes',
                    class: 'glyphicon glyphicon-ok-circle'
                };
            case 'AssignOrgChart':
                return {
                    text: 'Choose an Organization Chart',
                    class: 'glyphicon glyphicon-list-alt'
                };
            case 'AssignCarriers':
                return {
                    text: 'Assign Carriers',
                    class: 'glyphicon glyphicon-link'
                };
        }
    }
}]);