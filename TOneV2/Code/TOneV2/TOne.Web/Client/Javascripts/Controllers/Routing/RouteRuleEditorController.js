appControllers.controller('RouteRuleEditorController',
    function RouteRuleEditorController($scope, $http, $location,$routeParams, notify) {
        $scope.routeRule = null;
        $scope.templates = [
            { name: 'Override Route', url: '/Client/Templates/PartialTemplate/RouteOverrideTemplate.html', objectType: 'TOne.LCR.Entities.OverrideRouteActionData, TOne.LCR.Entities' },
            { name: 'Priority Rule', url: '/Client/Templates/PartialTemplate/PriorityTemplate.html', objectType: 'TOne.LCR.Entities.PriorityRouteActionData, TOne.LCR.Entities' },
            { name: 'Block Route', url: '/Client/Templates/PartialTemplate/RouteBlockTemplate.html', objectType: 'TOne.LCR.Entities.BlockRouteActionData, TOne.LCR.Entities' }
        ]
        $scope.editorTemplates = [
            { name: 'Zone', url: '/Client/Templates/PartialTemplate/ZoneTemplate.html', objectType: 'TOne.LCR.Entities.ZoneSelectionSet, TOne.LCR.Entities' },
            { name: 'Code', url: '/Client/Templates/PartialTemplate/CodeTemplate.html', objectType: 'TOne.LCR.Entities.CodeSelectionSet, TOne.LCR.Entities' }
        ]
        $scope.routeTemplates = [
          { name: 'Customer', url: '/Client/Templates/PartialTemplate/CustomerTemplate.html' },
          { name: 'Pool', url: '/Client/Templates/PartialTemplate/PoolTemplate.html' },
          { name: 'Product', url: '/Client/Templates/PartialTemplate/ProductTemplate.html' }
        ]
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
                 $scope.routetype = $scope.routeTemplates[0];                
                 $scope.BEDDate = $scope.routeRule.BeginEffectiveDate;
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
            $scope.routetype = $scope.routeTemplates[0];
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
                $scope[model] = $scope.routeTemplates[0];
                notify.closeAll();
                notify({ message: 'This module is under construction.',classes:"alert alert-danger"});
            }
            else {
                $scope[model] = val;
            }
           
        }  
        $scope.subViewConnector = {};
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
                       //  $location.path("/RouteRuleManager").replace();
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
