(function (appControllers) {

    "use strict";
    callEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function callEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var parentId;
        var parentEntity;

        loadParameters();
        defineScope();
        load();
        addTimer();

        function loadParameters() {
          
        };

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.close = function () {
                clearInterval(timer);
                $scope.modalContext.closeModal();
            };

           // $scope.title = "Call";
        };

        function load() {
           
        };



    };
    appControllers.controller('Demo_Module_CallEditorController', callEditorController);
})(appControllers);



var timer;
function addTimer(){
    //counter up
//        var countDownDate = localStorage.getItem('startDate');
//if (countDownDate) {
//    countDownDate = new Date(countDownDate);
//} else {
    countDownDate = new Date();
    localStorage.setItem('startDate', countDownDate);

 timer = setInterval(function () {
    var now = new Date().getTime();
    var distance = now - countDownDate.getTime();
    var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    var seconds = Math.floor((distance % (1000 * 60)) / 1000);
    document.getElementById("demo").innerHTML =hours + "h " + minutes + "m " + seconds + "s ";
}, 1000);

}