app.directive("contenteditable", function () {
    return {
        restrict: "A",
        require: "ngModel",
        link: function (scope, element, attrs, ngModel) {

            scope.$on("$destroy", function () {
                unbindListeners();
            });

            function read() {
                var newLine = String.fromCharCode(10);              
                var formattedValue = element.text().replace(/<br>/ig, newLine).replace(/\r/ig, '');
                ngModel.$setViewValue(formattedValue);              
                element.text(formattedValue);
            }

            ngModel.$render = function () {
                element.html(ngModel.$viewValue != undefined && ngModel.$viewValue.trim() || "");
            };

            element.bind("blur", onElementBlur);

            element.bind("paste", onElementPast);

            function onElementBlur(e) {
                scope.$apply(read);
            }

            function onElementPast(e) {
                e.preventDefault();
                document.execCommand('inserttext', false, e.clipboardData.getData('text/plain'));
            }
           

            function unbindListeners() {
                element.unbind("blur", onElementBlur);
                element.unbind("paste", onElementPast);
            };
        }
    };
});