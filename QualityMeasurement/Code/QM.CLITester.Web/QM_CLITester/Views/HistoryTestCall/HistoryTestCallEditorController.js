(function (appControllers) {

    "use strict";

    Qm_CliTester_HistoryTestCallEditorController.$inject = ['$scope', 'Qm_CliTester_TestCallAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function Qm_CliTester_HistoryTestCallEditorController($scope, Qm_CliTester_TestCallAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        var supplierName;
        var userName;
        var countryName;
        var zoneName;
        var callTestStatusDescription;
        var callTestResultDescription;
        var scheduleName;
        var pdd;
        var mos;
        var creationDate;
        var source;
        var destination;
        var receivedCli;
        var releaseCode;
        var ringDuration;
        var duration;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                supplierName = parameters.SupplierName;
                userName = parameters.UserName;
                countryName = parameters.CountryName;
                zoneName = parameters.ZoneName;
                callTestStatusDescription = parameters.CallTestStatusDescription;
                callTestResultDescription = parameters.CallTestResultDescription;
                scheduleName = parameters.ScheduleName;
                pdd = parameters.PDD;
                mos = parameters.MOS;
                creationDate = parameters.CreationDate;
                source = parameters.Source;
                destination = parameters.Destination;
                receivedCli = parameters.ReceivedCli;
                releaseCode = parameters.ReleaseCode;
                ringDuration = parameters.RingDuration;
                duration = parameters.Duration;
            }
        }
        function defineScope() {
            $scope.sendTestCallMail = function () {
                return sendMail();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;
            $scope.title = "Send Test Call Result";
            $scope.isLoading = false;
        }


        function buildCountryObjFromScope() {
            var obj = {
                SupplierName: (supplierName != null) ? supplierName : "",
                UserName: (userName != null) ? userName : "",
                CountryName: (countryName != null) ? countryName : "",
                ZoneName: (zoneName != null) ? zoneName : "",
                CallTestStatusDescription: (callTestStatusDescription != null) ? callTestStatusDescription : "",
                CallTestResultDescription: (callTestResultDescription != null) ? callTestResultDescription : "",
                ScheduleName: (scheduleName != null) ? scheduleName : "",
                Pdd: (pdd != null) ? pdd : "",
                Mos: (mos != null) ? mos : "",
                CreationDate: (creationDate != null) ? creationDate : "",
                Source: (source != null) ? source : "",
                Destination: (destination != null) ? destination : "",
                ReceivedCli: (receivedCli != null) ? receivedCli : "",
                RingDuration: (ringDuration != null) ? ringDuration : "",
                CallDuration: (duration != null) ? duration : "",
                ReleaseCode: (releaseCode != null) ? releaseCode : "",
                ToMail: $scope.toMail.join(";")
            };
            return obj;
        }
        
        function sendMail() {
            $scope.isLoading = true;

            var mailObject = buildCountryObjFromScope();

            Qm_CliTester_TestCallAPIService.SendMail(mailObject).then(function (response) {
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('Qm_CliTester_HistoryTestCallEditorController', Qm_CliTester_HistoryTestCallEditorController);
})(appControllers);
