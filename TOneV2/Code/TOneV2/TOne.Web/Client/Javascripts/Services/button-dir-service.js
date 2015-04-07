'use strict';

app.service('ButtonDirService', ['BaseDirService', function (BaseDirService) {

    return ({
        getTemplate: getTemplate
    });

    function getTemplate(type) {
        var buttonAttributes = getButtonAttributes(type);
        return '<button style="border-radius: 0px; border-color: transparent;  background-color: transparent; color: #FFF; " type="button" class="btn btn-default btncustom"'
        + 'aria-label="Left Align" ng-click="ctrl.onInternalClick()" ng-disabled="ctrl.isDisabled()">' + buttonAttributes.text
            + '<span style="padding-left:4px" class="' + buttonAttributes.class + ' aria-hidden="true" ng-show="ctrl.showIcon()"></span>'
            + '<img src="~/../images/loader-mask.gif" style="width:14px;margin-left:3px" ng-show="ctrl.showLoader()" />'
            +'</button>';
    }

    function getButtonAttributes(type) {
        switch (type) {
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
            case "Save":
                return {
                    text: "Save",
                    class: "glyphicon  glyphicon-floppy-disk"
                };
            case "Close":
                return {
                    text: "Close",
                    class: "glyphicon  glyphicon-remove-circle"
                };
        }
    }
}]);