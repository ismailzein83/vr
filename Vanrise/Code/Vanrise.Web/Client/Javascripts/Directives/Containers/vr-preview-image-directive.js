

app.directive('vrPreviewImage', [ function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            value: '=',
            height: '@',
            width: '@',
        },
        controller: function ($scope, $element, $attrs, $timeout) {
            var ctrl = this;
            ctrl.Style = {
                "height": ctrl.height,
                "width": ctrl.width

            };
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
            var imageTemplate = ' <img ng-if="ctrl.value!= null && ctrl.value!= 0 "  ng-src="api/VRFile/PreviewImage?fileId={{ctrl.value}}" style="width:100%;height:100%"/>'
                                + '<img ng-if="ctrl.value == null  ctrl.value!= 0 " ng-src="/Client/Images/no_image.jpg"   style="width:100%;height:100%">';
                 



            return startTemplate + labelTemplate + imageTemplate +  endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);



