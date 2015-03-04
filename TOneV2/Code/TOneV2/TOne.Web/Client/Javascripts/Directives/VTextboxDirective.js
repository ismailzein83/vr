'use strict';

var defaultCompile = function (attrs, obj) {
    if (attrs.type.toLowerCase() == obj.name) {
        if (attrs.icon == undefined) attrs.$set("icon", obj.dIcon);
        if (attrs.placeholder == undefined) attrs.$set("placeholder", obj.dPlaceholder);
    }
    return attrs;
};

var defaultCheckType = function (attrs, obj) {
    return (attrs.type.toLowerCase() == obj.name);
};

var templates = {
    dTemplate: "../../Client/Templates/Directives/VTextboxStandard.html",
    tempIcon: "../../Client/Templates/Directives/VTextboxIcon.html"
};

var defaultConfiguration = {
    name: "default",
    dPlaceholder: "",
    dIcon: "",
    dTemplateURL: templates.dTemplate,
    compile: function (attrs) {
        return defaultCompile(attrs, this);
    },
    checkType: function (attrs) {
        return defaultCheckType(attrs, this);
    }
};

var allTextBox = [
        {
            name: "search",
            dPlaceholder: "Search ...",
            dIcon: "search",
            dTemplateURL: templates.tempIcon,
            compile : function(attrs) {
                return defaultCompile(attrs, this);
            },
            checkType: function (attrs) {
                return defaultCheckType(attrs, this);
            }
        }
        ,
        {
            name: "mail",
            dPlaceholder: "Mail ...",
            dIcon: "mail",
            dTemplateURL: templates.tempIcon,
            compile: function (attrs) {
                return defaultCompile(attrs, this);
            },
            checkType: function (attrs) {
                return defaultCheckType(attrs, this);
            }
        }
];

app.directive('vTextbox', function () {
    return {
        restrict: 'E',
        scope: {
            holder: '@placeholder',
            icon: '@icon',
            text:'=text'
        },
        compile: function (element, attrs) {
            allTextBox.forEach(function (item) {
                attrs = item.compile(attrs);
            });
        },
        templateUrl: function (element, attrs) {
            for (var index = 0; index < allTextBox.length; ++index) {
                if (allTextBox[index].checkType(attrs)) return allTextBox[index].dTemplateURL;
            }
            return '../../Client/Templates/Directives/VTextbox' + attrs.type + '.html';

        }
    };
});