

//add this code on page.load event for each button btn.Attributes.Add("onkeydown", "DisableKeyCode('" + btn.UniqueID + "');");
function DisableKeyCode(btnId)
{
    if(event.which || event.keyCode)
    {
        if ((event.which == 13) || (event.keyCode == 13)) 
        {
            document.getElementById(btnId).click();
            return false;
        }
    } 
    else 
    {
        return true;
    }
}


function validateNumKey ()
{
 var inputKey =  event.keyCode;
 var returnCode = true;
 
 if ( inputKey > 47 && inputKey < 58 ) // numbers
 {
  return;
 }
 else
 {
  returnCode = false;
  event.keyCode = 0;
 }
 event.returnValue = returnCode;
}
  
 function validDate(obj){
 date=obj.value;
if (/[^\d/]|(\/\/)/g.test(date))  {obj.value=obj.value.replace(/[^\d/]/g,'');obj.value=obj.value.replace(/\/{2}/g,'/'); return }
if (/^\d{2}$/.test(date)){obj.value=obj.value+'/'; return }
if (/^\d{2}\/\d{2}$/.test(date)){obj.value=obj.value+'/'; return }
if (!/^\d{1,2}\/\d{1,2}\/\d{4}$/.test(date)) return

 test1=(/^\d{1,2}\/?\d{1,2}\/\d{4}$/.test(date))
 date=date.split('/')
 d=new Date();
 d.setFullYear(date[2],date[1]-1,date[0]);

 test2 = (1 * date[0] == d.getDate() && 1 * date[1] == (d.getMonth() + 1) && 1 * date[2] == d.getFullYear())
 
 if (test1 && test2) return true
 alert(" إنتبه إلى التاريخ")
 obj.select();
 obj.focus()
 return false
}
   
    function openBrowser(theURL) {
    var left = (screen.width/2)-150;
    var top = (screen.height/2)-150;
	features='toolbar=no,location=no,status=no,menubar=no,scrollbars=no,resizable=no,left='+left+',top='+top+',titlebar=no,width=100,height=100';
    window.open(theURL,'CNSS',features);
}

function reportPrint(Id) {
    document.getElementById(Id).ClientController.LoadPrintControl();
    window.setInterval("window.close()", 500);
}

