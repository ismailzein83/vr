

app.directive('vrPreviewImage', ['FileAPIService', function (FileAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            value: '=',
            height: '@',
            width: '@',
            usericon: '='
        },
        controller: function ($scope, $element, $attrs, $timeout) {
            var ctrl = this;
            ctrl.image = '';
            ctrl.Style = {
                "height": ctrl.height,
                "width": ctrl.width

            };
            ctrl.imageStyle = "";
            if (ctrl.usericon == true)
                ctrl.imageStyle = { 'border-radius': '14px' };
            ctrl.previewImage = function () {
                FileAPIService.PreviewImage(ctrl.value).then(function (response) {
                    if (response != null){
                        ctrl.image = response;
                        if (ctrl.usericon == true)
                            ctrl.imageStyle = { 'border-radius': '14px' };
                    }
                    else if ($attrs.usericon != undefined)
                        ctrl.image = "Client/Images/member.png";
                    else
                        ctrl.image = "/Client/Images/no_image.jpg";
                });
            };

            var updatewatch = $scope.$watch('ctrl.value', function () {
                ctrl.imageStyle = "";
                if (ctrl.value != null && ctrl.value != undefined && ctrl.value != 0) {
                    ctrl.previewImage();
                }
                else if ($attrs.usericon != undefined)
                    ctrl.image = "Client/Images/member.png";
                else
                    ctrl.image = "/Client/Images/no_image.jpg";
                
            });

            $scope.$on('$destroy', function () {
                updatewatch();
            });

        },
        controllerAs: 'ctrl',
        compile: function (element, attrs) {
        },
        bindToController: true,
        template: function (element, attrs) {

            var containerStyle = (attrs.usericon != undefined) ? "" : "padding:2px;border:1px solid #ccc;";
            var imageStyle = "width:100%;height:100%;";

            var startTemplate = '<div style="' + containerStyle + '" ng-style="ctrl.Style">';
            var endTemplate = '</div>';

            var labelTemplate = '';
            if (attrs.label != undefined)
                labelTemplate = '<vr-label>' + attrs.label + '</vr-label>';
            var imageTemplate = ' <img ng-src="{{ctrl.image}}"  style="' + imageStyle + '" ng-style="ctrl.imageStyle"/>';




            return startTemplate + labelTemplate + imageTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);



