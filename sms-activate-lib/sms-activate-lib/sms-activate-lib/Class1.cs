using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.InterfacesLibrary.ProjectModel.Collections;
using ZennoLab.InterfacesLibrary.ProjectModel.Enums;
using ZennoLab.Macros;
using Global.ZennoExtensions;
using ZennoLab.Emulation;
using System.Web;

namespace sms_activate_lib
{
    public class smsactivate
    {

        private string ApiKey = string.Empty;
        private string Proxy = string.Empty;
        private string Service = string.Empty;
        private string Forward = string.Empty;
        private string Operator = string.Empty;

  

        //Приватная функция получения баланса сервиса sms-activate.ru
        private string getbalance(string ApiKey, string Proxy)
        {
           string getbalance = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey + 
               "&action=getBalance", Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
            string balance = System.Text.RegularExpressions.Regex.Replace(getbalance, @".*?:", "");
            return balance;
        }

        //Приватная функция получения количества свободных номеров
        private string getNumbersStatus(string ApiKey, string Proxy)
        {
            string getNumbersStatus = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey + 
                "&action=getNumbersStatus", Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
            var jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, object> data = jsonser.Deserialize<Dictionary<string, object>>(getNumbersStatus);
            string vk = data["vk_0"].ToString();
            string ok = data["ok_0"].ToString();
            string wa = data["wa_0"].ToString();
            string vi = data["vi_0"].ToString();
            string tg = data["tg_0"].ToString();
            string wb = data["wb_0"].ToString();
            string go = data["go_0"].ToString();
            string av = data["av_0"].ToString();
            string av_1 = data["av_1"].ToString();
            string fb = data["fb_0"].ToString();
            string tw = data["tw_0"].ToString();
            string ub = data["ub_0"].ToString();
            string qw = data["qw_0"].ToString();
            string gt = data["gt_0"].ToString();
            string sn = data["sn_0"].ToString();
            string ig = data["ig_0"].ToString();
            string ss = data["ss_0"].ToString();
            string ym = data["ym_0"].ToString();
            string ya = data["ya_0"].ToString();
            string ma = data["ma_0"].ToString();
            string mm = data["mm_0"].ToString();
            string uk = data["uk_0"].ToString();
            string me = data["me_0"].ToString();
            string mb = data["mb_0"].ToString();
            string we = data["we_0"].ToString();
            string ot = data["ot_0"].ToString();
            string ot_1 = data["ot_1"].ToString();
            return ok;
        }

        //Приватная функция получения номера сервиса snms-activate.ru
        private string smsactivate_getNumber(string ApiKey, string Proxy, string Service, string Forward, string Operator)
        {
            string getnumber = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey
                + "&action=getNumber&service=" + Service
                + "&forward=" + Forward
                + "&operator=" + Operator
                , Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

             //Получаем номер
            string sms_activate_number = System.Text.RegularExpressions.Regex.Replace(getnumber, @".*:", "");

            //Получаем id
            string idtemp = System.Text.RegularExpressions.Regex.Replace(getnumber, @"ACCESS.*?:", "");
            string sms_activate_id = System.Text.RegularExpressions.Regex.Replace(idtemp, @":7.*", "");

            return sms_activate_number;
        }
          
        //Получаем номер сервиса sms-activate.ru
        public string getnumber(string ApiKey, string Proxy, string Service, string Forward, string Operator)
        {
            //Получаем баланс сервиса sms-activate.ru
            string smsactivate_balance = getbalance(ApiKey, Proxy);
            if (smsactivate_balance.Contains("NO_KEY"))
            {
                return "Не указан ключ api сервиса sms-activate";
            }

            string smsactivate_numberstatus = getNumbersStatus(ApiKey, Proxy);
            string asdasd = smsactivate_getNumber(ApiKey, Proxy, Service, Forward, Operator);
            return asdasd;

        }
    }
}
