

app.directive('vrPreviewImage', ['FileAPIService', function (FileAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            value: '=',
            height: '@',
            width: '@',
        },
        controller: function ($scope, $element, $attrs, $timeout) {
            var ctrl = this;
            ctrl.image = '';
            ctrl.Style = {
                "height": ctrl.height,
                "width": ctrl.width

            };
            ctrl.previewImage = function () {
                FileAPIService.PreviewImage(ctrl.value).then(function (response) {
                    if (response != null)
                        ctrl.image = response;
                    else
                        ctrl.image = "/Client/Images/no_image.jpg";
                });
            };

            $scope.$watch('ctrl.value', function () {                
                if (ctrl.value != null && ctrl.value != undefined && ctrl.value != 0) {
                    ctrl.previewImage();
                }
                else 
                    ctrl.image = "/Client/Images/no_image.jpg";
            });
          
        },
        controllerAs: 'ctrl',
        compile: function (element, attrs) {           
        },
        bindToController: true,
        template: function (element, attrs) {
            
            var startTemplate = '<div style="padding:2px;border:1px solid #ccc;" ng-style="ctrl.Style">';
            var endTemplate = '</div>';

            var labelTemplate = '';
            if (attrs.label != undefined)
                labelTemplate = '<vr-label>' + attrs.label + '</vr-label>';
            var imageTemplate = ' <img ng-src="{{ctrl.image}}"  style="width:100%;height:100%"/>';
                 



            return startTemplate + labelTemplate + imageTemplate +  endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);



