﻿appControllers.controller('RouteRuleEditorController2',
    function RouteRuleEditorController2($scope, $http, $location, $routeParams, notify) {
        $scope.BEDDate = "";
        var ctrl = this;
        ctrl.routetype = null;
        $scope.lstselectedtemp = "";
        $scope.EEDDate = "";
        $scope.ruletype = "";
        $scope.selectedtemp = function (items) {
            console.log(items)
        }
        $scope.isvalidcomp = function (model) {
            var s = "required-inpute";
            if ($scope[model] != undefined) {
                s = ($scope[model].url != '') ? "" : "required-inpute";
            }
            return s;
        }
        $scope.isvalidcompDate = function (model) {
            var s = "required-inpute";
            if ($scope[model] != undefined && $scope[model]!="") {
                var d = $scope[model];
                s = ($scope.isDate(d)) ? "" : "required-inpute";
            }
            return s ;
           

        }
        $scope.isvalidcompDateEED = function () {
            if (typeof ($scope.EEDDate) == "object" || typeof ($scope.EEDDate) == "string")
                return "";
            else if (typeof ($scope.EEDDate) == "undefined")
             return ($scope.isDate($scope.EEDDate)) ? "" : "required-inpute";


        }

        
        $scope.initErrortooltipBED = function () {
          
            if ($scope.showtd == true) {
                if (!$scope.isDate($scope.BEDDate)) {
                    var msg = "" ;
                    if (typeof ($scope.BEDDate) == "object" || typeof ($scope.BEDDate) == "string")
                        msg = "Begin Effective Date is required.";
                    if (typeof ($scope.BEDDate) == "undefined")
                        msg = "Begin Effective Date has an invalide format.";

                    $scope.msgd = msg

                    return true;
                }
            }
           

        }
        $scope.initErrortooltipEED = function () {

            if ($scope.showtde == true) {
                    var msg = "";                   
                    if (typeof ($scope.EEDDate) == "undefined") {
                        msg = "Begin Effective Date has an invalide format.";
                        $scope.msged = msg
                        return true;
                    }
                    else if(typeof ($scope.BEDDate) == "object" || typeof ($scope.BEDDate) == "string")
                        return false
                        
            }
          

        }
        $scope.subViewConnector = {};
        $scope.validateForm = function () {
            var obj = ($scope.subViewConnector.getCarrierAccountSet != undefined) ? $scope.subViewConnector.getCarrierAccountSet() : null;
             
            return ($scope.ruletype.url == '') || (!$scope.isDate($scope.dateToString($scope.BEDDate))) || $scope.isvalidcompDateEED() != "" || (obj == null || obj.Customers == undefined || obj.Customers.SelectedValues.length == 0);
        }
        
        $scope.routeRule = null;
       
        $scope.templates = [
            { name: 'Override Route', url: '/Client/Templates/PartialTemplate/RouteOverrideTemplate.html', objectType: 'TOne.LCR.Entities.OverrideRouteActionData, TOne.LCR.Entities' },
            { name: 'Priority Rule', url: '/Client/Templates/PartialTemplate/PriorityTemplate.html', objectType: 'TOne.LCR.Entities.PriorityRouteActionData, TOne.LCR.Entities' },
            { name: 'Block Route', url: '/Client/Templates/PartialTemplate/RouteBlockTemplate.html', objectType: 'TOne.LCR.Entities.BlockRouteActionData, TOne.LCR.Entities' }
        ]
       
        $scope.editorTemplates = [
            { name: 'Zone', url: '/Client/Templates/PartialTemplate/ZoneTemplate2.html', objectType: 'TOne.LCR.Entities.ZoneSelectionSet, TOne.LCR.Entities' },
            { name: 'Code', url: '/Client/Templates/PartialTemplate/CodeTemplate.html', objectType: 'TOne.LCR.Entities.CodeSelectionSet, TOne.LCR.Entities' }
        ]
        ctrl.routeTemplates = [
          { name: 'Customer', url: '/Client/Templates/PartialTemplate/CustomerTemplate2.html' },
          { name: 'Pool', url: '/Client/Templates/PartialTemplate/PoolTemplate.html' },
          { name: 'Product', url: '/Client/Templates/PartialTemplate/ProductTemplate.html'}
        ]

        console.log(ctrl.routeTemplates[0])
        if ($routeParams.RouteRuleId != 'undefined') {
            $http.get($scope.baseurl + "/api/Routing/GetRouteRuleDetails",
            {
                params: {
                    RouteRuleId: $routeParams.RouteRuleId
                }
            })
             .success(function (response) {
                 $scope.routeRule = response;
                 var tab = [];
                 ctrl.routetype = ctrl.routeTemplates[0];
                 $scope.BEDDate = new Date($scope.routeRule.BeginEffectiveDate);
                 $scope.EEDDate = $scope.routeRule.EndEffectiveDate;
                 $scope.Reason = $scope.routeRule.Reason;
                 $scope.ruletype = null;
                 $scope.ruletype = $scope.templates[$scope.findExsite($scope.templates, $scope.routeRule.ActionData.$type, 'objectType')];
                 $scope.editortype = null ;
                 $scope.editortype = $scope.editorTemplates[$scope.findExsite($scope.editorTemplates, $scope.routeRule.CodeSet.$type, 'objectType')];

             });
        }
        else {
            $scope.ruletype = { name: 'Select ..', url: '' };
            $scope.editortype = $scope.editorTemplates[0];
            ctrl.routetype = ctrl.routeTemplates[0];
        }        
       
        $scope.onloadRuletype = function () {            
            
        }
        
        $('.dropdown').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

         //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('.dropdown').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });
        
        var dropdownHidingTimeoutHandler;

        $('.dropdown-custom').on('mouseenter', function () {
            var $this = $(this);
            clearTimeout(dropdownHidingTimeoutHandler);
            if (!$this.hasClass('open')) {
                $('.dropdown-toggle', $this).dropdown('toggle');
            }
        });

        $('.dropdown-custom').on('mouseleave', function () {
            var $this = $(this);
            dropdownHidingTimeoutHandler = setTimeout(function () {
                if ($this.hasClass('open')) {
                    $('.dropdown-toggle', $this).dropdown('toggle');
                }
            }, 150);
        });        
        $scope.muteAction = function (e) {
            e.preventDefault();
            e.stopPropagation();
        }
      
        $scope.update = function (val, model) {
            if (model == 'routetype' && val.name != 'Customer') {
                $scope[model] = { "name": 'Customer', "url": '/Client/Templates/PartialTemplate/CustomerTemplate2.html' };
                notify.closeAll();
                notify({ message: 'This module is under construction.',classes:"alert alert-danger"});
            }
            else {
                $scope[model] = val;
            }
           
        }  
        $scope.update2 = function (data,items,last) {
            if (last.name != 'Customer') {  
              
                notify.closeAll();
                notify({ message: 'This module is under construction.', classes: "alert alert-danger" });
                return ctrl.routeTemplates[0];
            }
        }
        $scope.saveRule = function () {
          
            var routeRule =
                {
                    CodeSet: $scope.subViewConnector.getCodeSet(),
                    CarrierAccountSet: $scope.subViewConnector.getCarrierAccountSet(),
                    ActionData:  $scope.subViewConnector.getActionData(),
                    Type: "RouteRule",
                    BeginEffectiveDate: $scope.BEDDate,
                    EndEffectiveDate: ($scope.EEDDate),
                    Reason: $scope.Reason
                };
            $http.post($scope.baseurl + "/api/routing/SaveRouteRule",
                         routeRule)
                     .success(function (response) {
                         notify({ message: 'Route Rule has been saved successfully.', classes: "alert  alert-success" });
                         $location.path("/RouteRuleManager").replace();
                     });
        }
        $scope.cancel = function () {
            $location.path("/RouteRuleManager").replace();
        };

       
    });

function waitAlert(msg) {
    showAlertBootStrapMsg("attention", msg, true);
}
function hideWaitAlert() {
    $(".attention").fadeOut('slow');
    $('#notification').html('');
}
function showAlertBootStrapMsg(type, msg, wait) {
    $('#notification').html('<div class="alert ' + type + '" style="display: none;"> ' + msg + '  <img src="images/alert/close.png" alt="" class="close" data-dismiss="alert" ></div>');
    $("." + type).fadeIn('slow');
    if (!wait) {
        setTimeout(function () {
            $("." + type).fadeOut('slow');
            $('#notification').html('');
        }, 7000);
    }
    else {
        setTimeout(function () {
            $("." + type).fadeOut('slow');
            $('#notification').html('');
        }, wait);

    }
}
