using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.InterfacesLibrary.ProjectModel.Collections;
using ZennoLab.InterfacesLibrary.ProjectModel.Enums;
using ZennoLab.Macros;
using Global.ZennoExtensions;
using ZennoLab.Emulation;
using System.Web;

namespace KredixLib
{
    public class kredixlib
    {
        //Объявляем общие переменные
        //Ключи API
        private string ApiKey_smsreg = string.Empty;
        private string ApiKey_smsactivate = string.Empty;
        private string ApiKey_simsms = string.Empty;
        private string ApiKey_smsvk = string.Empty;
        private string ApiKey_smsarea = string.Empty;
        private string ApiKey_onlinesim = string.Empty;

        //Сервисы активации
        private string smsactivate = string.Empty;
        private string smsreg = string.Empty;
        private string simsms = string.Empty;
        private string smsvk = string.Empty;
        private string smsarea = string.Empty;
        private string onlinesim = string.Empty;

        //Общие переменные
        private string Proxy = string.Empty;
        private string Service = string.Empty;
        private string Forward = string.Empty;
        private string Operator = string.Empty;
        private string Number = string.Empty;
        private string Id = string.Empty;
        private string smsvk_balance = string.Empty;

        //Списки
        List<string> sms_services = new List<string>();

        public string getNumber (string Services_Activate, string Service_Id, string ApiKey_smsreg, string ApiKey_smsactivate, string ApiKey_simsms, string ApiKey_smsvk, string ApiKey_smsarea, string ApiKey_onlinesim)
        {
            //Number = string.Empty;
            //Id = string.Empty;
            string[] arrServices = Services_Activate.Split(','); //Поместили списко сервисов активации в массов

            for (int i=0;i<arrServices.Length; i++)
            {
                string service = arrServices[i];
                switch (service)
                {
                    case "sms-activate.ru":
                        //Получаем баланс sms-activate.ru
                        string smsactivate_getbalance = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey_smsactivate +
                            "&action=getBalance", Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        string smsactivate_balance = System.Text.RegularExpressions.Regex.Replace(smsactivate_getbalance, @".*?:", "");
                        if (Convert.ToInt32(smsactivate_balance) < 0)
                        {
                            break;
                        }
                        else
                        {
                            return smsactivate_balance;
                        }

                        /*
                        //Запрашиваем в sms-activate.ru количество свободных номеров
                        string getNumbersStatus = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey_smsactivate +
                            "&action=getNumbersStatus", Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                        var jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                        Dictionary<string, object> data = jsonser.Deserialize<Dictionary<string, object>>(getNumbersStatus);
                        switch (Service_Id)
                        {
                            case "vk":
                                string vk = data["vk_0"].ToString();
                                break;


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
                        }
                        */

                       // break;
                }
            }





              //Получаем баланс sms-reg.ru
            string smsreg_getbalance = ZennoPoster.HttpGet("http://api.sms-reg.com/getBalance.php?apikey=" + ApiKey_smsreg, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
            var smsreg_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, object> smsreg_data = smsreg_jsonser.Deserialize<Dictionary<string, object>>(smsreg_getbalance);
            string smsreg_response = smsreg_data["response"].ToString();
            string smsreg_balance = smsreg_data["balance"].ToString();

            //Получаем баланс simsms.org
            string simsms_getbalance = ZennoPoster.HttpGet("http://simsms.org/priemnik.php?metod=get_balance&service=opt4&apikey=" + ApiKey_simsms, Proxy, 
                "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
            var simsms_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, object> simsms_data = simsms_jsonser.Deserialize<Dictionary<string, object>>(simsms_getbalance);
            string simsms_response = simsms_data["response"].ToString();
            string simsms_balance = simsms_data["balance"].ToString();

            //Получаем баланс smsvk.net
            //Проверяем статус сим карты
            string smsvk_getSimStatus = ZennoPoster.HttpGet("http://smsvk.net/stubs/handler_api.php?api_key=" + ApiKey_smsvk + "&action=getSimStatus", Proxy,
                "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

            //Получаем баланс
            for (int i = 0; i < 10; i++)
            {
                if (smsvk_getSimStatus.Contains("OK"))
                {
                    string smsvk_getbalance = ZennoPoster.HttpGet("http://smsvk.net/stubs/handler_api.php?api_key=" + ApiKey_smsvk + "&action=getBalance", Proxy,
                        "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                    smsvk_balance = System.Text.RegularExpressions.Regex.Replace(smsvk_getbalance, @".*?:", "");
                }
                else
                {
                    System.Threading.Thread.Sleep(60000);
                }
            }

            //Получение баланса smsarea.org
            string smsarea_getbalance = ZennoPoster.HttpGet("http://sms-area.org/stubs/handler_api.php?api_key=" + ApiKey_smsarea + "&action=getBalance", Proxy,
                    "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
            string smsarea_balance = System.Text.RegularExpressions.Regex.Replace(smsarea_getbalance, @".*?:", "");
            sms_services.Add(smsarea_balance);

          return "OK";
        }

    }
}
