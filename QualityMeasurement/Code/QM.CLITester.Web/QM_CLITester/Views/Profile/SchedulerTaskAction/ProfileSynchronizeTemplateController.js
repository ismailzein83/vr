ProfileSynchronizeTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'QM_BE_ProfileAPIService'];

function ProfileSynchronizeTemplateController($scope, UtilsService, VRUIUtilsService, QM_BE_ProfileAPIService) {

    defineScope();
    load();
    function defineScope() {
       

    }

    function load() {
       
        loadAllControls();
    }
    function loadAllControls() {
        
    }
    

}
appControllers.controller('QM_CLITester_ProfileSynchronizeTemplateController', ProfileSynchronizeTemplateController);
